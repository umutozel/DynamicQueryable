using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using Xunit;
using Giver;
using DynamicQueryable.Tests.Model;

namespace DynamicQueryable.Tests {
    public class BasicTests {
        private readonly List<Order> _orders;
        private readonly IQueryable<Order> _query;

        public BasicTests() {
            _orders = Give<Order>
                .ToMe(o => o.OrderDetails = Give<OrderDetail>
                    .ToMe(od => od.Product = Give<Product>
                        .ToMe(p => p.Supplier = Give<Company>.Single())
                    ).Now(5)
                ).Now(20);

            _orders[3].Id = 5;
            _query = _orders.AsQueryable();
        }

        [Fact]
        public void Test_Where() {
            Assert.Equal(
                _query.Where(o => o.Id == 5),
                _query.Where("Id == 5")
            );
        }

        [Fact]
        public void Test_Select_Primitive() {
            var list = _query.Select(o => o.Id);
            var dynList = _query.Select("Id");

            Assert.True(Enumerable.SequenceEqual(list, dynList as IEnumerable<int>));
        }

        [Fact]
        public void Test_Select_Object() {
            var list = _query.Select(o => new {OrderId = o.Id, No = o.OrderNo}).ToList();
            var dynList = ((IEnumerable<object>) _query.Select("new (Id as OrderId, OrderNo as No)")).ToList();

            var lastItem = list.Last();
            var dynLastItem = dynList.Last();
            Assert.Equal(lastItem.OrderId, dynLastItem.GetType().GetProperty("OrderId").GetValue(dynLastItem));
        }

        [Fact]
        public void Test_SelectMany() {
            var list = _query.SelectMany(o => o.OrderDetails);
            var dynList = _query.SelectMany("OrderDetails");

            Assert.True(Enumerable.SequenceEqual(list, dynList as IEnumerable<OrderDetail>));
        }

        [Fact]
        public void Test_OrderBy() {
            var list = _query.OrderBy(o => o.Id);
            var dynList = _query.OrderBy("Id");

            Assert.True(Enumerable.SequenceEqual(list, dynList));
        }

        [Fact]
        public void Test_Skip() {
            var list = _query.Skip(5);
            IQueryable dynQuery = _query;
            var dynList = dynQuery.Skip(5);

            Assert.True(Enumerable.SequenceEqual(list, dynList as IEnumerable<Order>));
        }

        [Fact]
        public void Test_Take() {
            var list = _query.Take(5);
            IQueryable dynQuery = _query;
            var dynList = dynQuery.Take(5);

            Assert.True(Enumerable.SequenceEqual(list, dynList as IEnumerable<Order>));
        }

        [Fact]
        public void Test_GroupBy() {
            
        }

        [Fact]
        public void Test_SkipWhile() {

        }

        [Fact]
        public void Test_TakeWhile() {
            
        }

        [Fact]
        public void Test_Join() {
            
        }

        [Fact]
        public void Test_Any() {
            
        }

        [Fact]
        public void Test_All() {
            
        }

        [Fact]
        public void Test_Count() {
            
        }
    }
}