using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.CRUD;

namespace WebApp.Pages;

public class Employee : PageModel
{
    private EmployeeRepo repo;
    public List<DBModel.Employee> Employees;

    public Employee(EmployeeRepo repo)
    {
        this.repo = repo;
        Employees = new List<DBModel.Employee>();
    }
    
    public async Task OnGetAsync()
    {
        if (Employees.Count > 0) Employees.Clear();
        Employees =  (await repo.GetAllAsync()).ToList();
    }
    
    public async Task OnGetByIdAsync(int id)
    {
        DBModel.Employee? e = await repo.GetAsync(id);
        if (Employees.Count > 0) Employees.Clear();
        Employees.Append(e);
    }

    public async Task OnPostAsync(string name, string job, int salary, string schedule)
    {
        DBModel.Employee e = new DBModel.Employee(name, job, salary, schedule);
        await repo.CreateAsync(e);
    }

    public async Task OnPostDeleteAsync(int id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            await repo.DeleteAsync(id);
        }
    }
    
    public async Task OnPostUpdateAsync(int id, string name, string job, int salary, string schedule, int inVacation)
    {
        DBModel.Employee e = new DBModel.Employee(name, job, salary, schedule, inVacation);
        var existing = await repo.GetAsync(id);
        if (existing is not null)
        {
            await repo.UpdateAsync(id, e);
        }
    }
}