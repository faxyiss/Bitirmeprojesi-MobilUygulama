using Bitirme_Projesi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bitirme_Projesi.Services
{
	public interface IPostService
	{
		Task<bool> CreatePostAsync(Guid taskId, Guid userId, string title, string content);
		Task<List<PostResponseDto>> GetMyPostsAsync(Guid userId);
		Task<bool> DeletePostAsync(Guid postId);
		Task<PostDetailDto> GetPostDetailAsync(Guid postId);
		Task<int?> ToggleLikeAsync(Guid postId, Guid userId);

		// YENİ EKLENEN TIMELINE METOTLARI
		Task<PagedResponseDto<PostResponseDto>> GetRecommendedTimelineAsync(int page, int pageSize);
		Task<PagedResponseDto<PostResponseDto>> GetNearbyTimelineAsync(decimal lat, decimal lng, int page, int pageSize);
	}

	public class PostService : IPostService
	{
		private readonly HttpClient _httpClient;
		private const string BaseUrl = "http://31.210.36.10:5000/api/Post";

		public PostService()
		{
			_httpClient = new HttpClient();
		}

		public async Task<bool> CreatePostAsync(Guid taskId, Guid userId, string title, string content)
		{
			var postData = new { taskId, userId, title, content };
			var json = JsonSerializer.Serialize(postData);
			var body = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync($"{BaseUrl}/create", body);
			return response.IsSuccessStatusCode;
		}

		public async Task<List<PostResponseDto>> GetMyPostsAsync(Guid userId)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<PostResponseDto>>($"{BaseUrl}/user/{userId}");
				return response ?? new List<PostResponseDto>();
			}
			catch
			{
				return new List<PostResponseDto>();
			}
		}

		public async Task<bool> DeletePostAsync(Guid postId)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"{BaseUrl}/{postId}");
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}

		public async Task<PostDetailDto> GetPostDetailAsync(Guid postId)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<PostDetailDto>($"{BaseUrl}/detail/{postId}");
				return response;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Detay Çekme Hatası: {ex.Message}");
				return null;
			}
		}

		public async Task<int?> ToggleLikeAsync(Guid postId, Guid userId)
		{
			try
			{
				var response = await _httpClient.PostAsync($"{BaseUrl}/toggle-like/{postId}/{userId}", null);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<JsonElement>();
					return result.GetProperty("newLikeCount").GetInt32();
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		// --- YENİ: ÖNERİLENLERİ (EN ÇOK BEĞENİLENLER) GETİR ---
		public async Task<PagedResponseDto<PostResponseDto>> GetRecommendedTimelineAsync(int page, int pageSize)
		{
			try
			{
				var url = $"{BaseUrl}/timeline/recommended?page={page}&pageSize={pageSize}";
				var response = await _httpClient.GetFromJsonAsync<PagedResponseDto<PostResponseDto>>(url);
				return response;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Önerilenler Çekme Hatası: {ex.Message}");
				return null;
			}
		}

		// --- YENİ: YAKINDAKİLERİ (KONUMA GÖRE) GETİR ---
		public async Task<PagedResponseDto<PostResponseDto>> GetNearbyTimelineAsync(decimal lat, decimal lng, int page, int pageSize)
		{
			try
			{
				// InvariantCulture ile ondalıklı sayıların virgül(,) yerine nokta(.) ile gitmesini garantiliyoruz
				string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
				string lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);

				var url = $"{BaseUrl}/timeline/nearby?lat={latStr}&lng={lngStr}&page={page}&pageSize={pageSize}";
				var response = await _httpClient.GetFromJsonAsync<PagedResponseDto<PostResponseDto>>(url);
				return response;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Yakındakiler Çekme Hatası: {ex.Message}");
				return null;
			}
		}
	}
}