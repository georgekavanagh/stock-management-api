// StockItemsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock_Management_API.Entities;
using System.Linq.Expressions;

namespace Stock_Management_API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockItemsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public StockItemsController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/StockItems
        [HttpGet]
        public async Task<ActionResult<PaginatedStockItems>> GetStockItems(
                [FromQuery] int pageNumber = 0,
                [FromQuery] int pageSize = 10,
                [FromQuery] string sortBy = "Id",
                [FromQuery] string sortOrder = "asc",
                [FromQuery] string regNo = "",
                [FromQuery] string make = "",
                [FromQuery] string model = "",
                [FromQuery] string modelYear = "",
                [FromQuery] string kms = "",
                [FromQuery] string colour = "",
                [FromQuery] string vinNo = ""
            )
        {
            IQueryable<StockItem> query = _context.StockItems.Include(s => s.Accessories).Include(s => s.Images);

            // Apply filters
            query = ApplyFilters(query, regNo, make, model, modelYear, kms, colour, vinNo);

            // Get total count before applying pagination
            int totalCount = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = ApplySorting(query, sortBy, sortOrder);
            }

            // Pagination
            query = query.Skip(pageNumber * pageSize).Take(pageSize);

            var paginatedStockItems = new PaginatedStockItems
            {
                TotalCount = totalCount,
                StockItems = await query.ToListAsync()
            };

            return paginatedStockItems;
        }

        // GET: api/StockItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockItem>> GetStockItem(int id)
        {

            var stockItem = await _context.StockItems
                .Include(s => s.Accessories)
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockItem == null)
            {
                return NotFound();
            }

            return stockItem;
        }

        // POST: api/StockItems
        [HttpPost]
        public async Task<ActionResult<StockItem>> PostStockItem(StockItem stockItem)
        {
            // Modify Dtupdated field before saving
            stockItem.Dtcreated = DateTime.Now;
            _context.StockItems.Add(stockItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockItem), new { id = stockItem.Id }, stockItem);
        }

        // PUT: api/StockItems/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StockItem>> PutStockItem(int id, StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return BadRequest();
            }

            var existingStockItem = await _context.StockItems
                .Include(s => s.Accessories)
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingStockItem == null)
            {
                return NotFound();
            }

            // Update properties of existingStockItem with values from stockItem
            var stockItemProperties = typeof(StockItem).GetProperties();
            foreach (var property in stockItemProperties)
            {
                // Exclude Id and other properties that should not be updated
                if (property.Name != "Id" && property.Name != "Dtcreated" && property.CanWrite)
                {
                    var value = property.GetValue(stockItem);
                    property.SetValue(existingStockItem, value);
                }
            }

            // Modify Dtupdated field before saving
            existingStockItem.Dtupdated = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingStockItem);
        }

        // DELETE: api/StockItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockItem(int id)
        {
            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }

            _context.StockItems.Remove(stockItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockItemExists(int id)
        {
            return _context.StockItems.Any(e => e.Id == id);
        }

        private IQueryable<StockItem> ApplyFilters(
            IQueryable<StockItem> query,
            string regNoFilter,
            string make,
            string model,
            string modelYear,
            string kms,
            string colour,
            string vinNo)
        {

            if (!string.IsNullOrEmpty(regNoFilter))
            {
                query = query.Where(s => s.RegNo.Contains(regNoFilter));
            }

            if (!string.IsNullOrEmpty(make))
            {
                query = query.Where(s => s.Make.Contains(make));
            }

            if (!string.IsNullOrEmpty(model))
            {
                query = query.Where(s => s.Model.Contains(model));
            }

            if (!string.IsNullOrEmpty(modelYear))
            {
                query = query.Where(s => s.ModelYear.Contains(modelYear));
            }

            if (!string.IsNullOrEmpty(kms))
            {
                query = query.Where(s => s.Kms.Contains(kms));
            }

            if (!string.IsNullOrEmpty(colour))
            {
                query = query.Where(s => s.Colour.Contains(colour));
            }

            if (!string.IsNullOrEmpty(vinNo))
            {
                query = query.Where(s => s.VinNo.Contains(vinNo));
            }

            return query;
        }

        private static IQueryable<StockItem> ApplySorting(IQueryable<StockItem> query, string sortBy, string sortOrder)
        {
            var parameter = Expression.Parameter(typeof(StockItem), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<StockItem, object>>(Expression.Convert(property, typeof(object)), parameter);

            return sortOrder.ToLower() == "desc" ? query.OrderByDescending(lambda) : query.OrderBy(lambda);
        }
    }
}