using CitizenFX.Core;
using System.Threading.Tasks;
using NativeApi = CitizenFX.Core.Native.API;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout
{
    internal class Tickers
    {

        internal enum FriendlyName
        {

            DrawVehicleInfo,
            VehicleDestroyedCheck,
            UpdateSearchArea,
            FoundVehicleCheck,
            VehicleAttachedCheck,

        }

        internal static async Task DrawVehicleInfoTick(
            Callout callout
        )
        {
            NativeApi.DrawRect(0.0775f, 0.23f, 0.13f, 0.11f, 0, 0, 0, 150); // Main box
            NativeApi.DrawRect(0.0775f, 0.19f, 0.13f, 0.03f, 0, 0, 0, 175); // Title box

            SharedUtils.DrawTextOnScreen(0.043f, 0.172f, "Vehicle Information");
            SharedUtils.DrawTextOnScreen(0.044f, 0.21f, $"{callout.SpawnedVehicle.Colour} {callout.SpawnedVehicle.Make} {callout.SpawnedVehicle.Model}");
            SharedUtils.DrawTextOnScreen(0.06f, 0.24f, callout.SpawnedVehicle.Plate);

            await Task.Yield();
        }

        internal static async Task VehicleDestroyedCheckTick(
            Callout callout
        )
        {
            if (!callout.SpawnedVehicle.Vehicle.Exists() || callout.SpawnedVehicle.Vehicle.IsDead)
            {
                if (callout.SpawnedVehicle.Vehicle.Exists())
                {
                    callout.SpawnedVehicle.Vehicle.Delete();
                }

                SharedUtils.ShowNetworkedNotification(
                    text: "The vehicle no longer exists, or has been destroyed.",
                    sender: "~r~AttributeCalloutPack",
                    subject: "~r~Callout Ended",
                    flash: true,
                    isImportant: true
                );

                callout.EndCallout();
            }

            await Task.Yield();
        }

        internal static async Task UpdateSearchAreaTick(
            Callout callout
        )
        {
            callout.Marker?.Delete();
            await callout.SearchArea.Update();
        }

        internal static async Task FoundVehicleCheckTick(
            Callout callout
        )
        {
            await Task.Yield();

            if (Vector3.Distance(Game.PlayerPed.Position, callout.SpawnedVehicle.Vehicle.Position) >= 20f)
            {
                return;
            }

            if (Game.PlayerPed.IsInVehicle() && Vector3.Distance(Game.PlayerPed.Position, callout.SpawnedVehicle.Vehicle.Position) >= 5f)
            {
                return;
            }

            Callout.UnloadTicker(callout, FriendlyName.FoundVehicleCheck);
            Callout.UnloadTicker(callout, FriendlyName.UpdateSearchArea);
            Callout.LoadTicker(callout, FriendlyName.VehicleAttachedCheck, VehicleAttachedCheckTick);

            int blipHandle = callout.SearchArea.Blip.Handle;
            NativeApi.RemoveBlip(ref blipHandle);
            if (callout.SearchArea.Blip.Exists())
            {
                callout.SearchArea.Blip.Delete();
            }
            callout.SearchArea = null;

            callout.SpawnedVehicle.AttachBlip();

            SharedUtils.ShowNetworkedNotification(
                text: "Control, I've located the abandoned vehicle.",
                sender: "~b~You"
            );
            SharedUtils.ShowHelp("Write up your report and tow the vehicle.");
        }

        internal static async Task VehicleAttachedCheckTick(
            Callout callout
        )
        {
            await Task.Yield();

            if (callout.SpawnedVehicle.Vehicle.IsAttached())
            {
                Callout.UnloadTicker(callout, FriendlyName.VehicleAttachedCheck);
                SharedUtils.ShowNetworkedNotification(
                    text: "Control, tow has arrived and has towed the vehicle. I'll be back available for calls.",
                    sender: "~b~You"
                );
                callout.EndCallout();
            }
        }

    }
}
