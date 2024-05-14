using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.FivePD.AttributeCalloutPack
{
    public class CalloutUtils
    {

        internal static Dictionary<string, Models.LoadedTickers> LoadedTickers = new Dictionary<string, Models.LoadedTickers>();

        internal static Func<Task> LoadTicker(
            string calloutCaseId,
            Models.TickerFriendlyName friendlyName,
            Func<Task> ticker
        )
        {
            if (!LoadedTickers.ContainsKey(calloutCaseId))
            {
                LoadedTickers.Add(calloutCaseId, new Models.LoadedTickers());
            }

            if (LoadedTickers[calloutCaseId].ContainsKey(friendlyName))
            {
                LoadedTickers[calloutCaseId].Add(friendlyName, ticker);
                return ticker;
            }

            return null;
        }

        internal static Func<Task> UnloadTicker(
            string calloutCaseId,
            Models.TickerFriendlyName friendlyName
        )
        {
            if (LoadedTickers.TryGetValue(calloutCaseId, out Models.LoadedTickers tickers))
            {
                if (tickers.TryGetValue(friendlyName, out var ticker))
                {
                    tickers.Remove(friendlyName);
                    return ticker;
                }
            }

            return null;
        }

        internal static void CleanupCallout(
            string calloutCaseId
        )
        {
            if (LoadedTickers.TryGetValue(calloutCaseId, out _))
            {
                LoadedTickers.Remove(calloutCaseId);
            }
        }

    }
}
