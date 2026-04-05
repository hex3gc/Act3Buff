using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Act3Buff.Powers
{
    public sealed class EphemeralPower : CustomPowerModel
    {
        private class Data
        {
            public int counter = 0;
        }
        public override string? CustomPackedIconPath => "res://Act3Buff/images/powers/ephemeral.png";
        public override string? CustomBigIconPath => "res://Act3Buff/images/powers/ephemeral.png";
        public override string? CustomBigBetaIconPath => "res://Act3Buff/images/powers/ephemeral.png";
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override int DisplayAmount => GetInternalData<Data>().counter;
        protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>() { new DynamicVar("counter", 10m) };
        protected override object InitInternalData()
        {
            return new Data();
        }

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target.HasPower<EphemeralPower>() && result.UnblockedDamage > 0 && target.CombatState.CurrentSide == CombatSide.Player)
            {
                Data data = GetInternalData<Data>();
                data.counter++;
                InvokeDisplayAmountChanged();
                if (data.counter == Amount)
                {
                    await PowerCmd.Apply<IntangiblePower>(target, 1, target, null);
                }
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Enemy)
            {
                Data data = GetInternalData<Data>();
                data.counter = 0;
                InvokeDisplayAmountChanged();
            }
        }
    }
}