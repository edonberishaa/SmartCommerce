using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly SmartCommerceContext _context;

        public AiController(IHttpClientFactory httpClientFactory, SmartCommerceContext context)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            var sales = await _context.Sales
                .GroupBy(s => s.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .ToListAsync();

            var contextParts = sales.Select(s =>
            $"{s.TotalQuantity} {s.ProductName} bottles");
            var context = "Last period,we sold " + string.Join(", ", contextParts) + ".";

            var payload = new
            {
                question = request.Question,
                context = context
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/ask", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "AI service failed");
            }

                var aiResult = await response.Content.ReadFromJsonAsync<AskResponse>();

            return Ok(aiResult);
        }
    }
}
