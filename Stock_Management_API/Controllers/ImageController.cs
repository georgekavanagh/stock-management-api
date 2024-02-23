using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                    byte[] imageData = memoryStream.ToArray();

                    var image = new Image
                    {
                        Description = description,
                        ImageData = imageData,
                        StockItemId = stockItemId
                    };

                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetImage), new { id = image.Id }, image);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
