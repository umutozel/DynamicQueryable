using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Xunit;

namespace DynamicQueryable.Tests {
    public class GroupByNewProjectionRegressionTests {
        private sealed record Product(string Name, string Category, decimal Price, int Stock);

        [Fact]
        public void GroupBy_After_Select_NewProjection_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            // RED: burada parse kiriliyor (Expected [ at index 3, found ()
            var grouped = source.GroupBy("Category");
            var projected = grouped.Select("new(Key as Category, Count() as Total)");

            var list = projected.Cast<dynamic>().ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal(2, (int)list.Single(x => (string)x.Category == "Electronics").Total);
            Assert.Equal(2, (int)list.Single(x => (string)x.Category == "Furniture").Total);
            Assert.Equal(1, (int)list.Single(x => (string)x.Category == "Stationery").Total);
        }

        [Fact]
        public void GroupBy_After_Select_NewProjection_With_Sum_Avg_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var projected = source
                .GroupBy("Category")
                .Select("new(Key as Category, Sum(it.Price) as TotalPrice, Avg(it.Price) as AvgPrice)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(3, projected.Count);

            var electronics = projected.Single(x => (string)x.Category == "Electronics");
            var furniture = projected.Single(x => (string)x.Category == "Furniture");
            var stationery = projected.Single(x => (string)x.Category == "Stationery");

            Assert.Equal(1600m, (decimal)electronics.TotalPrice);
            Assert.Equal(500m, (decimal)furniture.TotalPrice);
            Assert.Equal(2m, (decimal)stationery.TotalPrice);

            Assert.Equal(800m, (decimal)electronics.AvgPrice);
            Assert.Equal(250m, (decimal)furniture.AvgPrice);
            Assert.Equal(2m, (decimal)stationery.AvgPrice);
        }

        [Fact]
        public void GroupBy_MultiKey_After_Select_NewProjection_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var projected = source
                .GroupBy("new(Category, Stock)")
                .Select("new(Key.Category, Key.Stock, Count() as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(5, projected.Count);

            Assert.Equal(1, (int)projected.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 5).Total);
            Assert.Equal(1, (int)projected.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 12).Total);
            Assert.Equal(1, (int)projected.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 8).Total);
            Assert.Equal(1, (int)projected.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 25).Total);
            Assert.Equal(1, (int)projected.Single(x => (string)x.Category == "Stationery" && (int)x.Stock == 200).Total);
        }

        [Fact]
        public void GroupBy_After_Select_NewProjection_With_AggregateMemberShorthand_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var projected = source
                .GroupBy("Category")
                .Select("new(Key as Category, Sum(Price) as TotalPrice, Average(Price) as AvgPrice)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(3, projected.Count);

            var electronics = projected.Single(x => (string)x.Category == "Electronics");
            var furniture = projected.Single(x => (string)x.Category == "Furniture");
            var stationery = projected.Single(x => (string)x.Category == "Stationery");

            Assert.Equal(1600m, (decimal)electronics.TotalPrice);
            Assert.Equal(500m, (decimal)furniture.TotalPrice);
            Assert.Equal(2m, (decimal)stationery.TotalPrice);

            Assert.Equal(800m, (decimal)electronics.AvgPrice);
            Assert.Equal(250m, (decimal)furniture.AvgPrice);
            Assert.Equal(2m, (decimal)stationery.AvgPrice);
        }

        [Fact]
        public void GroupBy_MultiKey_After_Select_NewProjection_With_ItKeySyntax_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var singleKeyProjected = source
                .GroupBy("new(Category)")
                .Select("new(it.Category, Count() as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(3, singleKeyProjected.Count);
            Assert.Equal(2, (int)singleKeyProjected.Single(x => (string)x.Category == "Electronics").Total);
            Assert.Equal(2, (int)singleKeyProjected.Single(x => (string)x.Category == "Furniture").Total);
            Assert.Equal(1, (int)singleKeyProjected.Single(x => (string)x.Category == "Stationery").Total);

            var multiKeyProjected = source
                .GroupBy("new(Category, Stock)")
                .Select("new(it.Category, it.Stock, Count() as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(5, multiKeyProjected.Count);
            Assert.Equal(1, (int)multiKeyProjected.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 5).Total);
            Assert.Equal(1, (int)multiKeyProjected.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 12).Total);
            Assert.Equal(1, (int)multiKeyProjected.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 8).Total);
            Assert.Equal(1, (int)multiKeyProjected.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 25).Total);
            Assert.Equal(1, (int)multiKeyProjected.Single(x => (string)x.Category == "Stationery" && (int)x.Stock == 200).Total);
        }

        [Fact]
        public void GroupBy_After_Select_NewProjection_With_ArithmeticAggregate_Should_Work() {
            // Aggregate arg is arithmetic across two element members. Pre-2.2.5
            // the shorthand regex only matched a single identifier, so
            // "Sum(Price + Stock)" reached Jokenizer unwrapped and threw
            // "Cannot find indexer on type IGrouping" — the outer scope
            // (IGrouping) doesn't own Price/Stock. NormalizeAggregateExpressionShorthand
            // now wraps into "Sum(x => Price + Stock)" and Jokenizer 1.3.5's
            // innermost-owner walk resolves each bare identifier as a member
            // of x (the element).
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var projected = source
                .GroupBy("Category")
                .Select("new(Key as Category, Sum(Price + Stock) as Weighted)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(3, projected.Count);
            var electronics = projected.Single(x => (string)x.Category == "Electronics");
            var furniture = projected.Single(x => (string)x.Category == "Furniture");
            var stationery = projected.Single(x => (string)x.Category == "Stationery");

            // Electronics: (1200+5) + (400+12) = 1617
            Assert.Equal(1617m, (decimal)electronics.Weighted);
            // Furniture:   (350+8)  + (150+25) = 533
            Assert.Equal(533m, (decimal)furniture.Weighted);
            // Stationery:  (2+200)             = 202
            Assert.Equal(202m, (decimal)stationery.Weighted);
        }

        [Fact]
        public void GlobalAggregate_ArithmeticExpression_Should_Work() {
            // Non-grouped global aggregation — the same shorthand extension
            // applies since GroupBy on a constant key still lands in the
            // Select-over-IGrouping projection form.
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
            }.AsQueryable();

            var projected = source
                .GroupBy("1")
                .Select("new(Sum(Price + Stock) as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Single(projected);
            // (1200+5) + (400+12) + (350+8) = 1975
            Assert.Equal(1975m, (decimal)projected[0].Total);
        }

        [Fact]
        public void GroupBy_KeyExpression_With_ItPrefixedMembers_Should_Work() {
            var source = new List<Product> {
                new("Laptop", "Electronics", 1200m, 5),
                new("Monitor", "Electronics", 400m, 12),
                new("Desk", "Furniture", 350m, 8),
                new("Chair", "Furniture", 150m, 25),
                new("Pen", "Stationery", 2m, 200),
            }.AsQueryable();

            var single = source
                .GroupBy("new(it.Category)")
                .Select("new(Key.Category as Category, Count() as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(3, single.Count);
            Assert.Equal(2, (int)single.Single(x => (string)x.Category == "Electronics").Total);
            Assert.Equal(2, (int)single.Single(x => (string)x.Category == "Furniture").Total);
            Assert.Equal(1, (int)single.Single(x => (string)x.Category == "Stationery").Total);

            var multi = source
                .GroupBy("new(it.Category, it.Stock)")
                .Select("new(Key.Category as Category, Key.Stock as Stock, Count() as Total)")
                .Cast<dynamic>()
                .ToList();

            Assert.Equal(5, multi.Count);
            Assert.Equal(1, (int)multi.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 5).Total);
            Assert.Equal(1, (int)multi.Single(x => (string)x.Category == "Electronics" && (int)x.Stock == 12).Total);
            Assert.Equal(1, (int)multi.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 8).Total);
            Assert.Equal(1, (int)multi.Single(x => (string)x.Category == "Furniture" && (int)x.Stock == 25).Total);
            Assert.Equal(1, (int)multi.Single(x => (string)x.Category == "Stationery" && (int)x.Stock == 200).Total);
        }
    }
}
