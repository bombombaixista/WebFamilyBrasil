using System.Security.Claims;

namespace Kanban.Helpers
{
    public static class UserDataHelper
    {
        // Normaliza o email para nome de pasta
        public static string NormalizeEmail(string email)
        {
            return email.Replace("@", "_").Replace(".", "_");
        }

        // Retorna o caminho da pasta do usuário
        public static string GetUserDataPath(IWebHostEnvironment env, ClaimsPrincipal user)
        {
            var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Usuário não logado.");

            var folderName = NormalizeEmail(email);
            var userPath = Path.Combine(env.ContentRootPath, "Data", "User2", folderName);

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            return userPath;
        }

        // Retorna o caminho completo de um arquivo dentro da pasta do usuário
        public static string GetFilePath(IWebHostEnvironment env, ClaimsPrincipal user, string fileName)
        {
            var userPath = GetUserDataPath(env, user);
            return Path.Combine(userPath, fileName);
        }

        // Cria arquivos iniciais vazios se não existirem
        public static void CriarEstruturaInicial(IWebHostEnvironment env, string email)
        {
            var folderName = NormalizeEmail(email);
            var basePath = Path.Combine(env.ContentRootPath, "Data", "User2", folderName);
            Directory.CreateDirectory(basePath);

            var arquivosIniciais = new[]
            {
                "tarefas.json", "pipeline.json", "agenda.json", "documento.json",
                "financeiro.json", "funcionarios.json", "fornecedores.json",
                "produtos.json", "movimentacoes.json", "pedidos.json", "transacoes.json"
            };

            foreach (var arquivo in arquivosIniciais)
            {
                var caminho = Path.Combine(basePath, arquivo);
                if (!System.IO.File.Exists(caminho))
                {
                    System.IO.File.WriteAllText(caminho, "[]");
                }
            }
        }
    }
}
