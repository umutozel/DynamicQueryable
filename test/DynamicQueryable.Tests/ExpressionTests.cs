using System;
using System.Linq;
using System.Linq.Dynamic;
using Giver;
using Xunit;

namespace DynamicQueryable.Tests {
    using System.Collections.Generic;
    using Fixture;

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
        public void ShouldHandleWhere() {
            var orders = _query.Where(o => o.Id > AvgId).ToList();
            var dynOrders1 = _query.Where("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders2 = ((IQueryable)_query).Where("Id > @0", AvgId).As<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
        }

        [Fact]
        public void ShouldHandleSelect() {
            var order = _query.Select(o => new { Id = o.Id, OrderDate = o.OrderDate }).First();
            var dynOrder = _query.Select("o => new { Id = o.Id, OrderDate = o.OrderDate }").As<dynamic>().First();

            Assert.Equal(order.OrderDate, dynOrder.OrderDate);
        }

        [Fact]
        public void ShouldHandleSelectMany() {
            var lines = _query.SelectMany(o => o.Lines).ToList();
            var dynLines1 = _query.SelectMany("o => o.Lines").As<OrderLine>().ToList();
            var dynLines2 = ((IQueryable)_query).SelectMany("o => o.Lines").As<OrderLine>().ToList();

            Assert.Equal(lines, dynLines1);
            Assert.Equal(lines, dynLines2);
        }

        [Fact]
        public void ShouldHandleOrderBy() {
            var order = _query.OrderBy(o => o.Id).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").As<object>().First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
        }

        [Fact]
        public void ShouldHandleOrderByDescending() {
            var order = _query.OrderByDescending(o => o.Id).First();
            var dynOrder1 = _query.OrderByDescending("o => o.Id").First();
            var dynOrder2 = ((IQueryable)_query).OrderByDescending("o => o.Id").As<object>().First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
        }

        [Fact]
        public void ShouldHandleThenBy() {
            var order = _query.OrderBy(o => o.Id).ThenBy(o => o.Price).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").ThenBy("o => o.Price").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").ThenBy("o => o.Price").As<object>().First();

            Assert.Equal(order, dynOrder1);
            Assert.Equal(order, dynOrder2);
        }

        [Fact]
        public void ShouldHandleThenByDescending() {
            var order = _query.OrderBy(o => o.Id).ThenByDescending(o => o.Price).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").ThenByDescending("o => o.Price").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").ThenByDescending("o => o.Price").As<object>().First();

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
            var dynOrders1 = _query.SkipWhile("o => o.Id > AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders2 = ((IQueryable)_query).SkipWhile("Id > @0", AvgId).As<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
        }

        [Fact]
        public void ShouldHandleTakeWhile() {
            var orders = _query.TakeWhile(o => o.Id < AvgId).ToList();
            var dynOrders1 = _query.TakeWhile("o => o.Id < AvgId", new Dictionary<string, object> { { "AvgId", AvgId } }).ToList();
            var dynOrders2 = ((IQueryable)_query).TakeWhile("Id < @0", AvgId).As<Order>().ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
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
    }
}
