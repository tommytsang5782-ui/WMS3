using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    internal class Core
    {
        public class PageSchema
        {
            public string PageName { get; set; }
            public List<TableColumnDef> Columns { get; set; } = new List<TableColumnDef>();
            public List<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
        }

        public class TableColumnDef
        {
            public string Title { get; set; }
            public string DataKey { get; set; }
            public int Width { get; set; }
        }

        public class FormFieldDef
        {
            public string Label { get; set; }
            public string DataKey { get; set; }
            public string ControlType { get; set; }
        }
    }
}
