using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class MyPostsPage : ContentPage
{
	private readonly IPostService _postService;

	public MyPostsPage()
	{
		InitializeComponent();
		_postService = new PostService(); // DI kullanıyorsan Constructor'dan alabilirsin
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadMyPosts();
	}

	private async Task LoadMyPosts()
	{
		var posts = await _postService.GetMyPostsAsync(UserSession.Instance.UserId);
		ColMyPosts.ItemsSource = posts;
	}

	private async void OnRemovePostClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button?.CommandParameter is PostResponseDto post)
		{
			// Silmeden önce kullanıcıdan onay alıyoruz
			bool isConfirmed = await DisplayAlertAsync("Gönderiyi Kaldır", "Bu gönderiyi silmek istediğine emin misin? Bu işlem gönderiyi akıştan kaldırır ve görevi tekrar paylaşılabilir hale getirir.", "Evet, Kaldır", "Vazgeç");

			if (isConfirmed)
			{
				// API'ye silme isteğini gönder
				bool isSuccess = await _postService.DeletePostAsync(post.Id);

				if (isSuccess)
				{
					await DisplayAlertAsync("Başarılı", "Gönderi başarıyla kaldırıldı.", "Tamam");

					// Listeyi yenilemek için sayfayı tazeleyen fonksiyonu çağırıyoruz
					await LoadMyPosts();
				}
				else
				{
					await DisplayAlertAsync("Hata", "Gönderi kaldırılırken bir sorun oluştu.", "Tamam");
				}
			}
		}
	}
	private async void OnPostDetailClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button?.CommandParameter is PostResponseDto post)
		{
			// Gönderi ID'sini parametre olarak gönderiyoruz
			await Shell.Current.GoToAsync($"PostDetail?postId={post.Id}");
		}
	}
}