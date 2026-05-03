using System;
using System.Collections.Generic;
using System.Text;

namespace Bitirme_Projesi.Models
{
	public class CompleteTaskRequestDto
	{
		public Guid TaskId { get; set; }
		public Guid UserId { get; set; }
		public Guid AfterPhotoId { get; set; }
		public double AfterLat { get; set; }
		public double AfterLng { get; set; }
		public string UserDescription { get; set; }
	}

	public class CompletedTaskDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		public int? Point { get; set; }
		public DateTime CompletedAt { get; set; }

		// Her iki fotoğraf yolu da burada olmalı
		public string InitialPhotoPath { get; set; } // YENİ
		public string AfterPhotoPath { get; set; }

		public bool IsShared { get; set; }
		// İlk Hali URL
		public string FullInitialPhotoUrl => string.IsNullOrEmpty(InitialPhotoPath)
			? "dumy_photo.png"
			: $"http://31.210.36.10:5000/Photos/{InitialPhotoPath}";

		// Son Hali URL (FullPhotoUrl ismini daha net olması için değiştirebilirsin)
		public string FullAfterPhotoUrl => string.IsNullOrEmpty(AfterPhotoPath)
			? "dumy_photo.png"
			: $"http://31.210.36.10:5000/Photos/{AfterPhotoPath}";

		public string PointDisplay => Point.HasValue ? $"{Point} Puan Kazanıldı!" : "Puan Bekleniyor...";

		public Color StatusColor => Status == "Onaylandı" ? Color.FromArgb("#40916C") :
								   (Status == "Reddedildi" ? Color.FromArgb("#BC4749") :
									Color.FromArgb("#F4A261"));

		// Paylaşım kuralı[cite: 19]
		public bool IsShareable => Point > 0 && Status == "Onaylandı" && !IsShared;
	}
}
