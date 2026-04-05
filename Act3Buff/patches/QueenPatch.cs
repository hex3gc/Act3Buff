using System.Collections.ObjectModel;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
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
///     Buffs the Queen's Binding power and Torch Head Amalgam.
///     Heavily enourages a deck with good draw, and with consistent enough damage to get rid of Torch Head quickly.
/// </summary>
internal static class QueenPatch
{
    // Increases Binding power by 1
    [HarmonyPatch]
    internal static class QueenPatch_Queen_PuppetStringsMove
    {
        [HarmonyPatch(typeof(Queen), "PuppetStringsMove")]
        internal static bool Prefix(Queen __instance, ref Task __result)
        {
            if (!Act3BuffConfig.QueenEnabled) { return true; }

            __result = Task.CompletedTask;
            return false;
        }
        [HarmonyPatch(typeof(Queen), "PuppetStringsMove")]
        internal static async void Postfix(Queen __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.QueenEnabled) { return; }

            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/queen/queen_cast");
            await CreatureCmd.TriggerAnim(__instance.Creature, "Cast", 0.5f);
            await PowerCmd.Apply<ChainsOfBindingPower>(targets, (int)Act3BuffConfig.QueenCardsAfflicted, __instance.Creature, null);
        }
    }

    // Add burns to beam attack, and increase frequency of beam attack
    [HarmonyPatch]
    internal static class QueenPatch_TorchHeadAmalgam_SoulBeamMove
    {
        [HarmonyPatch(typeof(TorchHeadAmalgam), "SoulBeamMove")]
        internal static async void Postfix(Doormaker __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.QueenEnabled) { return; }
            if (!Act3BuffConfig.QueenBurnsAdd) { return; }

            await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Draw, (int)Act3BuffConfig.QueenBurnsAddCount, false);
        }
    }
    [HarmonyPatch]
    internal static class QueenPatch_TorchHeadAmalgam_GenerateMoveStateMachine
    {
        [HarmonyPatch(typeof(TorchHeadAmalgam), "GenerateMoveStateMachine")]
        internal static bool Prefix(TorchHeadAmalgam __instance, ref MonsterMoveStateMachine __result)
        {
            if (!Act3BuffConfig.QueenEnabled) { return true; }

            List<MonsterState> list = new List<MonsterState>();
            MoveState moveState = new MoveState("TACKLE_1_MOVE", __instance.TackleMove, new SingleAttackIntent(__instance.TackleDamage));
            MoveState moveState2 = new MoveState("TACKLE_2_MOVE", __instance.TackleMove, new SingleAttackIntent(__instance.TackleDamage));
            MoveState moveState3 = Act3BuffConfig.QueenBurnsAdd ? new MoveState("BEAM_MOVE", __instance.SoulBeamMove, new MultiAttackIntent(__instance.SoulBeamDamage, 3), new StatusIntent((int)Act3BuffConfig.QueenBurnsAddCount)) : new MoveState("BEAM_MOVE", __instance.SoulBeamMove, new MultiAttackIntent(__instance.SoulBeamDamage, 3));
            MoveState moveState4 = new MoveState("TACKLE_3_MOVE", __instance.WeakTackleMove, new SingleAttackIntent(__instance.WeakTackleDamage));
            MoveState moveState5 = new MoveState("TACKLE_4_MOVE", __instance.WeakTackleMove, new SingleAttackIntent(__instance.WeakTackleDamage));
            moveState.FollowUpState = moveState2;
            moveState2.FollowUpState = moveState3;
            moveState3.FollowUpState = moveState4;
            moveState4.FollowUpState = Act3BuffConfig.QueenMoreBeams? moveState3 : moveState5;
            moveState5.FollowUpState = moveState3;
            list.Add(moveState);
            list.Add(moveState2);
            list.Add(moveState3);
            list.Add(moveState4);
            list.Add(moveState5);
            __result = new MonsterMoveStateMachine(list, moveState);
            return false;
        }
    }
}