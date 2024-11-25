using IntelliPath.Desktop.Configuration;
using IntelliPath.Desktop.Services;
using IntelliPath.Desktop.State;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

namespace IntelliPath.Desktop;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder.Configuration
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

        builder.Services.AddSingleton<AppState>();

        builder.Services.Configure<IdentityOptions>(options =>
			builder.Configuration.GetSection("Identity").Bind(options));

		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IGraphClientFactory, GraphClientFactory>();
		builder.Services.AddHttpClient<IMemoryClient, MemoryClient>(client =>
		{
		    client.BaseAddress = new Uri("https://localhost:7019");
		});
		
		

		builder.Services.AddFluentUIComponents();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
