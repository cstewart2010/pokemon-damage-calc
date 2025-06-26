namespace TheReplacement.PokemonDamageCalc.Services
{
    using TheReplacement.PokemonDamageCalc.DTOs;

    public interface IPokeService
    {
        public Task<List<RawPokemon>> GetPokemonAsync(string name);
        public Task<MoveData?> GetMoveAsync(string name);
        public Task<NatureData> GetNatureAsync(int index);
        public Task<ICollection<string>> GetPokedexAsync();
        public Task<ICollection<string>> GetMovesAsync();
        public Task<ICollection<string>> GetItemsAsync();
        public Task<List<string>> GetNaturesAsync();
    }
}
