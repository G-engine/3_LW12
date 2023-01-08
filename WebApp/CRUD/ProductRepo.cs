using System.Collections.Concurrent;
using DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class ProductRepo
{
    private static ConcurrentDictionary<int, Product> productsCache;
    private PharmacyContext db;

    public ProductRepo(PharmacyContext db)
    {
        this.db = db;
        productsCache = new ConcurrentDictionary<int, Product>(db.Products.ToDictionary(p => p.Id));
    }
    private Product UpdateCache(int id, Product product)
    {
        Product old;
        if (productsCache.TryGetValue(id, out old))
        {
            if (productsCache.TryUpdate(id, product, old))
            {
                return product;
            }
        }
        return null;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        EntityEntry<Product> added = await db.Products.AddAsync(product);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return productsCache.AddOrUpdate(product.Id, product, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Product? p = db.Products.Find(id);
        if(p != null)
        {
            db.Products.Remove(p);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return productsCache.TryRemove(p.Id, out p);
            }
        }
        return null;
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Product>>(() => productsCache.Values);
    }

    public Task<Product?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            productsCache.TryGetValue(id, out Product? p);
            return p;
        });
    }

    public async Task<Product> UpdateAsync(int id, Product product)
    {
        var old = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        old.SerialNumber = product.SerialNumber;
        old.Name = product.Name;
        old.Price = product.Price;
        old.Number = product.Number;
        old.SupplierId = product.SupplierId;
        old.Supplier = product.Supplier;
        
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            return UpdateCache(id, product);
        }
        return null;
    }
}