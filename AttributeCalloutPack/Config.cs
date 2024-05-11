using Newtonsoft.Json.Linq;
using NativeApi = CitizenFX.Core.Native.API;
using System;
using System.Reflection;
using CitizenFX.Core;

namespace Amethyst.FivePD.AttributeCalloutPack
{
    internal class Config : JObject
    {

        private readonly static JObject DefaultConfig = new JObject()
        {

        };

        internal static JObject LoadConfig(
            string calloutFolder = "",
            string filename = "config.json",
            string resourceName = "fivepd"
        )
        {
            if (string.IsNullOrWhiteSpace(calloutFolder))
            {
                calloutFolder = Assembly.GetCallingAssembly().GetName().Name.Replace(".net", "");
            }

            string configPath = $"callouts/{calloutFolder}/{filename}";

            JObject config = DefaultConfig;
            try
            {
                string data = NativeApi.LoadResourceFile(resourceName, $"/{configPath}");

                if (data == null)
                {
                    Debug.WriteLine($"'data' is null; config file not found - reverting to default!");
                }
                else
                {
                    config = JObject.Parse(data);
                }
            }
            catch (Exception)
            {
                var action = new Action(async () =>
                {
                    await BaseScript.Delay(5000);
                    Debug.WriteLine(
                        $"^6Could not find config. ^5Expected path: {configPath}\n^2Reverting to default config."
                    );
                    config = DefaultConfig;
                });
                action();
            }

            return config;
        }

        internal Config(
            string calloutFolder = "",
            string filename = "config.json",
            string resourceName = "fivepd"
        ) : base(LoadConfig(calloutFolder, filename, resourceName))
        {
            if (this == null)
            {
                Debug.WriteLine(
                    $"There was an issue whilst loading your provided default config!"
                );
            }
        }

    }
}
