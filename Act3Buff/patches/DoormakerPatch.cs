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
///     Buffs Doormaker to be more of a damage race boss with additional scaling and health.
/// </summary>
internal static class DoormakerPatch
{
    // Doormaker HP change
    [HarmonyPatch]
    internal static class DoormakerPatch_Doormaker_MinInitialHp
    {
        [HarmonyPatch(typeof(Doormaker), "MinInitialHp", MethodType.Getter)]
        internal static bool Prefix(Doormaker __instance, ref int __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, (int)Act3BuffConfig.DoormakerMaxHPHard, (int)Act3BuffConfig.DoormakerMaxHPEasy);
            return false;
        }
    }

    // Change Scrutiny attack
    [HarmonyPatch]
    internal static class DoormakerPatch_Doormaker_ScrutinyMove
    {
        [HarmonyPatch(typeof(Doormaker), "ScrutinyMove")]
        internal static bool Prefix(Doormaker __instance, ref Task __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }
            if (!Act3BuffConfig.DoormakerScrutinyIsMulti) { return true; }

            __result = Task.CompletedTask;
            return false;
        }

        [HarmonyPatch(typeof(Doormaker), "ScrutinyMove")]
        internal static async void Postfix(Doormaker __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return; }
            if (!Act3BuffConfig.DoormakerScrutinyIsMulti) { return; }

            await DamageCmd.Attack(__instance.ScrutinyDamage)
                .WithHitCount(2)
                .FromMonster(__instance)
                .WithAttackerAnim("Attack", 0.15f)
                .WithHitFx("vfx/vfx_bite")
                .Execute(null);
            await __instance.SwapPhasePower<GraspPower>();
            await Cmd.Wait(0.2f);
            __instance.UpdateVisual("monsters/beta/door_maker_placeholder_4.png");
        }
    }

    // Change base damage
    [HarmonyPatch]
    internal static class DoormakerPatch_Doormaker_HungerDamage
    {
        [HarmonyPatch(typeof(Doormaker), "HungerDamage", MethodType.Getter)]
        internal static bool Prefix(Doormaker __instance, ref int __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, (int)Act3BuffConfig.DoormakerHungerDamageHard, (int)Act3BuffConfig.DoormakerHungerDamageEasy);
            return false;
        }
    }
    [HarmonyPatch]
    internal static class DoormakerPatch_Doormaker_ScrutinyDamage
    {
        [HarmonyPatch(typeof(Doormaker), "ScrutinyDamage", MethodType.Getter)]
        internal static bool Prefix(Doormaker __instance, ref int __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, (int)Act3BuffConfig.DoormakerScrutinyDamageHard, (int)Act3BuffConfig.DoormakerScrutinyDamageEasy);
            return false;
        }
    }
    [HarmonyPatch]
    internal static class DoormakerPatch_Doormaker_GraspDamage
    {
        [HarmonyPatch(typeof(Doormaker), "GraspDamage", MethodType.Getter)]
        internal static bool Prefix(Doormaker __instance, ref int __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, (int)Act3BuffConfig.DoormakerGraspDamageHard, (int)Act3BuffConfig.DoormakerGraspDamageEasy);
            return false;
        }
    }

    // Change intents
    [HarmonyPatch]
    internal static class DoormakerPatch_GenerateMoveStateMachine
    {
        [HarmonyPatch(typeof(Doormaker), "GenerateMoveStateMachine")]
        internal static bool Prefix(Doormaker __instance, ref MonsterMoveStateMachine __result)
        {
            if (!Act3BuffConfig.DoormakerEnabled) { return true; }

            List<MonsterState> list = new List<MonsterState>();
            MoveState moveState = new MoveState("DRAMATIC_OPEN_MOVE", __instance.DramaticOpenMove, new SummonIntent());
            MoveState moveState2 = new MoveState("HUNGER_MOVE", __instance.HungerMove, new MultiAttackIntent(__instance.HungerDamage, 2), new BuffIntent());
            MoveState moveState3 = new MoveState("SCRUTINY_MOVE", __instance.ScrutinyMove, Act3BuffConfig.DoormakerScrutinyIsMulti ? new MultiAttackIntent(__instance.ScrutinyDamage, 2) : new SingleAttackIntent(__instance.ScrutinyDamage), new BuffIntent());
            MoveState moveState4 = new MoveState("GRASP_MOVE", __instance.GraspMove, new SingleAttackIntent(__instance.GraspDamage), new BuffIntent(), new DebuffIntent());
            moveState.FollowUpState = moveState2;
            moveState2.FollowUpState = moveState3;
            moveState3.FollowUpState = moveState4;
            moveState4.FollowUpState = moveState2;
            list.Add(moveState);
            list.Add(moveState2);
            list.Add(moveState3);
            list.Add(moveState4);
            __result = new MonsterMoveStateMachine(list, moveState);
            return false;
        }
    }
}