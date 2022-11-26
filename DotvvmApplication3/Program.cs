using DotVVM.Framework.Hosting;
using DotvvmApplication3;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddDataProtection();
builder.Services.AddAuthorization();
builder.Services.AddWebEncoders();
builder.Services.AddDotVVM<DotvvmStartup>();
builder.Services.AddSingleton<IDotvvmPresenter, CompiledViewPresenter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseDotVVM<DotvvmStartup>(app.Environment.ContentRootPath);

app.Run();
