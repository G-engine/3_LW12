using System.Collections.Concurrent;
using DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class SupplierRepo
{
    private static ConcurrentDictionary<int, Supplier> suppliersCache;
    private PharmacyContext db;

    public SupplierRepo(PharmacyContext db)
    {
        this.db = db;
        suppliersCache = new ConcurrentDictionary<int, Supplier>(db.Suppliers.ToDictionary(s=>s.Id));
    }
    private Supplier UpdateCache(int id, Supplier supplier)
    {
        Supplier old;
        if (suppliersCache.TryGetValue(id, out old))
        {
            if (suppliersCache.TryUpdate(id, supplier, old))
            {
                return supplier;
            }
        }
        return null;
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        EntityEntry<Supplier> added = await db.Suppliers.AddAsync(supplier);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return suppliersCache.AddOrUpdate(supplier.Id, supplier, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Supplier? s = db.Suppliers.Find(id);
        if(s != null)
        {
            db.Suppliers.Remove(s);
            
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return suppliersCache.TryRemove(id, out s);
            }
        }
        return null;
    }

    public Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Supplier>>(() => suppliersCache.Values);
    }

    public Task<Supplier?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            suppliersCache.TryGetValue(id, out Supplier? s);
            return s;
        });
    }

    public async Task<Supplier> UpdateAsync(int id, Supplier supplier)
    {
        var old = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        old.Name = supplier.Name;
        old.Products = supplier.Products;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, supplier);
        }
        return null;
    }
    
    public async Task<Supplier> UpdateNameAsync(int id, Supplier supplier)
    {
        var old = await db.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        old.Name = supplier.Name;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, supplier);
        }
        return null;
    }
}