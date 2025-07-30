namespace HastaneRandevu.Models
{
    public class Poliklinik
    {
        public int Id { get; set; }
        public string PoliklinikAdi { get; set; }
        public List <Randevu>? Randevular { get; set; }
    }
}
