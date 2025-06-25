using PokeApiNet;
using TheReplacement.PokemonDamageCalc.Client.Extensions;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly PokeApiClient _client;

        public PokeApiService()
        {
            _client = new PokeApiClient();
        }

        public async Task<List<Pokemon>> GetPokemonAsync(string name)
        {
            var pokemon = await _client.GetResourceAsync<Pokemon>(name);
            var species = await _client.GetResourceAsync(pokemon.Species);
            List<Pokemon> collection = [];
            foreach (var item in species.Varieties)
            {
                collection.Add(await _client.GetResourceAsync(item.Pokemon));
            }

            return collection;
        }

        public async Task<Move?> GetMoveAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var move = await _client.GetResourceAsync<Move>(name);
            return move;
        }

        public async Task<Nature> GetNatureAsync(NamedApiResource<Nature> resource)
        {
            return await _client.GetResourceAsync(resource);
        }

        public async Task<List<string>> GetPokedexAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<PokemonSpecies>(2000, 0);
            return list.Results.Select(x => x.Name.ToCapitalized()).ToList();
        }

        public async Task<List<string>> GetMovesAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<Move>(1000, 0);
            return list.Results.Select(x => x.Name.ToCapitalized()).ToList();
        }

        public async Task<List<NamedApiResource<Item>>> GetItemsAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<Item>(3000, 0);
            return list.Results;
        }

        public async Task<List<NamedApiResource<Nature>>> GetNatures()
        {
            var list = await _client.GetNamedResourcePageAsync<Nature>(30, 0);
            return list.Results;
        }
    }
}
