using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class FinalStand() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[2]
    {
        CardKeyword.Exhaust,
        CardKeyword.Retain
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[2]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsPlayer select c;
        foreach (Creature item in enumerable)
        {
            // Per ally code here
            if (!item.IsAlive)
            {
                await CreatureCmd.Heal(item, 1m);
            }
        }
        await PowerCmd.Apply<LastStandPower>(choiceContext, base.Owner.Creature, 1, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}