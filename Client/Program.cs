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

var app = builder.Build();
await app.RunAsync();

