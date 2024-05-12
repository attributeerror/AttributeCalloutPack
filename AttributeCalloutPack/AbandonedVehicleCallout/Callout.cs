using Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Models;
using Amethyst.FivePD.AttributeCalloutPack.Models;
using CitizenFX.Core;
using FivePD.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeApi = CitizenFX.Core.Native.API;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout
{
    [CalloutProperties("Abandoned Vehicle", "AttributeError", "1.2.0")]
    public class Callout : BaseCallout
    {

        private readonly List<VehicleHash> VehiclesToChooseFrom = new List<VehicleHash>() {
            VehicleHash.Blista, VehicleHash.Brioso, VehicleHash.Dilettante, VehicleHash.Dilettante2, VehicleHash.Issi2, VehicleHash.Panto, VehicleHash.Prairie, VehicleHash.Rhapsody,
            VehicleHash.Asea, VehicleHash.Asea2, VehicleHash.Asterope, VehicleHash.Cog55, VehicleHash.Cognoscenti, VehicleHash.Emperor, VehicleHash.Emperor2, VehicleHash.Emperor3, VehicleHash.Fugitive, VehicleHash.Glendale, VehicleHash.Ingot, VehicleHash.Intruder, VehicleHash.Premier, VehicleHash.Primo, VehicleHash.Regina, VehicleHash.Schafter2, VehicleHash.Stanier, VehicleHash.Stratum, VehicleHash.Superd, VehicleHash.Tailgater, VehicleHash.Warrener, VehicleHash.Washington,
            VehicleHash.CogCabrio, VehicleHash.Exemplar, VehicleHash.F620, VehicleHash.Felon, VehicleHash.Felon2, VehicleHash.Jackal, VehicleHash.Oracle, VehicleHash.Oracle2, VehicleHash.Sentinel, VehicleHash.Sentinel2,
            VehicleHash.Akuma, VehicleHash.Avarus, VehicleHash.Bagger, VehicleHash.Bati, VehicleHash.Bati2, VehicleHash.BF400, VehicleHash.CarbonRS, VehicleHash.Chimera, VehicleHash.Cliffhanger, VehicleHash.Daemon, VehicleHash.Daemon2, VehicleHash.Defiler, VehicleHash.Double, VehicleHash.Enduro, VehicleHash.Esskey, VehicleHash.Faggio, VehicleHash.Faggio2, VehicleHash.Faggio3, VehicleHash.FCR, VehicleHash.Gargoyle, VehicleHash.Hakuchou, VehicleHash.Hexer, VehicleHash.Innovation, VehicleHash.Lectro, VehicleHash.Manchez, VehicleHash.Nemesis, VehicleHash.Nightblade, VehicleHash.PCJ, VehicleHash.RatBike, VehicleHash.Ruffian, VehicleHash.Sanchez2, VehicleHash.Sanctus, VehicleHash.Shotaro, VehicleHash.Sovereign, VehicleHash.Thrust, VehicleHash.Vader, VehicleHash.Vindicator, VehicleHash.Vortex
        };
        private VehicleHash ChosenVehicleHash { get; set; }

        internal Vector3 SpawnPoint { get; set; }
        internal Models.Scenario ChosenScenario { get; set; }
        internal Models.ChosenVehicle SpawnedVehicle { get; set; }
        internal Models.SearchArea SearchArea { get; set; }

        public Callout()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.PlayerPed.GetPositionOffset(new Vector3((float)SharedUtils.Rng.Next(100, 300), (float)SharedUtils.Rng.Next(100, 300), 0.0f)));
            ChosenScenario = Extensions.EnumExtensions.GetRandomEnumValue<Models.Scenario>();
            ChosenVehicleHash = VehiclesToChooseFrom[SharedUtils.Rng.Next(VehiclesToChooseFrom.Count)];

            this.InitInfo(SpawnPoint);

            this.ShortName = "Abandoned Vehicle";
            this.CalloutDescription = $"A caller has reported a suspicious abandoned vehicle, respond to investigate. {ChosenScenario.GetScenarioMessage()}";
            this.ResponseCode = 2;

            this.StartDistance = 75f;
        }

        public override async Task OnAccept()
        {
            InitBlip();
            Marker?.Delete();

            Vehicle vehicle = await SpawnVehicle(ChosenVehicleHash, SpawnPoint);
            vehicle.IsPersistent = true;
            SpawnedVehicle = new Models.ChosenVehicle(vehicle);

            CalloutDescription += $"You are looking for a {SpawnedVehicle.Colour} {SpawnedVehicle.Make} {SpawnedVehicle.Model} with the plate {SpawnedVehicle.Plate}";
            SharedUtils.SendCalloutUpdateNotification($"You are looking for a ~o~{SpawnedVehicle.Colour} ~y~{SpawnedVehicle.Make} {SpawnedVehicle.Model}~s~. The plate is ~r~{SpawnedVehicle.Plate}~s~.", true);
            SharedUtils.SendCalloutUpdateNotification(ChosenScenario.GetScenarioMessage(), true);

            ChosenScenario.SetupScenario(SpawnedVehicle.Vehicle.Handle);

            Vector3 nearestRoadPosition = SharedUtils.GetNearestPositionOnRoadFromEntity(SpawnedVehicle.Vehicle.Handle);
            float heading = SharedUtils.Rng.Next(0, 361);
            SpawnedVehicle.Vehicle.Position = nearestRoadPosition;
            NativeApi.SetEntityCoords(SpawnedVehicle.Vehicle.Handle, nearestRoadPosition.X, nearestRoadPosition.Y, nearestRoadPosition.Z, false, false, false, true);
            NativeApi.SetEntityHeading(SpawnedVehicle.Vehicle.Handle, heading);
            SpawnPoint = nearestRoadPosition;

            SearchArea = new Models.SearchArea(SpawnPoint, 145f);

            LoadTicker(this, TickerFriendlyName.DrawVehicleInfo, Tickers.DrawVehicleInfoTick);
            LoadTicker(this, TickerFriendlyName.UpdateSearchArea, Tickers.UpdateSearchAreaTick);
            LoadTicker(this, TickerFriendlyName.FoundVehicleCheck, Tickers.FoundVehicleCheckTick);
            LoadTicker(this, TickerFriendlyName.VehicleDestroyedCheck, Tickers.VehicleDestroyedCheckTick);
        }

        public override async void OnCancelBefore()
        {
            base.OnCancelBefore();

            // Unloads all remaining tickers
            foreach (KeyValuePair<TickerFriendlyName, Func<Task>> loadedTicker in LoadedTickers)
            {
                UnloadTicker(this, loadedTicker.Key);
            }
            
            // Delete search area blip if it still exists
            if (SearchArea != null && SearchArea.Blip.Exists())
            {
                SearchArea.Blip.Delete();
            }

            // Make vehicle non-persistent so that FiveM can clear it up.
            SpawnedVehicle.Vehicle.IsPersistent = false;

            await Task.Yield();
        }

        public async override Task<bool> CheckRequirements()
        {
            await base.CheckRequirements();

            // TODO: add configuration option for percentage chance of callout occurring?
            return World.CurrentDayTime.Hours >= 9 && World.CurrentDayTime.Hours <= 20;
        }

    }
}
