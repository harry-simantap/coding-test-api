using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.api.Models;

namespace test.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? category,
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            int skip = (page - 1) * limit;
            var pagedData = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            var response = pagedData.Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Category = p.Category,
                Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Images) ?? [],
                CreatedAt = p.CreatedAt,
                CreatedBy = p.CreatedBy,
                CreatedById = p.CreatedById.ToString(),
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                UpdatedById = p.UpdatedById.ToString()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null)
            {
                return NotFound(new { message = "Produk tidak ditemukan." });
            }

            var response = new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Category = p.Category,
                Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Images) ?? [],
                CreatedAt = p.CreatedAt,
                CreatedBy = p.CreatedBy,
                CreatedById = p.CreatedById.ToString(),
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                UpdatedById = p.UpdatedById.ToString()
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDto s)
        {
            if (string.IsNullOrEmpty(s.Title) || s.Price <= 0 || string.IsNullOrEmpty(s.Category) || s.Images == null || s.Images.Count == 0)
            {
                return BadRequest(new { message = "title, price, category, dan minimal 1 image wajib diisi." });
            }

            var userIdAuth = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";

            var product = new Product
            {
                Title = s.Title,
                Price = s.Price,
                Description = s.Description,
                Category = s.Category,
                Images = System.Text.Json.JsonSerializer.Serialize(s.Images),
                CreatedAt = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System",
                CreatedById = Convert.ToInt32(userIdAuth),
                UpdatedAt = DateTime.Now,
                UpdatedBy = User.Identity?.Name ?? "System",
                UpdatedById = Convert.ToInt32(userIdAuth)
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDto s)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Produk tidak ditemukan." });
            }

            if (string.IsNullOrEmpty(s.Title) || s.Price <= 0 || string.IsNullOrEmpty(s.Category) || s.Images == null || s.Images.Count == 0)
            {
                return BadRequest(new { message = "title, price, category, dan minimal 1 image wajib diisi." });
            }

            product.Title = s.Title;
            product.Price = s.Price;
            product.Description = s.Description;
            product.Category = s.Category;
            product.Images = System.Text.Json.JsonSerializer.Serialize(s.Images);
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Produk berhasil diperbarui", data = product });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Produk tidak ditemukan." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Produk berhasil dihapus." });
        }
    }
}
