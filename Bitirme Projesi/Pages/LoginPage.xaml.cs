using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class LoginPage : ContentPage
{
	private readonly AuthService _authService;

	public LoginPage()
	{
		InitializeComponent();
		_authService = new AuthService();
	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		var email = EmailEntry.Text;
		var password = PasswordEntry.Text;

		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			await DisplayAlertAsync("Hata", "Lütfen tüm alanları doldurun.", "Tamam");
			return;
		}

		var isSuccess = await _authService.LoginAsync(email, password);

		if (isSuccess)
		{
			await Shell.Current.GoToAsync("//TimeLine");
		}
		else
		{
			await DisplayAlertAsync("Hata", "Giriş yapılamadı. Bilgilerinizi kontrol edin.", "Tamam");
		}
	}

	private async void OnRegisterClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Register");
	}
}