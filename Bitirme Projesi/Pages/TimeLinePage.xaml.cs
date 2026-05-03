using System.Collections.ObjectModel;

using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;

namespace Bitirme_Projesi.Pages;

public partial class TimeLinePage : ContentPage
{
	private readonly PostService _postService;
	private int _yakindakilerPage = 1;
	private int _onerilenlerPage = 1;
	private const int PageSize = 10;

	private bool _hasMoreYakindakiler = true;
	private bool _hasMoreOnerilenler = true;

	// Paralel API isteklerinin çakışmaması için iki ayrı bayrak (flag)
	private bool _isYakindakilerLoading = false;
	private bool _isOnerilenlerLoading = false;

	public ObservableCollection<PostResponseDto> YakindakilerListesi { get; set; } = new();
	public ObservableCollection<PostResponseDto> OnerilenlerListesi { get; set; } = new();

	public TimeLinePage()
	{
		InitializeComponent();

		_postService = new PostService();

		ColYakindakiler.ItemsSource = YakindakilerListesi;
		ColOnerilenler.ItemsSource = OnerilenlerListesi;

		// Daha Fazla Yükle (Sona kaydırınca tetiklenir)
		ColYakindakiler.RemainingItemsThreshold = 1;
		ColYakindakiler.RemainingItemsThresholdReached += OnYakindakilerScrolledToBottom;

		ColOnerilenler.RemainingItemsThreshold = 1;
		ColOnerilenler.RemainingItemsThresholdReached += OnOnerilenlerScrolledToBottom;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Sayfa İLK DEFA açılıyorsa (listeler boşsa)
		if (YakindakilerListesi.Count == 0 && OnerilenlerListesi.Count == 0)
		{
			// 1. Önce sayfanın UI tasarımının ekrana tamamen çizilmesine izin ver (150ms)
			// Bu sayede uygulama donmuş gibi hissettirmez.
			await Task.Delay(150);

			// 2. Çarkı gösterip verileri çekmeye başla
			await IlkVerileriYukleAsync();
		}
	}

	private async Task IlkVerileriYukleAsync()
	{
		// Çarkı görünür yap ve döndür
		LoadingSpinner.IsVisible = true;
		LoadingSpinner.IsRunning = true;

		// İki listeyi PARALEL olarak çek (Sayfanın dolma hızını iki katına çıkarır)
		await Task.WhenAll(LoadOnerilenlerAsync(), LoadYakindakilerAsync());

		// Veriler geldi, çarkı gizle ve durdur
		LoadingSpinner.IsRunning = false;
		LoadingSpinner.IsVisible = false;
	}

	private async Task LoadOnerilenlerAsync()
	{
		if (_isOnerilenlerLoading || !_hasMoreOnerilenler) return;
		_isOnerilenlerLoading = true;

		try
		{
			var result = await _postService.GetRecommendedTimelineAsync(_onerilenlerPage, PageSize);
			if (result != null)
			{
				foreach (var item in result.Items)
				{
					OnerilenlerListesi.Add(item);
				}
				_hasMoreOnerilenler = result.HasMore;
				if (result.HasMore) _onerilenlerPage++;
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Önerilenler Hatası: {ex.Message}");
		}
		finally
		{
			_isOnerilenlerLoading = false;
		}
	}

	private async Task LoadYakindakilerAsync()
	{
		if (_isYakindakilerLoading || !_hasMoreYakindakiler) return;
		_isYakindakilerLoading = true;

		try
		{
			// Kullanıcının anlık konumunu al
			var location = await Geolocation.Default.GetLastKnownLocationAsync() ??
						   await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

			if (location != null)
			{
				var result = await _postService.GetNearbyTimelineAsync(
					(decimal)location.Latitude,
					(decimal)location.Longitude,
					_yakindakilerPage,
					PageSize);

				if (result != null)
				{
					foreach (var item in result.Items)
					{
						YakindakilerListesi.Add(item);
					}
					_hasMoreYakindakiler = result.HasMore;
					if (result.HasMore) _yakindakilerPage++;
				}
			}
		}
		catch (FeatureNotSupportedException)
		{
			await DisplayAlert("Hata", "Cihazınız konum özelliğini desteklemiyor.", "Tamam");
		}
		catch (PermissionException)
		{
			await DisplayAlert("İzin Hatası", "Lütfen ayarlardan konum izni verin.", "Tamam");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Yakındakiler Hatası: {ex.Message}");
		}
		finally
		{
			_isYakindakilerLoading = false;
		}
	}

	private async void OnYakindakilerScrolledToBottom(object sender, EventArgs e)
	{
		await LoadYakindakilerAsync();
	}

	private async void OnOnerilenlerScrolledToBottom(object sender, EventArgs e)
	{
		await LoadOnerilenlerAsync();
	}

	// --- SEKMELER ARASI GEÇİŞ ---
	private void OnTabClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		string secim = button?.CommandParameter?.ToString();

		if (secim == "Yakindaki")
		{
			RefreshYakindakiler.IsVisible = true; // ColYakindakiler yerine RefreshYakindakiler yazdık
			RefreshOnerilenler.IsVisible = false; // ColOnerilenler yerine RefreshOnerilenler yazdık

			BtnYakindakiler.BackgroundColor = Color.FromArgb("#2D6A4F");
			BtnYakindakiler.TextColor = Colors.White;
			BtnOnerilenler.BackgroundColor = Colors.Transparent;
			BtnOnerilenler.TextColor = Color.FromArgb("#2D6A4F");
		}
		else if (secim == "Onerilen")
		{
			RefreshYakindakiler.IsVisible = false; // ColYakindakiler yerine RefreshYakindakiler yazdık
			RefreshOnerilenler.IsVisible = true;   // ColOnerilenler yerine RefreshOnerilenler yazdık

			BtnOnerilenler.BackgroundColor = Color.FromArgb("#2D6A4F");
			BtnOnerilenler.TextColor = Colors.White;
			BtnYakindakiler.BackgroundColor = Colors.Transparent;
			BtnYakindakiler.TextColor = Color.FromArgb("#2D6A4F");
		}
	}

	// --- DETAYA GİTME ---
	private async void OnOpenPostTapped(object sender, TappedEventArgs e)
	{
		var layout = sender as BindableObject;
		var secilenPaylasim = layout?.BindingContext as PostResponseDto;

		if (secilenPaylasim != null)
		{
			await Shell.Current.GoToAsync($"PostDetail?postId={secilenPaylasim.Id}", false);
		}
	}

	private async void OnLikeTapped(object sender, TappedEventArgs e)
	{
		var layout = sender as BindableObject;
		var secilenPaylasim = layout?.BindingContext as PostResponseDto;

		if (secilenPaylasim != null)
		{
			Guid currentUserId = UserSession.Instance.UserId;

			if (currentUserId != Guid.Empty)
			{
				try
				{
					// 1. API'ye beğeni isteği at
					var newLikeCount = await _postService.ToggleLikeAsync(secilenPaylasim.Id, currentUserId);

					if (newLikeCount.HasValue)
					{
						// 2. İşlemi Ana Ekrana (MainThread) taşı
						MainThread.BeginInvokeOnMainThread(() =>
						{
							// Sayıyı güncelle
							secilenPaylasim.LikeCount = newLikeCount.Value;

							// ÇÖZÜM: MAUI'nin listeyi yenilemesi için öğeyi çıkarıp aynı yerine koyuyoruz

							// Önce Yakındakiler listesinde mi diye bakıyoruz
							int indexYakindaki = YakindakilerListesi.IndexOf(secilenPaylasim);
							if (indexYakindaki >= 0)
							{
								YakindakilerListesi.RemoveAt(indexYakindaki);
								YakindakilerListesi.Insert(indexYakindaki, secilenPaylasim);
							}

							// Önerilenler listesinde mi diye bakıyoruz
							int indexOnerilen = OnerilenlerListesi.IndexOf(secilenPaylasim);
							if (indexOnerilen >= 0)
							{
								OnerilenlerListesi.RemoveAt(indexOnerilen);
								OnerilenlerListesi.Insert(indexOnerilen, secilenPaylasim);
							}
						});
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Beğeni Hatası: {ex.Message}");
				}
			}
			else
			{
				await DisplayAlert("Uyarı", "Beğeni yapmak için giriş yapmalısınız.", "Tamam");
			}
		}
	}

	private async void OnYakindakilerRefreshing(object sender, EventArgs e)
	{
		// 1. Sayfalama ayarlarını ve listeyi sıfırla
		_yakindakilerPage = 1;
		_hasMoreYakindakiler = true;
		YakindakilerListesi.Clear();

		// 2. API'den en güncel verileri tekrar çek
		await LoadYakindakilerAsync();

		// 3. Üstte dönen küçük yenileme çarkını kapat
		RefreshYakindakiler.IsRefreshing = false;
	}

	private async void OnOnerilenlerRefreshing(object sender, EventArgs e)
	{
		// 1. Sayfalama ayarlarını ve listeyi sıfırla
		_onerilenlerPage = 1;
		_hasMoreOnerilenler = true;
		OnerilenlerListesi.Clear();

		// 2. API'den en güncel verileri tekrar çek
		await LoadOnerilenlerAsync();

		// 3. Üstte dönen küçük yenileme çarkını kapat
		RefreshOnerilenler.IsRefreshing = false;
	}
}