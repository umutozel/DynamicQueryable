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
        public void ShouldFilterUsingOr() {
            var count = _query.Where(o => o.Id >= 0 || o.Price % 2 == 0).Count();
            var dynCount = _query.Where("Id >= 0 || Price%2 == 0").Count();

            Assert.Equal(count, dynCount);
        }
    }
}
