using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Stock_Management_API.Entities;

public partial class Image
{
    [Key]
    public int Id { get; set; }

    
    [StringLength(maximumLength: 255)]
    public string? Description { get; set; }

    public byte[]? ImageData { get; set; }
    public int StockItemId { get; set; }

    [JsonIgnore]
    public StockItem? StockItem { get; set; }
}
