using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheReplacement.PokemonDamageCalc.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTransient<IStatService, StatService>();
builder.Services.AddSingleton<IDamageService, DamageService>();
builder.Services.AddSingleton<IPokeService, PokeApiService>();

await builder.Build().RunAsync();
