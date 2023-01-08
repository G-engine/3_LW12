using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.CRUD;

namespace WebApp.Pages;

public class Product : PageModel
{
    private ProductRepo repo;
    private SupplierRepo sRepo;
    public List<DBModel.Product> Products;

    public Product(ProductRepo repo, SupplierRepo pRepo)
    {
        this.repo = repo;
        this.sRepo = pRepo;
        Products = new List<DBModel.Product>();
    }
    
    public async Task OnGetAsync()
    {
        if (Products.Count > 0) Products.Clear();
        Products =  (await repo.GetAllAsync()).ToList();
    }
    
    public async Task OnGetByIdAsync(int id)
    {
        DBModel.Product? p = await repo.GetAsync(id);
        if (Products.Count > 0) Products.Clear();
        Products.Append(p);
    }

    public async Task OnPostAsync(string name, int sn, int price, int number, int sId)
    {
        var s = await sRepo.GetAsync(sId);
        DBModel.Product p = new DBModel.Product(sn, name, price, number, sId) { Supplier = s };
        await repo.CreateAsync(p);
        s.Products.Add(p);
        await sRepo.UpdateAsync(sId, s);
    }

    public async Task OnPostDeleteAsync(int id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            await repo.DeleteAsync(id);
        }
    }

    public async Task OnPostUpdateAsync(int id, string name, int sn, int price, int number, int sId)
    {
        var s = await sRepo.GetAsync(sId);
        DBModel.Product p = new DBModel.Product(sn, name, price, number, sId) {Supplier = s};
        s.Products.Remove(s.Products.FirstOrDefault(p => p.Id == id));
        s.Products.Add(p);
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            await repo.UpdateAsync(id, p);
            await sRepo.UpdateAsync(sId, s);
        }
    }
}