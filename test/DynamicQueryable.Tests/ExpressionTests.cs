using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Jokenizer.Net;
using Xunit;

namespace DynamicQueryable.Tests;

using Fixture;
using Dyn = System.Linq.Dynamic.DynamicQueryable;

public class ExpressionTests {
    private readonly IQueryable<Order> _query;
    private readonly double _avgId;

    public ExpressionTests() {
        var orders = CreateOrders();
        _avgId = orders.Average(o => o.Id);
        _query = orders.AsQueryable();
    }

    private static List<Order> CreateOrders() {
        var random = new Random();

        var companies = Enumerable.Range(1, 10)
            .Select(i => new Company {
                Id = i,
                CompanyName = $"Company {i}",
                Phone = $"(555) {random.Next(100, 999)}-{random.Next(1000, 9999)}"
            })
            .ToList();

        var products = Enumerable.Range(1, 50)
            .Select(i => new Product {
                Id = i,
                Name = $"Product {i}",
                Supplier = companies[random.Next(companies.Count)]
            })
            .ToList();

        var orders = Enumerable.Range(1, 20)
            .Select(orderId => new Order {
                Id = orderId,
                OrderNo = $"ORD{orderId:0000}",
                OrderDate = DateTime.Today.AddDays(-random.Next(30)),
                Price = null,
                Lines = Enumerable.Range(1, random.Next(3, 15))
                    .Select(lineId => new OrderLine {
                        Id = lineId,
                        OrderId = orderId,
                        Count = random.Next(1, 10),
                        UnitPrice = Math.Round(random.NextDouble() * 100, 2),
                        Product = products[random.Next(products.Count)]
                    })
                    .ToList()
            })
            .ToList();

        foreach (var order in orders) {
            order.Price = order.Lines.Sum(line => (line.UnitPrice ?? 0) * (line.Count ?? 1));
        }

        return orders;
    }

    [Fact]
    public void ShouldHandleAggregate() {
        var query = _query.Select(o => o.Id);

        var sumId1 = query.Aggregate((i1, i2) => i1 + i2);
        var dynSumId1 = query.Aggregate("(i1, i2) => i1 + i2");
        Assert.Equal(sumId1, dynSumId1);

        var sumId2 = query.Aggregate(42, (i1, i2) => i1 + i2);
        var dynSumId2 = query.Aggregate(42, "(i1, i2) => i1 + i2");
        Assert.Equal(sumId2, dynSumId2);

        var sumId3 = query.Aggregate(42, (i1, i2) => i1 + i2, i => i.ToString());
        var dynSumId3 = query.Aggregate(42, "(i1, i2) => i1 + i2", "i => i.ToString()");
        Assert.Equal(sumId3, dynSumId3);

        Assert.Throws<ArgumentNullException>(() => Dyn.Aggregate(null!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(""));
        Assert.Throws<ArgumentNullException>(() => Dyn.Aggregate(null!, ((object?)null)!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(((object?)null)!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(42, ""));
        Assert.Throws<ArgumentNullException>(() => Dyn.Aggregate(null!, ((object?)null)!, "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(((object?)null)!, "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(42, "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Aggregate(42, "Id", ""));
    }

    [Fact]
    public void ShouldHandleAll() {
        var all = _query.All(o => o.Id != 42);
        var dynAll1 = _query.All("o => o.Id != @0", 42);
        var dynAll2 = _query.All("o => o.Id != Meaning", new Dictionary<string, object?> { { "Meaning", 42 } });

        Assert.Equal(all, dynAll1);
        Assert.Equal(all, dynAll2);
    }

    [Fact]
    public void ShouldHandleAny() {
        var order = _query.Any(o => o.Id < _avgId);
        var dynOrder1 = _query.Any("o => o.Id < @0", _avgId);
        var dynOrder2 = _query.Any("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder3 = ((IQueryable)_query).Any();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(_query.Any(), dynOrder3);
    }

    [Fact]
    public void ShouldHandleAverage() {
        var avg = _query.Average(o => o.Price);
        var dynAvg = _query.Average("o => o.Price");

        Assert.Equal(avg, dynAvg);
    }

    [Fact]
    public void ShouldHandleConcat() {
        var orders1 = _query.Skip(1).Take(4).AsQueryable();
        var orders2 = _query.Take(2).ToList();
        var concat = orders1.Concat(orders2).ToList();
        var dynConcat = ((IQueryable)orders1).Concat(orders2).Cast<Order>().ToList();

        Assert.Equal(concat, dynConcat);
    }

    [Fact]
    public void ShouldHandleContains() {
        var order = _query.Last();

        Assert.True(((IQueryable)_query).Contains(order));
    }

    [Fact]
    public void ShouldHandleCount() {
        var order = _query.Count(o => o.Id < _avgId);
        var dynOrder1 = _query.Count("o => o.Id < @0", _avgId);
        var dynOrder2 = _query.Count("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder3 = ((IQueryable)_query).Count();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(_query.Count(), dynOrder3);
    }

    [Fact]
    public void ShouldHandleDefaultIfEmpty() {
        var query = new List<Order>().AsQueryable();
        Assert.Equal(query.DefaultIfEmpty(), ((IQueryable)query).DefaultIfEmpty());
        var order = new Order();
        Assert.Equal(query.DefaultIfEmpty(order), ((IQueryable)query).DefaultIfEmpty(order));
    }

    [Fact]
    public void ShouldHandleDistinct() {
        var orders = new List<Order> {
            new() { Id = 1, Price = 1 },
            new() { Id = 2, Price = 1 },
            new() { Id = 3, Price = 2 }
        };
        orders.Add(orders[1]);

        var dynDistinct = ((IQueryable)orders.AsQueryable()).Distinct().Cast<Order>();
        Assert.Equal(orders.Distinct(), dynDistinct);
    }

    [Fact]
    public void ShouldHandleElementAt() {
        var order1 = _query.ElementAt(4);
        var dynOrder1 = ((IQueryable)_query).ElementAt(4);

        Assert.Equal(order1, dynOrder1);
        Assert.Null(_query.ElementAtOrDefault(42));
    }

    [Fact]
    public void ShouldHandleElementAtOrDefault() {
        var order = _query.ElementAtOrDefault(4);
        var dynOrder = ((IQueryable)_query).ElementAtOrDefault(4);

        Assert.Equal(order, dynOrder);
        Assert.Null(((IQueryable)_query).ElementAtOrDefault(42));
    }

    [Fact]
    public void ShouldHandleExcept() {
        var orders1 = _query.Take(4).AsQueryable();
        var orders2 = _query.Take(2).ToList();
        var except = orders1.Except(orders2).ToList();
        var dynExcept = ((IQueryable)orders1).Except(orders2).Cast<Order>().ToList();

        Assert.Equal(except, dynExcept);
    }

    [Fact]
    public void ShouldHandleFirst() {
        var order = _query.First(o => o.Id > _avgId);
        var dynOrder1 = _query.First("o => o.Id > @0", _avgId);
        var dynOrder2 = _query.First("o => o.Id > AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder3 = ((IQueryable)_query).First("Id > @0", _avgId);
        var dynOrder4 = ((IQueryable)_query).First();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(order, dynOrder3);
        Assert.Equal(_query.First(), dynOrder4);

        Assert.Throws<InvalidOperationException>(() => _query.Take(0).First("Id == 1"));
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).First());
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).First("Id == 1"));
        Assert.Throws<ArgumentNullException>(() => Dyn.First(null!, "Id > 1"));
    }

    [Fact]
    public void ShouldHandleFirstOrDefault() {
        var order = _query.FirstOrDefault(o => o.Id > _avgId);
        var dynOrder1 = _query.FirstOrDefault("o => o.Id > AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder2 = ((IQueryable)_query).FirstOrDefault("Id > @0", _avgId);
        var dynOrder3 = ((IQueryable)_query).FirstOrDefault();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(_query.FirstOrDefault(), dynOrder3);

        Assert.Null(_query.Take(0).FirstOrDefault("Id == 1"));
        Assert.Null(((IQueryable)_query.Take(0)).FirstOrDefault());
        Assert.Null(((IQueryable)_query.Take(0)).FirstOrDefault("Id == 1"));
    }

    [Fact]
    public void ShouldHandleGroupBy() {
        var lines = _query.SelectMany(o => o.Lines).ToList().AsQueryable();

        var group1 = lines.GroupBy(l => l.OrderId, l => l.Id, (k, lid) => lid.Count()).ToList();
        var dynGroup1 = lines.GroupBy("l => l.OrderId", "l => l.Id", "(k, lid) => lid.Count()").Cast<int>().ToList();

        var group2 = lines.GroupBy(l => l.OrderId, (k, l) => l.Count()).ToList();
        var dynGroup2 = lines.GroupBy("l => l.OrderId", "(k, l) => l.Count()").Cast<int>().ToList();

        var group3 = lines.GroupBy(l => l.OrderId).ToList();
        var dynGroup3 = lines.GroupBy("l => l.OrderId").Cast<IGrouping<int, OrderLine>>().ToList();

        Assert.Equal(group1, dynGroup1);
        Assert.Equal(group2, dynGroup2);
        Assert.Equal(group3, dynGroup3);
        Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy(""));
        Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null!, "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy("", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy("Id", ""));
        Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null!, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy("", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy("Id", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupBy("Id", "Id", ""));
    }

    [Fact]
    public void ShouldHandleGroupJoin() {
        var orders = _query.ToList().AsQueryable();
        var lines = _query.SelectMany(o => o.Lines).ToList().AsQueryable();

        var groupJoin = orders.GroupJoin(lines, o => o.Id, l => l.OrderId, (o, l) => o.Id + l.Max(x => x.Id)).ToList();
        var dynGroupJoin = orders.GroupJoin(lines, "o => o.Id", "l => l.OrderId", "(o, l) => o.Id + l.Max(x => x.Id)").Cast<int>().ToList();

        Assert.Equal(groupJoin, dynGroupJoin);

        Assert.Throws<ArgumentNullException>(() => Dyn.GroupJoin(null!, null!, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupJoin(null!, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupJoin(_query, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupJoin(_query, "Id", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.GroupJoin(_query, "Id", "Id", ""));
    }

    [Fact]
    public void ShouldHandleIntersect() {
        var orders1 = _query.Take(4).AsQueryable();
        var orders2 = _query.Take(2).ToList();
        var intersect = orders1.Intersect(orders2).ToList();
        var dynIntersect = ((IQueryable)orders1).Intersect(orders2).Cast<Order>().ToList();

        Assert.Equal(intersect, dynIntersect);
    }

    [Fact]
    public void ShouldHandleJoin() {
        var orders = _query.ToList().AsQueryable();
        var lines = _query.SelectMany(o => o.Lines).ToList().AsQueryable();

        var join = orders.Join(lines, o => o.Id, l => l.OrderId, (o, l) => o.Id + l.Id).ToList();
        var dynJoin = orders.Join(lines, "o => o.Id", "l => l.OrderId", "(o, l) => o.Id + l.Id").Cast<int>().ToList();

        Assert.Equal(join, dynJoin);

        Assert.Throws<ArgumentNullException>(() => Dyn.Join(null!, null!, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Join(null!, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Join(_query, "", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Join(_query, "Id", "", ""));
        Assert.Throws<ArgumentNullException>(() => _query.Join(_query, "Id", "Id", ""));
    }

    [Fact]
    public void ShouldHandleLast() {
        var order = _query.Last(o => o.Id < _avgId);
        var dynOrder1 = _query.Last("o => o.Id < @0", _avgId);
        var dynOrder2 = _query.Last("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder3 = ((IQueryable)_query).Last("Id < @0", _avgId);
        var dynOrder4 = ((IQueryable)_query).Last();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(order, dynOrder3);
        Assert.Equal(_query.Last(), dynOrder4);

        Assert.Throws<InvalidOperationException>(() => _query.Take(0).Last("Id == 1"));
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).Last());
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).Last("Id == 1"));
    }

    [Fact]
    public void ShouldHandleLastOrDefault() {
        var order = _query.LastOrDefault(o => o.Id < _avgId);
        var dynOrder1 = _query.LastOrDefault("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder2 = ((IQueryable)_query).LastOrDefault("Id < @0", _avgId);
        var dynOrder3 = ((IQueryable)_query).LastOrDefault();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(_query.LastOrDefault(), dynOrder3);

        Assert.Null(_query.Take(0).LastOrDefault("Id == 1"));
        Assert.Null(((IQueryable)_query.Take(0)).LastOrDefault());
        Assert.Null(((IQueryable)_query.Take(0)).LastOrDefault("Id == 1"));
    }

    [Fact]
    public void ShouldHandleLongCount() {
        var order = _query.LongCount(o => o.Id < _avgId);
        var dynOrder1 = _query.LongCount("o => o.Id < @0", _avgId);
        var dynOrder2 = _query.LongCount("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } });
        var dynOrder3 = ((IQueryable)_query).LongCount();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
        Assert.Equal(_query.LongCount(), dynOrder3);
    }

    [Fact]
    public void ShouldHandleMax() {
        var avg = _query.Max(o => o.Price);
        var dynAvg = _query.Max("o => o.Price");

        Assert.Equal(avg, dynAvg);
    }

    [Fact]
    public void ShouldHandleMin() {
        var avg = _query.Min(o => o.Price);
        var dynAvg = _query.Min("o => o.Price");

        Assert.Equal(avg, dynAvg);
    }

    [Fact]
    public void ShouldHandleOrderBy() {
        var order = _query.OrderBy(o => o.Id).First();
        var dynOrder1 = _query.OrderBy("o => o.Id").First();
        var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").Cast<object>().First();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
    }

    [Fact]
    public void ShouldHandleOrderByDescending() {
        var order = _query.OrderByDescending(o => o.Id).First();
        var dynOrder1 = _query.OrderByDescending("o => o.Id").First();
        var dynOrder2 = ((IQueryable)_query).OrderByDescending("o => o.Id").Cast<object>().First();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
    }

    [Fact]
    public void ShouldHandleReverse() {
        Assert.Equal(_query.Reverse(), ((IQueryable)_query).Reverse());
    }

    [Fact]
    public void ShouldHandleSelect() {
        var order = _query.Select(o => new { o.Id, o.OrderDate }).First();
        var dynOrder = _query.Select("o => new { Id = o.Id, OrderDate = o.OrderDate }").Cast<dynamic>().First();

        Assert.Equal(order.OrderDate, dynOrder.OrderDate);
    }

    [Fact]
    public void ShouldHandleSelectMany() {
        var lines = _query.SelectMany(o => o.Lines).ToList();
        var dynLines = _query.SelectMany("o => o.Lines").Cast<OrderLine>().ToList();

        Assert.Equal(lines, dynLines);
        Assert.Throws<ArgumentNullException>(() => Dyn.SelectMany(null!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.SelectMany(""));
    }

    [Fact]
    public void ShouldHandleSequenceEqual() {
        var orders = _query.Skip(4).Take(2).ToList();

        Assert.True(((IQueryable)_query.Skip(4).Take(2)).SequenceEqual(orders));
    }

    [Fact]
    public void ShouldHandleSingle() {
        var orders = new List<Order> {
            new() { Id = 1, Price = 1 },
            new() { Id = 2, Price = 1 },
            new() { Id = 3, Price = 2 }
        };
        var query = orders.AsQueryable();

        var dynOrder1 = query.Single("o => o.Id > @0", 2);
        var dynOrder2 = ((IQueryable)query).Single("Id > SearchId", new Dictionary<string, object?> { { "SearchId", 2 } });
        var dynOrder3 = ((IQueryable)query.Take(1)).Single();

        Assert.Equal(orders[2], dynOrder1);
        Assert.Equal(dynOrder1, dynOrder2);
        Assert.Equal(orders[0], dynOrder3);

        Assert.Throws<InvalidOperationException>(() => query.Single("o => o.Id > 1"));
        Assert.Throws<InvalidOperationException>(() => query.Single("o => o.Id > 3"));
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)query.Take(0)).Single());
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)query).Single("o => o.Id > 1"));
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)query).Single("o => o.Id > 3"));
    }

    [Fact]
    public void ShouldHandleSingleOrDefault() {
        var orders = new List<Order> {
            new() { Id = 1, Price = 1 },
            new() { Id = 2, Price = 1 },
            new() { Id = 3, Price = 2 }
        };
        var query = orders.AsQueryable();

        var dynOrder1 = query.SingleOrDefault("o => o.Id > @0", 2);
        var dynOrder2 = ((IQueryable)query).SingleOrDefault("Id > SearchId", new Dictionary<string, object?> { { "SearchId", 2 } });
        var dynOrder3 = ((IQueryable)query.Take(1)).SingleOrDefault();

        Assert.Equal(orders[2], dynOrder1);
        Assert.Equal(dynOrder1, dynOrder2);
        Assert.Equal(orders[0], dynOrder3);

        Assert.Null(query.SingleOrDefault("o => o.Id > 3"));
        Assert.Null(((IQueryable)query.Take(0)).SingleOrDefault());
        Assert.Null(((IQueryable)query).SingleOrDefault("o => o.Id > 3"));

        Assert.Throws<InvalidOperationException>(() => query.SingleOrDefault("o => o.Id > 1"));
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)query).SingleOrDefault());
        Assert.Throws<InvalidOperationException>(() => ((IQueryable)query).SingleOrDefault("o => o.Id > 1"));
    }

    [Fact]
    public void ShouldHandleSkip() {
        var orders = _query.Skip(3);
        var dynOrders = ((IQueryable)_query).Skip(3);

        Assert.Equal(orders, dynOrders);
    }

    [Fact]
    public void ShouldHandleSkipWhile() {
        var orders = _query.SkipWhile(o => o.Id > _avgId).ToList();
        var dynOrders1 = _query.SkipWhile("o => o.Id > @0", _avgId).ToList();
        var dynOrders2 = _query.SkipWhile("o => o.Id > AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } }).ToList();
        var dynOrders3 = ((IQueryable)_query).SkipWhile("Id > @0", _avgId).Cast<Order>().ToList();

        Assert.Equal(orders, dynOrders1);
        Assert.Equal(orders, dynOrders2);
        Assert.Equal(orders, dynOrders3);
    }

    [Fact]
    public void ShouldHandleSum() {
        var avg = _query.Sum(o => o.Price);
        var dynAvg = _query.Sum("o => o.Price");

        Assert.Equal(avg, dynAvg);
    }

    [Fact]
    public void ShouldHandleTake() {
        var orders = _query.Take(3);
        var dynOrders = ((IQueryable)_query).Take(3);

        Assert.Equal(orders, dynOrders);
        Assert.Throws<ArgumentNullException>(() => Dyn.Take(null!, 1));
    }

    [Fact]
    public void ShouldHandleTakeWhile() {
        var orders = _query.TakeWhile(o => o.Id < _avgId).ToList();
        var dynOrders1 = _query.TakeWhile("o => o.Id < @0", _avgId).ToList();
        var dynOrders2 = _query.TakeWhile("o => o.Id < AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } }).ToList();
        var dynOrders3 = ((IQueryable)_query).TakeWhile("Id < @0", _avgId).Cast<Order>().ToList();

        Assert.Equal(orders, dynOrders1);
        Assert.Equal(orders, dynOrders2);
        Assert.Equal(orders, dynOrders3);
    }

    [Fact]
    public void ShouldHandleThenBy() {
        var order = _query.OrderBy(o => o.Id).ThenBy(o => o.Price).First();
        var dynOrder1 = _query.OrderBy(o => o.Id).ThenBy("o => o.Price").First();
        var dynOrder2 = ((IQueryable)_query.OrderBy(o => o.Id)).ThenBy("o => o.Price").Cast<object>().First();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
    }

    [Fact]
    public void ShouldHandleThenByDescending() {
        var order = _query.OrderBy(o => o.Id).ThenByDescending(o => o.Price).First();
        var dynOrder1 = _query.OrderBy(o => o.Id).ThenByDescending("o => o.Price").First();
        var dynOrder2 = ((IQueryable)_query.OrderBy(o => o.Id)).ThenByDescending("o => o.Price").Cast<object>().First();

        Assert.Equal(order, dynOrder1);
        Assert.Equal(order, dynOrder2);
    }

    [Fact]
    public void ShouldHandleUnion() {
        var orders1 = _query.Skip(1).Take(4).AsQueryable();
        var orders2 = _query.Take(2).ToList();
        var union = orders1.Union(orders2).ToList();
        var dynUnion = ((IQueryable)orders1).Union(orders2).Cast<Order>().ToList();

        Assert.Equal(union, dynUnion);
    }

    [Fact]
    public void ShouldHandleWhere() {
        var orders = _query.Where(o => o.Id > _avgId).ToList();
        var dynOrders1 = _query.Where("o => o.Id > @0", _avgId).ToList();
        var dynOrders2 = _query.Where("o => o.Id > AvgId", new Dictionary<string, object?> { { "AvgId", _avgId } }).ToList();
        var dynOrders3 = ((IQueryable)_query).Where("Id > @0", _avgId).Cast<Order>().ToList();

        Assert.Equal(orders, dynOrders1);
        Assert.Equal(orders, dynOrders2);
        Assert.Equal(orders, dynOrders3);
        Assert.Throws<ArgumentNullException>(() => _query.Where(""));
    }

    [Fact]
    public void ShouldHandleZip() {
        var lineCounts = _query.Select(o => o.Lines.Count).ToList();

        var zip = _query.Zip(lineCounts, (o, l) => o.Id + l).ToList();
        var dynZip = Dyn.Zip(_query, lineCounts, "(o, l) => o.Id + l").Cast<int>().ToList();

        Assert.Equal(zip, dynZip);

        Assert.Throws<ArgumentNullException>(() => Dyn.Zip<int>(null!, null!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.Zip<int>(null!, ""));
        Assert.Throws<ArgumentNullException>(() => _query.Zip<int>([], ""));
    }

    [Fact]
    public void ShouldHandleEnumerableParameter() {
        var source = new[] { 1, 2, 3, 4, 5 };
        var sample = new[] { 2, 4 };
        var result = source.AsQueryable().Where($"i => @0.Contains(i)", sample).ToList();

        Assert.Equal([2, 4], result);
    }

    [Fact]
    public void ShouldHandleBooleanOperatorGrouping() {
        var list = new List<Entity> {
            new() {
                IsActive = true,
                IsSomething = 1
            },
            new() {
                IsActive = false,
                IsSomething = 1
            },
            new() {
                IsActive = true,
                IsSomething = 2
            },
            new() {
                IsActive = false,
                IsSomething = 2
            },
        }.AsQueryable();

        var linqResult = list.Where(o => (o.IsActive && o.IsSomething == 1) || (o.IsActive == false && o.IsSomething == 2)).ToArray();
        var dynamicResult = list.Where("o => (o.IsActive && o.IsSomething == 1) || (o.IsActive == false && o.IsSomething == 2)").ToArray();

        Assert.Equal(linqResult, dynamicResult);
    }

    [Fact]
    public void NullableTests() {
        const string searchText = "1";
        var data = new List<Person> {
            new() { Name = "proof search 1 me" },
            new() { Name = "This has age 1", Age = 1 },
            new() { Name = "This has non null number 1", Number = 1 },
            new() { Name = "This address has zip 1", Address = new Address { Zip = 1 } },
            new() { Name = "This address has non null number 1", Address = new Address { Number = 1 } }
        }.AsQueryable();
        var parameters = new Dictionary<string, object?> { { "searchText", searchText } };

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var r1 = data.Where(i => i.Name != null && i.Name.Contains(searchText)).ToList();
        var d1 = data.Where("i => i.Name != null && i.Name.Contains(searchText)", parameters).ToList();
        Assert.Equal(r1, d1);

        var r2 = data.OrderByDescending(i => i.Name).ToList();
        var d2 = data.OrderByDescending("i => i.Name").ToList();
        Assert.Equal(r2, d2);

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        var r3 = data.Where(i => i.Age != null && i.Age.ToString().Contains(searchText)).ToList();
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        var func = Evaluator.ToFunc<bool>(
            "i.Age != null && i.Age.ToString().Contains(searchText)",
            new Dictionary<string, object?> {{ "searchText", searchText }, { "i", data.First() }}
        );
        var v = func();
        var d3 = data.Where("i => i.Age != null && i.Age.ToString().Contains(searchText)", parameters).ToList();
        Assert.Equal(r3, d3);

        var r4 = data.Where(i => i.Number != null && i.Number!.ToString()!.Contains(searchText)).ToList();
        var d4 = data.Where("i => i.Number != null && i.Number.ToString().Contains(searchText)", parameters).ToList();
        Assert.Equal(r4, d4);

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        var r5 = data.Where(i => i.Address != null! && i.Address.Zip != null && i.Address.Zip.ToString().Contains(searchText)).ToList();
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        var d5 = data.Where("i => i.Address != null && i.Address.Zip != null && i.Address.Zip.ToString().Contains(searchText)", parameters).ToList();
        Assert.Equal(r5, d5);

        var r6 = data.Where(i => i.Address != null! && i.Address.Number != null && i.Address.Number!.ToString()!.Contains(searchText)).ToList();
        var d6 = data.Where("i => i.Address != null && i.Address.Number != null && i.Address.Number.ToString().Contains(searchText)", parameters).ToList();
        Assert.Equal(r6, d6);
    }
}
