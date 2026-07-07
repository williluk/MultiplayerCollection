using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(TokenCardPool))]
public class LendEnergy() : CustomCardModel(0, CardType.Skill,
    CardRarity.Token, TargetType.None)
{

    public SpiritOrb? mySpiritOrb = null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
    {
        new DynamicVar("boostValue", 10)
    };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[2]
    {
        HoverTipFactory.FromOrb<SpiritOrb>(),
        HoverTipFactory.FromKeyword(ExtraKeywords.Temporary)
    };
    
    protected override bool HasEnergyCostX => true;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    {
        ExtraKeywords.Temporary
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (mySpiritOrb == null)
            return;
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        var add = IsUpgraded ? 1 : 0;
        await mySpiritOrb.BoostDamage(base.DynamicVars["boostValue"].BaseValue * (base.ResolveEnergyXValue() + add));
    }
    
}