using System;
using System.Collections.Generic;
using System.Text;

namespace Bitirme_Projesi.Models
{
	public class CommentResponseDto
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public Guid UserId { get; set; }
		public string CommentText { get; set; }
		public int LikeCount { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Username { get; set; }
		public string ProfilePhotoPath { get; set; }
		public string FullProfilePhotoUrl
		{
			get
			{
				// Eğer path boşsa varsayılan avatarı kullan
				if (string.IsNullOrEmpty(ProfilePhotoPath))
					return "user_avatar.png";

				// Path zaten tam adres ise direkt döndür
				if (ProfilePhotoPath.StartsWith("http"))
					return ProfilePhotoPath;

				// Değilse, kendi sunucu adresini başına ekle
				return $"http://31.210.36.10:5000/Photos/{ProfilePhotoPath.TrimStart('/')}";
			}
		}
	}

	public class CreateCommentDto
	{
		public Guid PostId { get; set; }
		public Guid UserId { get; set; }
		public string CommentText { get; set; }
	}
}
