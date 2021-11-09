using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PrimeNG.TableFilter.Models;
using Xunit;

namespace PrimeNG.TableFilter.Test
{
    public class PrimeNGTableFilterIQueryableTest
    {
        private static IQueryable<TestData> GenerateMockTestData()
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
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableShort=6},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableInt=12},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableInt=13},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B2", NullableLong=55},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableLong=56},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B3", NullableFloat=(float?)5.1},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B3", NullableFloat=(float?)5.2},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T7", NullableDouble=10.2},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "T7", NullableDouble=10.3},
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableDecimal = (decimal?)22.5 },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B1", NullableDecimal = (decimal?)22.6 },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B2", DateTime2 = new DateTime(2021,11,5) },
                new TestData { DateTime1 = new DateTime(2021, 11, 1), Num1 = 90, String1 = "B2", DateTime2 = new DateTime(2021,11,8) },
            }.AsQueryable();
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
            Assert.Equal(1, dataSet.Count());
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
            Assert.Equal(1, dataSet.Count());
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
            Assert.Equal(dataSet.First().Num1, dataSet.FirstOrDefault()?.Num1);
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
            Assert.Equal("B2", dataSet.LastOrDefault()?.String1);
        }
        
        [Fact]
        public void List_Multiple_Sorting_Mix_Desc_Asc_Duplicate_First_Test()
        {
            int rows = 10;
            var filter = GenerateFilterTableFromJson("{ filters: {}, first: 0, globalFilter: null, rows: "+rows+", sortField: \"\",sortOrder: -1, multiSortMeta: [{ field: \"string1\", order: -1 }, { field: \"num1\", order: 1 }] }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(rows, dataSet.Count());
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
            var dt = new DateTime(2019, 2, 17);
            int counted = dataSet.Count(x => x.DateTime2.HasValue && x.DateTime2.Value == dt);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal("Test7", dataSet.FirstOrDefault()?.String1);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact]
        public void List_No_Filter_Payload_Test()
        {
            var filter = GenerateFilterTableFromJson("{}");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
            Assert.Equal(0, dataSet.Count());
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
            int counted = dataSet.Count(x => !x.String1.Contains("Test5"));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact] 
        public void List_Field_Filter_Not_Equals_In_Test1_Test()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { string1: { value: \"Test1\", matchMode: \"notEquals\" } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.String1 != "Test1");
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
        [Trait("Category", "Nullable")]
        [Trait("Type", "Bool")]
        public void NullableBoolFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: false, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Bool")]
        public void NullableBoolFilterShouldReturnTestDataWithoutNullableBoolFalse()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: false, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableBool.HasValue && x.NullableBool == false);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Bool")]
        public void NullableBoolFilterShouldReturnTestDataWithoutNullableBoolTrue()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: true, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 50,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableBool.HasValue && x.NullableBool == true);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Bool")]
        public void NullableBoolFilterShouldReturnInitialTestDataWhenNullableBoolNull()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: null, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 50,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Bool")]
        public void NullableBoolFilterShouldReturnTwoElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableBool: { value: true, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(2, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 5, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnLessThan_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 6, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnLessOrEquals_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 6, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableShort <= 6);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted,totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnGreaterThan_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 5, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnGreaterOrEquals_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 5, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableShort >= 5);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnTestDataWithoutNullableShort5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort: { value: 5, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableShort.HasValue && x.NullableShort.Value == 5);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnNullableShort5OrNullableShort6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort:  [ { matchMode: \"equals\", operator: \"or\", value: 5 }, { matchMode: \"equals\", operator: \"or\", value: 6 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableShort.HasValue && (x.NullableShort.Value == 5 || x.NullableShort.Value == 6));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturTestDataWithoutNullableShort5AndNullableShort6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort:  [ { matchMode: \"notEquals\", operator: \"and\", value: 5 }, { matchMode: \"notEquals\", operator: \"and\", value: 6 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => !(x.NullableShort.HasValue && (x.NullableShort.Value == 5 || x.NullableShort.Value == 6)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Short")]
        public void NullablShortFilterShouldReturnNullableShort5OrNullableShort6In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableShort:   { matchMode: \"in\", operator: \"and\", value: [5,6] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => (x.NullableShort.HasValue && (x.NullableShort.Value == 5 || x.NullableShort.Value == 6)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 12, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnLessThan_13()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 13, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnLessOrEquals_13()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 13, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableInt <= 13);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnGreaterThan_12()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 12, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnGreaterOrEquals_12()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 12, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableInt >= 12);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnTestDataWithoutNullableInt12()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt: { value: 12, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableInt.HasValue && x.NullableInt.Value == 12);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnNullableInt12OrNullableInt13()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt:  [ { matchMode: \"equals\", operator: \"or\", value: 12 }, { matchMode: \"equals\", operator: \"or\", value: 13 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableInt.HasValue && (x.NullableInt.Value == 12 || x.NullableInt.Value == 13));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturTestDataWithoutNullableInt12AndNullableInt13()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt:  [ { matchMode: \"notEquals\", operator: \"and\", value: 12 }, { matchMode: \"notEquals\", operator: \"and\", value: 13 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => !(x.NullableInt.HasValue && (x.NullableInt.Value == 12 || x.NullableInt.Value == 13)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Int")]
        public void NullableIntFilterShouldReturnNullableInt12OrNullableInt13In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableInt:   { matchMode: \"in\", operator: \"and\", value: [12,13] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => (x.NullableInt.HasValue && (x.NullableInt.Value == 12 || x.NullableInt.Value == 13)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 55, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnLessThan_56()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 56, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnLessOrEquals_56()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 56, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableLong <= 56);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnGreaterThan_55()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 55, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnGreaterOrEquals_55()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 55, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableLong >= 55);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnTestDataWithoutNullableLong55()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong: { value: 55, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableLong.HasValue && x.NullableLong.Value == 55);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnNullableLong55OrNullableLong56()
                    
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong:  [ { matchMode: \"equals\", operator: \"or\", value: 55 }, { matchMode: \"equals\", operator: \"or\", value: 56 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableLong.HasValue && (x.NullableLong.Value == 55 || x.NullableLong.Value == 56));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturTestDataWithoutNullableLong55AndNullableLong56()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong:  [ { matchMode: \"notEquals\", operator: \"and\", value: 55 }, { matchMode: \"notEquals\", operator: \"and\", value: 56 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => !(x.NullableLong.HasValue && (x.NullableLong.Value == 55 || x.NullableLong.Value == 56)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Long")]
        public void NullableLongFilterShouldReturnNullableLong55OrNullableLong56In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableLong:   { matchMode: \"in\", operator: \"and\", value: [55,56] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => (x.NullableLong.HasValue && (x.NullableLong.Value == 55 || x.NullableLong.Value == 56)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.1, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnLessThan_5_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.2, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnLessOrEquals_5_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.2, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableFloat <= (float?)5.2);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnGreaterThan_5_1()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.1, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnGreaterOrEquals_5_1()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.1, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableFloat >= (float?)5.1);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnTestDataWithoutNullableFloat5_1()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat: { value: 5.1, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            float? value = (float?)5.1;
            int counted = dataSet.Count(x => x.NullableFloat.HasValue && x.NullableFloat.Value == value);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnNullableFloat5_1OrNullableFloat5_2()

        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat:  [ { matchMode: \"equals\", operator: \"or\", value: 5.1 }, { matchMode: \"equals\", operator: \"or\", value: 5.2 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            float? value1 = (float?)5.1;
            float? value2 = (float?)5.2;
            int counted = dataSet.Count(x => x.NullableFloat.HasValue && (x.NullableFloat.Value == value1 || x.NullableFloat.Value ==value2));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturTestDataWithoutNullableFloat5_1AndNullableFloat5_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat:  [ { matchMode: \"notEquals\", operator: \"and\", value: 5.1 }, { matchMode: \"notEquals\", operator: \"and\", value: 5.2 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            float? value1 = (float?)5.1;
            float? value2 = (float?)5.2;
            int counted = dataSet.Count(x => !(x.NullableFloat.HasValue && (x.NullableFloat.Value == value1 || x.NullableFloat.Value == value2)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Float")]
        public void NullableFloatFilterShouldReturnNullableFloat5_1OrNullableFloat5_2In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableFloat:   { matchMode: \"in\", operator: \"and\", value: [5.1,5.2] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            float? value1 = (float?)5.1;
            float? value2 = (float?)5.2;
            int counted = dataSet.Count(x => (x.NullableFloat.HasValue && (x.NullableFloat.Value == value1 || x.NullableFloat.Value == value2)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.2, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")] 
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnLessThan_10_3()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.3, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnLessOrEquals_10_3()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.3, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableDouble <= (double?)10.3);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnGreaterThan_10_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.2, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnGreaterOrEquals_10_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.2, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableDouble >= (double?)10.2);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }

        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnTestDataWithoutNullableFloat10_2()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble: { value: 10.2, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => x.NullableDouble.HasValue && x.NullableDouble.Value == 10.2);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnNullableDouble10_2OrNullableDouble10_3()

        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble:  [ { matchMode: \"equals\", operator: \"or\", value: 10.2 }, { matchMode: \"equals\", operator: \"or\", value: 10.3 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableDouble.HasValue && (x.NullableDouble.Value == 10.2 || x.NullableDouble.Value == 10.3));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturTestDataWithoutNullableDouble10_2AndNullableDouble10_3()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble:  [ { matchMode: \"notEquals\", operator: \"and\", value: 10.2 }, { matchMode: \"notEquals\", operator: \"and\", value: 10.3 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => !(x.NullableDouble.HasValue && (x.NullableDouble.Value == 10.2 || x.NullableDouble.Value == 10.3)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Double")]
        public void NullableDoubleFilterShouldReturnNullableDouble10_2OrNullableDouble10_3In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDouble:   { matchMode: \"in\", operator: \"and\", value: [10.2,10.3] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            int counted = dataSet.Count(x => (x.NullableDouble.HasValue && (x.NullableDouble.Value == 10.2 || x.NullableDouble.Value == 10.3)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")] 
        public void NullableDecimalFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.5, matchMode: \"equals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")]
        public void NullableDecimalFilterShouldReturnLessThan_22_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.6, matchMode: \"lt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")]
        public void NullableDecimalFilterShouldReturnLessOrEquals_22_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.6, matchMode: \"lte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableDecimal <= (decimal?)22.6);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")]
        public void NullableDecimalFilterShouldReturnGreaterThan_22_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.5, matchMode: \"gt\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")]
        public void NullableDecimalFilterShouldReturnGreaterOrEquals_22_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.5, matchMode: \"gte\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int counted = dataSet.Count(x => x.NullableDecimal >= (decimal?)22.5);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")] 
        public void NullableDecimalFilterShouldReturnTestDataWithoutNullableDecimal22_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal: { value: 22.5, matchMode: \"notEquals\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            decimal? value = (decimal?)22.5;
            int counted = dataSet.Count(x => x.NullableDecimal.HasValue && x.NullableDecimal.Value == value);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")] 
        public void NullableDecimalFilterShouldReturnNullableDecimal22_5OrNullableDecimal22_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal:  [ { matchMode: \"equals\", operator: \"or\", value: 22.5 }, { matchMode: \"equals\", operator: \"or\", value: 22.6 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            decimal? value1 = (decimal ?)22.5;
            decimal? value2 = (decimal ?)22.6;
            int counted = dataSet.Count(x => x.NullableDecimal.HasValue && (x.NullableDecimal.Value == value1 || x.NullableDecimal.Value == value2));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")] 
        public void NullableDecimalFilterShouldReturTestDataWithoutNullableDecimal22_5AndNullableDecimal22_6()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal:  [ { matchMode: \"notEquals\", operator: \"and\", value: 22.5 }, { matchMode: \"notEquals\", operator: \"and\", value: 22.6 } ] } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            decimal? value1 = (decimal?)22.5;
            decimal? value2 = (decimal?)22.6;
            int counted = dataSet.Count(x => !(x.NullableDecimal.HasValue && (x.NullableDecimal.Value == value1 || x.NullableDecimal.Value == value2)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "Decimal")] 
        public void NullableDecimalFilterShouldReturnNullableDecimal22_5OrNullableDecimal22_6In()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { nullableDecimal:   { matchMode: \"in\", operator: \"and\", value: [22.5,22.6] } } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            decimal? value1 = (decimal?)22.5;
            decimal? value2 = (decimal?)22.6;
            int counted = dataSet.Count(x => (x.NullableDecimal.HasValue && (x.NullableDecimal.Value == value1 || x.NullableDecimal.Value == value2)));
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "DateTime")]
        public void NullableDateTimeFilterShouldReturnSingleElement()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime2: { value: \"2021-11-05T00:00:00.000Z\", matchMode: \"dateIs\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Single(dataSet);
        }
        [Trait("Category", "Nullable")]
        [Fact]
        [Trait("Type", "DateTime")]
        public void NullableDateTimeFilterShouldReturnTestDataWithoutDateTime2_2021_11_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime2: { value: \"2021-11-05T00:00:00.000Z\", matchMode: \"dateIsNot\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            DateTime dateTime = new DateTime(2021, 11, 5);
            int counted = dataSet.Count(x => x.DateTime2.HasValue && x.DateTime2.Value == dateTime);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal(initialCount - counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "DateTime")]
        public void NullableDateTimeFilterShouldReturnTestDataBeforeDateTime2_2021_11_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime2: { value: \"2021-11-05T00:00:00.000Z\", matchMode: \"dateBefore\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            DateTime dateTime = new DateTime(2021, 11, 5);
            int counted = dataSet.Count(x => x.DateTime2.HasValue && x.DateTime2.Value < dateTime);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal( counted, totalRecord);
        }
        [Fact]
        [Trait("Category", "Nullable")]
        [Trait("Type", "DateTime")]
        public void NullableDateTimeFilterShouldReturnTestDataAfterDateTime2_2021_11_5()
        {
            var filter = GenerateFilterTableFromJson("{ filters: { dateTime2: { value: \"2021-11-05T00:00:00.000Z\", matchMode: \"dateAfter\" }  } , first: 0, globalFilter: null, multiSortMeta: undefined, rows: 10,sortOrder: -1 }");
            var dataSet = GenerateMockTestData();
            int initialCount = dataSet.Count();
            DateTime dateTime = new DateTime(2021, 11, 5);
            int counted = dataSet.Count(x => x.DateTime2.HasValue && x.DateTime2.Value > dateTime);
            dataSet = dataSet.PrimengTableFilter(filter, out var totalRecord);
            Assert.Equal( counted, totalRecord);
        }
    }
}
