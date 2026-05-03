using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages.Views;

public partial class HeaderViews : ContentView
{
	public HeaderViews()
	{
		InitializeComponent();

		// 1. Sayfa ilk açıldığında mevcut verileri yükle
		UpdateUI();

		// 2. Profil her güncellendiğinde (EditProfile'dan sonra) burayı da yenile
		UserSession.Instance.OnProfileUpdated += () =>
		{
			MainThread.BeginInvokeOnMainThread(UpdateUI);
		};
	}

	private void UpdateUI()
	{
		// İsim güncelleme
		LblHeaderName.Text = !string.IsNullOrEmpty(UserSession.Instance.FullName)
			? UserSession.Instance.FullName
			: "Kullanıcı Adı";

		// Fotoğraf güncelleme
		if (!string.IsNullOrEmpty(UserSession.Instance.PhotoUrl))
		{
			ImgHeaderProfile.Source = UserSession.Instance.PhotoUrl;
		}
		else
		{
			ImgHeaderProfile.Source = "user_avatar.png";
		}
	}

	private void OnSettingsClicked(object sender, EventArgs e)
	{

	}
}