namespace HastaneRandevu.Models
{
    public class Doktor
    {
        internal object BransId;

        public int Id { get; set; }

        public string Ad { get; set; }

        public string? Unvan { get; set; }  

        public string? FotoUrl { get; set; } 

        public string? Detaylar { get; set; } 

        public int PoliklinikId { get; set; }

        public Poliklinik? Poliklinik { get; set; }
    }
}
