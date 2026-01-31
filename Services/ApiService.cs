using Kanban.Models;
using Kanban.Services;
using System.Text.Json;

namespace Kanban.Services
{
    public class ApiService
    {
        private readonly UserService _userService;

        public ApiService(UserService userService)
        {
            _userService = userService;
        }

        // =========================
        // CADASTRO DE USUÁRIO
        // =========================
        public void RegistrarUsuario(string nome, string email, string senha, string plano)
        {
            var user = new User
            {
                Nome = nome,
                Email = email,
                Plano = plano,
                DataCadastro = DateTime.Now,
                DataExpiracao = DateTime.Now.AddMonths(1)
            };

            // passa a senha separada para gerar o hash
            _userService.CriarUsuario(user, senha);
        }

        // =========================
        // LOGIN DE USUÁRIO
        // =========================
        public bool ValidarLogin(string email, string senha)
        {
            return _userService.ValidarLogin(email, senha);
        }

        // =========================
        // CONSULTA DE USUÁRIO
        // =========================
        public User? ObterUsuario(string email)
        {
            return _userService.GetByEmail(email);
        }

        // =========================
        // LISTAR TODOS
        // =========================
        public List<User> ListarUsuarios()
        {
            return _userService.GetAllUsers();
        }
    }
}
