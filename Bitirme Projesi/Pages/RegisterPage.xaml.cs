using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class RegisterPage : ContentPage
{
	private readonly AuthService _authService;

	public RegisterPage()
	{
		InitializeComponent();
		_authService = new AuthService();
	}

	private async void OnSignUpClicked(object sender, EventArgs e)
	{
		var fullName = FullNameEntry.Text;
		var email = EmailEntry.Text;
		var password = PasswordEntry.Text;
		var confirmPassword = ConfirmPasswordEntry.Text;

		if (password != confirmPassword)
		{
			await DisplayAlertAsync("Hata", "Şifreler uyuşmuyor!", "Tamam");
			return;
		}

		var isSuccess = await _authService.RegisterAsync(fullName, email, password);

		if (isSuccess)
		{
			await DisplayAlertAsync("Başarılı", "Hesabınız oluşturuldu. Giriş yapabilirsiniz.", "Tamam");
			await Shell.Current.GoToAsync(".."); // Giriş sayfasına geri dön
		}
		else
		{
			await DisplayAlertAsync("Hata", "Kayıt sırasında bir sorun oluştu.", "Tamam");
		}
	}

	private async void OnBackToLoginClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}