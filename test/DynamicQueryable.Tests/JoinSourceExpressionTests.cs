using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Xunit;

namespace DynamicQueryable.Tests;

using Fixture;

// Regression coverage for the inner-source expression fix: Join/GroupJoin used
// to embed the inner IQueryable as Expression.Constant(inner), which opaque-boxes
// it for relational providers — EF Core answered "could not be translated" for
// every dynamic Join whose inner side was a DbSet, even on the same DbContext.
// Queryable's own methods splice inner.Expression instead (see
// Queryable.GetSourceExpression); these tests pin that shape on the dynamic
// surface. Found live via Qlery same-connection JoinStep execution (2026-08-05).
public class JoinSourceExpressionTests {
    private static readonly IQueryable<Order> Orders = Enumerable.Range(1, 3)
        .Select(i => new Order { Id = i, OrderNo = $"ORD{i:0000}" })
        .AsQueryable();

    private static readonly IQueryable<OrderLine> Details = Enumerable.Range(1, 3)
        .Select(i => new OrderLine { Id = i * 10, OrderId = i, Count = i })
        .AsQueryable();

    [Fact]
    public void Join_SplicesInnerQueryableExpression() {
        var joined = Orders.Join(Details, "o => o.Id", "d => d.OrderId",
            "(o, d) => new { Id = o.Id, Count = d.Count }");

        var call = Assert.IsAssignableFrom<MethodCallExpression>(joined.Expression);
        Assert.Equal("Join", call.Method.Name);
        // The inner side must participate with ITS OWN expression tree, not a
        // Constant wrapper around the IQueryable object.
        Assert.Same(Details.Expression, call.Arguments[1]);
    }

    [Fact]
    public void GroupJoin_SplicesInnerQueryableExpression() {
        var joined = Orders.GroupJoin(Details, "o => o.Id", "d => d.OrderId",
            "(o, ds) => new { Id = o.Id, Count = ds.Count() }");

        var call = Assert.IsAssignableFrom<MethodCallExpression>(joined.Expression);
        Assert.Equal("GroupJoin", call.Method.Name);
        Assert.Same(Details.Expression, call.Arguments[1]);
    }

    [Fact]
    public void Zip_KeepsPlainEnumerableAsConstant() {
        var seconds = new List<int> { 1, 2, 3 };
        var zipped = System.Linq.Dynamic.DynamicQueryable.Zip(Orders, seconds, "(o, n) => new { Id = o.Id, N = n }");

        var call = Assert.IsAssignableFrom<MethodCallExpression>(zipped.Expression);
        Assert.Equal("Zip", call.Method.Name);
        // A non-queryable second stays a Constant — only IQueryable sides have
        // an expression tree to splice.
        var constant = Assert.IsAssignableFrom<ConstantExpression>(call.Arguments[1]);
        Assert.Same(seconds, constant.Value);
    }

    [Fact]
    public void Join_StillExecutesInMemory() {
        var list = Orders.Join(Details, "o => o.Id", "d => d.OrderId",
                "(o, d) => new { OrderNo = o.OrderNo, Count = d.Count }")
            .Cast<object>().ToList();
        Assert.Equal(3, list.Count);
    }
}
