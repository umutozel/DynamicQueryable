using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Xunit;
using Giver;
using DynamicQueryable.Tests.Model;

namespace DynamicQueryable.Tests {

    public class ExpressionTests {
        private readonly IQueryable<Order> _query;

        public ExpressionTests() {
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
        public void Test_Ternary() {
            var sum = _query.Select(o => o.Id > 0 ? 1 : 0).Sum();
            var dynSum = _query.Select("Id > 0 ? 1 : 0").Cast<int>().Sum();

            Assert.Equal(sum, dynSum);
        }

        [Fact]
        public void Test_Or() {
            var count = _query.Where(o => o.Id >= 0 || o.Price%2 == 0).Count();
            var dynCount = _query.Where("Id >= 0 || Price%2 == 0").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void Test_And() {
            var count = _query.Where(o => o.Id <= 0 && o.Price % 2 < 1).Count();
            var dynCount = _query.Where("Id <= 0 && Price%2 < 1").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void Test_NotEqual() {
            var count = _query.Where(o => o.Id != 0 && o.Price != 0).Count();
            var dynCount = _query.Where("Id != 0 && Price <> 0").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void Test_Additive() {
            var count = _query.Where(o => o.Id + 10 - 50.0 > 0).Count();
            var dynCount = _query.Where("Id + 10 - 50.0 > 0").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void Test_String_Concat() {
            var list = _query.Select(o => "No: " + o.OrderNo);
            var dynList = _query.Select("\"No: \" & OrderNo").Cast<string>();

            Assert.True(Enumerable.SequenceEqual(list, dynList));
        }

        [Fact]
        public void Test_Multiplicative() {
            var list = _query.Select(o => (o.Price * 5) / (o.Id % 3));
            var dynList = _query.Select("(Price * 5) / (Id % 3)").Cast<double?>();

            Assert.True(Enumerable.SequenceEqual(list, dynList));
        }


        [Fact]
        public void Test_Unary() {
            var count = _query.Where(o => !(-1 * (-o.Id) > 0)).Count();
            var dynCount = _query.Where("!(-1 * -Id > 0)").Count();

            Assert.Equal(count, dynCount);
        }

        [Fact]
        public void Test_DateTime_Parse() {
            var local = DateTime.Now.Date;
            var utc = local.ToUniversalTime();
            var unspecified = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, DateTimeKind.Unspecified);
            var forceUtc = DateTime.SpecifyKind(local, DateTimeKind.Utc);
            var orders = new List<Order> {
                new Order {Id = 1, OrderDate = unspecified},
                new Order {Id = 2, OrderDate = utc},
                new Order {Id = 3, OrderDate = local},
                new Order {Id = 4, OrderDate = forceUtc},
            }.AsQueryable();

            var unspecifiedStr = utc.ToString("yyyy-MM-ddTHH:mm:ss");
            var utcStr = utc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Unspecified is left untouched
            Assert.Equal(2, orders.Where($"OrderDate == \"{unspecifiedStr}\"", DateTimeType.Unspecified).Count());
            Assert.Equal(2, orders.Where($"OrderDate == \"{unspecifiedStr}\"", DateTimeType.Utc).Count());

            // these unspecified dates should convert to local
            Assert.Equal(3, orders.Where($"OrderDate == \"{unspecifiedStr}\"", DateTimeType.Local).FirstOrDefault()?.Id);
            Assert.Equal(3, orders.Where($"OrderDate == \"{unspecifiedStr}\"").FirstOrDefault()?.Id);

            // If string contains zone information, dates are parsed and converted to local dates
            Assert.Equal(2, orders.Where($"OrderDate == \"{utcStr}\"", DateTimeType.Utc).Count());
            Assert.Equal(3, orders.Where($"OrderDate == \"{utcStr}\"").FirstOrDefault()?.Id);

            Assert.Equal(2, orders.Where($"OrderDate == \"{utcStr}\"", DateTimeType.ForceUtc).Count());
        }
    }
}