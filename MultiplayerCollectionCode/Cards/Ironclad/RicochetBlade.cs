using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.GameInfo.Objects;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class RicochetBlade() : CustomCardModel(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingRandomOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        List<RicochetBlade> list = Create(play.Target.Player, base.CombatState).ToList();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, creator: base.Owner));
    }

    public static IEnumerable<RicochetBlade> Create(Player owner, ICombatState combatState)
    {
        List<RicochetBlade> list = new List<RicochetBlade>();

        var card = combatState.CreateCard<RicochetBlade>(owner);
        card.AddKeyword(CardKeyword.Ethereal);
        card.AddKeyword(CardKeyword.Exhaust);
        list.Add(card);
        
        return list;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4);
    }
}