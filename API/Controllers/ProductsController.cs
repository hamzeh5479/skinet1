using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext context;

        public ProductsController(StoreContext context)
        {
            this.context = context;
        }
         [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return  await context.products.ToListAsync();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await context.products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            context.products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if(product.id != id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            context.Entry(product).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await context.products.FindAsync(id);
            if(product == null) return NotFound();
            context.products.Remove(product);
            await context.SaveChangesAsync();
            return NoContent();

        }
        
        private bool ProductExists(int id)
        {
            return context.products.Any(x => x.id == id);
        }

    }
}