using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class AidThem() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    { 
        CardKeyword.Exhaust
    };
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        List<CardModel> list = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 999999999), context: choiceContext, player: base.Owner, filter: null, source: this)).ToList();
        foreach (CardModel item in list)
        {
            CardModel cardModel = base.CombatState.CreateCard<MinionSacrifice>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            await CardCmd.Transform(item, cardModel);
        }
    }

    protected override void OnUpgrade()
    {

    }
}