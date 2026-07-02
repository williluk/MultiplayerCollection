using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class Invigorate() : CustomCardModel(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new HealVar(5m), new DynamicVar("HpThreshold", 50m), ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int numValidPlayers = 0;
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
        foreach (Creature item in enumerable)
        {
            if (item.CurrentHp <= item.MaxHp * (base.DynamicVars["HpThreshold"].BaseValue / 100m))
            {
                numValidPlayers++;
            }
        }
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue * numValidPlayers);
        // Apply to self separately 
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Heal.UpgradeValueBy(2m);
    }
}