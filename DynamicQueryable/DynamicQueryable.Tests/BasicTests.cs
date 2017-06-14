using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Xunit;
using Giver;
using DynamicQueryable.Tests.Model;

namespace DynamicQueryable.Tests {

    public class BasicTests {
        private readonly List<Order> _orders;

        public BasicTests() {
            _orders = Give<Order>
                .ToMe(o => o.OrderDetails = Give<OrderDetail>
                    .ToMe(od => od.Product = Give<Product>
                        .ToMe(p => p.Supplier = Give<Company>.Single())
                    ).Now(5)
                ).Now(20);

            _orders[3].Id = 5;
        }

        [Fact]
        public void Test_Where() {
            var query = _orders.AsQueryable();
            var dynQuery = query;

            query = query.Where(o => o.Id == 5);
            dynQuery = dynQuery.Where("Id == 5");

            Assert.Equal(query.Count(), dynQuery.Count());
        }
    }
}
