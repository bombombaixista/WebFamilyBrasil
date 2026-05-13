namespace WebFamilyBrasil.Models
{
    public class Insumo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; } // Ex: Semente, Fertilizante, Defensivo
        public int Quantidade { get; set; }
        public decimal CustoUnitario { get; set; }
    }
}
