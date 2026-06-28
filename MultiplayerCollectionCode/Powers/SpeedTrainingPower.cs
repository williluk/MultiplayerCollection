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
using MegaCrit.Sts2.Core.Localization.DynamicVars;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class SpeedTrainingPower : CustomPowerModel
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
    
    private class Data
    {
        public int cardsDrawn;

        public int triggerCount;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => base.Amount - GetInternalData<Data>().cardsDrawn % base.Amount;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] { new CardsVar(4) };

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == base.Owner && fromHandDraw == false)
        {
            Data data = GetInternalData<Data>();
            data.cardsDrawn += 1;
            int triggers = (data.cardsDrawn / base.Amount) - data.triggerCount;
            if (triggers > 0)
            {
                MainFile.Logger.Info($"MultiplayerCollection: triggers {triggers}, cardsDrawn {data.cardsDrawn}, baseAmount {base.Amount}");
                Flash();
                IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner) where c.IsAlive && c.IsPlayer && c != base.Owner select c;
                foreach (Creature item in enumerable)
                {
                    await CardPileCmd.Draw(choiceContext, 1, item.Player);
                }
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        GetInternalData<Data>().cardsDrawn = GetInternalData<Data>().cardsDrawn % base.Amount;
        return Task.CompletedTask;
    }
}