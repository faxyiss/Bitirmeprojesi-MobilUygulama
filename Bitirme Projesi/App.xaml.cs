using Bitirme_Projesi.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Bitirme_Projesi
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			CheckLoginStatus();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			return new Window(new AppShell());
		}

		private async void CheckLoginStatus()
		{
			string userId = await SecureStorage.Default.GetAsync("user_id");

			// Her iki durumda da AppShell'i kullanmalıyız ki rotalar çalışsın
			MainPage = new AppShell();

			if (string.IsNullOrEmpty(userId))
			{
				// Eğer giriş yapılmamışsa direkt Login sayfasına fırlat
				await Shell.Current.GoToAsync("//Login");
			}
		}
	}
}