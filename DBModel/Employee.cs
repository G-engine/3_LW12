using System;
using System.Collections.Generic;

namespace DBModel;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Schedule { get; set; } = null!;

    public int Salary { get; set; }

    public string Job { get; set; } = null!;

    public bool InVacation { get; set; }
    
    public Employee()
    {
        Name = "";
        Job = "";
        Salary = 0;
        Schedule = "";
        InVacation = false;
    }
    
    public Employee(string name, string job, int salary, string schedule)
    {
        Name = name;
        Job = job;
        Salary = salary;
        Schedule = schedule;
        InVacation = false;
    }
    
    public Employee(string name, string job, int salary, string schedule, int inVacation)
    {
        Name = name;
        Job = job;
        Salary = salary;
        Schedule = schedule;
        InVacation = inVacation > 0;
    }
}
