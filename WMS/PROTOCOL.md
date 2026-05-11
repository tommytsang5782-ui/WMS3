# WMS 三端通訊協定說明

## 變更摘要（現況）

| 項目         | 現況（PC / Android 一致） |
|--------------|-----------------------------|
| 請求         | 一行 JSON（`CommuForm`，換行結尾） |
| 回應         | **4 位元組長度前綴（BigEndian）+ JSON 內文（UTF-16BE）** |
| 伺服器發送   | `SendUnifiedResponse`（binary frame） |
| 客戶端接收   | 先讀 4 bytes 長度，再讀 body（Android 由 `LengthFieldBasedFrameDecoder` 完成） |

## 概述

- **WMS**：伺服器（C#）
- **WMSClient**：PC 客戶端（C# WinForms）
- **CTWMS2**：Android 客戶端（Kotlin + Netty）

## 統一協定（修訂後）

### 編碼
- 一律使用 **BigEndianUnicode (UTF-16BE)**。

### 請求（Client → Server）
- 一則訊息 = **一行 JSON**（以 `\r\n` 或 `\n` 結尾）。
- 伺服器以 `StreamReader.ReadLine()` 讀取。
- 請求格式（CommuForm）：`{ "Command", "Action", "Table", "Str" }`。
  - **統一**：資料庫操作一律使用 `Command = "SQL"`，Action = Select/Insert/Update/Delete，Table = 表名，Str = "@" + JSON 參數（PC 與 Android 相同）。另 Android 連線時會送 `"Connect"`，裝置註冊用 `Command = "New"`。
  - **Metadata（瘦客戶端）**：`Command = "GetConfig"`，`Table` 為 `Shell` 或實體表名（如 `User`），`Str = "@{\"scope\":\"Shell|Page\",\"userId\":\"...\"}"`。

### 回應（Server → Client）
- **所有客戶端（PC / Android，所有 Command 包含 SQL / GetConfig / Connect）**：
  1. 先送 4 位元組長度（BigEndian）。
  2. 再送 JSON 內文（BigEndianUnicode / UTF-16BE）。
- **WMSClient**：使用 `ReceiveLargeData()` 先讀長度再讀 body。
- **CTWMS2**：`NettyTcpClient` 使用 `LengthFieldBasedFrameDecoder(0,4,0,4)` + `StringDecoder(UTF-16BE)`。

### GetConfig 範例（Metadata-Driven）

#### 取 Shell 菜單

```json
{"Command":"GetConfig","Action":"","Table":"Shell","Str":"@{\"scope\":\"Shell\",\"userId\":\"admin\"}"}
```

#### 取 User 頁面配置

```json
{"Command":"GetConfig","Action":"","Table":"User","Str":"@{\"scope\":\"Page\",\"userId\":\"admin\"}"}
```

#### 回傳（節錄）

```json
{
  "TableName":"User",
  "KeyFields":["UserID"],
  "ListPage":{"Columns":[{"Field":"UserID","Type":"String","Label":"User ID"}]},
  "CardPage":{"Fields":[{"Field":"Password","Type":"String","Label":"Password"}]},
  "Menu":[{"Text":"Master Data","FormKind":"Crud","Children":[{"Text":"User","FormName":"User","FormKind":"Crud"}]}]
}
```

### 辨識客戶端
- 伺服器依 **dic2** 是否標記為 Android 區分：
  - 送過 `SQL` + `Action = "Connect"` 的連線會登記為 Android（dic2[ip] = "Android"）。
  - `Command == "SQL"`（無論 Android / PC）最終回應都走 `SendUnifiedResponse`（4-byte + body）。
  - Android 仍有額外分塊推送機制（`BlockDataModel`），但分塊資料本身仍包在上述長度框架中。

### 錯誤回應
- 失敗時伺服器仍回傳 JSON（例如：`{"Code":-2,"Msg":"查詢失敗：..."}`），**傳輸層格式同樣是 4-byte length + body**。
- 客戶端需能處理陣列（成功）或物件（錯誤），例如用 JToken 判斷型別再解析。

---

## 客戶端應發送的訊息格式（資料庫操作）

所有資料庫相關請求使用 **Command = "SQL"**，一行 JSON 一個請求。

### 請求結構（CommuForm）

| 欄位 | 說明 | 範例 |
|------|------|------|
| **Command** | 固定 `"SQL"`（資料庫操作） | `"SQL"` |
| **Action** | 操作類型 | `"Select"` / `"Insert"` / `"Update"` / `"Delete"` |
| **Table** | 表名（與伺服器 case 對應） | `"User"`, `"Item"`, `"PackingHeader"` 等 |
| **Str** | 一律以 `"@"` 開頭，後面接 JSON 字串 | 見下表 |

### Str 參數內容（去掉開頭 `@` 後為 JSON）

| Action | Str 格式 | 說明 |
|--------|----------|------|
| **Select** | `"@" + JSON.stringify(篩選物件)` | 篩選物件可為空 `{}` 表示查全部；有主鍵/欄位則查符合的列 |
| **Insert** | `"@" + JSON.stringify(單筆實體)` | 要新增的一筆資料 |
| **Update** | `"@" + JSON.stringify([舊或鍵, 新實體])` | 兩元素陣列：第一筆為原鍵/舊實體，第二筆為新資料 |
| **Delete** | `"@" + JSON.stringify(要刪除的實體)` | 至少需含主鍵的實體 |

### 支援的 Table 名稱（與伺服器一致）

- User, Item, CustomerGroup, Printer  
- PackingHeader, PackingLine, Mapping  
- LabelHeader, LabelLine, ScanLabelString  
- Prescan, OuterCarton, InnerCarton  
- PrescanOuterCarton, PrescanInnerCarton, PackingMapping  
- ScannedPackingHeader, ScannedPackingLine, ScannedPackingMapping  
- ClosedPrescan, ClosedPrescanOuterCarton, ClosedPrescanInnerCarton  
- Company, ODataSetup, Setup  

### 回應格式

- **Select**：成功回傳 JSON 陣列（該表查詢結果）；失敗為物件 `{ Code, Msg }`。
- **Insert / Update / Delete**：成功多為 `{ Code, Msg, EffectedRows }` 或直接回傳影響列數相關結構；失敗為 `{ Code, Msg }`。

### 回應筆數說明（避免混淆）

- **Update**：成功時伺服器可能回傳 **2 筆** 記錄（例如 `[舊實體, 新實體]`），供客戶端比對或更新 UI。
- **Select / Insert / Delete**：成功時回傳 **1 筆** 或單一結構（Select 為陣列、Insert/Delete 為單筆或影響列數）。  
此為設計如此，客戶端解析時依 Action 判斷預期筆數即可。

### 精簡資料庫操作

若只需部分表，可在伺服器 `Form1.cs` 的 `Command == "SQL"` 底下，依 **Action**（Select/Update/Insert/Delete）刪除不需要的 **Table** case；未列出的 Table 會走 default 回傳「未知表」。客戶端只發送實際用到的 Table 與 Action 即可。

---

## 原始碼與檔案編碼（避免中文亂碼）

- **請將所有 C# / Kotlin 原始碼存成 UTF-8**（含註解與字串內的中文）。
- 若用 PowerShell 或腳本寫入檔案，請指定編碼，例如：`Set-Content -Encoding UTF8` 或 `Out-File -Encoding utf8`；否則預設編碼可能造成中文變成亂碼。
- 專案與編輯器建議統一使用 **UTF-8**（可選 BOM），以便三端與協定文件一致。

---

## BlockHeader 與 CommuForm.Command 的區別（是否重複？）

兩者**不重複**，但都有一個「指令／類型」欄位，容易搞混，說明如下：

| 項目 | 用途 | 所在層級 | 常見值 |
|------|------|----------|--------|
| **CommuForm.Command** | 整則**訊息**的類型（請求／回應） | 每則訊息的頂層 JSON | `"SQL"`, `"New"`, `"Open"`；回應時如 `"Reply"` |
| **BlockHeader.command**（WMS）/ **BlockHeader.Command**（Android） | **分塊傳輸**時，這一「塊」的類型 | 僅出現在 SplitSend 的**分塊**結構裡（BlockDataModel.BlockHeader） | `"Reply"`, `"Insert"`（與 Action 搭配表示本塊用途） |

- **CommuForm**：一則完整訊息 = 一個 CommuForm（Command + Action + Table + Str）。
- **BlockHeader**：伺服器對 Android 做**分塊推送**（例如大量 User/Item）時，每一塊會包在 `BlockDataModel { BlockHeader, BlockData }` 裡；BlockHeader 描述「這一塊是第幾塊、共幾塊、表名、動作」等，**不是**整則訊息的 Command。

**建議（可選改善）**：若希望語意更清楚，可將 BlockHeader 的 `command` 改名為 `messageType` 或 `chunkType`，表示「本塊的訊息類型」，與頂層的 CommuForm.Command 區隔。目前兩端都有 `Command` 欄位且語意略為重疊，但層級不同，不影響現有協定運作。
