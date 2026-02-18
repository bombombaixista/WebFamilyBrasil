using Kanban.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Kanban.Services
{
    public interface IMercadoLivreAuthService
    {
        Task<MercadoLivreToken> GetValidTokenAsync(Guid clienteId);
        Task<MercadoLivreToken> SaveInitialTokenAsync(Guid clienteId, MercadoLivreToken token);
    }

    public class MercadoLivreAuthService : IMercadoLivreAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public MercadoLivreAuthService(
            AppDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _clientId = configuration["MercadoLivre:ClientId"]!;
            _clientSecret = configuration["MercadoLivre:ClientSecret"]!;
        }

        // 🔹 Retorna token válido (renova se necessário)
        public async Task<MercadoLivreToken> GetValidTokenAsync(Guid clienteId)
        {
            var cliente = await _dbContext.Clientes2.FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null || string.IsNullOrEmpty(cliente.MercadoLivreAccessToken))
                throw new Exception("Cliente não conectado ao Mercado Livre.");

            // Se expirado, renova
            if (cliente.MercadoLivreTokenExpiraEm <= DateTime.UtcNow)
            {
                var newToken = await RefreshTokenAsync(cliente);
                return newToken;
            }

            return new MercadoLivreToken
            {
                AccessToken = cliente.MercadoLivreAccessToken!,
                RefreshToken = cliente.MercadoLivreRefreshToken!,
                ExpirationDate = cliente.MercadoLivreTokenExpiraEm!.Value,
                UserId = cliente.MercadoLivreUserId ?? 0
            };
        }

        // 🔹 Salva o primeiro token obtido no Callback
        public async Task<MercadoLivreToken> SaveInitialTokenAsync(Guid clienteId, MercadoLivreToken token)
        {
            var cliente = await _dbContext.Clientes2.FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            cliente.MercadoLivreAccessToken = token.AccessToken;
            cliente.MercadoLivreRefreshToken = token.RefreshToken;
            cliente.MercadoLivreTokenExpiraEm = token.ExpirationDate;
            cliente.MercadoLivreUserId = token.UserId;
            cliente.MercadoLivreConectado = true;

            await _dbContext.SaveChangesAsync();
            return token;
        }

        // 🔹 Fluxo de refresh
        private async Task<MercadoLivreToken> RefreshTokenAsync(Cliente2 cliente)
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

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            var newToken = new MercadoLivreToken
            {
                AccessToken = json.GetProperty("access_token").GetString()!,
                RefreshToken = json.GetProperty("refresh_token").GetString()!,
                TokenType = json.GetProperty("token_type").GetString()!,
                ExpiresIn = json.GetProperty("expires_in").GetInt32(),
                ExpirationDate = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32()),
                UserId = json.GetProperty("user_id").GetInt64()
            };

            // Atualiza cliente
            cliente.MercadoLivreAccessToken = newToken.AccessToken;
            cliente.MercadoLivreRefreshToken = newToken.RefreshToken;
            cliente.MercadoLivreTokenExpiraEm = newToken.ExpirationDate;
            cliente.MercadoLivreUserId = newToken.UserId;

            await _dbContext.SaveChangesAsync();

            return newToken;
        }
    }
}
