using System.Collections.ObjectModel;

namespace Bitirme_Projesi.Pages;

// 1. VERİ MODELİ
public class GorevModel
{
	public string Baslik { get; set; }
	public int Puan { get; set; }
	public string ResimUrl { get; set; }
	public string Aciklama { get; set; }
}

public partial class TimeLinePage : ContentPage
{
	// 2. LİSTELER
	public ObservableCollection<GorevModel> YakindakilerListesi { get; set; } = new();
	public ObservableCollection<GorevModel> OnerilenlerListesi { get; set; } = new();

	public TimeLinePage()
	{
		InitializeComponent();

		// Verileri hemen bağlayalım (İçleri boş olsa da hata vermez)
		ColYakindakiler.ItemsSource = YakindakilerListesi;
		ColOnerilenler.ItemsSource = OnerilenlerListesi;
	}

	// PERFORMANS: Sayfa açılırken donmayı engellemek için veriyi burada yüklüyoruz
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Eğer listeler zaten doluysa tekrar yükleyip cihazı yormayalım
		if (YakindakilerListesi.Count == 0)
		{
			await Task.Delay(100); // Animasyonun bitmesi için minik bir nefes payı
			VerileriYukle();
		}
	}

	private void VerileriYukle()
	{
		// Yakındakiler Verileri
		YakindakilerListesi.Add(new GorevModel { Baslik = "Sahil Kenarı Plastik Atıkları", Puan = 50, ResimUrl = "dumy_photo.png", Aciklama = "Lodos sonrası sahile vuran plastik şişelerin temizlenmesi gerekiyor." });
		YakindakilerListesi.Add(new GorevModel { Baslik = "Orman Yürüyüş Yolu", Puan = 75, ResimUrl = "dumy_photo.png", Aciklama = "Piknikçilerden kalan cam şişeler yangın riski taşıyor." });

		// Önerilenler Verileri
		OnerilenlerListesi.Add(new GorevModel { Baslik = "Mahalle Parkı Temizliği", Puan = 30, ResimUrl = "dumy_photo.png", Aciklama = "Çocuk parkı etrafındaki ambalaj atıklarını temizleyin." });
		OnerilenlerListesi.Add(new GorevModel { Baslik = "Göl Kenarı İyileştirme", Puan = 100, ResimUrl = "dumy_photo.png", Aciklama = "Yüksek puanlı endüstriyel temizlik görevi!" });
	}

	// SEKMELER ARASI GEÇİŞ
	private void OnTabClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		string secim = button?.CommandParameter?.ToString();

		if (secim == "Yakindaki")
		{
			ColYakindakiler.IsVisible = true;
			ColOnerilenler.IsVisible = false;

			BtnYakindakiler.BackgroundColor = Color.FromArgb("#2D6A4F");
			BtnYakindakiler.TextColor = Colors.White;
			BtnOnerilenler.BackgroundColor = Colors.Transparent;
			BtnOnerilenler.TextColor = Color.FromArgb("#2D6A4F");
		}
		else if (secim == "Onerilen")
		{
			ColYakindakiler.IsVisible = false;
			ColOnerilenler.IsVisible = true;

			BtnOnerilenler.BackgroundColor = Color.FromArgb("#2D6A4F");
			BtnOnerilenler.TextColor = Colors.White;
			BtnYakindakiler.BackgroundColor = Colors.Transparent;
			BtnYakindakiler.TextColor = Color.FromArgb("#2D6A4F");
		}
	}

	// "AÇ" BUTONUNA TIKLANDIĞINDA (GÖNDERİ DETAYI)
	private async void OnOpenPostTapped(object sender, TappedEventArgs e)
	{
		var layout = sender as BindableObject;
		var secilenPaylasim = layout?.BindingContext as GorevModel;

		if (secilenPaylasim != null)
		{
			// Detay sayfasına gidiyoruz (Animasyonu donmaması için 'false' yaptık)
			// Not: AppShell.xaml.cs içinde "PostDetail" rotasını kaydetmiş olmalısın
			await Shell.Current.GoToAsync("PostDetail", false);
		}
	}

	// BOŞ METOTLAR (XAML'da tanımlıysa hata vermemesi için)
	private void OnLikeTapped(object sender, TappedEventArgs e) { /* Beğeni mantığı */ }
	private void OnCommentTapped(object sender, TappedEventArgs e) { /* Yorum mantığı */ }
}