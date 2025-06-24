using PokeApiNet;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public static class PokeService
    {
        private static readonly PokeApiClient Client;

        static PokeService()
        {
            Client = new PokeApiClient();
        }

        public static async Task<List<Pokemon>> GetPokemonAsync(string name)
        {
            var pokemon = await Client.GetResourceAsync<Pokemon>(name);
            var species = await Client.GetResourceAsync(pokemon.Species);
            List<Pokemon> collection = [];
            foreach (var item in species.Varieties)
            {
                collection.Add(await Client.GetResourceAsync(item.Pokemon));
            }

            return collection;
        }

        public static async Task<Move?> GetMoveAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var move = await Client.GetResourceAsync<Move>(name);
            return move;
        }

        public static async Task<Nature> GetNatureAsync(NamedApiResource<Nature> resource)
        {
            return await Client.GetResourceAsync(resource);
        }

        public static async Task<List<string>> GetPokedexAsync()
        {
            var list = await Client.GetNamedResourcePageAsync<PokemonSpecies>(2000, 0);
            return list.Results.Select(x => x.Name.ToCapitalized()).ToList();
        }

        public static async Task<List<string>> GetMovesAsync()
        {
            var list = await Client.GetNamedResourcePageAsync<Move>(1000, 0);
            return list.Results.Select(x => x.Name.ToCapitalized()).ToList();
        }

        public static async Task<List<NamedApiResource<Nature>>> GetNatures()
        {
            var list = await Client.GetNamedResourcePageAsync<Nature>(30, 0);
            return list.Results;
        }
    }
}
