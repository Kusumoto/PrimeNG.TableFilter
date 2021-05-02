# PrimeNG.TableFilter

[![NuGet version](https://badge.fury.io/nu/PrimeNG.TableFilter.svg)](https://badge.fury.io/nu/PrimeNG.TableFilter)
[![Build Status](https://travis-ci.org/Kusumoto/PrimeNG.TableFilter.svg)](https://travis-ci.org/Kusumoto/PrimeNG.TableFilter)

Helper for use the PrimeNG table load lazy filter in backend use LINQ to Entity

## Breaking change in version 2.0
If you use PrimeNG.TableFilter version 1.x.x you will need to migrate your implementation code. Please delete total record variable  and change to get total record from variable out in extension. You can looking for implementation guide line in unit test.

## .Net Platform Support

- .Net Standard 2.0
- .Net Framework 4.0 more then
- Mono Framework

## Feature

- Handle all command from PrimeNG table filter frontend
- Filter criteria from PrimeNG table filter in Iterators and Entities
- Convert PrimeNG table filter payload to SQL Query (use LINQ to Entities)

## Release Note
[Completely handle PrimeNG table load lazy in ASP.NET use PrimeNG.TableFilter (kusumotolab.com)](https://kusumotolab.com/completely-handle-primeng-table-load-lazy-in-asp-net-use-primeng-tablefilter/)


## Install via Nuget Package Manager

```sh
PM> Install-Package PrimeNG.TableFilter
```

## How to use

1. Handle PrimeNG table filter payload in ASP.Net MVC Controller use "TableFilterModel" in parameter from body

```C#
[HttpPost("[action]")]
        public BaseTableResponseEntity<ClassRoomGridModel> GetClassRoom([FromBody] TableFilterModel tableFilterPayload)
            => _classRoomService.GetClassRoom(tableFilterPayload);
```

2. Implement programming logic for get your data from iterators or entities and use extension method "PrimengTableFilter" in your iterators or entities


```C#
public BaseTableResponseEntity<ClassRoomGridModel> GetClassRoom(TableFilterModel filterPayload)
        {
            var result = _classRoomRepository.Gets()
                .Select(o =>
                    new ClassRoomGridModel
                    {
                        Code = o.Code,
                        Id = o.Id,
                        TotalSeat = o.TotalSeat,
                        BuildingName = o.BuildingName.Description,
                        Name = o.Name,
                        Remark = o.Remark,
                        ActiveFlag = o.ActiveFlag,
                        Type = GenerateClassRoomTypeToText(o.Type)
                    }
                );
            result = result.PrimengTableFilter(filterPayload, out  var totalRecord);
            return MvcHelper.ResponseTableData(totalRecord, result);
        }
```

MIT License

Copyright (c) 2021 Weerayut Hongsa
