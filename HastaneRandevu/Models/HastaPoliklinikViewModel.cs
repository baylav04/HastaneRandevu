using System.Collections.Generic;

namespace HastaneRandevu.Models
{
    public class HastaPoliklinikViewModel
    {
        public string AdSoyad { get; set; }
        public string TcNo { get; set; }
        public List<string> Poliklinikler { get; set; }
    }
}
