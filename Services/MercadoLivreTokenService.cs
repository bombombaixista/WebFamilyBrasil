using Kanban.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Kanban.Services
{
    public class MercadoLivreTokenService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public MercadoLivreTokenService(
            AppDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _clientId = configuration["MercadoLivre:ClientId"]!;
            _clientSecret = configuration["MercadoLivre:ClientSecret"]!;
        }

        // 🔹 Salva o token inicial (usado pelo fluxo de OAuth)
        public async Task<MercadoLivreToken> SaveInitialTokenAsync(Guid clienteId, MercadoLivreToken token)
        {
            var cliente = await _dbContext.Clientes2.FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) throw new Exception("Cliente não encontrado.");

            cliente.MercadoLivreAccessToken = token.AccessToken;
            cliente.MercadoLivreRefreshToken = token.RefreshToken;
            cliente.MercadoLivreTokenExpiraEm = token.ExpirationDate;
            cliente.MercadoLivreUserId = token.UserId;
            cliente.MercadoLivreConectado = true;

            await _dbContext.SaveChangesAsync();
            return token;
        }

        // 🔹 Retorna token válido (renova se expirou)
        public async Task<MercadoLivreToken> GetValidTokenAsync(Guid clienteId)
        {
            var cliente = await _dbContext.Clientes2.FindAsync(clienteId);
            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            if (string.IsNullOrEmpty(cliente.MercadoLivreAccessToken))
                throw new Exception("Cliente não conectado ao Mercado Livre.");

            // Se expirou, renova
            if (cliente.MercadoLivreTokenExpiraEm <= DateTime.UtcNow)
            {
                var httpClient = _httpClientFactory.CreateClient();

                var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "refresh_token", cliente.MercadoLivreRefreshToken! }
                };

                var response = await httpClient.PostAsync(
                    "https://api.mercadolibre.com/oauth/token",
                    new FormUrlEncodedContent(requestBody)
                );

                response.EnsureSuccessStatusCode();
                var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

                cliente.MercadoLivreAccessToken = json.GetProperty("access_token").GetString()!;
                cliente.MercadoLivreRefreshToken = json.GetProperty("refresh_token").GetString()!;
                cliente.MercadoLivreTokenExpiraEm = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32());
                cliente.MercadoLivreUserId = json.GetProperty("user_id").GetInt64();
                cliente.MercadoLivreConectado = true;

                await _dbContext.SaveChangesAsync();
            }

            return new MercadoLivreToken
            {
                AccessToken = cliente.MercadoLivreAccessToken!,
                RefreshToken = cliente.MercadoLivreRefreshToken!,
                ExpirationDate = cliente.MercadoLivreTokenExpiraEm!.Value,
                UserId = cliente.MercadoLivreUserId ?? 0
            };
        }
    }
}
