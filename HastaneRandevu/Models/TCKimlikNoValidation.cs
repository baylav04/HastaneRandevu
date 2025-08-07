using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HastaneRandevu.Validations
{
    public class TCKimlikNoValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("TC Kimlik No boş olamaz.");

            string tc = value.ToString();

            if (tc.Length != 11)
                return new ValidationResult("TC Kimlik No 11 haneli olmalıdır.");

            if (!tc.All(char.IsDigit))
                return new ValidationResult("TC Kimlik No sadece rakamlardan oluşmalıdır.");

            if (tc.StartsWith("0"))
                return new ValidationResult("TC Kimlik No 0 ile başlayamaz.");

            int[] digits = tc.Select(c => c - '0').ToArray();
            int sumOdd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int sumEven = digits[1] + digits[3] + digits[5] + digits[7];

            int digit10 = ((sumOdd * 7) - sumEven) % 10;
            if (digit10 < 0) digit10 += 10; // negatif olabilir, düzeltiyoruz

            int digit11 = digits.Take(10).Sum() % 10;

            if (digits[9] != digit10 || digits[10] != digit11)
                return new ValidationResult("Geçersiz TC Kimlik Numarası.");

            return ValidationResult.Success;
        }
    }
}
