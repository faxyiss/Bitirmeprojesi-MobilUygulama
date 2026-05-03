using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;
using System.Collections.ObjectModel;

namespace Bitirme_Projesi.Pages;

[QueryProperty(nameof(PostId), "postId")]
public partial class PostDetailPage : ContentPage
{
	private readonly IPostService _postService;
	private readonly CommentService _commentService;

	private ObservableCollection<CommentResponseDto> _comments;
	public string PostId { get; set; }
	public PostDetailPage()
	{
		InitializeComponent();
		_postService = new PostService();
		_commentService = new CommentService();
		_comments = new ObservableCollection<CommentResponseDto>();
		ColComments.ItemsSource = _comments;

		Shell.SetBackButtonBehavior(this, new BackButtonBehavior
		{
			Command = new Command(async () => {
				await Shell.Current.GoToAsync("//Profile");
			}),
			IsEnabled = true
		});
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadDataAsync();
		
	}
	private async Task LoadDataAsync()
	{
		try
		{
			if (!string.IsNullOrEmpty(PostId))
			{
				// Gönderi Detayını Çek
				var detail = await _postService.GetPostDetailAsync(Guid.Parse(PostId));
				if (detail != null)
				{
					this.BindingContext = detail;
				}

				// Yorumları Çek
				var comments = await _commentService.GetCommentsByPostIdAsync(Guid.Parse(PostId));
				_comments.Clear();
				foreach (var comment in comments)
				{
					_comments.Add(comment);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Veri yükleme hatası: {ex.Message}");
		}
	}
	// Yorum Penceresini Aç
	private void OnOpenCommentPopupClicked(object sender, EventArgs e)
	{
		CommentPopup.IsVisible = true;
		EdiNewComment.Focus();
	}
	// Yorum Penceresini Kapat
	private void OnCloseCommentPopupClicked(object sender, EventArgs e)
	{
		CommentPopup.IsVisible = false;
		EdiNewComment.Text = string.Empty;
	}

	// Yorumu Gönder
	private async void OnSendCommentClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(EdiNewComment.Text))
		{
			await DisplayAlertAsync("Uyarı", "Boş yorum gönderemezsiniz.", "Tamam");
			return;
		}

		// TODO: Giriş yapmış kullanıcının ID'sini buradan çekmelisin (Örn: Preferences)
		Guid currentUserId = UserSession.Instance.UserId;
		var newCommentDto = new CreateCommentDto
		{
			PostId = Guid.Parse(PostId),
			UserId = currentUserId,
			CommentText = EdiNewComment.Text.Trim()
		};

		var addedComment = await _commentService.AddCommentAsync(newCommentDto);

		if (addedComment != null)
		{
			// Yeni yorumu listenin en başına (index 0) ekliyoruz
			_comments.Insert(0, addedComment);
			OnCloseCommentPopupClicked(null, null);
		}
		else
		{
			await DisplayAlertAsync("Hata", "Yorum gönderilemedi.", "Tamam");
		}
	}

	private async void OnLikeTapped(object sender, TappedEventArgs e)
	{
		if (string.IsNullOrEmpty(PostId)) return;

		// TODO: Uygulamanın giriş yapmış kullanıcı ID'sini buradan almalısın. 
		// Örnek olarak UserSession veya Preferences kullanıyorsundur:
		Guid currentUserId = UserSession.Instance.UserId;

		// API'ye isteği at ve yeni beğeni sayısını al
		var newLikeCount = await _postService.ToggleLikeAsync(Guid.Parse(PostId), currentUserId);

		if (newLikeCount.HasValue)
		{
			// Ekrandaki sayıyı manuel olarak anında güncelle
			LblLikeCount.Text = $"{newLikeCount.Value} Beğeni";

			// İstersen burada kalp ikonunu doldurup boşaltabilirsin (heart_filled.png)
			// ImgHeart.Source = "heart_filled.png"; 
		}
	}

	private async void OnCommentLikeTapped(object sender, TappedEventArgs e)
	{
		// DİKKAT: Artık tıklanan alan bir HorizontalStackLayout değil, Grid (veya genel View)
		var layout = sender as View;
		var clickedComment = layout?.BindingContext as CommentResponseDto;

		if (clickedComment != null)
		{
			Guid currentUserId = UserSession.Instance.UserId;

			var newLikeCount = await _commentService.ToggleCommentLikeAsync(clickedComment.Id, currentUserId);

			if (newLikeCount.HasValue)
			{
				clickedComment.LikeCount = newLikeCount.Value;

				var index = _comments.IndexOf(clickedComment);
				if (index >= 0)
				{
					_comments.RemoveAt(index);
					_comments.Insert(index, clickedComment);
				}
			}
		}
	}
}