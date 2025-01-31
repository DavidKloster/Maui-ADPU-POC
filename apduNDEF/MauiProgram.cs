﻿#if ANDROID
using apduNDEF.Platforms.Android;
#endif
using Microsoft.Extensions.Logging;

namespace apduNDEF
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif
#if ANDROID
     builder.Services.AddSingleton< NfcService>();
#endif
       
            return builder.Build();
        }
    }
}
