using Amethyst.FivePD.AttributeCalloutPack.Models;
using CitizenFX.Core;
using System.Threading.Tasks;
using NativeApi = CitizenFX.Core.Native.API;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout
{

    /// <summary>
    /// Partial class of Callout to allow for splitting ticker logic out into its own file.
    /// </summary>
    public partial class AbandonedVehicleCallout
    {

        internal async Task DrawVehicleInfoTick()
        {
            NativeApi.DrawRect(0.0775f, 0.23f, 0.13f, 0.11f, 0, 0, 0, 150); // Main box
            NativeApi.DrawRect(0.0775f, 0.19f, 0.13f, 0.03f, 0, 0, 0, 175); // Title box

            SharedUtils.DrawTextOnScreen(0.043f, 0.172f, "Vehicle Information");
            SharedUtils.DrawTextOnScreen(0.044f, 0.21f, $"{SpawnedVehicle.Colour} {SpawnedVehicle.Make} {SpawnedVehicle.Model}");
            SharedUtils.DrawTextOnScreen(0.06f, 0.24f, SpawnedVehicle.Plate);
            await Task.Yield();
        }

        internal async Task VehicleDestroyedCheckTick()
        {
            if (!SpawnedVehicle.Vehicle.Exists() || SpawnedVehicle.Vehicle.IsDead)
            {
                if (SpawnedVehicle.Vehicle.Exists())
                {
                    SpawnedVehicle.Vehicle.Delete();
                }

                SharedUtils.ShowNetworkedNotification(
                    text: "The vehicle no longer exists, or has been destroyed.",
                    sender: "~r~AttributeCalloutPack",
                    subject: "~r~Callout Ended",
                    flash: true,
                    isImportant: true
                );

                EndCallout();
            }

            await Task.Yield();
        }

        internal async Task UpdateSearchAreaTick()
        {
            Marker?.Delete();
            await SearchArea.Update();
        }

        internal async Task FoundVehicleCheckTick()
        {
            await Task.Yield();

            if (Vector3.Distance(Game.PlayerPed.Position, SpawnedVehicle.Vehicle.Position) >= 20f)
            {
                return;
            }

            if (Game.PlayerPed.IsInVehicle() && Vector3.Distance(Game.PlayerPed.Position, SpawnedVehicle.Vehicle.Position) >= 5f)
            {
                return;
            }

            Tick -= CalloutUtils.UnloadTicker(this.CaseID, TickerFriendlyName.AVCFoundVehicleCheck);
            Tick -= CalloutUtils.UnloadTicker(this.CaseID, TickerFriendlyName.AVCUpdateSearchArea);
            Tick += CalloutUtils.LoadTicker(this.CaseID, TickerFriendlyName.AVCVehicleAttachedCheck, VehicleAttachedCheckTick);

            int blipHandle = SearchArea.Blip.Handle;
            NativeApi.RemoveBlip(ref blipHandle);
            if (SearchArea.Blip.Exists())
            {
                SearchArea.Blip.Delete();
            }
            SearchArea = null;

            SpawnedVehicle.AttachBlip();

            SharedUtils.SendCalloutUpdateNotification(
                text: "Control, I've located the abandoned vehicle.",
                isFromDispatch: false
            );
            SharedUtils.ShowHelp("Write up your report and tow the vehicle.");
        }

        internal async Task VehicleAttachedCheckTick()
        {
            await Task.Yield();

            if (SpawnedVehicle.Vehicle.IsAttached())
            {
                Tick -= CalloutUtils.UnloadTicker(this.CaseID, TickerFriendlyName.AVCVehicleAttachedCheck);
                SharedUtils.SendCalloutUpdateNotification(
                    text: "Control, tow has arrived and has towed the vehicle. I'll be back available for calls.",
                    isFromDispatch: false
                );
                EndCallout();
            }
        }

    }
}
