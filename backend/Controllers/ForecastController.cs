using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly SmartCommerceContext _context;
        private readonly HttpClient _http;

        public ForecastController(SmartCommerceContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _http = httpClientFactory.CreateClient();
        }

        [HttpGet("{productName}")]
        public async Task<IActionResult> GetForecast(string productName)
        {
            // 🧠 Fetch historical sales for product
            var salesData = await _context.Sales
                .Where(s => s.ProductName == productName)
                .OrderBy(s => s.SaleDate)
                .Select(s => new
                {
                    date = s.SaleDate.ToString("yyyy-MM-dd"),
                    quantity = s.Quantity
                })
                .ToListAsync();

            if (salesData.Count < 10)
                return BadRequest("Not enough data to forecast");

            // 🧠 Send to Python forecasting API
            var response = await _http.PostAsJsonAsync("http://localhost:8001/forecast", new { sales_data = salesData });

            if (!response.IsSuccessStatusCode)
                return StatusCode(500, "Forecasting service error");

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
    }
}
