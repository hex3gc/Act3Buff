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
using Act3Buff.Powers;
using Godot;

namespace Act3Buff.Patches;

/// <summary>
///     Soul Nexus needs a gimmick, so here he counters multi-attacks
/// </summary>
internal static class SoulNexusPatch
{
    [HarmonyPatch]
    internal static class SoulNexusPatch_SoulNexus_AfterAddedToRoom
    {
        [HarmonyPatch(typeof(SoulNexus), "AfterAddedToRoom")]
        internal static async void Postfix(SoulNexus __instance)
        {
            if (!Act3BuffConfig.SoulNexusEnabled) { return; }

            await PowerCmd.Apply<EphemeralPower>(__instance.Creature, (int)Act3BuffConfig.SoulNexusHits, __instance.Creature, null);
            __instance.Creature.PowerApplied += AfterPowerApplied;
            __instance.Creature.PowerRemoved += AfterPowerRemoved;
        }
    }

    private static void AfterPowerApplied(PowerModel power)
    {
        if (power is IntangiblePower)
        {
            (NCombatRoom.Instance?.GetCreatureNode(power.Owner))?.GetSpecialNode<CanvasGroup>("%CanvasGroup")?.SetSelfModulate(StsColors.halfTransparentWhite);
        }
    }

    private static void AfterPowerRemoved(PowerModel power)
    {
        if (power is IntangiblePower)
        {
            (NCombatRoom.Instance?.GetCreatureNode(power.Owner))?.GetSpecialNode<CanvasGroup>("%CanvasGroup")?.SetSelfModulate(Colors.White);
        }
    }
}