# DynamicQueryable
Construct Linq queries using strings.

[![Build status](https://ci.appveyor.com/api/projects/status/odi0k0rsdbkk5mqn?svg=true)](https://ci.appveyor.com/project/umutozel/dynamicqueryable)
[![Coverage Status](https://coveralls.io/repos/github/umutozel/DynamicQueryable/badge.svg?branch=master)](https://coveralls.io/github/umutozel/DynamicQueryable?branch=master)
[![NuGet Badge](https://buildstats.info/nuget/DynamicQueryable)](https://www.nuget.org/packages/DynamicQueryable/)
[![GitHub issues](https://img.shields.io/github/issues/umutozel/DynamicQueryable.svg)](https://github.com/umutozel/DynamicQueryable/issues)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/umutozel/DynamicQueryable/master/LICENSE)

[![GitHub stars](https://img.shields.io/github/stars/umutozel/DynamicQueryable.svg?style=social&label=Star)](https://github.com/umutozel/DynamicQueryable)
[![GitHub forks](https://img.shields.io/github/forks/umutozel/DynamicQueryable.svg?style=social&label=Fork)](https://github.com/umutozel/DynamicQueryable)

# Installation

#### Package Manager
```
dotnet add package DynamicQueryable
```
#### .Net CLI
```
dotnet add package DynamicQueryable
```

# Getting Started

Start with adding System.Linq.Dynamic namespace to usings.

```csharp
// you can use inline values
query.Where("o => o.Id > 5").ToList();
// or you can pass ordered values, @0 will be replaced with first argument
query.Where("o => o.Id > @0", 5).ToList();
// or you can use named variables, AvgId will be replaced with value from given dictionary 
query.Where("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
```

# Supported Methods
Aggregate, All, Any, Average, Concat, Contains, Count, 
DefaultIfEmpty, Distinct, Except, ElementAt, ElementAtOrDefault, 
First, FirstOrDefault, GroupBy, GroupJoin, Intersect, Join, 
Last, LastOrDefault, LongCount, Max, Min, OrderBy, OrderByDescending, 
Reverse, Select, SelectMany, SequenceEqual, Single, SingleOrDefault, 
Skip, SkipWhile, Sum, Take, TakeWhile, ThenBy, ThenByDescending, Union, Where, Zip

