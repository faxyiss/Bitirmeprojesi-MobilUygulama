using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;
using Microsoft.Maui.Devices.Sensors; // Konum için
using Microsoft.Maui.Media; // Fotoğraf/Kamera için

namespace Bitirme_Projesi.Pages;

public partial class StartQuestPage : ContentPage
{
	// Konum verisini ilerde kullanmak istersen diye değişkenlerde tutuyoruz
	private double? _latitude;
	private double? _longitude;

	private readonly IPhotoService _photoService;
	private readonly IActiveTaskService _activeTaskService;
	private FileResult _capturedPhoto; // Çekilen fotoğrafı burada tutuyoruz
	public StartQuestPage(IPhotoService photoService, IActiveTaskService activeTaskService)
	{
		InitializeComponent();
		_photoService = photoService;
		_activeTaskService = activeTaskService;
	}

	// --- 1. FOTOĞRAF İŞLEMLERİ ---

	private async void OnCameraClicked(object sender, EventArgs e)
	{
		try
		{
			_capturedPhoto = await MediaPicker.Default.CapturePhotoAsync();
			if (_capturedPhoto != null)
			{
				var stream = await _capturedPhoto.OpenReadAsync();
				ImgSelected.Source = ImageSource.FromStream(() => stream);
			}
		}
		catch (Exception ex) { /* Hata yönetimi */ }
	}

	private async void OnGalleryClicked(object sender, EventArgs e)
	{
		try
		{
			FileResult photo = await MediaPicker.Default.PickPhotoAsync();
			if (photo != null)
			{
				var stream = await photo.OpenReadAsync();
				ImgSelected.Source = ImageSource.FromStream(() => stream);
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Hata", $"Galeriye erişilemedi: {ex.Message}", "Tamam");
		}
	}

	// --- 2. KONUM DOĞRULAMA İŞLEMLERİ ---

	private async void OnGetLocationClicked(object sender, EventArgs e)
	{
		try
		{
			BtnGetLocation.IsEnabled = false;
			LblLocation.Text = "Konum saptanıyor...";
			LblLocation.TextColor = Colors.Orange;

			// Konum izni ve veri alma (Orta hassasiyet hız için idealdir)
			GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
			Location location = await Geolocation.Default.GetLocationAsync(request);

			if (location != null)
			{
				_latitude = location.Latitude;
				_longitude = location.Longitude;

				LblLocation.Text = $"📍 Konum Doğrulandı ({_latitude:F4}, {_longitude:F4})";
				LblLocation.TextColor = Color.FromArgb("#2D6A4F"); // Yeşil
				BtnGetLocation.Text = "KONUMU GÜNCELLE";
			}
			else
			{
				throw new Exception("Konum boş döndü.");
			}
		}
		catch (Exception)
		{
			LblLocation.Text = "❌ Konum Alınamadı!";
			LblLocation.TextColor = Color.FromArgb("#BC4749"); // Kırmızı
			await DisplayAlertAsync("Konum Hatası", "GPS açık olduğundan ve izin verdiğinizden emin olun.", "Tekrar Dene");
		}
		finally
		{
			BtnGetLocation.IsEnabled = true;
		}
	}

	// --- 3. AKSİYON BUTONLARI ---

	private async void OnStartQuestClicked(object sender, EventArgs e)
	{
		// 1. Validasyonlar
		if (_capturedPhoto == null)
		{
			await DisplayAlertAsync("Hata", "Lütfen bir fotoğraf çekin.", "Tamam");
			return;
		}
		if (_latitude == null || string.IsNullOrWhiteSpace(EntTitle.Text))
		{
			await DisplayAlertAsync("Hata", "Eksik bilgileri doldurun.", "Tamam");
			return;
		}

		// 2. Fotoğrafı Yükle (IPhotoService kullanılıyor)
		// UserSession.Instance.UserId üzerinden kullanıcı bilgisini alıyoruz
		var photoId = await _photoService.UploadPhotoAsync(_capturedPhoto, UserSession.Instance.UserId);

		if (photoId == null)
		{
			await DisplayAlertAsync("Hata", "Fotoğraf sunucuya yüklenemedi.", "Tamam");
			return;
		}

		// 3. Görevi Kaydet (IActiveTaskService kullanılıyor)
		var taskDto = new CreateActiveTaskDto
		{
			UserId = UserSession.Instance.UserId,
			Title = EntTitle.Text,
			InitialPhotoId = photoId.Value,
			InitialLat = _latitude.Value,
			InitialLng = _longitude.Value
		};

		var isSuccess = await _activeTaskService.CreateTaskAsync(taskDto);

		if (isSuccess)
		{
			await DisplayAlertAsync("Başarılı", "Görev başlatıldı!", "Tamam");
			await Shell.Current.GoToAsync(".."); // Bir önceki sayfaya dön
		}
		else
		{
			await DisplayAlertAsync("Hata", "Görev kaydedilirken bir hata oluştu.", "Tamam");
		}
	}

	private async void OnCancelClicked(object sender, EventArgs e)
	{
		// Kullanıcıyı bir önceki sayfaya geri atar
		await Shell.Current.GoToAsync("..", false);
	}
}