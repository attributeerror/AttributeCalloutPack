using CitizenFX.Core;
using System;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Extensions
{
    internal static class FiveMExtensions
    {

        internal static string GetVehicleColourName(
            this VehicleColor color
        )
        {
            string name = Enum.GetName(typeof(VehicleColor), color);
            return name.AddSpacesToString(true);
        }

        internal static void KeepTask(
            this Ped @this
        )
        {
            @this.BlockPermanentEvents = true;
            @this.AlwaysKeepTask = true;
        }

    }
}
