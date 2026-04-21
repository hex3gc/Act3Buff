/*
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Act3Buff.Cards;

[Pool(typeof(WatcherCardPool))]
public sealed class ScratchStatus : CardModel
{
    public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => new global::<> z__ReadOnlySingleElementList<DynamicVar>(new DamageVar(2m, ValueProp.Unpowered | ValueProp.Move));
    public override IEnumerable<CardKeyword> CanonicalKeywords => new global::<> z__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Unplayable);
    public override bool HasTurnEndInHandEffect => true;
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    public ScratchStatus() : base(1, CardType.Status, CardRarity.Status, TargetType.None) { }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner.Creature));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.Damage, this);
    }
}
*/