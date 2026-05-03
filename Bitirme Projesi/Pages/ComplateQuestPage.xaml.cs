using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

[QueryProperty(nameof(TaskId), "taskId")]
public partial class ComplateQuestPage : ContentPage
{
	private readonly IPhotoService _photoService;
	private readonly IActiveTaskService _activeTaskService;

	private double? _lat;
	private double? _lon;
	private FileResult _afterPhoto;
	public string TaskId { get; set; }
	// Görev ismini parametre olarak alıyoruz
	public ComplateQuestPage(IPhotoService photoService, IActiveTaskService activeTaskService)
	{
		InitializeComponent();
		_photoService = photoService;
		_activeTaskService = activeTaskService;
	}

	// --- FOTOĞRAF İŞLEMLERİ ---
	private async void OnCameraClicked(object sender, EventArgs e)
	{
		// "var photo" yerine doğrudan "_afterPhoto" değişkenini kullanıyoruz
		_afterPhoto = await MediaPicker.Default.CapturePhotoAsync();

		if (_afterPhoto != null)
		{
			var stream = await _afterPhoto.OpenReadAsync();
			ImgAfter.Source = ImageSource.FromStream(() => stream);
		}
	}

	private async void OnGalleryClicked(object sender, EventArgs e)
	{
		// Aynı şekilde "var photo" yerine "_afterPhoto"
		_afterPhoto = await MediaPicker.Default.PickPhotoAsync();

		if (_afterPhoto != null)
		{
			var stream = await _afterPhoto.OpenReadAsync();
			ImgAfter.Source = ImageSource.FromStream(() => stream);
		}
	}

	// --- KONUM DOĞRULAMA ---
	private async void OnVerifyLocationClicked(object sender, EventArgs e)
	{
		try
		{
			BtnVerifyLocation.IsEnabled = false;
			var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

			if (location != null)
			{
				_lat = location.Latitude;
				_lon = location.Longitude;
				LblLocation.Text = $"📍 Konum Kanıtı Alındı: {_lat:F4}, {_lon:F4}";
				LblLocation.TextColor = Color.FromArgb("#2D6A4F");
			}
		}
		catch (Exception)
		{
			await DisplayAlertAsync("Hata", "Şu anki konumuna ulaşılamadı.", "Tamam");
		}
		finally
		{
			BtnVerifyLocation.IsEnabled = true;
		}
	}

	// --- GÖREVİ BİTİR ---
	private async void OnFinishQuestClicked(object sender, EventArgs e)
	{
		// 1. Zorunlu alan kontrolleri
		if (_afterPhoto == null)
		{
			await DisplayAlertAsync("Uyarı", "Görevi bitirmek için temizlenmiş alanın fotoğrafını eklemelisiniz.", "Tamam");
			return;
		}
		if (_lat == null)
		{
			await DisplayAlertAsync("Uyarı", "Görevi bitirmek için konum kanıtı sunmalısınız.", "Tamam");
			return;
		}

		// 2. Önce "Sonrası" fotoğrafını sunucuya yükle (PhotoService ile)
		var photoId = await _photoService.UploadPhotoAsync(_afterPhoto, UserSession.Instance.UserId);

		if (photoId == null)
		{
			await DisplayAlertAsync("Hata", "Fotoğraf sunucuya yüklenemedi.", "Tamam");
			return;
		}

		// 3. Backend'e gönderilecek veriyi (DTO) hazırla
		var requestDto = new CompleteTaskRequestDto
		{
			TaskId = Guid.Parse(TaskId), // String olarak gelen ID'yi Guid'e çeviriyoruz
			UserId = UserSession.Instance.UserId,
			AfterPhotoId = photoId.Value,
			AfterLat = _lat.Value,
			AfterLng = _lon.Value,
			UserDescription = EdiDescription.Text ?? "" // [cite: 53, 54]
		};

		// 4. Görevi Bitirme Servisini Çağır
		bool isSuccess = await _activeTaskService.CompleteTaskAsync(requestDto);

		if (isSuccess)
		{
			// Başarı mesajı ve yönlendirme
			await DisplayAlertAsync("Tebrikler!", "Göreviniz başarıyla tamamlandı ve analiz sürecine alındı. Onaylandığında puanınız hesabınıza eklenecek!", "Harika");

			// Ana sayfadaki Görevler sekmesine dön
			await Shell.Current.GoToAsync("//Quests", false);
		}
		else
		{
			await DisplayAlertAsync("Hata", "Görev tamamlanırken bir sorun oluştu. Lütfen tekrar deneyin.", "Tamam");
		}
	}

	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..", false);
	}
}