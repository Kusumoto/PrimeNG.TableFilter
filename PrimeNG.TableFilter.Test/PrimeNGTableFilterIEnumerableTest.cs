using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PrimeNG.TableFilter.Models;
using Xunit;

namespace PrimeNG.TableFilter.Test
{
    public class PrimeNGTableFilterIEnumerableTest
    {
        private static IEnumerable<TestData> GenerateMockTestData()
        {
            return new List<TestData>
            {
                new TestData { DateTime1  = new DateTime(2019, 1, 10), Num1 = 1, String1 = "Test1", DateTime2 = new DateTime(2019, 2, 10) },
                new TestData { DateTime1 = new DateTime(2019, 1, 11), Num1 = 2, String1 = "Test2" },
                new TestData { DateTime1 = new DateTime(2019, 1, 12), Num1 = 3, String1 = "Test3" },
                new TestData { DateTime1 = new DateTime(2019, 1, 13), Num1 = 4, String1 = "Test4" },
                new TestData { DateTime1 = new DateTime(2019, 1, 14), Num1 = 5, String1 = "Test5" },
                new TestData { DateTime1 = new DateTime(2019, 1, 15), Num1 = 6, String1 = "Test5" },
                new TestData { DateTime1 = new DateTime(2019, 1, 16), Num1 = 7, String1 = "Test6" },
                new TestData { DateTime1 = new DateTime(2019, 1, 17), Num1 = 8, String1 = "Test7", DateTime2 = new DateTime(2019, 2, 17) },
                new TestData { DateTime1 = new DateTime(2019, 1, 18), Num1 = 9, String1 = "Test7" },
                new TestData { DateTime1 = new DateTime(2019, 1, 18), Num1 = 9, String1 = "Best1" },
                new TestData { DateTime1 = new DateTime(2019, 1, 18), Num1 = 9, String1 = "Best2" },
                new TestData { DateTime1 = new DateTime(2019, 1, 18), Num1 = 9, String1 = "Best3" },
                // nullable TestData
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T1", NullableBool=false },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T1", NullableBool=true },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T1", NullableBool=true },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableShort=5},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableInt=12},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B2", NullableLong=55 },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B3", NullableFloat=(float?)5.1},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T7", NullableDouble=10.2},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableDecimal = (decimal?)22.5 },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B2", DateTime2 = new DateTime(2021,11,5) },
            };
        }

        private TableFilterModel GenerateFilterTableFromJson(string json)
            => JsonConvert.DeserializeObject<TableFilterModel>(json);
        
        [Fact]
        public void Order_Table_Field_Num1_By_Asc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"num1\",sortOrder: 1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(1, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(counted, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_Num1_By_Desc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(90, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(counted, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_Contains_String1_Where_Test3_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test\", matchMode: \"contains\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData().Where(o => o.Num1 == 1);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_Equals_String1_Where_Test3_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test3\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test3", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_Num1_Where_2_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: \"6\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test5", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_Num1_Where_7_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: \"7\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test6", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_Field_DateTime1_Where_2019_1_16_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime1: { value: \"2019-01-16T00:00:00.000Z\", matchMode: \"dateIs\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test6", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_Where_Multi_Field_Num1_String1_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test7\", matchMode: \"contains\" }, num1: { value: \"9\", matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
            Assert.Equal(9, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(1, totalRecord);
        }

        [Fact]
        public void Order_Table_First_3_Row_5_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {} , first: 3, globalFilter: null, multiSortMeta: undefined, rows: 5, sortField: \"\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(5, dataSet.Count());
            Assert.Equal(4, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(8, dataSet.LastOrDefault()?.Num1);
            Assert.Equal(counted, totalRecord);
        }

        [Fact]
        public void List_Filter_Test7_In_List_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { num1: { value: [\"1\",\"7\",\"9\"], matchMode: \"in\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(6, dataSet.Count());
            Assert.Equal(6, totalRecord);
        }

        [Fact]
        public void List_Multiple_Sorting_Asc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: 10, sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"num1\", order: 1 }, { field: \"string1\", order: 1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(10, dataSet.Count());
            Assert.Equal(counted, totalRecord);
            Assert.Equal(1, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(9, dataSet.LastOrDefault()?.Num1);
        }
        [Fact]
        public void List_Multiple_Sorting_Desc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: 10, sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"num1\", order: -1 }, { field: \"string1\", order: -1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(10, dataSet.Count());
            Assert.Equal(counted, totalRecord);
            Assert.Equal(90, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(90, dataSet.LastOrDefault()?.Num1);
        }
        [Fact]
        public void List_Multiple_Sorting_Mix_Asc_Desc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: 10, sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"num1\", order: 1 }, { field: \"string1\", order: -1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(10, dataSet.Count());
            Assert.Equal(counted, totalRecord);
            Assert.Equal(1, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(9, dataSet.LastOrDefault()?.Num1);
            Assert.Equal("Test1", dataSet.FirstOrDefault()?.String1);
            Assert.Equal("Best3", dataSet.LastOrDefault()?.String1);
        }
        
        [Fact]
        public void List_Multiple_Sorting_Mix_Desc_Asc_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: 10, sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"num1\", order: -1 }, { field: \"string1\", order: 1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(10, dataSet.Count());
            Assert.Equal(counted, totalRecord);
            Assert.Equal(90, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(90, dataSet.LastOrDefault()?.Num1);
            Assert.Equal("B1", dataSet.FirstOrDefault()?.String1);
            Assert.Equal("T7", dataSet.LastOrDefault()?.String1);
        }
        
        [Fact]
        public void List_Multiple_Sorting_Mix_Desc_Asc_Duplicate_First_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: 10, sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"string1\", order: -1 }, { field: \"num1\", order: 1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(10, dataSet.Count());
            Assert.Equal(counted, totalRecord);
            Assert.Equal(8, dataSet.FirstOrDefault()?.Num1);
            Assert.Equal(90, dataSet.LastOrDefault()?.Num1);
            Assert.Equal("Test7", dataSet.FirstOrDefault()?.String1);
            Assert.Equal("T7", dataSet.LastOrDefault()?.String1);
        }


        [Fact]
        public void List_Field_DateTime2_Where_2019_2_17_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime2: { value: \"2019-02-17T00:00:00.000Z\", matchMode: \"dateIs\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10, sortField: \"Num1\",sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test7", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(1, totalRecord);
        }
        
        [Fact]
        public void List_No_Filter_Payload_Test()
        {
            var filter = GenerateFilterTableFromJson("{}");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
            Assert.Empty(dataSet);
        }
        
        [Fact]
        public void List_Field_Filter_Null_Value_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: null, matchMode: \"startsWith\" }, num1: { value: null, matchMode: \"startsWith\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }

        [Fact] 
        public void List_Field_Filter_Not_Found_In_Entity_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string2: { value: \"Test\", matchMode: \"equals\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact] 
        public void List_Field_Filter_Not_Contain_In_Test1_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test5\", matchMode: \"notContains\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x=>!x.String1.Contains("Test5"));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact] 
        public void List_Field_Filter_Not_Equals_In_Test1_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test1\", matchMode: \"notEquals\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => !x.String1.Contains("Test1"));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact]
        public void List_Field_Filter_Multiple_Or_Condition_In_Field_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: [ { matchMode: \"contains\", operator: \"or\", value: \"Test7\" }, { matchMode: \"contains\", operator: \"or\", value: \"Test1\" } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(3, totalRecord);
        }
        
        [Fact]
        public void List_Field_Filter_Multiple_And_Condition_In_Field_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: [ { matchMode: \"contains\", operator: \"and\", value: \"Best\" }, { matchMode: \"contains\", operator: \"and\", value: \"3\" } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(1, totalRecord);
        }
        [Fact]
        public void NullableBoolFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: false, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        public void NullableBoolFilterShouldReturnTwoElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: true, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(2,totalRecord);
        }
        [Fact]
        public void NullablShortFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 5, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
    }
}