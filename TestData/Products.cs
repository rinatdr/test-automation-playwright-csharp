namespace Swag_Labs.TestData;

public record Product(string Name, decimal Price);

public static class Products
{
    public static readonly Product Backpack = new("Sauce Labs Backpack", 29.99m);
    public static readonly Product BikeLight = new("Sauce Labs Bike Light", 9.99m);
    public static readonly Product BoltTShirt = new("Sauce Labs Bolt T-Shirt", 15.99m);
    public static readonly Product FleeceJacket = new("Sauce Labs Fleece Jacket", 49.99m);
    public static readonly Product Onesie = new("Sauce Labs Onesie", 7.99m);
    public static readonly Product RedTShirt = new("Test.allTheThings() T-Shirt (Red)", 15.99m);

    public static readonly Product[] All = [Backpack, BikeLight, BoltTShirt, FleeceJacket, Onesie, RedTShirt];
}
