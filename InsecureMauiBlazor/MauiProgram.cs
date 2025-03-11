using InsecureMauiBlazor.Services;
using Microsoft.Extensions.Logging;

namespace InsecureMauiBlazor
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IDataStorageService, InsecureDataStorageService>();
            builder.Services.AddSingleton<INetworkService, InsecureNetworkService>();
            builder.Services.AddSingleton<ICryptoService, InsecureCryptoService>();
            builder.Services.AddSingleton<IAuthService, InsecureAuthService>();
            builder.Services.AddSingleton<NativeFeatures>();

            return builder.Build();
        }
    }
}
