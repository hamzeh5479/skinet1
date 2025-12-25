using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext context;

        public ProductRepository(StoreContext context)
        {
            this.context = context;
        }

        public void AddProduct(Product product)
        {
            context.products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            context.Remove(product);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await context.products.FindAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            return await context.products.ToListAsync(); 
        }

        public bool ProductExists(int id)
        {
            return context.products.Any(x => x.id == id);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateProduct(Product product)
        {
            context.Entry(product).State = EntityState.Modified;
        }
    }
}