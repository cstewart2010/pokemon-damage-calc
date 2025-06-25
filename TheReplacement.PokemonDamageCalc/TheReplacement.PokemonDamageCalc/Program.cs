using TheReplacement.PokemonDamageCalc.Client.Pages;
using TheReplacement.PokemonDamageCalc.Client.Services;
using TheReplacement.PokemonDamageCalc.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddTransient<IStatService, StatService>();
builder.Services.AddSingleton<IDamageService, DamageService>();
builder.Services.AddSingleton<IPokeApiService, PokeApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TheReplacement.PokemonDamageCalc.Client._Imports).Assembly);

app.Run();
