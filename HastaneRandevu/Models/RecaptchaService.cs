using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class RecaptchaResponse
{
    public bool success { get; set; }
    public string challenge_ts { get; set; }
    public string hostname { get; set; }
    public string[] error_codes { get; set; }
}

public class RecaptchaService
{
    private readonly string _secretKey = "6Lc3XpsrAAAAALTf1A_PmGCOptRkHal7cx3Hohg_";

    public async Task<bool> VerifyToken(string token)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={token}",
            null);

        var json = await response.Content.ReadAsStringAsync();
        var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(json);
        return recaptchaResponse.success;
    }
}
