namespace REST.Models;

public class Suppl
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Prod> Products { get; set; } = new List<Prod>();
}