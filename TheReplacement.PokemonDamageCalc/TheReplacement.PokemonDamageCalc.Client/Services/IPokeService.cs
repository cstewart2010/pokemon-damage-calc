using PokeApiNet;
using TheReplacement.PokemonDamageCalc.Client.DTOs;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public interface IPokeService
    {
        public Task<List<RawPokemon>> GetPokemonAsync(string name);
        public Task<MoveData?> GetMoveAsync(string name);
        public Task<NatureData> GetNatureAsync(int index);
        public Task<List<string>> GetPokedexAsync();
        public Task<List<string>> GetMovesAsync();
        public Task<List<string>> GetItemsAsync();
        public Task<List<string>> GetNatures();
    }
}
