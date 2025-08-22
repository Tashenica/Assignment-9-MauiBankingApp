using Microsoft.Extensions.Logging;
using MauiBankingExercise.Services;
using MauiBankingExercise.ViewModels;
using MauiBankingExercise.Views;
using MauiBankingExercise.Converters;

namespace MauiBankingExercise
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

            // Register Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<IBankingService, BankingService>();

            // Register ViewModels
            builder.Services.AddTransient<CustomerSelectionViewModel>();
            builder.Services.AddTransient<CustomerDashboardViewModel>();
            builder.Services.AddTransient<TransactionViewModel>();

            // Register Views
            builder.Services.AddTransient<CustomerSelectionPage>();
            builder.Services.AddTransient<CustomerDashboardPage>();
            builder.Services.AddTransient<TransactionPage>();

            // Register Converters
            builder.Services.AddSingleton<TransactionColorConverter>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}