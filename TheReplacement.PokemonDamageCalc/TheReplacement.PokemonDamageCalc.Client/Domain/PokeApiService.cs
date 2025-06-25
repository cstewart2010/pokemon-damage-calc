using PokeApiNet;
using TheReplacement.PokemonDamageCalc.Client.Constants;
using TheReplacement.PokemonDamageCalc.Client.DataModel;
using TheReplacement.PokemonDamageCalc.Client.DTOs;
using TheReplacement.PokemonDamageCalc.Client.Extensions;

namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public class PokeApiService : IPokeService
    {
        private readonly PokeApiClient _client;
        private readonly List<NamedApiResource<Nature>> _natures;

        public PokeApiService()
        {
            _client = new PokeApiClient();
            _natures = _client.GetNamedResourcePageAsync<Nature>(30, 0).Result.Results;
        }

        public async Task<List<RawPokemon>> GetPokemonAsync(string name)
        {
            var pokemon = await _client.GetResourceAsync<Pokemon>(name);
            var species = await _client.GetResourceAsync(pokemon.Species);
            List<Pokemon> collection = [];
            foreach (var item in species.Varieties)
            {
                collection.Add(await _client.GetResourceAsync(item.Pokemon));
            }

            return [.. collection.Select(x => new RawPokemon
            {
                SpeciesName = x.Name,
                Stats = new BaseStats
                {
                    HP = GetBaseStat(x, Stats.HP),
                    Attack = GetBaseStat(x, Stats.Attack),
                    Defense = GetBaseStat(x, Stats.Defense),
                    SpecialAttack = GetBaseStat(x, Stats.SpecialAttack),
                    SpecialDefense = GetBaseStat(x, Stats.SpecialDefense),
                    Speed = GetBaseStat(x, Stats.Speed),
                },
                Types = x.Types.Select(y => y.Type.Name).ToList(),
                Weight = x.Weight,
                Abilities = [.. x.Abilities.Select(y => new AbilityData
                {
                    Name = y.Ability.Name,
                    IsHidden = y.IsHidden,
                })],
            })];
        }

        public async Task<MoveData?> GetMoveAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var move = await _client.GetResourceAsync<Move>(name);
            return new MoveData
            {
                Name = move.Name,
                BasePower = move.Power,
                DamageClass = move.DamageClass.Name,
                Target = move.Target.Name,
                Type = move.Type.Name,
                FlavorText = move.FlavorTextEntries.FirstOrDefault(x => x.Language.Name == "en" && x.VersionGroup.Name == "scarlet-violet")?.FlavorText ?? "Not present in Scarlet/Violet"
            };
        }

        public async Task<NatureData> GetNatureAsync(int index)
        {
            var nature = await _client.GetResourceAsync(_natures[index]);

            return new NatureData
            {
                Name = nature.Name,
                IncreasedStat = nature.IncreasedStat?.Name,
                DecreasedStat = nature?.DecreasedStat?.Name
            };
        }

        public async Task<List<string>> GetPokedexAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<PokemonSpecies>(2000, 0);
            return [.. list.Results.Select(x => x.Name.ToCapitalized())];
        }

        public async Task<List<string>> GetMovesAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<Move>(1000, 0);
            return list.Results.Select(x => x.Name.ToCapitalized()).ToList();
        }

        public async Task<List<string>> GetItemsAsync()
        {
            var list = await _client.GetNamedResourcePageAsync<Item>(3000, 0);
            return [.. list.Results.Select(x => x.Name.ToCapitalized())];
        }

        public async Task<List<string>> GetNatures()
        {
            return await Task.Run<List<string>>(() =>
            {
                return [.. _natures.Select(x => x.Name.ToCapitalized())];
            });
        }

        private static int GetBaseStat(Pokemon pokemon, string statName)
        {
            return pokemon.Stats.First(x => x.Stat.Name == statName).BaseStat;
        }
    }
}
