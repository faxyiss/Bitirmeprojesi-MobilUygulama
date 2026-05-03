using System.Net.Http.Json;
using System.Text.Json;
using Bitirme_Projesi.Models;

namespace Bitirme_Projesi.Services
{
	public class CommentService
	{
		private readonly HttpClient _httpClient;

		// API'nin base URL'ini buraya yaz. Kendi lokal IP adresini (örn: 192.168.1.100) kullanman gerekebilir.
		private readonly string _baseUrl = "http://31.210.36.10:5000/api/Comment";

		public CommentService()
		{
			_httpClient = new HttpClient();
		}

		// Yorumları Getir
		public async Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(Guid postId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_baseUrl}/post/{postId}");
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<List<CommentResponseDto>>();
					return result ?? new List<CommentResponseDto>();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Yorumları çekerken hata: {ex.Message}");
			}
			return new List<CommentResponseDto>();
		}

		// Yorum Ekle
		public async Task<CommentResponseDto?> AddCommentAsync(CreateCommentDto dto)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync(_baseUrl, dto);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadFromJsonAsync<CommentResponseDto>();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Yorum eklerken hata: {ex.Message}");
			}
			return null;
		}

		// Yorum Beğen / Beğenmekten Vazgeç
		public async Task<int?> ToggleCommentLikeAsync(Guid commentId, Guid userId)
		{
			try
			{
				var response = await _httpClient.PostAsync($"{_baseUrl}/toggle-like/{commentId}/{userId}", null);
				if (response.IsSuccessStatusCode)
				{
					using var responseStream = await response.Content.ReadAsStreamAsync();
					using var jsonDocument = await JsonDocument.ParseAsync(responseStream);
					return jsonDocument.RootElement.GetProperty("newLikeCount").GetInt32();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Beğeni hatası: {ex.Message}");
			}
			return null;
		}
	}
}