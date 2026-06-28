using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class PoisonEnchantment : CustomEnchantmentModel
{
    
    public override bool HasExtraCardText => true;

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }
    

    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (base.Status == EnchantmentStatus.Normal)
        {
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, 3m, base.Card.Owner.Creature, cardPlay.Card, false);
        }
    }
}