using System.Net.Http.Json;

namespace Bitirme_Projesi.Services
{
	public class AuthService
	{
		private readonly HttpClient _httpClient;
		private const string BaseUrl = "http://31.210.36.10:5000/api/Auth/"; // Docker IP'n

		public AuthService()
		{
			_httpClient = new HttpClient();
		}

		// Giriş İşlemi
		public async Task<bool> LoginAsync(string email, string password)
		{
			var response = await _httpClient.PostAsJsonAsync(BaseUrl + "login", new { Email = email, Password = password });
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<UserResponse>();
				await SecureStorage.Default.SetAsync("user_id", result.Id);

				// KRİTİK EKLEME: Session'ı dolduruyoruz ki diğer sayfalar bu ID'yi kullanabilsin
				UserSession.Instance.UserId = Guid.Parse(result.Id);

				return true;
			}
			return false;
		}

		// Kayıt İşlemi
		public async Task<bool> RegisterAsync(string fullName, string email, string password)
		{
			var response = await _httpClient.PostAsJsonAsync(BaseUrl + "register", new { FullName = fullName, Email = email, Password = password });
			return response.IsSuccessStatusCode;
		}
	}

	public class UserResponse { public string Id { get; set; } }
}