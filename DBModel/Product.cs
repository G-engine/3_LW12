using System;
using System.Collections.Generic;

namespace DBModel;

public partial class Product
{
    public int Id { get; set; }

    public int SerialNumber { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Number { get; set; }

    public int SupplierId { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    
    public Product()
    {
        SerialNumber = 0;
        Name = "";
        Price = 0;
        Number = 0;
        SupplierId = 0;
        Supplier = null;
    }

    public Product(int serialNumber, string name, int price, int number, Supplier supplier)
    {
        SerialNumber = serialNumber;
        Name = name;
        Price = price;
        Number = number;
        SupplierId = supplier.Id;
        Supplier = supplier;
    }
    
    public Product(int serialNumber, string name, int price, int number, int supplierId)
    {
        SerialNumber = serialNumber;
        Name = name;
        Price = price;
        Number = number;
        SupplierId = supplierId;
    }
}
