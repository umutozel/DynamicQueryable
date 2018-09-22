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
            var count = _query.Where(o => o.Id >= 0 || o.Price % 2 == 0).Count();
            var dynCount = _query.Where("Id >= 0 || Price%2 == 0").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void ShouldHandleSelect() {
            var order = _query.Select(o => new { Id = o.Id, OrderDate = o.OrderDate }).First();
            var dynOrder = _query.Select("o => new { Id = o.Id, OrderDate = o.OrderDate }").First();

            Assert.Equal(order.OrderDate, order.OrderDate);
        }

        [Fact]
        public void ShouldHandleSelectMany() {
            var count = _query.SelectMany(o => o.Lines).Count();
            var dynCount = _query.SelectMany("o => o.Lines").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void ShouldHandleOrderBy() {
            var order = _query.OrderBy(o => o.Id).First();
            var dynOrder = _query.OrderBy("o => o.Id").First();

            Assert.Equal(order.Id, dynOrder.Id);
        }
    }
}
