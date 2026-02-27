namespace Kanban.Models.MercadoLivre
{
    public class SearchResponseDto
    {
        public string? Query { get; set; }
        public PagingDto? Paging { get; set; }
        public List<ProdutoDto>? Results { get; set; }
    }

    public class PagingDto
    {
        public int Total { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }

    public class ProdutoDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public double Price { get; set; }
        public string? Thumbnail { get; set; }
        public string? Permalink { get; set; }
    }
}