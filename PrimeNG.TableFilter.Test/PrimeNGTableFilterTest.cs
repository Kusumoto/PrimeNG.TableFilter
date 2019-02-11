using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PrimeNG.TableFilter.Models;
using Xunit;

namespace PrimeNG.TableFilter.Test
{
    public class PrimeNGTableFilterTest
    {
        public class TestData
        {
            public int Num1 { get; set; }
            public string String1 { get; set; }
            public DateTime DateTime1 { get; set; }
        }

        public IQueryable<TestData> GenerateMockTestData()
        {
            return new List<TestData>
            {
                new TestData { DateTime1  = new DateTime(2019, 1, 10), Num1 = 1, String1 = "Test1" },
                new TestData { DateTime1 = new DateTime(2019, 1, 11), Num1 = 2, String1 = "Test2" },
                new TestData { DateTime1 = new DateTime(2019, 1, 12), Num1 = 3, String1 = "Test3" },
                new TestData { DateTime1 = new DateTime(2019, 1, 13), Num1 = 4, String1 = "Test4" },
                new TestData { DateTime1 = new DateTime(2019, 1, 14), Num1 = 5, String1 = "Test5" },
                new TestData { DateTime1 = new DateTime(2019, 1, 15), Num1 = 6, String1 = "Test5" },
                new TestData { DateTime1 = new DateTime(2019, 1, 16), Num1 = 7, String1 = "Test6" },
                new TestData { DateTime1 = new DateTime(2019, 1, 17), Num1 = 8, String1 = "Test7" },
                new TestData { DateTime1 = new DateTime(2019, 1, 18), Num1 = 9, String1 = "Test7" }
            }.AsQueryable();
        }

        public TableFilterModel GenerateFilterTableFromJson(string json)
            => JsonConvert.DeserializeObject<TableFilterModel>(json);

        [Fact]
        public void Order_Table_Field_Num1_By_Asc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: 1 }");
            var dataSet = GenerateMockTestData();
            dataSet.PrimengTableFilter(filter);
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(1, dataSet.FirstOrDefault()?.Num1);
        }

        [Fact]
        public void Order_Table_Field_Num1_By_Desc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(9, dataSet.FirstOrDefault()?.Num1);
        }

        [Fact]
        public void Order_Table_Field_Contains_String1_Where_Test3_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test\", matchMode: \"contains\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData().Where(o => o.Num1 == 1);
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(1, dataSet.Count());
        }

        [Fact]
        public void Order_Table_Field_Equals_String1_Where_Test3_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test3\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal("Test3", dataSet.FirstOrDefault()?.String1);
        }

        [Fact]
        public void Order_Table_Field_Num1_Where_2_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: \"6\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal("Test5", dataSet.FirstOrDefault()?.String1);
        }

        [Fact]
        public void Order_Table_Field_Num1_Where_7_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: \"7\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal("Test6", dataSet.FirstOrDefault()?.String1);
        }

        [Fact]
        public void Order_Table_Field_DateTime1_Where_2019_1_16_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime1: { value: \"2019-01-16T00:00:00.000Z\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal("Test6", dataSet.FirstOrDefault()?.String1);
        }

        [Fact]
        public void Order_Table_Where_Multi_Field_Num1_String1_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test7\", matchMode: \"contains\" }, num1: { value: \"9\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(1, dataSet.Count());
            Assert.Equal(9, dataSet.FirstOrDefault()?.Num1);
        }

        [Fact]
        public void Order_Table_First_3_Row_5_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 3, globalFilter: null, multiSortMeta: undefined, rows: 5, sortField: \"\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(5, dataSet.Count());
            Assert.Equal(4, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(8, dataSet.LastOrDefault()?.Num1);
        }

        [Fact]
        public void List_Filter_Test7_In_List_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: [\"1\",\"7\",\"9\"], matchMode: \"in\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter);
            Assert.Equal(3, dataSet.Count());
        }

    }
}
