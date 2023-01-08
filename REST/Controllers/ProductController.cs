using Microsoft.AspNetCore.Mvc;
using DBModel;
using REST.Models;
using WebApp.CRUD;

namespace REST.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : Controller
{
    private ProductRepo repo;
    private SupplierRepo sRepo;
    public ProductController(ProductRepo repo, SupplierRepo sRepo)
    {
        this.repo = repo;
        this.sRepo = sRepo;
    }
    
    [HttpGet]
    [ProducesResponseType(200,Type = typeof(IEnumerable<Prod>))]
    public async Task<IEnumerable<Prod>> GetProducts()
    {
        var products = await repo.GetAllAsync();
        List<Prod> prods = new List<Prod>();
        foreach (var p in products)
        {
            prods.Add(new Prod()
            {
                Id = p.Id, Name = p.Name, SerialNumber = p.SerialNumber,
                Price = p.Price, Number = p.Number, SupplierId = p.SupplierId
            });
        }
        return prods;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Product))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProduct(int id)
    {
        Product p = await repo.GetAsync(id);
        if (p is null)
        {
            return NotFound();
        }
        return Ok(p);
    }  
    
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Product))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Prod p)
    {
        if (p is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Supplier supplier = await sRepo.GetAsync(p.SupplierId);
        Product product = new Product(p.SerialNumber, p.Name, p.Price, p.Number, supplier);
        Product added = await repo.CreateAsync(product);
        return CreatedAtRoute(
            routeName: nameof(GetProduct),
            routeValues: new { id = added.Id },
            value: added);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] Prod p)
    {
        if (p.Id != id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        Supplier supplier = await sRepo.GetAsync(p.SupplierId);
        Product product = new Product(p.SerialNumber, p.Name, p.Price, p.Number, supplier);
        supplier.Products.Remove(supplier.Products.FirstOrDefault(p => p.Id == id));
        supplier.Products.Add(product);
        
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        await repo.UpdateAsync(id, product);
        await sRepo.UpdateAsync(p.SupplierId, supplier);
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
        bool? deleted = await repo.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }
        return BadRequest($"Product {id} was found but failed to delete.");
    }
}