using CitizenFX.Core;
using NativeApi = CitizenFX.Core.Native.API;
using Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Extensions;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Models
{
    internal struct ChosenVehicle
    {

        internal Vehicle Vehicle { get; private set; }
        internal string Colour { get; private set; }
        internal string Make { get; private set; }
        internal string Model { get; private set; }
        internal string Plate { get; private set; }
        internal Blip Blip { get; set; }

        internal ChosenVehicle(
            Vehicle vehicle
        )
        {
            Vehicle = vehicle;

            int primaryColour = -1;
            int secondaryColour = -1;
            NativeApi.GetVehicleColours(Vehicle.Handle, ref primaryColour, ref secondaryColour);
            Colour = ((VehicleColor)primaryColour).GetVehicleColourName();

            Model = vehicle.DisplayName;
            Make = NativeApi.GetMakeNameFromVehicleModel((uint)NativeApi.GetEntityModel(Vehicle.Handle));
            Plate = NativeApi.GetVehicleNumberPlateText(Vehicle.Handle);

            Blip = null;
        }
        
        internal void AttachBlip()
        {
            Blip = Vehicle.AttachBlip();
            Blip.Color = BlipColor.Red;
            Blip.ShowRoute = true;
        }

    }
}
