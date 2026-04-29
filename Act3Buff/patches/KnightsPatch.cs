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
using MegaCrit.Sts2.Core.ValueProps;

namespace Act3Buff.Patches;

/// <summary>
///     Shifts some of the knights' survivability from HP to unique buffs, giving more ways to approach dealing with them
///     Also prevents focusing the Spectral Knight to rid yourself of Ethereal, giving you more of a deck-based challenge
/// </summary>
internal static class KnightsPatch
{
    // Spectral HP change
    [HarmonyPatch]
    internal static class KnightsPatch_SpectralKnight_MinInitialHp
    {
        [HarmonyPatch(typeof(SpectralKnight), "MinInitialHp", MethodType.Getter)]
        internal static bool Prefix(SpectralKnight __instance, ref int __result)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, (int)Act3BuffConfig.KnightsGhostHPHard, (int)Act3BuffConfig.KnightsGhostHPEasy);
            return false;
        }
    }

    // Magi HP change
    [HarmonyPatch]
    internal static class KnightsPatch_MagiKnight_MinInitialHp
    {
        [HarmonyPatch(typeof(MagiKnight), "MinInitialHp", MethodType.Getter)]
        internal static bool Prefix(MagiKnight __instance, ref int __result)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, (int)Act3BuffConfig.KnightsMagiHPHard, (int)Act3BuffConfig.KnightsMagiHPEasy);
            return false;
        }
    }

    // Spectral Knight intangible
    [HarmonyPatch]
    internal static class KnightsPatch_MonsterModel_AfterAddedToRoom
    {
        [HarmonyPatch(typeof(MonsterModel), "AfterAddedToRoom")]
        internal static async void Postfix(MonsterModel __instance)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return; }
            
            if (__instance is SpectralKnight)
            {
                await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), __instance.Creature, (int)Act3BuffConfig.KnightsGhostIntangible, __instance.Creature, null);
            }
        }
    }

    // Magi Knight block
    [HarmonyPatch]
    internal static class KnightsPatch_MagiKnight_PowerShieldBlock
    {
        [HarmonyPatch(typeof(MagiKnight), "PowerShieldBlock", MethodType.Getter)]
        internal static bool Prefix(MagiKnight __instance, ref int __result)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return true; }
            if (!Act3BuffConfig.KnightsMagiBlock) { return true; }

            __result = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, (int)Act3BuffConfig.KnightsMagiBlockHard, (int)Act3BuffConfig.KnightsMagiBlockEasy);
            return false;
        }
    }
    [HarmonyPatch]
    internal static class KnightsPatch_MagiKnight_GenerateMoveStateMachine
    {
        [HarmonyPatch(typeof(MagiKnight), "GenerateMoveStateMachine")]
        internal static bool Prefix(MagiKnight __instance, ref MonsterMoveStateMachine __result)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return true; }
            if (!Act3BuffConfig.KnightsMagiBlock) { return true; }

            List<MonsterState> list = new List<MonsterState>();
            MoveState moveState = new MoveState("FIRST_POWER_SHIELD_MOVE", __instance.PowerShieldMove, new SingleAttackIntent(__instance.PowerShieldDamage), new DefendIntent());
            MoveState moveState2 = new MoveState("DAMPEN_MOVE", __instance.DampenMove, new DebuffIntent(), new DefendIntent());
            MoveState moveState3 = new MoveState("PREP_MOVE", __instance.PrepMove, new DefendIntent());
            MoveState moveState4 = new MoveState("MAGIC_BOMB", __instance.MagicBombMove, new SingleAttackIntent(__instance.BombDamage));
            MoveState moveState5 = new MoveState("RAM_MOVE", __instance.SpearMove, new SingleAttackIntent(__instance.SpearDamage));
            moveState.FollowUpState = moveState2;
            moveState2.FollowUpState = moveState5;
            moveState5.FollowUpState = moveState3;
            moveState3.FollowUpState = moveState4;
            moveState4.FollowUpState = moveState5;
            list.Add(moveState);
            list.Add(moveState2);
            list.Add(moveState5);
            list.Add(moveState3);
            list.Add(moveState4);
            __result = new MonsterMoveStateMachine(list, moveState);
            return false;
        }
    }
    [HarmonyPatch]
    internal static class KnightsPatch_MagiKnight_DampenMove
    {
        [HarmonyPatch(typeof(MagiKnight), "DampenMove")]
        internal static async void Postfix(MagiKnight __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return; }
            if (!Act3BuffConfig.KnightsMagiBlock) { return; }

            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/magi_knight/magi_knight_cast_shield");
            await CreatureCmd.TriggerAnim(__instance.Creature, "ShieldAttack", 0.6f);
            await CreatureCmd.GainBlock(__instance.Creature, __instance.PowerShieldBlock, ValueProp.Move, null);
        }
    }
    [HarmonyPatch]
    internal static class KnightsPatch_MagiKnight_PrepMove
    {
        [HarmonyPatch(typeof(MagiKnight), "PrepMove")]
        internal static bool Prefix(MagiKnight __instance, IReadOnlyList<Creature> targets, ref Task __result)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return true; }
            if (!Act3BuffConfig.KnightsMagiBlock) { return true; }

            __result = Task.CompletedTask;
            return false;
        }
        [HarmonyPatch(typeof(MagiKnight), "PrepMove")]
        internal static async void Postfix(MagiKnight __instance, IReadOnlyList<Creature> targets)
        {
            if (!Act3BuffConfig.KnightsEnabled) { return; }
            if (!Act3BuffConfig.KnightsMagiBlock) { return; }

            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/magi_knight/magi_knight_cast_shield");
            await CreatureCmd.TriggerAnim(__instance.Creature, "ShieldAttack", 0.6f);
            await CreatureCmd.GainBlock(__instance.Creature, AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, (int)Act3BuffConfig.KnightsMagiPrepHard, (int)Act3BuffConfig.KnightsMagiPrepEasy), ValueProp.Move, null);
        }
    }
}