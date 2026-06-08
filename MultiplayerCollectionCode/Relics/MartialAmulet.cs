using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
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
        List<CardModel> results = new List<CardModel>();
        foreach (Player p in base.Owner.RunState.Players)
        {
            GD.Print("----> Iterating on players");
            if (p != base.Owner)
            {
                IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(p).Cards.Where(Filter).ToList();
                List<CardModel> cardsToRemove = [];
                
                foreach (CardModel item in enumerable)
                {
                    //GD.Print("----> Iterating on strikes");

                    cardsToRemove.Add(item);
                    //CardCreationOptions options = new CardCreationOptions(new <>z__ReadOnlySingleElementList<CardPoolModel>(base.Owner.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.Uniform, (CardModel c) => c.Rarity == CardRarity.Rare).WithFlags(CardCreationFlags.NoUpgradeRoll);
                    results.Add(base.Owner.RunState.CreateCard(item.CanonicalInstance, base.Owner));
                }
                //GD.Print("----> executing removal");
                
                await CardPileCmd.RemoveFromDeck(cardsToRemove);
            }
        }
        //GD.Print("----> executing add");
        if (results.Count > 0)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(results, PileType.Deck));
        }
    }
    
    private bool Filter(CardModel card)
    {
        bool flag = card.IsBasicStrikeOrDefend && card.Tags.Contains(CardTag.Strike);
        bool flag2 = flag;
        // not sure what this stuff is for. Copied from All For One
        if (flag2)
        {
            CardType type = card.Type;
            bool flag3 = (uint)(type - 1) <= 2u;
            flag2 = flag3;
        }
        return flag2;
    }
}