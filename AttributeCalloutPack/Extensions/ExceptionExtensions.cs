using System;
using System.Collections.Generic;

namespace Amethyst.FivePD.AttributeCalloutPack.Extensions
{
    public static class ExceptionExtensions
    {

        public static List<string> GetInnerExceptionMessages(
            this Exception ex
        )
        {
            List<string> results = new List<string>();

            do
            {
                results.Add($"{ex.Message}\n{ex.StackTrace}");
                ex = ex.InnerException ?? null;
            }
            while (ex != null);

            return results;
        }

    }
}
