using System.Net.Http.Json;

namespace Bitirme_Projesi.Services
{
	// 1. Interface (Arayüz) Tanımlaması
	public interface IPhotoService
	{
		Task<Guid?> UploadPhotoAsync(FileResult photoFile, Guid userId);
	}

	// 2. Servis Sınıfı
	public class PhotoService : IPhotoService
	{
		private readonly HttpClient _httpClient;

		// AuthService'de kullandığın IP adresini burada da kullanıyoruz
		private const string BaseUrl = "http://31.210.36.10:5000/api/Photo";

		public PhotoService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<Guid?> UploadPhotoAsync(FileResult photoFile, Guid userId)
		{
			if (photoFile == null) return null;

			try
			{
				using var stream = await photoFile.OpenReadAsync();
				using var content = new MultipartFormDataContent();

				// 1. Dosya içeriği: İsim 'file' olmalı (Backend ile aynı)
				var fileContent = new StreamContent(stream);
				fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photoFile.ContentType);
				content.Add(fileContent, "file", photoFile.FileName); // "file" anahtarı kritik!

				// 2. Kullanıcı ID'si:
				content.Add(new StringContent(userId.ToString()), "userId");

				// İsteği gönder
				var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);

				if (response.IsSuccessStatusCode)
				{
					// Başarılıysa Guid ID'yi dön
					return await response.Content.ReadFromJsonAsync<Guid>();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Yükleme Hatası: {ex.Message}");
				await Shell.Current.DisplayAlertAsync("Hata", ex.Message, "Tamam");
			}
			return null;
		}
	}
}