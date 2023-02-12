using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PrimeNG.TableFilter.Models
{
    public class TableFilterModel
    {
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
        public int First { get; set; } = 0;
        public int Rows { get; set; } = 10;
        public string SortField { get; set; }
        public int SortOrder { get; set; }
        public List<TableFilterSortMeta> MultiSortMeta { get; set; } = new List<TableFilterSortMeta>();
    }
}
