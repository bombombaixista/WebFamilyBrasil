using Kanban.Models;
using System.Text.Json;

namespace Kanban.Services
{
    public class UsuarioService
    {
        private readonly string _path = Path.Combine("Data", "Json", "usuarios.json");

        // Lista todos os usuários
        public List<User> Listar()
        {
            if (!File.Exists(_path)) return new List<User>();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        // Busca usuário por email
        public User? GetPorEmail(string email)
        {
            return Listar().FirstOrDefault(u => u.Email == email);
        }

        // Cria usuário
        public void Criar(User user)
        {
            var usuarios = Listar();
            usuarios.Add(user);
            File.WriteAllText(_path, JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
