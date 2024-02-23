using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Stock_Management_API.Entities;

public partial class StockManagementContext : DbContext
{
    public StockManagementContext()
    {
    }

    public StockManagementContext(DbContextOptions<StockManagementContext> options)
        : base(options)
    {
    }

    public DbSet<Accessory> Accessories { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<StockItem> StockItems { get; set; }
}
