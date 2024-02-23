using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock_Management_API.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Stock_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ImageController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/Image/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetImage(int id)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            return image;
        }

        // POST: api/Image
        [HttpPost]
        public async Task<ActionResult<Image>> PostImage([FromForm] IFormFile file, [FromForm] string description, [FromForm] int stockItemId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("Description is required");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    byte[] imageData = memoryStream.ToArray();

                    Console.WriteLine($"Image data length: {imageData.Length}");

                    var image = new Image
                    {
                        Description = description,
                        ImageData = imageData,
                        StockItemId = stockItemId
                    };

                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();

                    return image;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // DELETE: api/Image/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Image>> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound("Image not found");
            }

            try
            {
                _context.Images.Remove(image);

                // Remove the image from the related StockItem's Images collection
                var stockItem = await _context.StockItems
                    .Include(si => si.Images)
                    .FirstOrDefaultAsync(si => si.Images.Any(i => i.Id == id));

                if (stockItem != null)
                {
                    var imageToRemove = stockItem.Images.FirstOrDefault(i => i.Id == id);
                    if (imageToRemove != null)
                    {
                        stockItem.Images.Remove(imageToRemove);
                    }
                }

                await _context.SaveChangesAsync();

                return image;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}
