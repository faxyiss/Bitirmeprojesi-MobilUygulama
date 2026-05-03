using Bitirme_Projesi.Services;
using Bitirme_Projesi.Pages; // Sayfaların namespace'ini ekle
using Microsoft.Extensions.Logging;

namespace Bitirme_Projesi
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			// 1. Servis Kaydı
			builder.Services.AddSingleton<HttpClient>(); // Eğer ProfileService içinde HttpClient kullanıyorsan bunu da ekle
			builder.Services.AddSingleton<IProfileService, ProfileService>();
			builder.Services.AddSingleton<IPhotoService, PhotoService>();

			builder.Services.AddTransient<IPhotoService, PhotoService>();
			builder.Services.AddTransient<IActiveTaskService, ActiveTaskService>();
			builder.Services.AddTransient<StartQuestPage>();
			builder.Services.AddTransient<QuestsPage>();
			builder.Services.AddTransient<ComplateQuestPage>();
			// 2. Sayfa Kayıtları (KRİTİK ADIM)
			// Bu kayıtlar olmazsa constructor'daki servisler doldurulamaz!
			builder.Services.AddTransient<ProfilePage>();
			builder.Services.AddTransient<EditProfilePage>();
			

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}