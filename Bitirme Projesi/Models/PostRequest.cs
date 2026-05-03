using System;
using System.Collections.Generic;
using System.Text;

namespace Bitirme_Projesi.Models
{
	public class PostResponseDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public string InitialPhotoPath { get; set; }
		public string AfterPhotoPath { get; set; }

		public string FullInitialPhotoUrl => string.IsNullOrEmpty(InitialPhotoPath)
			? "dumy_photo.png" : $"http://31.210.36.10:5000/Photos/{InitialPhotoPath}";

		public string FullAfterPhotoUrl => string.IsNullOrEmpty(AfterPhotoPath)
			? "dumy_photo.png" : $"http://31.210.36.10:5000/Photos/{AfterPhotoPath}";
	}

	public class PostDetailDto
	{
		public Guid PostId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }

		public decimal PostLat { get; set; }
		public decimal PostLng { get; set; }

		public string LocationName { get; set; }

		public string InitialPhotoPath { get; set; }
		public string AfterPhotoPath { get; set; }
		public int TaskPoint { get; set; }
		public int LikeCount { get; set; }

		// Görsellerin Tam URL'leri
		public string FullInitialPhotoUrl => string.IsNullOrEmpty(InitialPhotoPath)
			? "dumy_photo.png" : $"http://31.210.36.10:5000/Photos/{InitialPhotoPath}";

		public string FullAfterPhotoUrl => string.IsNullOrEmpty(AfterPhotoPath)
			? "dumy_photo.png" : $"http://31.210.36.10:5000/Photos/{AfterPhotoPath}";
	}
}
