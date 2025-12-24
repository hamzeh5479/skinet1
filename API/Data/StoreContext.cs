using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Config;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Data;


    
public class StoreContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfigration).Assembly);

    }
}



 
