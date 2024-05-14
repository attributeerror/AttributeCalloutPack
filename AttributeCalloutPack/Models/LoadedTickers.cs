using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.FivePD.AttributeCalloutPack.Models
{
    internal class LoadedTickers : Dictionary<TickerFriendlyName, Func<Task>>
    {
    }
}
