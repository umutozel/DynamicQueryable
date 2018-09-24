using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Giver;
using Xunit;

namespace DynamicQueryable.Tests {
    using Fixture;
    using Dyn = System.Linq.Dynamic.DynamicQueryable;

    public class ExpressionTests {
        private readonly IQueryable<Order> _query;
        private readonly double AvgId;

        public ExpressionTests() {
            var orders = Give<Order>
                .ToMe(o => o.Lines = Give<OrderLine>
                    .ToMe(od => {
                        od.OrderId = o.Id;
                        od.Order = o;
                        od.Product = Give<Product>.ToMe(p => p.Supplier = Give<Company>.Single());
                    }).Now(new Random().Next(3, 15))
                ).Now(20);
            AvgId = orders.Average(o => o.Id);

            _query = orders.AsQueryable();
        }

        [Fact]
        public void ShouldThrowForNullArgs() {
            Assert.Throws<ArgumentNullException>(() => Dyn.Where(_query, ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.First(null, "Id > 1"));
            Assert.Throws<ArgumentNullException>(() => Dyn.Take(null, 1));

            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null, ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null, "", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, "", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, "Id", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(null, "", "", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, "", "", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, "Id", "", ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.GroupBy(_query, "Id", "Id", ""));

            Assert.Throws<ArgumentNullException>(() => Dyn.SelectMany(null, ""));
            Assert.Throws<ArgumentNullException>(() => Dyn.SelectMany(_query, ""));
        }

        [Fact]
        public void ShouldHandleWhere() {
            var orders = _query.Where(o => o.Id > AvgId).ToList();
            var dynOrders1 = _query.Where("o => o.Id > @0", AvgId).ToList();
            var dynOrders2 = _query.Where("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders3 = ((IQueryable)_query).Where("Id > @0", AvgId).Cast<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
            Assert.Equal(orders, dynOrders3);
        }

        [Fact]
        public void ShouldHandleSelect() {
            var order = _query.Select(o => new { Id = o.Id, OrderDate = o.OrderDate }).First();
            var dynOrder = _query.Select("o => new { Id = o.Id, OrderDate = o.OrderDate }").Cast<dynamic>().First();

            Assert.Equal(order.OrderDate, dynOrder.OrderDate);
        }

        [Fact]
        public void ShouldHandleSelectMany() {
            var lines = _query.SelectMany(o => o.Lines).ToList();
            var dynLines1 = _query.SelectMany("o => o.Lines").Cast<OrderLine>().ToList();
            var dynLines2 = ((IQueryable)_query).SelectMany("o => o.Lines").Cast<OrderLine>().ToList();

            Assert.Equal(lines, dynLines1);
            Assert.Equal(lines, dynLines2);
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
        public void ShouldHandleThenBy() {
            var order = _query.OrderBy(o => o.Id).ThenBy(o => o.Price).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").ThenBy("o => o.Price").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").ThenBy("o => o.Price").Cast<object>().First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
        }

        [Fact]
        public void ShouldHandleThenByDescending() {
            var order = _query.OrderBy(o => o.Id).ThenByDescending(o => o.Price).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").ThenByDescending("o => o.Price").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").ThenByDescending("o => o.Price").Cast<object>().First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
        }

        [Fact]
        public void ShouldHandleTake() {
            var orders = _query.Take(3);
            var dynOrders = ((IQueryable)_query).Take(3);

            Assert.Equal(orders, dynOrders);
        }

        [Fact]
        public void ShouldHandleSkip() {
            var orders = _query.Skip(3);
            var dynOrders = ((IQueryable)_query).Skip(3);

            Assert.Equal(orders, dynOrders);
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
        }

        [Fact]
        public void ShouldHandleSkipWhile() {
            var orders = _query.SkipWhile(o => o.Id > AvgId).ToList();
            var dynOrders1 = _query.SkipWhile("o => o.Id > @0", AvgId).ToList();
            var dynOrders2 = _query.SkipWhile("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders3 = ((IQueryable)_query).SkipWhile("Id > @0", AvgId).Cast<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
        }

        [Fact]
        public void ShouldHandleTakeWhile() {
            var orders = _query.TakeWhile(o => o.Id < AvgId).ToList();
            var dynOrders1 = _query.TakeWhile("o => o.Id < @0", AvgId).ToList();
            var dynOrders2 = _query.TakeWhile("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders3 = ((IQueryable)_query).TakeWhile("Id < @0", AvgId).Cast<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders3);
        }

        [Fact]
        public void ShouldExecuteAverage() {
            var avg = _query.Average(o => o.Price);
            var dynAvg1 = _query.Average("o => o.Price");
            var dynAvg2 = ((IQueryable)_query).Select("o => o.Price").Average();
            var dynAvg3 = _query.Average("o => o.Price");
            var dynAvg4 = ((IQueryable)_query).Average("o => o.Price");

            Assert.Equal(avg, dynAvg1);
            Assert.Equal(avg, dynAvg2);
            Assert.Equal(avg, dynAvg3);
            Assert.Equal(avg, dynAvg4);
        }

        [Fact]
        public void ShouldExecuteSum() {
            var avg = _query.Sum(o => o.Price);
            var dynAvg1 = _query.Sum("o => o.Price");
            var dynAvg2 = ((IQueryable)_query).Select("o => o.Price").Sum();
            var dynAvg3 = _query.Sum("o => o.Price");
            var dynAvg4 = ((IQueryable)_query).Sum("o => o.Price");

            Assert.Equal(avg, dynAvg1);
            Assert.Equal(avg, dynAvg2);
            Assert.Equal(avg, dynAvg3);
            Assert.Equal(avg, dynAvg4);
        }

        [Fact]
        public void ShouldExecuteMax() {
            var avg = _query.Max(o => o.Price);
            var dynAvg1 = _query.Max("o => o.Price");
            var dynAvg2 = ((IQueryable)_query).Select("o => o.Price").Max();
            var dynAvg3 = _query.Max("o => o.Price");
            var dynAvg4 = ((IQueryable)_query).Max("o => o.Price");

            Assert.Equal(avg, dynAvg1);
            Assert.Equal(avg, dynAvg2);
            Assert.Equal(avg, dynAvg3);
            Assert.Equal(avg, dynAvg4);
        }

        [Fact]
        public void ShouldExecuteMin() {
            var avg = _query.Min(o => o.Price);
            var dynAvg1 = _query.Min("o => o.Price");
            var dynAvg2 = ((IQueryable)_query).Select("o => o.Price").Min();
            var dynAvg3 = _query.Min("o => o.Price");
            var dynAvg4 = ((IQueryable)_query).Min("o => o.Price");

            Assert.Equal(avg, dynAvg1);
            Assert.Equal(avg, dynAvg2);
            Assert.Equal(avg, dynAvg3);
            Assert.Equal(avg, dynAvg4);
        }

        [Fact]
        public void ShouldExecuteFirst() {
            var order = _query.First(o => o.Id > AvgId);
            var dynOrder1 = _query.First("o => o.Id > @0", AvgId);
            var dynOrder2 = _query.First("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder3 = ((IQueryable)_query).First("Id > @0", AvgId);
            var dynOrder4 = ((IQueryable)_query).First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(order, dynOrder3);
            Assert.Equal(_query.First(), dynOrder4);

            Assert.Throws<InvalidOperationException>(() => _query.Take(0).First("Id == 1"));
            Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).First());
            Assert.Throws<InvalidOperationException>(() => ((IQueryable)_query.Take(0)).First("Id == 1"));
        }

        [Fact]
        public void ShouldExecuteFirstOrDefault() {
            var order = _query.FirstOrDefault(o => o.Id > AvgId);
            var dynOrder1 = _query.FirstOrDefault("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder2 = ((IQueryable)_query).FirstOrDefault("Id > @0", AvgId);
            var dynOrder3 = ((IQueryable)_query).FirstOrDefault();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(_query.FirstOrDefault(), dynOrder3);

            Assert.Null(_query.Take(0).FirstOrDefault("Id == 1"));
            Assert.Null(((IQueryable)_query.Take(0)).FirstOrDefault());
            Assert.Null(((IQueryable)_query.Take(0)).FirstOrDefault("Id == 1"));
        }

        [Fact]
        public void ShouldExecuteSingle() {
            var orders = new List<Order> {
                new Order { Id = 1, Price = 1 },
                new Order { Id = 2, Price = 1 },
                new Order { Id = 3, Price = 2 }
            };
            var query = orders.AsQueryable();

            var dynOrder1 = query.Single("o => o.Id > @0", 2);
            var dynOrder2 = ((IQueryable)query).Single("Id > SearchId", new Dictionary<string, object> { { "SearchId", 2 } });
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
        public void ShouldExecuteSingleOrDefault() {
            var orders = new List<Order> {
                new Order { Id = 1, Price = 1 },
                new Order { Id = 2, Price = 1 },
                new Order { Id = 3, Price = 2 }
            };
            var query = orders.AsQueryable();

            var dynOrder1 = query.SingleOrDefault("o => o.Id > @0", 2);
            var dynOrder2 = ((IQueryable)query).SingleOrDefault("Id > SearchId", new Dictionary<string, object> { { "SearchId", 2 } });
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
        public void ShouldExecuteLast() {
            var order = _query.Last(o => o.Id < AvgId);
            var dynOrder1 = _query.Last("o => o.Id < @0", AvgId);
            var dynOrder2 = _query.Last("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder3 = ((IQueryable)_query).Last("Id < @0", AvgId);
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
        public void ShouldExecuteLastOrDefault() {
            var order = _query.LastOrDefault(o => o.Id < AvgId);
            var dynOrder1 = _query.LastOrDefault("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder2 = ((IQueryable)_query).LastOrDefault("Id < @0", AvgId);
            var dynOrder3 = ((IQueryable)_query).LastOrDefault();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(_query.LastOrDefault(), dynOrder3);

            Assert.Null(_query.Take(0).LastOrDefault("Id == 1"));
            Assert.Null(((IQueryable)_query.Take(0)).LastOrDefault());
            Assert.Null(((IQueryable)_query.Take(0).LastOrDefault("Id == 1")));
        }

        [Fact]
        public void ShoulHandleElementAt() {
            var order1 = _query.ElementAt(4);
            var dynOrder1 = ((IQueryable)_query).ElementAt(4);

            Assert.Equal(order1, dynOrder1);
            Assert.Null(_query.ElementAtOrDefault(42));
        }

        [Fact]
        public void ShouldExecuteElementAtOrDefault() {
            var order = _query.ElementAtOrDefault(4);
            var dynOrder = ((IQueryable)_query).ElementAtOrDefault(4);

            Assert.Equal(order, dynOrder);
            Assert.Null(((IQueryable)_query).ElementAtOrDefault(42));
        }

        [Fact]
        public void ShouldExecuteCount() {
            var order = _query.Count(o => o.Id < AvgId);
            var dynOrder1 = _query.Count("o => o.Id < @0", AvgId);
            var dynOrder2 = _query.Count("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder3 = ((IQueryable)_query).Count("Id < @0", AvgId);
            var dynOrder4 = ((IQueryable)_query).Count();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(order, dynOrder3);
            Assert.Equal(_query.Count(), dynOrder4);
        }

        [Fact]
        public void ShouldExecuteLongCount() {
            var order = _query.LongCount(o => o.Id < AvgId);
            var dynOrder1 = _query.LongCount("o => o.Id < @0", AvgId);
            var dynOrder2 = _query.LongCount("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder3 = ((IQueryable)_query).LongCount("Id < @0", AvgId);
            var dynOrder4 = ((IQueryable)_query).LongCount();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(order, dynOrder3);
            Assert.Equal(_query.LongCount(), dynOrder4);
        }

        [Fact]
        public void ShouldHandleAll() {
            var all = _query.All(o => o.Id != 42);
            var dynAll1 = _query.All("o => o.Id != @0", 42);
            var dynAll2 = _query.All("o => o.Id != Meaning", new Dictionary<string, object> { { "Meaning", 42 } });

            Assert.Equal(all, dynAll1);
            Assert.Equal(all, dynAll2);
        }

        [Fact]
        public void ShouldExecuteAny() {
            var order = _query.Any(o => o.Id < AvgId);
            var dynOrder1 = _query.Any("o => o.Id < @0", AvgId);
            var dynOrder2 = _query.Any("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } });
            var dynOrder3 = ((IQueryable)_query).Any();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
            Assert.Equal(_query.Any(), dynOrder3);
        }

        [Fact]
        public void ShouldExecuteContains() {
            var order = _query.Last();

            Assert.True(((IQueryable)_query).Contains(order));
        }

        [Fact]
        public void ShouldExecuteSequenceEqual() {
            var orders = _query.Skip(4).Take(2).ToList();

            Assert.True(((IQueryable)_query.Skip(4).Take(2)).SequenceEqual(orders));
        }

        [Fact]
        public void ShouldExecuteDistinct() {
            var orders = new List<Order> {
                new Order { Id = 1, Price = 1 },
                new Order { Id = 2, Price = 1 },
                new Order { Id = 3, Price = 2 }
            };
            orders.Add(orders[1]);

            var dynDistinct = ((IQueryable)orders.AsQueryable()).Distinct().Cast<Order>();
            Assert.Equal(orders.Distinct(), dynDistinct);
        }

        [Fact]
        public void ShouldExecuteReverse() {
            Assert.Equal(_query.Reverse(), ((IQueryable)_query).Reverse());
        }

        [Fact]
        public void ShouldExecuteDefaultIfEmpty() {
            var query = new List<Order>().AsQueryable();
            Assert.Equal(query.DefaultIfEmpty(), ((IQueryable)query).DefaultIfEmpty());
            var order = new Order();
            Assert.Equal(query.DefaultIfEmpty(order), ((IQueryable)query).DefaultIfEmpty(order));
        }

        [Fact]
        public void ShouldExecuteExcept() {
            var orders1 = _query.Take(4).AsQueryable();
            var orders2 = _query.Take(2).ToList();
            var except = orders1.Except(orders2).ToList();
            var dynExcept = ((IQueryable)orders1).Except(orders2).Cast<Order>().ToList();

            Assert.Equal(except, dynExcept);
        }

        [Fact]
        public void ShouldExecuteIntersect() {
            var orders1 = _query.Take(4).AsQueryable();
            var orders2 = _query.Take(2).ToList();
            var intersect = orders1.Intersect(orders2).ToList();
            var dynIntersect = ((IQueryable)orders1).Intersect(orders2).Cast<Order>().ToList();

            Assert.Equal(intersect, dynIntersect);
        }

        [Fact]
        public void ShouldExecuteUnion() {
            var orders1 = _query.Skip(1).Take(4).AsQueryable();
            var orders2 = _query.Take(2).ToList();
            var union = orders1.Union(orders2).ToList();
            var dynUnion = ((IQueryable)orders1).Union(orders2).Cast<Order>().ToList();

            Assert.Equal(union, dynUnion);
        }

        [Fact]
        public void ShouldExecuteConcat() {
            var orders1 = _query.Skip(1).Take(4).AsQueryable();
            var orders2 = _query.Take(2).ToList();
            var concat = orders1.Concat(orders2).ToList();
            var dynConcat = ((IQueryable)orders1).Concat(orders2).Cast<Order>().ToList();

            Assert.Equal(concat, dynConcat);
        }
    }
}
