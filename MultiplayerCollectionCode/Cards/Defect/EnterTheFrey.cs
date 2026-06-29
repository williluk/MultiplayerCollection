using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(DefectCardPool))]
public class EnterTheFrey() : CustomCardModel(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllAllies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
        foreach (Creature item in enumerable)
        {
            // Per ally code here
            List<Frey> list = Frey.Create(item.Player, 1, base.CombatState).ToList();
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(list.FirstOrDefault());
            }
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, creator: base.Owner));
        }
    }
    
}