namespace REST.Models;

public class Prod
{
    public int Id { get; set; }

    public int SerialNumber { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Number { get; set; }

    public int SupplierId { get; set; }
}