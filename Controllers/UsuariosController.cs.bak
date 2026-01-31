using Kanban.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuariosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("WebFamilyHome4");
            var response = await client.GetAsync("usuarios"); // ajuste conforme os endpoints da API

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var usuarios = System.Text.Json.JsonSerializer.Deserialize<List<Usuario>>(json);

            return View(usuarios);
        }
    }
}
