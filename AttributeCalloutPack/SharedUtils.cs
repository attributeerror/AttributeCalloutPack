using CitizenFX.Core;
using NativeApi = CitizenFX.Core.Native.API;
using System;
using Amethyst.FivePD.AttributeCalloutPack.Models;

namespace Amethyst.FivePD.AttributeCalloutPack
{
    internal class SharedUtils
    {

        internal static readonly Random Rng = new Random(DateTime.UtcNow.Millisecond);

        internal static Vector3 GetNearestPositionOnRoadFromEntity(
            int entityHandle
        )
        {
            float correctionX = 20;
            Vector3 coords = Entity.FromHandle(entityHandle).Position;
            while (!NativeApi.IsPointOnRoad(coords.X, coords.Y, coords.Z, entityHandle))
            {
                correctionX -= 0.5f;
                coords = NativeApi.GetOffsetFromEntityInWorldCoords(entityHandle, correctionX, 0, 0);
            }

            return coords;
        }

        internal static void RunActionWithChance(
            int chance,
            Action onSuccess,
            Action onFailure = null
        )
        {
            if (Rng.Next(100) < chance)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFailure?.Invoke();
            }
        }

        internal static void ShowNetworkedNotification(
            string text,
            string sender = "~b~Control",
            string subject = "~m~Callout Update",
            string txDict = "CHAR_CALL911",
            string txName = "CHAR_CALL911",
            NotificationIconType iconType = NotificationIconType.Nothing,
            HudColour backgroundColour = HudColour.None,
            bool flash = false,
            bool isImportant = false,
            bool saveToBrief = true
        )
        {
            NativeApi.BeginTextCommandThefeedPost("STRING");
            NativeApi.AddTextComponentSubstringPlayerName(text);
            if (backgroundColour != HudColour.None)
            {
                NativeApi.ThefeedNextPostBackgroundColor((int)backgroundColour);
            }
            NativeApi.EndTextCommandThefeedPostMessagetext(txDict, txName, flash, (int)iconType, sender, subject);
            NativeApi.EndTextCommandThefeedPostTicker(isImportant, saveToBrief);
        }

        internal static void ShowNotification(
            string text
        )
        {
            NativeApi.SetNotificationTextEntry("STRING");
            NativeApi.AddTextComponentString(text);
            NativeApi.DrawNotification(false, true);
        }

        internal static void ShowHelp(
            string text
        )
        {
            NativeApi.SetTextComponentFormat("STRING");
            NativeApi.AddTextComponentString(text);
            NativeApi.DisplayHelpTextFromStringLabel(0, false, false, -1);
        }

        internal static void DrawTextOnScreen(
            float x,
            float y,
            string text
        )
        {
            NativeApi.SetTextFont(4);
            NativeApi.SetTextProportional(true);
            NativeApi.SetTextScale(0.0f, 0.45f);
            NativeApi.SetTextColour(255, 255, 255, 255);
            NativeApi.SetTextDropshadow(0, 0, 0, 0, 255);
            NativeApi.SetTextEdge(1, 0, 0, 0, 150);
            NativeApi.SetTextOutline();

            NativeApi.SetTextEntry("STRING");
            NativeApi.AddTextComponentString(text);
            NativeApi.DrawText(x, y);
        }

    }
}
