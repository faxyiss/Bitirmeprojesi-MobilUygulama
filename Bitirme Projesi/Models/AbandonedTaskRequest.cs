namespace Bitirme_Projesi.Models
{
	public class CancelTaskDto
	{
		public Guid TaskId { get; set; }
		public Guid UserId { get; set; }
		public string Reason { get; set; } = "Cancel"; // Kullanıcı butona basarak iptal ettiği için "Cancel"
	}
	public class AbandonedTaskDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Reason { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime AbandonedAt { get; set; }
		public string PhotoPath { get; set; }

		public string FullPhotoUrl => string.IsNullOrEmpty(PhotoPath)
			? "dumy_photo.png"
			: $"http://31.210.36.10:5000/Photos/{PhotoPath}";

		// Arayüzde "İptal Nedeni: Zaman Aşımı" gibi göstermek için yardımcı özellik
		public string DisplayReason => Reason == "Timeout" ? "Zaman Aşımı" : "Kullanıcı İptali";
	}
}
