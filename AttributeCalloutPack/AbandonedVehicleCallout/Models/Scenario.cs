using CitizenFX.Core;
using NativeApi = CitizenFX.Core.Native.API;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Models
{

    internal enum Scenario
    {

        Abandoned,
        FrontendCollision,
        RearendCollision,
        BrokenDown,
        TiresSlashed,

    }

    internal static class ScenarioExtensions
    {

        internal static string GetScenarioMessage(
            this Scenario @this
        )
        {
            switch (@this)
            {
                case Scenario.Abandoned:
                    return "The vehicle has been abandoned for a few days and no-one has seen the owner.";
                case Scenario.FrontendCollision:
                    return "The vehicle looks to have been in a front-end collision and has been abandoned.";
                case Scenario.RearendCollision:
                    return "The vehicle looks to have been in a rear-end collision and has been abandoned.";
                case Scenario.BrokenDown:
                    return "The vehicle has broken down and there's smoke coming from the engine.";
                case Scenario.TiresSlashed:
                    return "The vehicle has been abandoned and some of the tires have been slashed.";
                default:
                    return "Invalid scenario?";
            }
        }

        internal static async void SetupScenario(
            this Scenario @this,
            int spawnedVehicleHandle
        )
        {
            if (@this != Scenario.Abandoned && @this != Scenario.TiresSlashed)
            {
                NativeApi.SetVehicleEngineHealth(spawnedVehicleHandle, 3f); // make vehicle smoke
                NativeApi.SetVehicleDoorOpen(spawnedVehicleHandle, (int)VehicleDoorIndex.FrontRightDoor, false, false);

                int doorProbability = SharedUtils.RandomProvider.GetThreadRandom().Next(10);
                if (doorProbability % 2 == 0)
                {
                    int vehicleDoor;
                    do
                    {
                        await BaseScript.Delay(0);
                        vehicleDoor = (int)Extensions.EnumExtensions.GetRandomEnumValue<VehicleDoorIndex>();
                    }
                    while (!NativeApi.DoesVehicleHaveDoor(spawnedVehicleHandle, vehicleDoor));

                    NativeApi.SetVehicleDoorOpen(spawnedVehicleHandle, vehicleDoor, false, false);
                }
            }

            if (@this == Scenario.FrontendCollision)
            {
                NativeApi.SmashVehicleWindow(spawnedVehicleHandle, (int)VehicleWindowIndex.ExtraWindow3); // Smash windscreen
                NativeApi.SetVehicleDoorOpen(spawnedVehicleHandle, (int)VehicleDoorIndex.Hood, false, false); // Pop hood to simulate front-end collision
            }
            else if (@this == Scenario.RearendCollision)
            {
                NativeApi.SmashVehicleWindow(spawnedVehicleHandle, 8); // Smash rear windscreen - not sure why VehicleWindowIndex doesn't have one for this?
                NativeApi.SetVehicleDoorOpen(spawnedVehicleHandle, (int)VehicleDoorIndex.Trunk, false, false); // Pop trunk to simulate rear-end collision.
            }
            else if (@this == Scenario.BrokenDown)
            {
                NativeApi.SetVehicleUndriveable(spawnedVehicleHandle, true);
                NativeApi.SetVehicleEngineOn(spawnedVehicleHandle, false, true, true);
            }
            else if (@this == Scenario.TiresSlashed)
            {
                int tiresToSlash = SharedUtils.RandomProvider.GetThreadRandom().Next(8);
                for (int i = 0; i < tiresToSlash; i++)
                {
                    NativeApi.SetVehicleTyreBurst(spawnedVehicleHandle, i, SharedUtils.RandomProvider.GetThreadRandom().Next(100) % 3 == 0, SharedUtils.RandomProvider.GetThreadRandom().Next(500, 1000));
                }
            }
        }

    }

}
