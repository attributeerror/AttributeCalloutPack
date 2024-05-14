using System;
using System.ComponentModel;
using System.Reflection;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Extensions
{
    internal static class EnumExtensions
    {

        internal static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(SharedUtils.RandomProvider.GetThreadRandom().Next(values.Length));
        }

        internal static string GetEnumDescription<T>(
            this T @this
        ) where T : Enum
        {
            Type type = @this.GetType();
            string name = Enum.GetName(type, @this);
            if (!string.IsNullOrWhiteSpace(name))
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }

            return @this.ToString();
        }

    }
}
