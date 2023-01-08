using System.Collections.Concurrent;
using DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApp.CRUD;

public class EmployeeRepo
{
    private static ConcurrentDictionary<int, Employee> customersCache;
    private PharmacyContext db;

    public EmployeeRepo(PharmacyContext db)
    {
        this.db = db;
        customersCache = new ConcurrentDictionary<int, Employee>(db.Employees.ToDictionary(e=>e.Id));
    }
    private Employee UpdateCache(int id, Employee employee)
    {
        Employee old;
        if (customersCache.TryGetValue(id, out old))
        {
            if (customersCache.TryUpdate(id, employee, old))
            {
                return employee;
            }
        }
        return null;
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        EntityEntry<Employee> added = await db.Employees.AddAsync(employee);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return customersCache.AddOrUpdate(employee.Id, employee, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        Employee? e = db.Employees.Find(id);
        if(e != null)
        {
            db.Employees.Remove(e);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return customersCache.TryRemove(id, out e);
            }
        }
        return null;
    }

    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Employee>>(() => customersCache.Values);
    }

    public Task<Employee?> GetAsync(int id)
    {
        return Task.Run(() =>
        {
            customersCache.TryGetValue(id, out Employee? e);
            return e;
        });
    }

    public async Task<Employee> UpdateAsync(int id, Employee employee)
    {
        var old = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        
        old.Name = employee.Name;
        old.Job = employee.Job;
        old.Salary = employee.Salary;
        old.Schedule = employee.Schedule;
        old.InVacation = employee.InVacation;
        int affected = await db.SaveChangesAsync();
        if (affected > 0)
        {
            return UpdateCache(id, employee);
        }
        return null;
    }
}