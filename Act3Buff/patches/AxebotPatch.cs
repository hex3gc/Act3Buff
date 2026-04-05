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
using MegaCrit.Sts2.Core.Runs;

namespace Act3Buff.Patches;

/// <summary>
///     Axebots are a rather easy fight; debuffs on kill can make for more interesting choices per turn
/// </summary>
internal static class AxebotPatch
{
    [HarmonyPatch]
    internal static class AxebotPatch_StockPower_AfterDeath
    {
        [HarmonyPatch(typeof(StockPower), "AfterDeath")]
        internal static async void Postfix(StockPower __instance, PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
        {
            if (!Act3BuffConfig.AxebotEnabled) { return; }

            if (target == __instance.Owner)
            {
                await PowerCmd.Apply<VulnerablePower>(__instance.Owner.CombatState.Creatures.Where((Creature c) => c.Side == CombatSide.Player), (decimal)Act3BuffConfig.AxebotVulnAdd, null, null);
                await PowerCmd.Apply<WeakPower>(__instance.Owner.CombatState.Creatures.Where((Creature c) => c.Side == CombatSide.Player), (decimal)Act3BuffConfig.AxebotWeakAdd, null, null);
            }
        }
    }
}