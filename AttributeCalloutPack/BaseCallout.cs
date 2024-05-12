using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FivePdApi = FivePD.API;

namespace Amethyst.FivePD.AttributeCalloutPack
{
    public class BaseCallout : FivePdApi.Callout
    {

        internal static Dictionary<Models.TickerFriendlyName, Func<Task>> LoadedTickers = new Dictionary<Models.TickerFriendlyName, Func<Task>>();

        internal static void LoadTicker(
            BaseCallout callout,
            Models.TickerFriendlyName friendlyName,
            Func<BaseCallout, Task> ticker
        )
        {
            if (!LoadedTickers.ContainsKey(friendlyName))
            {
                Task addedTicker() => ticker(callout);
                callout.Tick += addedTicker;
                LoadedTickers.Add(friendlyName, addedTicker);
            }
        }

        internal static void UnloadTicker(
            BaseCallout callout,
            Models.TickerFriendlyName friendlyName
        )
        {
            if (LoadedTickers.TryGetValue(friendlyName, out var ticker))
            {
                callout.Tick -= ticker;
                LoadedTickers.Remove(friendlyName);
            }
        }

    }
}
