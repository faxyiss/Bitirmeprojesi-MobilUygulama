using Bitirme_Projesi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Bitirme_Projesi.Services
{
	public interface IActiveTaskService
	{
		Task<bool> CreateTaskAsync(CreateActiveTaskDto model);
		Task<List<ActiveTaskDto>> GetMyActiveTasksAsync(Guid userId);
		Task<List<AbandonedTaskDto>> GetMyAbandonedTasksAsync(Guid userId); // Interface'e ekle
		Task<bool> CancelTaskAsync(CancelTaskDto model);
		Task<bool> DeleteAbandonedTaskAsync(Guid taskId, Guid userId);
		Task<bool> CompleteTaskAsync(CompleteTaskRequestDto request);

		Task<List<CompletedTaskDto>> GetMyCompletedTasksAsync(Guid userId);
	}

	public class ActiveTaskService : IActiveTaskService
	{
		private readonly HttpClient _httpClient;
		private const string BaseUrl = "http://31.210.36.10:5000/api/ActiveTask";

		public ActiveTaskService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> CreateTaskAsync(CreateActiveTaskDto model)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/create", model);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Görev Hatası: {ex.Message}");
				return false;
			}
		}

		public async Task<List<ActiveTaskDto>> GetMyActiveTasksAsync(Guid userId)
		{
			try
			{
				// Backend'deki ilgili uç noktaya GET isteği atıyoruz
				var response = await _httpClient.GetFromJsonAsync<List<ActiveTaskDto>>($"{BaseUrl}/user/{userId}");
				return response ?? new List<ActiveTaskDto>();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Görevleri Çekme Hatası: {ex.Message}");
				return new List<ActiveTaskDto>();
			}
		}

		

		public async Task<List<AbandonedTaskDto>> GetMyAbandonedTasksAsync(Guid userId)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<AbandonedTaskDto>>($"{BaseUrl}/abandoned/{userId}");
				return response ?? new List<AbandonedTaskDto>();
			}
			catch { return new List<AbandonedTaskDto>(); }
		}


		public async Task<bool> CancelTaskAsync(CancelTaskDto model)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/cancel", model);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"İptal Hatası: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> DeleteAbandonedTaskAsync(Guid taskId, Guid userId)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"{BaseUrl}/abandoned/{taskId}/user/{userId}");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Silme Hatası: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> CompleteTaskAsync(CompleteTaskRequestDto request)
		{
			try
			{
				// Backend'deki [HttpPost("complete")] uç noktasına JSON olarak istek atıyoruz
				var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/complete", request);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Görev bitirme hatası: {ex.Message}");
				return false;
			}
		}

		public async Task<List<CompletedTaskDto>> GetMyCompletedTasksAsync(Guid userId)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<CompletedTaskDto>>($"{BaseUrl}/completed/{userId}");
				return response ?? new List<CompletedTaskDto>();
			}
			catch { return new List<CompletedTaskDto>(); }
		}
	}
}
