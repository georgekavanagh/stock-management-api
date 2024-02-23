using System;
using System.Collections.Generic;

namespace Stock_Management_API.Entities;

public partial class StockItem
{
    public string? RegNo { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? ModelYear { get; set; }

    public string? Kms { get; set; }

    public string? Colour { get; set; }

    public string? VinNo { get; set; }

    public decimal? RetailPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public DateTime? Dtcreated { get; set; }

    public DateTime? Dtupdated { get; set; }

    public int Id { get; set; }

    public ICollection<Accessory> Accessories { get; set; }

    public ICollection<Image> Images { get; set; }
}
