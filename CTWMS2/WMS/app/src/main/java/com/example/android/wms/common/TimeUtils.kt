import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.Locale

/**
 * 時間工具類：獲取手機本地時區的時間字串
 */
object TimeUtils {
    // 常用時間格式（按需調整）
    private val DEFAULT_FORMATTER = DateTimeFormatter.ofPattern(
            "yyyyMMdd-HHmmss",
            Locale.getDefault() // 適配手機本地語言（如中文/英文）
    )

    /**
     * 獲取手機本地時間字串（預設格式：yyyy-MM-dd HH:mm:ss）
     */
    fun getLocalTimeStr(): String {
        return LocalDateTime.now().format(DEFAULT_FORMATTER)
    }

    /**
     * 自訂格式獲取手機本地時間字串
     * @param pattern 時間格式，如 "yyyyMMddHHmmss"、"HH:mm"
     */
    fun getLocalTimeStr(pattern: String): String {
        val formatter = DateTimeFormatter.ofPattern(pattern, Locale.getDefault())
        return LocalDateTime.now().format(formatter)
    }
}
