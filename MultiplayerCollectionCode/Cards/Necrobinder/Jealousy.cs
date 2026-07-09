using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Jealousy() : CustomCardModel(1, CardType.Skill,
    CardRarity.Common, TargetType.AnyAlly)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        CardModel cardModel = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(play.Target.Player), base.Owner, prefs, 
            filter: (CardModel c) => c.Type != CardType.Power && c.Enchantment == null)).FirstOrDefault();
        if (cardModel != null)
        {
            CardCmd.Enchant<Glam>(cardModel, 1);
            CardCmd.ApplyKeyword(cardModel, CardKeyword.Ethereal);
        }
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}