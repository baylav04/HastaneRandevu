namespace HastaneRandevu.DTOs
{
    public class HastaSifreDegistirDTO
    {        
        public required string TCKimlikNo { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Sifre { get; set; }
    }
}
