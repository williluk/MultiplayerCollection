using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Relics;


[Pool(typeof(IroncladRelicPool))]
public class MartialAmulet() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    public override bool HasUponPickupEffect => true;

    

    public override async Task AfterObtained()
    {
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        foreach (Player p in base.Owner.RunState.Players)
        {
            if (p != base.Owner)
            {
                IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(p).Cards.ToList();
                List<CardModel> cardsToRemove = [];
                
                foreach (CardModel item in enumerable)
                {
                    if (item.Rarity == CardRarity.Basic && item.Tags.Contains(CardTag.Strike))
                    {
                        cardsToRemove.Add(item);

                        CardModel card = base.Owner.RunState.CreateCard(item, base.Owner);
                        List<CardPileAddResult> list = results;
                        list.Add(await CardPileCmd.Add(card, PileType.Deck));
                    }
                }
                await CardPileCmd.RemoveFromDeck(cardsToRemove);
                
            }
        }
        CardCmd.PreviewCardPileAdd(results, 2f);
    }
}