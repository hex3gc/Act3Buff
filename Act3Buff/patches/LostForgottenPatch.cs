using System.Collections.ObjectModel;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.GameInfo.Objects;
using Act3Buff.Config;

namespace Act3Buff.Patches;

/// <summary>
///     Not sure why Lost and Forgotten were nerfed; brought them back up to previous power levels
/// </summary>
internal static class LostForgottenPatch
{
    [HarmonyPatch]
    internal static class LostForgottenPatch_TheForgotten_DebilitatingSmogDexStealAmount
    {
        [HarmonyPatch(typeof(TheForgotten), "DebilitatingSmogDexStealAmount", MethodType.Getter)]
        internal static bool Prefix(TheForgotten __instance, ref int __result)
        {
            if (!Act3BuffConfig.LostForgottenEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
            return false;
        }
    }
    [HarmonyPatch]
    internal static class LostForgottenPatch_TheLost_DebilitatingSmogStrengthStealAmount
    {
        [HarmonyPatch(typeof(TheLost), "DebilitatingSmogStrengthStealAmount", MethodType.Getter)]
        internal static bool Prefix(TheLost __instance, ref int __result)
        {
            if (!Act3BuffConfig.LostForgottenEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
            return false;
        }
    }
}