using CitizenFX.Core;
using NativeApi = CitizenFX.Core.Native.API;
using System.Threading.Tasks;

namespace Amethyst.FivePD.AttributeCalloutPack.AbandonedVehicleCallout.Models
{
    internal class SearchArea
    {

        private readonly Vector3 _coords;
        private readonly float _radius;

        internal Blip Blip { get; set; }
        internal bool ShouldRouteShow
        {
            get
            {
                float distanceToArea = Vector3.Distance(Game.PlayerPed.Position, _coords);
                return distanceToArea > _radius;
            }
        }

        internal SearchArea(
            Vector3 coords,
            float radius = 145f
        )
        {
            _coords = coords;
            _radius = radius;

            Blip = CreateBlip();
        }

        internal Blip CreateBlip()
        {
            Blip blip = new Blip(NativeApi.AddBlipForRadius(_coords.X, _coords.Y, _coords.Z, _radius))
            {
                Name = "Search Area",
                Color = BlipColor.Yellow,
                Alpha = 45,
                ShowRoute = ShouldRouteShow
            };

            return blip;
        }

        internal async Task Update()
        {
            if (!Blip.Exists())
            {
                Blip = CreateBlip();
            }
            else
            {
                Blip.ShowRoute = ShouldRouteShow;
            }

            await Task.Yield();
        }

    }
}
