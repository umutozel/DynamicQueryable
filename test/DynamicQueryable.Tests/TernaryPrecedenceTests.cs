using System;
using System.Linq;
using System.Linq.Dynamic;
using Xunit;

namespace DynamicQueryable.Tests;

using Fixture;

// Regression coverage for the Jokenizer.Net ternary-precedence fix (>= 1.3.4):
// "a > 2 ? x : y" used to parse as "a > (2 ? x : y)" — the recursive
// right-side parse swallowed the ternary — and Expression.Condition threw
// "Argument must be boolean (Parameter ''test'')". Every ternary whose
// predicate ends in a binary comparison was affected; these lock the fixed
// behaviour through DynamicQueryable''s public Select surface.
public class TernaryPrecedenceTests {
    private readonly IQueryable<Order> _query = Enumerable.Range(1, 5)
        .Select(i => new Order {
            Id = i,
            OrderNo = $"ORD{i:0000}",
            OrderDate = DateTime.Today.AddDays(-i * 10),
            Price = i % 2 == 0 ? i * 10.0 : null,
        })
        .AsQueryable();

    [Fact]
    public void Ternary_SimpleDateComparison() {
        var list = _query.Select("Id > 2 ? \"new\" : \"old\"").Cast<string>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_DateParamComparison() {
        var list = _query.Select("OrderDate >= @0 ? \"new\" : \"old\"", DateTime.Today.AddDays(-25))
            .Cast<string>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_AndOfTwoComparisons() {
        var list = _query.Select("OrderDate >= @0 && OrderDate <= @1 ? \"new\" : \"old\"",
                DateTime.Today.AddDays(-40), DateTime.Today)
            .Cast<string>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_NullableComparison() {
        var list = _query.Select("Price > 15 ? \"big\" : \"small\"").Cast<string>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_NullableAndComparison() {
        var list = _query.Select("Price != null && Price > 15 ? \"big\" : \"small\"")
            .Cast<string>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_InsideNewProjection() {
        var list = _query.Select("new (OrderNo, OrderDate >= @0 ? \"new\" : \"old\" as Cohort)",
                DateTime.Today.AddDays(-25))
            .Cast<object>().ToList();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Ternary_MonthEquality() {
        var list = _query.Select("OrderDate.Month == @0 ? 1 : 0", DateTime.Today.Month)
            .Cast<int>().ToList();
        Assert.Equal(5, list.Count);
    }
}
