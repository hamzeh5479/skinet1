using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, 
        string? sort, string? search, int pageNumber = 1, int pageSize = 10)
        {
            var query = context.products.AsQueryable();
            if(!string.IsNullOrWhiteSpace(brand))
                query = query.Where(x => x.Brand == brand);
             if(!string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.Type == type);
            if(!string.IsNullOrWhiteSpace(sort))
            {
                query = sort switch
                {
                    "priceAsc" => query.OrderBy(x => x.Price),
                    "priceDesc" => query.OrderByDescending(x => x.Price),
                    _ => query.OrderBy(x => x.Name)
                };
            }
            // 🔍 search filter (case-insensitive, partial match)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    p.Description.ToLower().Contains(s) ||
                    p.Brand.ToLower().Contains(s) ||
                    p.Type.ToLower().Contains(s));
            }

                // pagination
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;
            

            var skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
            Console.Write( "You have : " + context.products.Count() + " itmes in the list.");

            return  await query.ToListAsync();
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
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetBrands()
        {
            return await context.products
                .Select(x => x.Brand)
                .Distinct()
                .ToListAsync();
        }
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetTypes()
        {
            return await context.products
                .Select(x => x.Type)
                .Distinct()
                .ToListAsync();
        }

        [HttpGet("count")]
        public async Task<int> GetProductCount()
        {
            return await context.products.CountAsync();
        }



    // [HttpPost("seed")]
    // public async Task<IActionResult> SeedProducts()
    // {
    //     if (await context.products.AnyAsync())
    //         return BadRequest("Database already has products");

    //     var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");
    //     var json = await System.IO.File.ReadAllTextAsync(filePath);

    //     var products = JsonSerializer.Deserialize<List<Product>>(json,
    //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
    //     if (products == null || products.Count == 0)
    //         return BadRequest("No products found in JSON file");

    //     await context.products.AddRangeAsync(products);
    //     await context.SaveChangesAsync();

    //     return Ok("Products seeded successfully");
    // }

        
        private bool ProductExists(int id)
        {
            return context.products.Any(x => x.id == id);
        }

    }
}