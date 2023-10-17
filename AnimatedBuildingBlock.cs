using Rust;
using UnityEngine;

public class AnimatedBuildingBlock : StabilityEntity
{
	private bool animatorNeedsInitializing = true;

	private bool animatorIsOpen = true;

	private bool isAnimating = false;

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			UpdateAnimationParameters(init: true);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateAnimationParameters(init: true);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		UpdateAnimationParameters(init: false);
	}

	protected void UpdateAnimationParameters(bool init)
	{
		if (!Object.op_Implicit((Object)(object)model) || !Object.op_Implicit((Object)(object)model.animator) || !model.animator.isInitialized)
		{
			return;
		}
		bool flag = animatorNeedsInitializing || animatorIsOpen != IsOpen() || (init && isAnimating);
		bool flag2 = animatorNeedsInitializing || init;
		if (flag)
		{
			isAnimating = true;
			((Behaviour)model.animator).enabled = true;
			model.animator.SetBool("open", animatorIsOpen = IsOpen());
			if (flag2)
			{
				model.animator.fireEvents = false;
				if (((Behaviour)model.animator).isActiveAndEnabled)
				{
					model.animator.Update(0f);
					model.animator.Update(20f);
				}
				PutAnimatorToSleep();
			}
			else
			{
				model.animator.fireEvents = base.isClient;
				if (base.isServer)
				{
					SetFlag(Flags.Busy, b: true);
				}
			}
		}
		else if (flag2)
		{
			PutAnimatorToSleep();
		}
		animatorNeedsInitializing = false;
	}

	protected void OnAnimatorFinished()
	{
		if (!isAnimating)
		{
			PutAnimatorToSleep();
		}
		isAnimating = false;
	}

	private void PutAnimatorToSleep()
	{
		if (!Object.op_Implicit((Object)(object)model) || !Object.op_Implicit((Object)(object)model.animator))
		{
			Debug.LogWarning((object)(((Component)this).transform.GetRecursiveName() + " has missing model/animator"), (Object)(object)((Component)this).gameObject);
			return;
		}
		((Behaviour)model.animator).enabled = false;
		if (base.isServer)
		{
			SetFlag(Flags.Busy, b: false);
		}
		OnAnimatorDisabled();
	}

	protected virtual void OnAnimatorDisabled()
	{
	}
}
