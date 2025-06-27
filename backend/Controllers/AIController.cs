using backend.Models;
using Microsoft.AspNetCore.Mvc;
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

        public AiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QAPayload payload)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/ask", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "AI service failed");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            // Optional: parse as JSON
            var jsonDoc = JsonDocument.Parse(responseBody);
            var answer = jsonDoc.RootElement.GetProperty("answer").GetString();

            return Ok(new { answer });
        }
    }
}
