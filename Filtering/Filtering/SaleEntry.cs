namespace Filtering;

public class SaleEntry
{
    public string? Item { get; set; }
    public decimal Price { get; set; }
    public string? Location { get; set; }

    public override string ToString()
    {
        return $"{nameof(Item)}: {Item}, {nameof(Price)}: {Price}, {nameof(Location)}: {Location}";
    }
}