using Bitirme_Projesi.Models;

namespace Bitirme_Projesi.Services
{
	public class UserSession
	{
		public static UserSession Instance { get; } = new UserSession();

		public Guid UserId { get; set; }
		public string FullName { get; set; }
		public string PhotoUrl { get; set; }
		public string Bio { get; set; }

		// Bilgiler güncellendiğinde arayüzü haberdar etmek için event
		public event Action OnProfileUpdated;

		public void UpdateSession(string name, string photo, string bio)
		{
			FullName = name;
			PhotoUrl = photo;
			Bio = bio;
			OnProfileUpdated?.Invoke(); // Abone olan tüm sayfalar kendini yeniler
		}

		public void UpdateFromDto(UserProfileDto profile)
		{
			FullName = profile.FullName;
			Bio = profile.Bio;

			// URL sonuna ?t=123123 gibi benzersiz bir sayı ekleyerek 
			// Image kontrolünün "bu yeni bir resim" demesini sağlıyoruz.
			if (!string.IsNullOrEmpty(profile.ProfilePhotoPath))
			{
				PhotoUrl = $"{profile.ProfilePhotoPath}?t={DateTime.Now.Ticks}";
			}

			OnProfileUpdated?.Invoke();
		}
	}
}
