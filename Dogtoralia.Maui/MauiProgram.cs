using Dogtoralia.Maui.Core.Services;
using Dogtoralia.Maui.Core.ViewModels;
using Dogtoralia.Maui.Views;
using Microsoft.Extensions.Logging;

namespace Dogtoralia.Maui
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

            builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) });

            builder.Services.AddSingleton<IClinicApiService, ClinicApiService>();
            builder.Services.AddSingleton<IVeterinarianApiService, VeterinarianApiService>();
            builder.Services.AddSingleton<IPetApiService, PetApiService>();
            builder.Services.AddSingleton<IPetOwnerApiService, PetOwnerApiService>();

            builder.Services.AddTransient<ClinicsViewModel>();
            builder.Services.AddTransient<ClinicDetailViewModel>();
            builder.Services.AddTransient<VeterinariansViewModel>();
            builder.Services.AddTransient<VeterinarianDetailViewModel>();
            builder.Services.AddTransient<PetsViewModel>();
            builder.Services.AddTransient<PetDetailViewModel>();
            builder.Services.AddTransient<PetEditViewModel>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ClinicsPage>();
            builder.Services.AddTransient<ClinicDetailPage>();
            builder.Services.AddTransient<VeterinariansPage>();
            builder.Services.AddTransient<VeterinarianDetailPage>();
            builder.Services.AddTransient<PetsPage>();
            builder.Services.AddTransient<PetDetailPage>();
            builder.Services.AddTransient<PetEditPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
