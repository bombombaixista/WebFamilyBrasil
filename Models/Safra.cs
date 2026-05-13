namespace WebFamilyBrasil.Models
{
    public class Safra
    {
        public int Id { get; set; }
        public string Talhao { get; set; }
        public string Cultura { get; set; }
        public DateTime DataPlantio { get; set; }
        public DateTime DataColheita { get; set; }
        public decimal CustoEstimado { get; set; }
        public decimal ProducaoEsperada { get; set; }
    }
}
