using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;

[Pool(typeof(SilentCardPool))]

public class FlurryOfBlades() : CustomCardModel(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    {
        CardKeyword.Sly
    };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[2]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Sly),
        HoverTipFactory.FromCard<Shiv>()
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        /*IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        int handSize = enumerable.Count();
        await CardCmd.Discard(choiceContext, enumerable);*/
        await PowerCmd.Apply<FlurryOfBladesPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}