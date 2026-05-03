using Bitirme_Projesi.Models;
using Bitirme_Projesi.Services;
using System.Collections.ObjectModel;

namespace Bitirme_Projesi.Pages;

public partial class QuestsPage : ContentPage
{
	private readonly IActiveTaskService _activeTaskService;
	public ObservableCollection<ActiveTaskDto> AktifListe { get; set; } = new();
	public ObservableCollection<AbandonedTaskDto> BirakilanListe { get; set; } = new();
	public ObservableCollection<CompletedTaskDto> BitenListe { get; set; } = new();



	public QuestsPage(IActiveTaskService activeTaskService)
	{
		InitializeComponent();
		_activeTaskService = activeTaskService;
		BindingContext = this; // Bunu ekle

		ColAktif.ItemsSource = AktifListe;
		ColBirakilan.ItemsSource = BirakilanListe;
		ColBiten.ItemsSource = BitenListe;
	}
	private async Task AktifGorevleriYukleAsync()
	{
		try
		{
			// Session'daki kullanıcı ID'sini alıyoruz
			var userId = UserSession.Instance.UserId;

			// Servisten gerçek verileri çekiyoruz
			var tasks = await _activeTaskService.GetMyActiveTasksAsync(userId);

			// Listeyi temizleyip UI'ı yeni verilerle güncelliyoruz
			AktifListe.Clear();
			foreach (var task in tasks)
			{
				AktifListe.Add(task);
			}
		}
		catch (Exception ex)
		{
			// .NET 10 ile gelen DisplayAlertAsync kullanarak hata gösterimi
			await DisplayAlertAsync("Hata", $"Görevler yüklenirken bir sorun oluştu: {ex.Message}", "Tamam");
		}
	}

	private async Task BirakilanGorevleriYukleAsync()
	{
		var tasks = await _activeTaskService.GetMyAbandonedTasksAsync(UserSession.Instance.UserId);
		BirakilanListe.Clear();
		foreach (var task in tasks)
		{
			BirakilanListe.Add(task);
		}
	}

	private async Task BitenGorevleriYukleAsync()
	{
		var tasks = await _activeTaskService.GetMyCompletedTasksAsync(UserSession.Instance.UserId);
		BitenListe.Clear();
		foreach (var task in tasks)
		{
			BitenListe.Add(task);
		}
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await AktifGorevleriYukleAsync();
		await BirakilanGorevleriYukleAsync();
		await BitenGorevleriYukleAsync();
	}


	private void OnTabClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		string secim = button.CommandParameter.ToString();

		ColAktif.IsVisible = ColBirakilan.IsVisible = ColBiten.IsVisible = false;

		// Buton renklerini sıfırla
		ResetBtn(BtnAktif); ResetBtn(BtnBirakilan); ResetBtn(BtnBiten);

		if (secim == "Aktif") { ColAktif.IsVisible = true; SetActive(BtnAktif); }
		else if (secim == "Birakilan") { ColBirakilan.IsVisible = true; SetActive(BtnBirakilan); }
		else if (secim == "Biten") { ColBiten.IsVisible = true; SetActive(BtnBiten); }
	}

	private void SetActive(Button b) { b.BackgroundColor = Color.FromArgb("#2D6A4F"); b.TextColor = Colors.White; }
	private void ResetBtn(Button b) { b.BackgroundColor = Colors.Transparent; b.TextColor = Color.FromArgb("#2D6A4F"); }

	private async void OnNewQuestClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("StartQuest");
	}

	private async void OnGorevibitirClicked(object sender, EventArgs e)
	{
		var button = sender as Button;

		// Tıklanan satırdaki görevin Id'sini CommandParameter'dan alıyoruz
		if (button?.CommandParameter is Guid taskId)
		{
			// Id'yi diğer sayfaya parametre olarak gönderiyoruz
			await Shell.Current.GoToAsync($"ComplateQuest?taskId={taskId}");
		}
		else
		{
			// Hata durumu (veya şimdilik düz geçiş)
			await Shell.Current.GoToAsync("ComplateQuest");
		}
	}

	private async void OnShareQuestClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button?.CommandParameter is CompletedTaskDto task)
		{
			// İki fotoğrafı da URL güvenli hale getirip gönderiyoruz[cite: 20]
			string beforeUrl = Uri.EscapeDataString(task.FullInitialPhotoUrl);
			string afterUrl = Uri.EscapeDataString(task.FullAfterPhotoUrl);

			await Shell.Current.GoToAsync($"ShareQuest?taskId={task.Id}&title={task.Title}&before={beforeUrl}&after={afterUrl}");
		}
	}

	private async void OnGorevIptalClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button?.CommandParameter is Guid taskId)
		{
			// 1. Kullanıcıya emin olup olmadığını soralım
			bool isConfirmed = await DisplayAlertAsync("Görevi İptal Et", "Bu görevi bırakmak istediğinize emin misiniz?", "Evet, İptal Et", "Vazgeç");

			if (isConfirmed)
			{
				// 2. DTO'yu hazırla
				var cancelDto = new CancelTaskDto
				{
					TaskId = taskId,
					UserId = UserSession.Instance.UserId,
					Reason = "Cancel"
				};

				// 3. Servisi çağır
				bool isSuccess = await _activeTaskService.CancelTaskAsync(cancelDto);

				if (isSuccess)
				{
					await DisplayAlertAsync("Başarılı", "Görev iptal edildi ve bırakılanlar listesine eklendi.", "Tamam");

					// 4. Listeleri yenile (Aktiflerden silinip, Bırakılanlara düşmesi için)
					await AktifGorevleriYukleAsync();
					await BirakilanGorevleriYukleAsync();
				}
				else
				{
					await DisplayAlertAsync("Hata", "Görev iptal edilirken bir sorun oluştu.", "Tamam");
				}
			}
		}
	}

	private async void OnGorevSilClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button?.CommandParameter is Guid taskId)
		{
			// 1. Kullanıcıdan onay al (Yanlışlıkla silmeyi önlemek için)
			bool isConfirmed = await DisplayAlertAsync("Kalıcı Olarak Sil", "Bu görevi geçmişinizden tamamen silmek istediğinize emin misiniz?", "Evet, Sil", "Vazgeç");

			if (isConfirmed)
			{
				// 2. Servisi çağırarak backend'den sil
				bool isSuccess = await _activeTaskService.DeleteAbandonedTaskAsync(taskId, UserSession.Instance.UserId);

				if (isSuccess)
				{
					// 3. Başarılı olursa listeyi yenile
					await BirakilanGorevleriYukleAsync();
				}
				else
				{
					await DisplayAlertAsync("Hata", "Görev silinirken bir sorun oluştu.", "Tamam");
				}
			}
		}
	}
}