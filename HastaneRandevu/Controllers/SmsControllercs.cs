using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using HastaneRandevu.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly Context _context;
        private readonly HttpClient _httpClient;

        public SmsController(Context context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// SMS gönderme endpoint'i. (POST ve GET ile test edilebilir)
        /// </summary>
        /// <param name="id">Randevu Id</param>
        /// <param name="telefonNo">Telefon numarası</param>
        /// <param name="mesaj">Mesaj içeriği</param>
        [HttpPost("send")]
        public async Task<IActionResult> SendSms(int id, string telefonNo, string mesaj)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
                return NotFound("Randevu bulunamadı.");

            try
            {
                // Sahte SMS sağlayıcı URL (gerçek değil, simülasyon)
                string smsApiUrl = "https://fake-sms-provider.com/api/send";

                var payload = new
                {
                    phone = telefonNo,
                    message = mesaj
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                // NOT: Aşağıdaki satır, gerçek bir SMS servisine bağlanmadığı için hata verebilir.
                // Test ortamı için istersen bu satırı yorum satırına alıp altındaki sahte response ile devam edebilirsin.
                //var response = await _httpClient.PostAsync(smsApiUrl, jsonContent);

                // SİMÜLASYON (gerçek API'yi kullanmıyorsan aşağıdaki satırı kullan)
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

                if (response.IsSuccessStatusCode)
                {
                    randevu.SmsGonderildi = true;
                    await _context.SaveChangesAsync();
                    return Ok("SMS gönderildi ve randevu kaydı güncellendi.");
                }

                return StatusCode((int)response.StatusCode, "SMS gönderilemedi.");
            }
            catch (Exception ex)
            {
                // Gerçek sistemde loglaman tavsiye edilir
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// SMS iletildi bilgisini günceller.
        /// </summary>
        /// <param name="id">Randevu Id</param>
        /// <param name="iletildi">SMS iletildi mi?</param>
        [HttpPost("delivery-status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, bool iletildi)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
                return NotFound("Randevu bulunamadı.");

            randevu.SmsIletildi = iletildi;
            await _context.SaveChangesAsync();

            return Ok("İletim bilgisi güncellendi.");
        }
    }
}
