using PokeApiNet;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public interface IPokeApiService
    {
        public Task<List<Pokemon>> GetPokemonAsync(string name);
        public Task<Move?> GetMoveAsync(string name);
        public Task<Nature> GetNatureAsync(NamedApiResource<Nature> resource);
        public Task<List<string>> GetPokedexAsync();
        public Task<List<string>> GetMovesAsync();
        public Task<List<NamedApiResource<Item>>> GetItemsAsync();
        public Task<List<NamedApiResource<Nature>>> GetNatures();
    }
}
