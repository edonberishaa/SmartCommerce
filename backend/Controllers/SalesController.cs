using backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly SmartCommerceContext _context;

        public SalesController(SmartCommerceContext context)
        {
            _context = context;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestSales()
        {

            var salesSummary = await _context.Sales
                .GroupBy(s => s.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .ToListAsync();
            return Ok(salesSummary);
        }
    }
}
