namespace DynamicQueryable.Tests.Fixture;

public class OrderLine {
    public int Id { get; set; }
    public Product? Product;
    public int ProductId { get; set; }
    public Order? Order { get; set; }
    public int OrderId { get; set; }
    public int? Count { get; set; }
    public double? UnitPrice { get; set; }
}