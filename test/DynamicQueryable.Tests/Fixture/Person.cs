namespace DynamicQueryable.Tests.Fixture;

public class Person {
    public string Name { get; set; }
    public int Age { get; set; }
    public int? Number { get; set; }
    public Address Address { get; set; } = new();
}
