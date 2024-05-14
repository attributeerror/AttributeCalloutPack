using CitizenFX.Core;
using NativeApi = CitizenFX.Core.Native.API;
using System;
using Amethyst.FivePD.AttributeCalloutPack.Models;
using FivePD.API;
using FivePD.API.Utils;
using Amethyst.FivePD.AttributeCalloutPack.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Amethyst.FivePD.AttributeCalloutPack
{
    internal class SharedUtils
    {

        private const string _logHeader = "^3================= ^*^6AttributeCalloutPack ^3=================^r";

        internal static void LogCalloutDebug(
            string message,
            [CallerMemberName] string callerMember = "",
            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            string debugLog = $"{_logHeader}\n";
            if (!string.IsNullOrEmpty(callerMember))
            {
                debugLog += $"[{callerMember}:{callerLineNumber}] ";
            }
            debugLog += $"{message}\n{_logHeader}";

            Debug.WriteLine(debugLog);
        }

        internal static void LogCalloutError(
            Exception ex,
            [CallerMemberName] string callerMember = "",
            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            List<string> exceptionMessages = ex.GetInnerExceptionMessages();

            string debugLog = $"{_logHeader}\n";
            if (string.IsNullOrEmpty(callerMember))
            {
                debugLog += $"{_logHeader}\n^8An error has occurred!^r";
            }
            else
            {
                debugLog += $"{_logHeader}\n^8An error has occurred at {callerMember}:{callerLineNumber}!^r";
            }

            for (int i = 1; i <=  exceptionMessages.Count; i++)
            {
                string message = exceptionMessages[i];
                debugLog += $"\n^5Exception #{i}^r: {message}";
            }

            debugLog += $"\n{_logHeader}";

            Debug.WriteLine(debugLog);
        }

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
            if (RandomProvider.GetThreadRandom().Next(100) < chance)
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
            string sender = "~y~Dispatch",
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

        internal static void SendCalloutUpdateNotification(
            string text,
            bool isFromDispatch
        )
        {
            ShowNetworkedNotification(text, isFromDispatch ? "~y~Dispatch" : $"~b~{GetPlayerCallsign()}");
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

        internal static string GetPlayerCallsign()
        {
            PlayerData playerData = Utilities.GetPlayerData();
            string callsign = $"{playerData.DisplayName} {playerData.Callsign}";
            return callsign.Length < 1 ? "You" : callsign;
        }

        internal static class RandomProvider
        {
            private static int seed = Environment.TickCount;

            private readonly static ThreadLocal<Random> ThreadRandom = new ThreadLocal<Random>
                (() => new Random(Interlocked.Increment(ref seed)));

            internal static Random GetThreadRandom()
            {
                return ThreadRandom.Value;
            }
        }

    }
}
