using MDR_FuiPortal.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
        new HttpClient 
        { 
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
        });
builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();

/***************************************************************
 *Hosted Blazor WebAssembly
If the app is a hosted Blazor WebAssembly app:

In the in the Server project (Program.cs):
Adjust the path of UseBlazorFrameworkFiles (for example, app.UseBlazorFrameworkFiles("/base/path");).
Configure calls to UseStaticFiles (for example, app.UseStaticFiles("/base/path");).
In the Client project:
Configure <StaticWebAssetBasePath> in the project file to match the path for serving static web assets (for example, <StaticWebAssetBasePath>base/path</StaticWebAssetBasePath> ).
Configure the <base> tag, per the guidance in the Configure the app base path section.
For an example of hosting multiple Blazor WebAssembly apps in a hosted Blazor WebAssembly solution, see Multiple hosted ASP.NET Core Blazor WebAssembly apps, where approaches are explained for domain/port hosting and subpath hosting of multiple Blazor WebAssembly client apps.
 * 
 ***************************************************************/
