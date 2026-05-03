using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class ProfilePage : ContentPage
{
	private readonly IProfileService _profileService;

	public ProfilePage(IProfileService profileService)
	{
		InitializeComponent();
		_profileService = profileService;
	}

	// Sayfa her ekrana geldiğinde veriyi tazeler
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Event aboneliğini burada başlatıp OnDisappearing'de bitirmek daha güvenlidir
		UserSession.Instance.OnProfileUpdated += RefreshUI;

		await LoadProfile();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// Bellek sızıntısını önlemek için aboneliği kaldır
		UserSession.Instance.OnProfileUpdated -= RefreshUI;
	}

	private void RefreshUI()
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			LblFullName.Text = UserSession.Instance.FullName;
			LblBio.Text = UserSession.Instance.Bio;
			ImgProfile.Source = UserSession.Instance.PhotoUrl;
		});
	}

	private async Task LoadProfile()
	{
		var profile = await _profileService.GetProfileAsync(UserSession.Instance.UserId);

		if (profile != null)
		{
			LblPoints.Text = profile.TotalPoints.ToString("N0"); // Örn: 1,250 
			LblTasks.Text = profile.CompletedTaskCount.ToString(); // Örn: 12 [cite: 15]
			LblPosts.Text = profile.PostCount.ToString(); // Örn: 4 [cite: 16]

			// 2. Profil bilgilerini Session üzerinden güncelle (Bu RefreshUI'ı tetikler)
			UserSession.Instance.UpdateSession(profile.FullName, profile.ProfilePhotoPath, profile.Bio);
		}
	}
	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		bool answer = await DisplayAlertAsync("Çıkış", "Hesabınızdan çıkış yapmak istediğinize emin misiniz?", "Evet", "Hayır");
		if (answer)
		{
			await Shell.Current.GoToAsync("//Login", false);
		}
	}

	private async void OnEditProfileClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("EditProfile");
	}

	private async void OnMyPostsClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("MyPosts");
	}
}