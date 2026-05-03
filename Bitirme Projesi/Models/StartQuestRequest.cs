namespace Bitirme_Projesi.Models
{
	public class CreateActiveTaskDto
	{
		public Guid UserId { get; set; }
		public string Title { get; set; }
		public Guid InitialPhotoId { get; set; } // PhotoService'den dönen ID
		public double InitialLat { get; set; }
		public double InitialLng { get; set; }
	}
	public class ActiveTaskDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public double InitialLat { get; set; }
		public double InitialLng { get; set; }
		public DateTime CreatedAt { get; set; }
		public string PhotoPath { get; set; } // Backend'den gelen dosya adı (örn: abc.jpg)

		// MAUI'de Image kontrolünün doğrudan okuyabilmesi için tam URL'yi oluşturan yardımcı özellik
		public string FullPhotoUrl => string.IsNullOrEmpty(PhotoPath)
			? "dumy_photo.png"
			: $"http://31.210.36.10:5000/Photos/{PhotoPath}";
	}
}
