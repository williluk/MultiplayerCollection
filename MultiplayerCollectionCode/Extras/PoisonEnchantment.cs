using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class PoisonEnchantment : CustomEnchantmentModel
{
    
    public override bool HasExtraCardText => true;

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }
    
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
    {
        new DynamicVar("PoisonValue", 0m)
    };

    public override void RecalculateValues()
    {
        base.DynamicVars["PoisonValue"].BaseValue = base.Card.DynamicVars.Damage.BaseValue * 0.5m;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (base.Status == EnchantmentStatus.Normal)
        {
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, base.Card.DynamicVars.Damage.BaseValue * 0.5m, base.Card.Owner.Creature, cardPlay.Card, false);
        }
    }
}