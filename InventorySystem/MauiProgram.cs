using Microsoft.AspNetCore.Components.WebView.Maui;
using InventorySystem.Data;
using MudBlazor;
using MudBlazor.Services;

namespace InventorySystem;

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
        //registering MudBlazor service
        builder.Services.AddMudServices();

#endif

        return builder.Build();
	}
}
