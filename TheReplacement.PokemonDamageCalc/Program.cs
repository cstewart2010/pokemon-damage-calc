using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheReplacement.PokemonDamageCalc;
using TheReplacement.PokemonDamageCalc.Domain;
using TheReplacement.PokemonDamageCalc.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<IStatService, StatService>();
builder.Services.AddSingleton<IDamageService, DamageService>();
builder.Services.AddSingleton<IPokeService, PokeApiService>();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
