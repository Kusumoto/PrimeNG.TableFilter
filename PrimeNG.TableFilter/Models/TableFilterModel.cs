using System.Collections.Generic;

namespace PrimeNG.TableFilter.Models
{
    public class TableFilterModel
    {
        public Dictionary<string, TableFilterContext> Filters { get; set; }
        public int First { get; set; }
        public int Rows { get; set; }
        public string SortField { get; set; }
        public int SortOrder { get; set; }
    }
}
