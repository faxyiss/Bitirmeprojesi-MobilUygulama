using Bitirme_Projesi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Bitirme_Projesi.Services
{
	public interface IProfileService
	{
		Task<UserProfileDto> GetProfileAsync(Guid userId);
		Task<bool> UpdateProfileAsync(UserProfileUpdateDto dto);
	}
	public class ProfileService : IProfileService
	{
		private readonly HttpClient _httpClient;
		private const string BaseUrl = "http://31.210.36.10:5000/api/UserProfile";

		public ProfileService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<UserProfileDto> GetProfileAsync(Guid userId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{BaseUrl}/{userId}");
				if (response.IsSuccessStatusCode)
				{
					var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();

					if (profile != null && !string.IsNullOrEmpty(profile.ProfilePhotoPath))
					{
						// Fotoğraf adını tam internet adresine çeviriyoruz
						// Uzantı ve klasör yoluna dikkat et (wwwroot/Photos -> /Photos/)
						string fullUrl = $"http://31.210.36.10:5000/Photos/{profile.ProfilePhotoPath}";

						// MAUI'nin eski fotoğrafı göstermesini engellemek için zaman damgası ekle
						profile.ProfilePhotoPath = $"{fullUrl}?t={DateTime.Now.Ticks}";
					}
					else if (profile != null)
					{
						// Eğer fotoğraf yoksa varsayılan bir görsel ata
						profile.ProfilePhotoPath = "user_avatar.png";
					}

					return profile;
				}
			}
			catch (Exception ex) { /* Loglama */ }
			return null;
		}

		public async Task<bool> UpdateProfileAsync(UserProfileUpdateDto dto)
		{
			try
			{
				var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/update", dto);
				return response.IsSuccessStatusCode;
			}
			catch (Exception) { return false; }
		}
	}
}
