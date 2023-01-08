using Microsoft.AspNetCore.Mvc;
using DBModel;
using REST.Models;
using WebApp.CRUD;

namespace REST.Controllers;

[ApiController]
[Route("[controller]")]
public class SupplierController : Controller
{
    private SupplierRepo repo;
    private ProductRepo pRepo;
    public SupplierController(SupplierRepo repo, ProductRepo pRepo)
    {
        this.repo = repo;
        this.pRepo = pRepo;
    }
    
    [HttpGet]
    [ProducesResponseType(200,Type = typeof(IEnumerable<Suppl>))]
    public async Task<IEnumerable<Suppl>> GetSuppliers()
    {
        var suppliers = await repo.GetAllAsync();
        List<Suppl> suppls = new List<Suppl>();
        foreach (var s in suppliers)
        {
            suppls.Add(new Suppl() { Id = s.Id, Name = s.Name });
        }
        return suppls;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Supplier))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSupplier(int id)
    {
        Supplier s = await repo.GetAsync(id);
        if (s is null)
        {
            return NotFound();
        }
        return Ok(s);
    }  
    
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Supplier))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Suppl s)
    {
        if (s is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Supplier supplier = new Supplier() { Name = s.Name };
        Prod p = s.Products.FirstOrDefault();
        Product product = new Product(p.SerialNumber, p.Name, p.Price, p.Number, supplier);
        supplier.Products.Add(product);
        Supplier added = await repo.CreateAsync(supplier);
        return CreatedAtRoute(
            routeName: nameof(GetSupplier),
            routeValues: new { id = added.Id },
            value: added);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] Supplier s)
    {
        if (s.Id != id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        await repo.UpdateNameAsync(id, s);
        return new NoContentResult();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        foreach (var p in existing.Products.ToList())
            await pRepo.DeleteAsync(p.Id);
        bool? deleted = await repo.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }
        return BadRequest($"Supplier {id} was found but failed to delete.");
    }
}