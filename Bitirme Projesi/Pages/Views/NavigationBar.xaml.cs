namespace Bitirme_Projesi.Pages.Views;

public partial class NavigationBar : ContentView
{
	public NavigationBar()
	{
		InitializeComponent();

		this.Loaded += OnBarLoaded;
	}

	
	private void OnBarLoaded(object sender, EventArgs e)
	{
		// O an ekranda tam olarak hangi sayfa açık?
		var aktifSayfa = Shell.Current.CurrentPage;

		// Sayfa isimlerini kendi projendeki isimlere göre düzelt
		if (aktifSayfa is TimeLinePage)
		{
			RenkleriGuncelle("Akis");
		}
		else if (aktifSayfa.GetType().Name == "QuestsPage") // Eğer referans eklemediysen bu şekilde isimden de bulabilirsin
		{
			RenkleriGuncelle("Gorevler");
		}
		else if (aktifSayfa.GetType().Name == "ProfilePage")
		{
			RenkleriGuncelle("Profil");
		}
	}

	private void RenkleriGuncelle(string aktifSekme)
	{
		// 1. Önce Hepsini Pasif Yap (Arka plan şeffaf, yazılar gri)
		LayoutAkis.BackgroundColor = Colors.Transparent;
		LayoutGorevler.BackgroundColor = Colors.Transparent;
		LayoutProfil.BackgroundColor = Colors.Transparent;

		LblAkis.TextColor = Color.FromArgb("#707070");
		LblGorevler.TextColor = Color.FromArgb("#707070");
		LblProfil.TextColor = Color.FromArgb("#707070");

		// 2. Sadece Aktif Olanın Arka Planını Yeşil, Yazısını Beyaz Yap
		if (aktifSekme == "Akis")
		{
			LayoutAkis.BackgroundColor = Color.FromArgb("#2D6A4F"); // Arka Plan Yeşil
			LblAkis.TextColor = Colors.White; // Yazı Beyaz
		}
		else if (aktifSekme == "Gorevler")
		{
			LayoutGorevler.BackgroundColor = Color.FromArgb("#2D6A4F");
			LblGorevler.TextColor = Colors.White;
		}
		else if (aktifSekme == "Profil")
		{
			LayoutProfil.BackgroundColor = Color.FromArgb("#2D6A4F");
			LblProfil.TextColor = Colors.White;
		}
	}

	// TIKLAMA OLAYLARI (Sayfadayken tekrar tıklamayı engeller)
	private async void OnAkisTapped(object sender, TappedEventArgs e)
	{
		if (Shell.Current.CurrentPage is not TimeLinePage)
			await Shell.Current.GoToAsync("//TimeLine");
	}

	private async void OnGorevlerTapped(object sender, TappedEventArgs e)
	{
		if (Shell.Current.CurrentPage.GetType().Name != "QuestsPage")
			await Shell.Current.GoToAsync("//Quests");
	}

	private async void OnProfilTapped(object sender, TappedEventArgs e)
	{
		if (Shell.Current.CurrentPage.GetType().Name != "ProfilePage")
			await Shell.Current.GoToAsync("//Profile");
	}
}