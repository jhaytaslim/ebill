using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebill.Data.Models;

public class Product
{
    [Key]
    public int id { get; set; }
    public int name { get; set; }

    [InverseProperty("product")]
    public List<Item> items { get; set; }
}

public class Item
{
    [Key]
    public int id { get; set; }
    public int name { get; set; }

    [ForeignKey("productId")]
    public Product product { get; set; }
}

public class Settings
{

    public Settings()
    {
        this.secret = "secret";
        this.iv = "iv";
    }

    [Key]
    public int id { get; set; }
    public string secret { get; set; }
    public string iv { get; set; }
    public string billerName { get; set; }
}


