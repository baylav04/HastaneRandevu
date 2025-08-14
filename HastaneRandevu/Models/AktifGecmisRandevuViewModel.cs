using System.Collections.Generic;

namespace HastaneRandevu.Models
{
    public class AktifGecmisRandevuViewModel
    {
        public List<Randevu> AktifRandevular { get; set; } = new();
        public List<Randevu> GecmisRandevular { get; set; } = new();
    }
}
