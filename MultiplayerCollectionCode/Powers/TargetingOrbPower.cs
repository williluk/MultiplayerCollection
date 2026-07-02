using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class TargetingOrbPower : CustomPowerModel, ITemporaryPower
{
	private bool _shouldIgnoreNextInstance;

	public override PowerType Type
	{
		get
		{
			if (!IsPositive)
			{
				return PowerType.Debuff;
			}
			return PowerType.Buff;
		}
	}

	public override PowerStackType StackType => PowerStackType.Counter;

	public AbstractModel OriginModel { get; }

	public PowerModel InternallyAppliedPower => ModelDb.Power<FocusPower>();

	protected virtual bool IsPositive => true;

	private int Sign
	{
		get
		{
			if (!IsPositive)
			{
				return -1;
			}
			return 1;
		}
	}
	
	public void IgnoreNextInstance()
	{
		_shouldIgnoreNextInstance = true;
	}

	public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
	{
		if (_shouldIgnoreNextInstance)
		{
			_shouldIgnoreNextInstance = false;
		}
		else
		{
			await PowerCmd.Apply<FocusPower>(new ThrowingPlayerChoiceContext(), target, (decimal)Sign * amount, applier, cardSource, silent: true);
		}
	}

	public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
	{
		if (!(amount == (decimal)base.Amount) && power == this)
		{
			if (_shouldIgnoreNextInstance)
			{
				_shouldIgnoreNextInstance = false;
			}
			else
			{
				await PowerCmd.Apply<FocusPower>(choiceContext, base.Owner, (decimal)Sign * amount, applier, cardSource, silent: true);
			}
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(base.Owner))
		{
			Flash();
			await PowerCmd.Remove(this);
			await PowerCmd.Apply<FocusPower>(choiceContext, base.Owner, -Sign * base.Amount, base.Owner, null);
		}
	}
}