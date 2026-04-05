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
///     Globehead now prohibits powers much more, forcing you to rely on your skills and attacks
/// </summary>
internal static class GlobeheadPatch
{
    // Galvanize power increase
    [HarmonyPatch]
    internal static class GlobeheadPatch_Globehead_AfterAddedToRoom
    {
        [HarmonyPatch(typeof(GlobeHead), "AfterAddedToRoom")]
        internal static bool Prefix(GlobeHead __instance, ref Task __result)
        {
            if (!Act3BuffConfig.GlobeheadEnabled) { return true; }

            __result = Task.CompletedTask;
            return false;
        }
        [HarmonyPatch(typeof(GlobeHead), "AfterAddedToRoom")]
        internal static async void Postfix(GlobeHead __instance)
        {
            if (!Act3BuffConfig.GlobeheadEnabled) { return; }

            await PowerCmd.Apply<GalvanicPower>(__instance.Creature, (int)Act3BuffConfig.GlobeheadPowerDamage, __instance.Creature, null);
        }
    }
}