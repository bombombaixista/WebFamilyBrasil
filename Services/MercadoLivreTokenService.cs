using System.Text.Json;
using WebFamily.Models;

namespace WebFamily.Services
{
    public class MercadoLivreTokenService
    {
        private readonly string _filePath;

        public MercadoLivreTokenService(IWebHostEnvironment env)
        {
            // Caminho do arquivo JSON dentro da pasta Data
            _filePath = Path.Combine(env.ContentRootPath, "Data", "mercadolivre_token.json");
        }

        /// <summary>
        /// Lê o token salvo em JSON, se existir e ainda estiver válido.
        /// </summary>
        public async Task<MercadoLivreToken?> GetValidTokenAsync()
        {
            if (!File.Exists(_filePath))
                return null;

            var json = await File.ReadAllTextAsync(_filePath);
            var token = JsonSerializer.Deserialize<MercadoLivreToken>(json);

            if (token == null) return null;

            // Verifica se o token ainda não expirou
            if (token.ExpirationDate > DateTime.UtcNow)
                return token;

            return null;
        }

        /// <summary>
        /// Salva ou atualiza o token em JSON.
        /// </summary>
        public async Task SaveTokenAsync(MercadoLivreToken newToken)
        {
            var json = JsonSerializer.Serialize(newToken, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
