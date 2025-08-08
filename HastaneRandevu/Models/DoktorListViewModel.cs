using System.Collections.Generic;

namespace HastaneRandevu.Models
{
    public class DoktorListViewModel
    {
        public List<Doktor> Doktorlar { get; set; }
        public List<string> Branslar { get; set; }
        public string SeciliBrans { get; set; }
        public string Arama { get; set; }
    }
}