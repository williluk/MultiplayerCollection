using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class RoyalArmsEnchantment : CustomEnchantmentModel
{
    
    public override bool HasExtraCardText => false;
    
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (base.Status == EnchantmentStatus.Normal)
        {
            await CreatureCmd.Damage((PlayerChoiceContext) new BlockingPlayerChoiceContext(), (IEnumerable<Creature>) base.Card.CombatState.HittableEnemies, (decimal)3, ValueProp.Unpowered, base.Card.Owner.Creature, (CardModel) null);
            IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, (Decimal) 1, base.Card.Owner);
        }
    }
}