using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Xunit;
using Giver;
using DynamicQueryable.Tests.Model;

namespace DynamicQueryable.Tests {

    public class BasicTests {
        private readonly IQueryable<Order> _query;

        public BasicTests() {
            var orders = Give<Order>
                .ToMe(o => o.OrderDetails = Give<OrderDetail>
                    .ToMe(od => od.Product = Give<Product>
                        .ToMe(p => p.Supplier = Give<Company>.Single())
                    ).Now(5)
                ).Now(20);

            orders[3].Id = 5;
            _query = orders.AsQueryable();
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

            Assert.True(Enumerable.SequenceEqual(list, (IEnumerable<int>) dynList));
        }

        [Fact]
        public void Test_Select_Object() {
            var list = _query.Select(o => new {OrderId = o.Id, No = o.OrderNo}).ToList();
            var dynList = ((IEnumerable<object>) _query.Select("new (Id as OrderId, OrderNo as No)")).ToList();

            var lastItem = list.Last();
            var dynLastItem = (dynamic)dynList.Last();
            Assert.Equal(lastItem.OrderId, dynLastItem.OrderId);
        }

        [Fact]
        public void Test_SelectMany() {
            var list = _query.SelectMany(o => o.OrderDetails);
            var dynList = _query.SelectMany("OrderDetails");

            Assert.True(Enumerable.SequenceEqual(list, (IEnumerable<OrderDetail>) dynList));
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

            Assert.True(Enumerable.SequenceEqual(list, (IEnumerable<Order>) dynList));
        }

        [Fact]
        public void Test_Take() {
            var list = _query.Take(5);
            IQueryable dynQuery = _query;
            var dynList = dynQuery.Take(5);

            Assert.True(Enumerable.SequenceEqual(list, (IEnumerable<Order>)dynList));
        }

        [Fact]
        public void Test_GroupBy() {
            var group = _query.GroupBy(o => o.OrderNo);
            var dynGroup = (IQueryable<IGrouping<string, Order>>)_query.GroupBy("OrderNo", "it");

            Assert.True(Enumerable.SequenceEqual(group.First(), dynGroup.First()));
        }

        [Fact]
        public void Test_SkipWhile() {
            var tenMax = _query.Take(10).Max(o => o.Price);
            var list = _query.SkipWhile(o => o.Price > tenMax);
            var dynList = _query.SkipWhile("Price > @0", tenMax);

            Assert.True(Enumerable.SequenceEqual(list, dynList));
        }

        [Fact]
        public void Test_TakeWhile() {
            var tenMax = _query.Take(10).Max(o => o.Price);
            var list = _query.TakeWhile(o => o.Price < tenMax);
            var dynList = _query.TakeWhile("Price < @0", tenMax);

            Assert.True(Enumerable.SequenceEqual(list, dynList));
        }

        [Fact]
        public void Test_Join() {
            
        }

        [Fact]
        public void Test_Any() {
            var dynQuery = (IQueryable)_query;

            Assert.True(dynQuery.Any());
        }

        [Fact]
        public void Test_All() {
            var minId = _query.Min(o => o.Id);
            var dynQuery = (IQueryable)_query;

            Assert.True(dynQuery.All("Id > @0", minId - 1));
        }

        [Fact]
        public void Test_Count() {
            var dynQuery = (IQueryable)_query;

            Assert.Equal(_query.Count(), dynQuery.Count());
        }
    }
}