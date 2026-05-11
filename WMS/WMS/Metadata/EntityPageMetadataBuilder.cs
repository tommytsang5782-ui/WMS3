using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace WMS.Metadata
{
    internal sealed class EntityPageMetadataBuilder
    {
        public PageConfig BuildPageConfig(
            string pageCode,
            Dictionary<string, Type> tableTypeMapping,
            Func<List<MenuItem>> menuFactory)
        {
            if (string.Equals(pageCode, "Shell", StringComparison.OrdinalIgnoreCase))
            {
                return new PageConfig { Menu = menuFactory() };
            }

            if (!tableTypeMapping.TryGetValue(pageCode, out Type entityType))
            {
                return new PageConfig
                {
                    TableName = pageCode,
                    KeyFields = new List<string>(),
                    Menu = menuFactory(),
                    ListPage = new ListPageConfig { Columns = new List<FieldInfo>(), Buttons = new List<ButtonInfo>() },
                    CardPage = new CardPageConfig { Fields = new List<FieldInfo>(), Buttons = new List<ButtonInfo>() }
                };
            }

            return new PageConfig
            {
                TableName = pageCode,
                KeyFields = GetKeyFields(entityType),
                Menu = menuFactory(),
                ListPage = new ListPageConfig
                {
                    Columns = BuildFields(entityType, forListPage: true),
                    Buttons = new List<ButtonInfo>
                    {
                        new ButtonInfo { Text = "New", Name = "btnNew", Command = "New" },
                        new ButtonInfo { Text = "Edit", Name = "btnEdit", Command = "Edit" },
                        new ButtonInfo { Text = "Delete", Name = "btnDelete", Command = "Delete" },
                        new ButtonInfo { Text = "Refresh", Name = "btnRefresh", Command = "Refresh" }
                    }
                },
                CardPage = new CardPageConfig
                {
                    Fields = BuildFields(entityType, forListPage: false),
                    Buttons = new List<ButtonInfo>
                    {
                        new ButtonInfo { Text = "Save", Name = "btnSave", Command = "Save" },
                        new ButtonInfo { Text = "Cancel", Name = "btnCancel", Command = "Cancel" }
                    }
                }
            };
        }

        /// <summary>建立/最後修改審計欄位（與 PC CardPage 自動寫入邏輯一致）。</summary>
        private static bool IsAuditPropertyName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return name.Equals("CreateUser", StringComparison.OrdinalIgnoreCase)
                || name.Equals("CreationDate", StringComparison.OrdinalIgnoreCase)
                || name.Equals("LastModifyUser", StringComparison.OrdinalIgnoreCase)
                || name.Equals("LastModifyDate", StringComparison.OrdinalIgnoreCase);
        }

        private List<FieldInfo> BuildFields(Type entityType, bool forListPage)
        {
            var fields = new List<FieldInfo>();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && p.Name != "_PrimaryKey");

            foreach (var prop in properties)
            {
                bool isKey = prop.GetCustomAttribute<KeyFieldAttribute>()?.IsPrimaryKey == true;
                bool required = prop.GetCustomAttribute<RequiredAttribute>() != null;
                string label = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;
                bool readOnlyOnList = forListPage && isKey;
                bool readOnlyOnCard = !forListPage && IsAuditPropertyName(prop.Name);
                fields.Add(new FieldInfo
                {
                    Field = prop.Name,
                    Type = Nullable.GetUnderlyingType(prop.PropertyType)?.Name ?? prop.PropertyType.Name,
                    Label = label,
                    ReadOnly = readOnlyOnList || readOnlyOnCard,
                    Required = required
                });
            }

            return fields;
        }

        private List<string> GetKeyFields(Type entityType)
        {
            var keys = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<KeyFieldAttribute>()?.IsPrimaryKey == true)
                .Select(p => p.Name)
                .ToList();

            if (keys.Count == 0)
            {
                var keyProp = ResolvePrimaryKeyProperty(entityType);
                if (keyProp != null)
                {
                    keys.Add(keyProp.Name);
                }
            }

            return keys;
        }

        /// <summary>
        /// 回推實際主鍵屬性（例如 UserID/No/Code），避免把 _PrimaryKey 當成 UI key 欄位。
        /// </summary>
        private PropertyInfo ResolvePrimaryKeyProperty(Type entityType)
        {
            if (entityType == null) return null;

            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.Name != "_PrimaryKey")
                .ToList();
            if (props.Count == 0) return null;

            var tempEntity = Activator.CreateInstance(entityType);
            var pkProp = entityType.GetProperty("_PrimaryKey", BindingFlags.Public | BindingFlags.Instance);
            if (tempEntity == null || pkProp == null) return props.FirstOrDefault();

            foreach (var p in props)
            {
                try
                {
                    object probe = BuildProbeValue(p.PropertyType);
                    if (probe == null) continue;
                    p.SetValue(tempEntity, probe);
                    var currentPk = pkProp.GetValue(tempEntity);
                    if (currentPk != null && currentPk.Equals(probe))
                        return p;
                }
                catch
                {
                    // ignore and continue probing
                }
            }
            return props.FirstOrDefault();
        }

        private object BuildProbeValue(Type t)
        {
            if (t == typeof(string)) return Guid.NewGuid().ToString("N");
            if (t == typeof(int)) return 999;
            if (t == typeof(long)) return 999999L;
            if (t == typeof(Guid)) return Guid.NewGuid();
            return null;
        }
    }
}
