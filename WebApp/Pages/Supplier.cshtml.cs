using DBModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.CRUD;

namespace WebApp.Pages;

public class Supplier : PageModel
{
    private SupplierRepo repo;
    private ProductRepo pRepo;
    public List<DBModel.Supplier> Suppliers;

    public Supplier(SupplierRepo repo, ProductRepo pRepo)
    {
        this.repo = repo;
        this.pRepo = pRepo;
        Suppliers = new List<DBModel.Supplier>();
    }
    
    public async Task OnGetAsync()
    {
        if (Suppliers.Count > 0) Suppliers.Clear();
        Suppliers =  (await repo.GetAllAsync()).ToList();
    }
    
    public async Task OnGetByIdAsync(int id)
    {
        DBModel.Supplier? s = await repo.GetAsync(id);
        if (Suppliers.Count > 0) Suppliers.Clear();
        Suppliers.Append(s);
    }

    public async Task OnPostAsync(string name, int pSN, string pName, int pPrice, int pNumber)
    {
        DBModel.Supplier s = new DBModel.Supplier {Name = name};
        DBModel.Product p = new DBModel.Product(pSN, pName, pPrice, pNumber, s);
        await pRepo.CreateAsync(p);
        s.Products.Add(p);
        try
        {
            await repo.CreateAsync(s);
        }
        catch (Exception e) { }
    }

    public async Task OnPostDeleteAsync(int id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            foreach (var p in existing.Products.ToList())
                await pRepo.DeleteAsync(p.Id);
            await repo.DeleteAsync(id);
        }
    }

    public async Task OnPostUpdateAsync(int id, string name)
    {
        DBModel.Supplier s = new DBModel.Supplier {Name = name};
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            await repo.UpdateNameAsync(id, s);
        }
    }
}


