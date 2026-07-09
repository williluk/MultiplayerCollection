using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Runs;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class FilterPower : CustomPowerModel
{
    //Loads from MutiplayerCollection/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Counter;
        
    // CODE GOES HERE
    public async override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner) where c.IsAlive && c.IsPlayer select c;
            List<CardModel> cardsToAdd = [];
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                IEnumerable<CardModel> cards = PileType.Discard.GetPile(item.Player).Cards.Where(Filter);
                int max = (cards.Count() > base.Amount) ? base.Amount : cards.Count();
                for (int i = 0; i < max - 1; i++)
                {
                    CardModel card = cards.ElementAt(i);
                    MainFile.Logger.Info($"----> i = {i}");

                    if (card != null)
                    {
                        CardModel newCard = base.CombatState.CreateCard(card.CanonicalInstance, base.Owner.Player);
                        cardsToAdd.Add(newCard);
                        await CardPileCmd.RemoveFromCombat(card);
                    }
                }
                
            }
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cardsToAdd, PileType.Discard, creator: base.Owner.Player));
        }
    }
    
    private bool Filter(CardModel card)
    {
        return card.Type == CardType.Status;
    }
}