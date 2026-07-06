using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(DefectCardPool))]
public class Symbiosis() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    {
        CardKeyword.Exhaust
    };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[2]
    {
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<BioOrb>()
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        if (DynamicTargetType._dynamicTargetType.Get(this) == TargetType.AnyAlly)
            await OrbCmd.Channel<BioOrb>(choiceContext, play.Target.Player);
        else
            await OrbCmd.Channel<BioOrb>(choiceContext, base.Owner);
    }
     
    protected override void OnUpgrade()
    {
        DynamicTargetType._dynamicTargetType.Set(this, TargetType.AnyAlly);
    }
}