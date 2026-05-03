using System;
using System.Collections.Generic;
using System.Text;

namespace Bitirme_Projesi.Models
{
	public class UserProfileDto
	{
		public Guid UserId { get; set; }
		public string FullName { get; set; }
		public string Bio { get; set; }
		public int TotalPoints { get; set; }
		public int CompletedTaskCount { get; set; }
		public int PostCount { get; set; }
		public string ProfilePhotoPath { get; set; }
	}

	public class UserProfileUpdateDto
	{
		public Guid UserId { get; set; }
		public string FullName { get; set; }
		public string Bio { get; set; }
		public Guid? NewPhotoId { get; set; }
	}
}
