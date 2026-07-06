using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;

[Pool(typeof(SilentCardPool))]
public class Pickpocket() : CustomCardModel(0, CardType.Skill,
    CardRarity.Rare, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromKeyword(ExtraKeywords.Temporary)
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel card = PileType.Hand.GetPile(play.Target.Player).Cards.LastOrDefault();
        if (card != null)
        {
            CardModel newCard = card.CreateClone();
            AccessTools.Field(typeof(CardModel), "_owner").SetValue(newCard, base.Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(newCard);
                CardCmd.Upgrade(card);
            }
            PickpocketedCardModifier mod = (PickpocketedCardModifier)ModelDb.GetById<CardModifier>(ModelDb.GetId<PickpocketedCardModifier>());
            mod.originalOwner = play.Target.Player;
            newCard.AddModifier(mod);
            newCard.AddKeyword(ExtraKeywords.Stolen);
            await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, creator: base.Owner);
            await CardPileCmd.RemoveFromCombat(card);
        }
    }
    
}