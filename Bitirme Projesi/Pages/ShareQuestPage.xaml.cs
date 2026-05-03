using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;
using System.Text.Json;
using System.Text;

namespace Bitirme_Projesi.Pages;

[QueryProperty(nameof(BeforePhoto), "before")]
[QueryProperty(nameof(AfterPhoto), "after")]
[QueryProperty(nameof(TaskId), "taskId")]
[QueryProperty(nameof(PostTitle), "title")]
public partial class ShareQuestPage : ContentPage
{
	public string BeforePhoto { get; set; } // İlk halinin URL'si
	public string AfterPhoto { get; set; }  // Son halinin URL'si
	public string TaskId { get; set; }
	public string PostTitle { get; set; }
	public ShareQuestPage()
	{
		InitializeComponent();
	}

	// Sayfa ekranda göründüğünde verileri arayüze yerleştiriyoruz
	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Başlığı yerleştir[cite: 4]
		EntShareTitle.Text = PostTitle;

		// İLK HALİ: URL boş değilse görseli yükle
		if (!string.IsNullOrEmpty(BeforePhoto))
			ImgBefore.Source = Uri.UnescapeDataString(BeforePhoto);

		// SON HALİ: URL boş değilse görseli yükle[cite: 3]
		if (!string.IsNullOrEmpty(AfterPhoto))
			ImgAfter.Source = Uri.UnescapeDataString(AfterPhoto);
	}


	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..", false); // Bir önceki sayfaya dön
	}

	private async void OnSharePostClicked(object sender, EventArgs e)
	{
		// Butonu geçici olarak kapat
		var btn = (Button)sender;
		btn.IsEnabled = false;

		// Paylaştığın PostService'i kullanıyoruz
		var postService = new PostService();

		// Servisi çağır (Guid.Parse ile string ID'yi Guid'e çeviriyoruz)
		var isSuccess = await postService.CreatePostAsync(
			Guid.Parse(TaskId),
			UserSession.Instance.UserId, //[cite: 16]
			EntShareTitle.Text,
			EdiShareDescription.Text);

		if (isSuccess)
		{
			await DisplayAlertAsync("Başarılı", "Gönderin haritada ve akışta paylaşıldı!", "Tamam");

			// Ana akış sayfasına (TimeLine) yönlendir
			await Shell.Current.GoToAsync("//TimeLine", false);
		}
		else
		{
			await DisplayAlertAsync("Hata", "Paylaşım başarısız oldu. Bu görev zaten paylaşılmış olabilir.", "Tamam");
			btn.IsEnabled = true;
		}
	}
}