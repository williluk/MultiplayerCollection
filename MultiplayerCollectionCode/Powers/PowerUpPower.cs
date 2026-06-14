using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Runs;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class PowerUpPower : CustomPowerModel
{
    //Loads from MutiplayerCollection/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None;

    private bool _used = false;
        
    // CODE GOES HERE
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Power && cardPlay.Card.Owner.Creature == base.Owner)
        {
            if (base.CombatState == null)
                return;
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner) where c.IsAlive && c.IsPlayer && c != base.Owner select c;
            foreach (Creature item in enumerable)
            {
                CardModel newCard = CardFactory.GetDistinctForCombat(item.Player, [cardPlay.Card.CanonicalInstance], 1, base.Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                if (newCard != null)
                {
                    await CardCmd.AutoPlay(context, newCard, item, AutoPlayType.Default);
                }
            }
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == base.Owner)
        {
            _used = true;
        }

        return Task.CompletedTask;
    }
}