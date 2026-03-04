using Kanban.Models;

namespace Kanban.Services
{
    public class ProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }

        public List<AfiliadoProduto> ObterFavoritos()
        {
            return _context.Set<AfiliadoProduto>()
                .Where(p => p.Favorito)
                .ToList();
        }

        public void FavoritarProduto(int id)
        {
            var produto = _context.Set<AfiliadoProduto>().Find(id);
            if (produto != null)
            {
                produto.Favorito = true;
                _context.SaveChanges();
            }
        }
    }
}
