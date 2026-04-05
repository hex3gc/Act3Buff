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
///     Makes the first phases more threatening by having more Intangible turns and adding Burns.
///     The last phase gains a stacking Constrict effect to resemble Spire Growth and add time pressure.
/// </summary>
internal static class TestSubjectPatch
{
    // Add Intangible on phases 1 and 2
    [HarmonyPatch]
    internal static class TestSubjectPatch_TestSubject_AfterAddedToRoom
    {
        [HarmonyPatch(typeof(TestSubject), "AfterAddedToRoom")]
        internal static async void Postfix(TestSubject __instance)
        {
            if (!Act3BuffConfig.TestSubjectEnabled) { return; }
            if (!Act3BuffConfig.TestSubjectIntangiblePhases) { return; }

            await PowerCmd.Apply<NemesisPower>(__instance.Creature, 1m, __instance.Creature, null);
        }
    }
    [HarmonyPatch]
    internal static class TestSubjectPatch_TestSubject_RespawnMove
    {
        [HarmonyPatch(typeof(TestSubject), "RespawnMove")]
        internal static async void Postfix(TestSubject __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.TestSubjectEnabled) { return; }
            if (!Act3BuffConfig.TestSubjectIntangiblePhases) { return; }

            await Cmd.Wait(2f);
            await PowerCmd.Apply<NemesisPower>(__instance.Creature, 1m, __instance.Creature, null);
        }
    }

    // Add constrict debuff
    [HarmonyPatch]
    internal static class TestSubjectPatch_TestSubject_Phase3LacerateMove
    {
        [HarmonyPatch(typeof(TestSubject), "Phase3LacerateMove")]
        internal static async void Postfix(TestSubject __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.TestSubjectEnabled) { return; }
            if (!Act3BuffConfig.TestSubjectAttack1Constrict) { return; }

            await PowerCmd.Apply<ConstrictPower>(targets, AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, (int)Act3BuffConfig.TestSubjectConstrictAmountHard, (int)Act3BuffConfig.TestSubjectConstrictAmountEasy), __instance.Creature, null);
        }
    }
    [HarmonyPatch]
    internal static class TestSubjectPatch_TestSubject_BigPounceMove
    {
        [HarmonyPatch(typeof(TestSubject), "BigPounceMove")]
        internal static async void Postfix(TestSubject __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.TestSubjectEnabled) { return; }
            if (!Act3BuffConfig.TestSubjectAttack2Constrict) { return; }

            await PowerCmd.Apply<ConstrictPower>(targets, AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, (int)Act3BuffConfig.TestSubjectConstrictAmountHard, (int)Act3BuffConfig.TestSubjectConstrictAmountEasy), __instance.Creature, null);
        }
    }

    // Change moveset
    [HarmonyPatch]
    internal static class TestSubjectPatch_TestSubject_GenerateMoveStateMachine
    {
        [HarmonyPatch(typeof(TestSubject), "GenerateMoveStateMachine")]
        internal static bool Prefix(TestSubject __instance, ref MonsterMoveStateMachine __result)
        {
            if (!Act3BuffConfig.TestSubjectEnabled) { return true; }

            List<MonsterState> list = new List<MonsterState>();
            __instance.DeadState = new MoveState("RESPAWN_MOVE", __instance.RespawnMove, new HealIntent(), new BuffIntent())
            {
                MustPerformOnceBeforeTransitioning = true
            };
            MoveState moveState0 = new MoveState("BURNING_GROWL_MOVE_EARLY", __instance.BurningGrowlMove, new StatusIntent(__instance.BurningGrowlBurnCount), new BuffIntent());
            MoveState moveState = new MoveState("BITE_MOVE", __instance.BiteMove, new SingleAttackIntent(__instance.BiteDamage));
            MoveState moveState2 = new MoveState("SKULL_BASH_MOVE", __instance.SkullBashMove, new SingleAttackIntent(__instance.SkullBashDamage), new DebuffIntent());
            MoveState moveState3 = new MoveState("MULTI_CLAW_MOVE", __instance.MultiClawMove, new MultiAttackIntent(__instance.MultiClawDamage, () => __instance.MultiClawTotalCount));
            MoveState moveState4 = !Act3BuffConfig.TestSubjectAttack1Constrict ? new MoveState("PHASE3_LACERATE_MOVE", __instance.Phase3LacerateMove, new MultiAttackIntent(__instance.Phase3LacerateDamage, 3)) : new MoveState("PHASE3_LACERATE_MOVE", __instance.Phase3LacerateMove, new MultiAttackIntent(__instance.Phase3LacerateDamage, 3), new DebuffIntent());
            MoveState moveState5 = !Act3BuffConfig.TestSubjectAttack2Constrict ? new MoveState("BIG_POUNCE_MOVE", __instance.BigPounceMove, new SingleAttackIntent(__instance.BigPounceDamage)) : new MoveState("BIG_POUNCE_MOVE", __instance.BigPounceMove, new SingleAttackIntent(__instance.BigPounceDamage), new DebuffIntent());
            MoveState moveState6 = new MoveState("BURNING_GROWL_MOVE", __instance.BurningGrowlMove, new StatusIntent(__instance.BurningGrowlBurnCount), new BuffIntent());
            ConditionalBranchState conditionalBranchState = new ConditionalBranchState("REVIVE_BRANCH");
            moveState0.FollowUpState = moveState;
            moveState.FollowUpState = moveState2;
            moveState2.FollowUpState = moveState;
            moveState3.FollowUpState = moveState3;
            moveState4.FollowUpState = moveState5;
            moveState5.FollowUpState = moveState6;
            moveState6.FollowUpState = moveState4;
            __instance.DeadState.FollowUpState = conditionalBranchState;
            conditionalBranchState.AddState(moveState3, () => __instance.Respawns < 2);
            conditionalBranchState.AddState(moveState4, () => __instance.Respawns >= 2);
            list.Add(__instance.DeadState);
            if (Act3BuffConfig.TestSubjectBurningOpener) { list.Add(moveState0); }
            list.Add(moveState);
            list.Add(moveState2);
            list.Add(moveState3);
            list.Add(moveState4);
            list.Add(moveState5);
            list.Add(moveState6);
            list.Add(conditionalBranchState);
            __result = new MonsterMoveStateMachine(list, Act3BuffConfig.TestSubjectBurningOpener ? moveState0 : moveState);
            return false;
        }
    }
}