using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class EditProfilePage : ContentPage
{
	private readonly IProfileService _profileService;
	private readonly IPhotoService _photoService; // Yeni Servis Eklendi
	private FileResult _selectedPhoto;
	private Guid? _newPhotoId = null;

	// Servisi Dependency Injection ile alıyoruz
	public EditProfilePage(IProfileService profileService, IPhotoService photoService)
	{
		InitializeComponent();
		_profileService = profileService;
		_photoService = photoService;

		FullNameEntry.Text = UserSession.Instance.FullName;
		BioEditor.Text = UserSession.Instance.Bio;
		// Profil fotoğrafı URL birleştirmesi eklenebilir
	}

	private async void OnChangePhotoTapped(object sender, EventArgs e)
	{
		_selectedPhoto = await MediaPicker.Default.PickPhotoAsync();
		if (_selectedPhoto != null)
		{
			var stream = await _selectedPhoto.OpenReadAsync();
			ProfileImage.Source = ImageSource.FromStream(() => stream);
		}
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		try
		{
			// Yeni fotoğraf seçildiyse önce sunucuya gönder ve ID'sini al
			if (_selectedPhoto != null)
			{
				_newPhotoId = await _photoService.UploadPhotoAsync(_selectedPhoto, UserSession.Instance.UserId);
				if (_newPhotoId == null)
				{
					await DisplayAlertAsync("Hata", "Fotoğraf sunucuya yüklenemedi.", "Tamam");
					return;
				}
			}

			var updateDto = new UserProfileUpdateDto
			{
				UserId = UserSession.Instance.UserId,
				FullName = FullNameEntry.Text,
				Bio = BioEditor.Text,
				NewPhotoId = _newPhotoId
			};

			var success = await _profileService.UpdateProfileAsync(updateDto);

			if (success)
			{
				var updatedProfile = await _profileService.GetProfileAsync(UserSession.Instance.UserId);

				if (updatedProfile != null)
				{
					// Session'ı en taze veriyle güncelle
					UserSession.Instance.UpdateFromDto(updatedProfile);
				}

				await DisplayAlertAsync("Başarılı", "Profiliniz güncellendi.", "Tamam");
				await Navigation.PopAsync();
			}
			else
			{
				await DisplayAlertAsync("Hata", "Sunucu güncellemeyi reddetti.", "Tamam");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Sistem Hatası", ex.Message, "Tamam");
		}
	}

	private async void OnCancelClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}