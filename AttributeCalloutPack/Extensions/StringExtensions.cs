namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Extensions
{
    internal static class StringExtensions
    {

        internal static string AddSpacesToString(
            this string @this,
            bool preserveAcronyms = true
        )
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return string.Empty;
            }

            string result = string.Empty;
            result += @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (char.IsUpper(@this[i]))
                {   
                    if ((@this[i - 1] != ' ' && !char.IsUpper(@this[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(@this[i - 1]) && i < @this.Length - 1 && !char.IsUpper(@this[i + 1]))
                    )
                    {
                        result += ' ';
                    }
                }
                result += @this[i];
            }

            return result;
        }

    }
}
