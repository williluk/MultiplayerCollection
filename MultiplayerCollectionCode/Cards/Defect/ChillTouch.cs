using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(DefectCardPool))]
public class ChillTouch() : CustomCardModel(1, CardType.Skill,
    CardRarity.Common, TargetType.AllAllies)
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
            await OrbCmd.Channel<FrostOrb>(choiceContext, item.Player);
            if (base.IsUpgraded)
            {
                await Cmd.CustomScaledWait(0.1f, 0.25f);
                await OrbCmd.Channel<FrostOrb>(choiceContext, item.Player);
            }
        }
    }
}