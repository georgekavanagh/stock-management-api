namespace Stock_Management_API.Entities
{
    public class PaginatedStockItems
    {
        public int TotalCount { get; set; }
        public List<StockItem> StockItems { get; set; }
    }
}
