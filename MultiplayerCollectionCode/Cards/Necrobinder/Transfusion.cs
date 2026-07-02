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

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Transfusion() : CustomCardModel(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromCard<LifeForce>(),
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<NoDrawPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, this);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != null && c.IsTransformable && c is Soul).ToList();
        foreach (CardModel item in list)
        {
            CardModel cardModel = base.CombatState.CreateCard<LifeForce>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            await CardCmd.Transform(item, cardModel);
        }
    }
}