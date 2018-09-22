using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Giver;
using Xunit;

namespace DynamicQueryable.Tests {
    using Fixture;

    public class ExpressionTests {
        private readonly IQueryable<Order> _query;

        public ExpressionTests() {
            var orders = Give<Order>
                .ToMe(o => o.Lines = Give<OrderLine>
                    .ToMe(od => od.Product = Give<Product>
                        .ToMe(p => p.Supplier = Give<Company>.Single())
                    ).Now(5)
                ).Now(20);

            orders[3].Id = 5;
            _query = orders.AsQueryable();
        }

        [Fact]
        public void ShouldHandleWhere() {
            var orders = _query.Where(o => o.Id >= 0 || o.Price % 2 == 0).ToList();
            var dynOrders1 = _query.Where("Id >= 0 || Price%2 == 0").ToList();
            var dynOrders2 = ((IQueryable)_query).Where("Id >= 0 || Price%2 == 0").ToList();

            Assert.Equal(orders, dynOrders1);
            Assert.Equal(orders, dynOrders2);
        }

        [Fact]
        public void ShouldHandleSelect() {
            var order = _query.Select(o => new { Id = o.Id, OrderDate = o.OrderDate }).First();
            var dynOrder = (dynamic)_query.Select("o => new { Id = o.Id, OrderDate = o.OrderDate }").First();

            Assert.Equal(order.OrderDate, dynOrder.OrderDate);
        }

        [Fact]
        public void ShouldHandleSelectMany() {
            var lines = _query.SelectMany(o => o.Lines).ToList();
            var dynLines1 = _query.SelectMany("o => o.Lines").ToList();
            var dynLines2 = ((IQueryable)_query).SelectMany("o => o.Lines").ToList();

            Assert.Equal(lines, dynLines1);
            Assert.Equal(lines, dynLines2);
        }

        [Fact]
        public void ShouldHandleOrderBy() {
            var order = _query.OrderBy(o => o.Id).First();
            var dynOrder1 = _query.OrderBy("o => o.Id").First();
            var dynOrder2 = ((IQueryable)_query).OrderBy("o => o.Id").First();

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
    }
}
