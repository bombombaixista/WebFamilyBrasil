using Kanban.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Kanban.Services
{
    public class UserService
    {
        private readonly string _baseDataPath = "Data";

        public UserService()
        {
            if (!Directory.Exists(_baseDataPath))
                Directory.CreateDirectory(_baseDataPath);

            var userJsonPath = Path.Combine(_baseDataPath, "User.json");
            if (!File.Exists(userJsonPath))
                File.WriteAllText(userJsonPath, "[]");
        }

        // =========================
        // CRIAR USUÁRIO
        // =========================
        public void CriarUsuario(User user, string senha)
        {
            var usuarios = GetAllUsers();

            // gera Id automático como GUID
            user.Id = Guid.NewGuid();

            // gera hash da senha
            var hasher = new PasswordHasher<User>();
            user.SenhaHash = hasher.HashPassword(user, senha);

            usuarios.Add(user);
            SalvarUsuarios(usuarios);

            // cria pasta individual do usuário
            var safeUser = user.Email.Replace("@", "_").Replace(".", "_");
            var userPath = Path.Combine(_baseDataPath, safeUser);
            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);
        }

        // =========================
        // LISTAR TODOS
        // =========================
        public List<User> GetAllUsers()
        {
            var userJsonPath = Path.Combine(_baseDataPath, "User.json");
            return JsonSerializer.Deserialize<List<User>>(File.ReadAllText(userJsonPath)) ?? new List<User>();
        }

        // =========================
        // SALVAR
        // =========================
        public void SalvarUsuarios(List<User> usuarios)
        {
            var userJsonPath = Path.Combine(_baseDataPath, "User.json");
            File.WriteAllText(userJsonPath, JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true }));
        }

        // =========================
        // BUSCAR POR EMAIL
        // =========================
        public User? GetByEmail(string email)
        {
            var usuarios = GetAllUsers();
            return usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // =========================
        // VALIDAR LOGIN
        // =========================
        public bool ValidarLogin(string email, string senha)
        {
            var user = GetByEmail(email);
            if (user == null) return false;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.SenhaHash, senha);

            return result == PasswordVerificationResult.Success;
        }

        // =========================
        // CAMINHO DA PASTA DO USUÁRIO
        // =========================
        public string GetUserPath(string email)
        {
            var safeUser = email.Replace("@", "_").Replace(".", "_");
            return Path.Combine(_baseDataPath, safeUser);
        }
    }
}
