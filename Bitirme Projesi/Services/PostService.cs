using Bitirme_Projesi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Bitirme_Projesi.Services
{
	public interface IPostService
	{
		Task<bool> CreatePostAsync(Guid taskId, Guid userId, string title, string content);
	    Task<List<PostResponseDto>> GetMyPostsAsync(Guid userId);
		Task<bool> DeletePostAsync(Guid postId);
		Task<PostDetailDto> GetPostDetailAsync(Guid postId);
		Task<int?> ToggleLikeAsync(Guid postId, Guid userId);
	}
	public class PostService : IPostService
	{
		private readonly HttpClient _httpClient;
		private const string ApiUrl = "http://31.210.36.10:5000/api/Post/create";

		public PostService()
		{
			_httpClient = new HttpClient();
		}

		public async Task<bool> CreatePostAsync(Guid taskId, Guid userId, string title, string content)
		{
			var postData = new { taskId, userId, title, content };
			var json = JsonSerializer.Serialize(postData);
			var body = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(ApiUrl, body);
			return response.IsSuccessStatusCode;
		}
		public async Task<List<PostResponseDto>> GetMyPostsAsync(Guid userId)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<PostResponseDto>>($"http://31.210.36.10:5000/api/Post/user/{userId}");
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
				var response = await _httpClient.DeleteAsync($"http://31.210.36.10:5000/api/Post/{postId}");
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
				// Backend'deki detail uç noktasına bağlanıyoruz
				var response = await _httpClient.GetFromJsonAsync<PostDetailDto>($"http://31.210.36.10:5000/api/Post/detail/{postId}");
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
				var response = await _httpClient.PostAsync($"http://31.210.36.10:5000/api/Post/toggle-like/{postId}/{userId}", null);

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
	}
}
