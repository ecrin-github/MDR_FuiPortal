using MDR_FuiPortal.Server;
using Microsoft.Fast.Components.FluentUI;


var options = new WebApplicationOptions() { WebRootPath = "wwwroot"};
var builder = WebApplication.CreateBuilder(options);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddFluentUIComponents();

builder.Services.AddSingleton<ICredentials, Credentials>();
builder.Services.AddSingleton<ILookUpRepo, LookUpRepo>();
builder.Services.AddSingleton<ITreeRepo, TreeRepo>();
builder.Services.AddScoped<IObjectRepo, ObjectRepo>();
builder.Services.AddScoped<IStudyRepo, StudyRepo>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

/***************************************************************
 From the docs online but so far have not had to implement this...
 The app currentlyu seems to find everything by default.

*Hosted Blazor WebAssembly
If the app is a hosted Blazor WebAssembly app:

In the Server project (Program.cs):
Adjust the path of UseBlazorFrameworkFiles (for example, app.UseBlazorFrameworkFiles("/base/path");).
Configure calls to UseStaticFiles (for example, app.UseStaticFiles("/base/path");).
In the Client project:
Configure <StaticWebAssetBasePath> in the project file to match the path for serving static web assets 
(for example, <StaticWebAssetBasePath>base/path</StaticWebAssetBasePath> ).
Configure the <base> tag, per the guidance in the Configure the app base path section.
For an example of hosting multiple Blazor WebAssembly apps in a hosted Blazor WebAssembly solution, 
see Multiple hosted ASP.NET Core Blazor WebAssembly apps, where approaches are explained for 
domain/port hosting and subpath hosting of multiple Blazor WebAssembly client apps.
* 
 ***************************************************************/

app.UseBlazorFrameworkFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseStaticFiles();

app.Run();
