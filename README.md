# DynamicQueryable
Construct Linq queries using strings.

[![Build and Test](https://github.com/umutozel/DynamicQueryable/actions/workflows/build.yml/badge.svg)](https://github.com/umutozel/DynamicQueryable/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/umutozel/DynamicQueryable/graph/badge.svg?token=5A9hHTDVFc)](https://codecov.io/gh/umutozel/DynamicQueryable)
[![NuGet Badge](https://img.shields.io/nuget/v/DynamicQueryable.svg)](https://www.nuget.org/packages/DynamicQueryable/)
![NuGet Downloads](https://img.shields.io/nuget/dt/DynamicQueryable.svg)
[![GitHub issues](https://img.shields.io/github/issues/umutozel/DynamicQueryable.svg)](https://github.com/umutozel/DynamicQueryable/issues)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/umutozel/DynamicQueryable/master/LICENSE)

[![GitHub stars](https://img.shields.io/github/stars/umutozel/DynamicQueryable.svg?style=social&label=Star)](https://github.com/umutozel/DynamicQueryable)
[![GitHub forks](https://img.shields.io/github/forks/umutozel/DynamicQueryable.svg?style=social&label=Fork)](https://github.com/umutozel/DynamicQueryable)

Play with **jupyter notebook readme** on:

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/umutozel/DynamicQueryable/master)

# Installation

#### Package Manager
```
Install-Package DynamicQueryable
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

# License
DynamicQueryable is under the [MIT License](LICENSE).

<a href="https://www.jetbrains.com/"><img src="jetbrains.png" alt="drawing" width="150"/></a> Thanks to [JetBrains](https://www.jetbrains.com/) for providing me with free licenses to their great tools.
