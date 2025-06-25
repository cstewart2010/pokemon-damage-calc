namespace TheReplacement.PokemonDamageCalc.Client.Services
{
    public static class StringExtensions
    {
        public static string ToCapitalized(this string str)
        {
            return string.Join(" ", str.Split('-').Select(x => x.ToCapitalizedInternal()));
        }

        public static string ToLowerCaseKebab(this string str)
        {
            return string.Join("-", str.ToLower().Split(' '));
        }

        private static string ToCapitalizedInternal(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (str.Length <= 1) return str.ToUpper();
            var firstChar = char.ToUpper(str[0]);
            return firstChar + str.Substring(1);
        }
    }
}
