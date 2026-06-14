using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Defect;

[Pool(typeof(DefectCardPool))]
public class VolatileReactor() : CustomCardModel(0, CardType.Skill,
    CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var added = IsUpgraded ? 1 : 0;
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c != base.Owner.Creature select c;
        foreach (Creature item in enumerable)
        {
            // Per ally code here
            await PlayerCmd.GainEnergy(base.ResolveEnergyXValue() + added, item.Player);
        }
        NFireBurningVfx child = NFireBurningVfx.Create(base.Owner.Creature, 1f, goingRight: false);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        
        CardModel card = base.CombatState.CreateCard<Burn>(base.Owner);
        IEnumerable<CardModel> list = CardFactory.GetForCombat(base.Owner, [card], base.ResolveEnergyXValue() + added, base.Owner.RunState.Rng.CombatCardGeneration);
        IEnumerable<CardModel> list2 = CardFactory.GetForCombat(base.Owner, [card], base.ResolveEnergyXValue() + added, base.Owner.RunState.Rng.CombatCardGeneration);

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true));
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list2, PileType.Draw, addedByPlayer: true));

        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {

    }
}