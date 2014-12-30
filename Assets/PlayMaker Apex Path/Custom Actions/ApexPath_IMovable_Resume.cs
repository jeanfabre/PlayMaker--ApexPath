// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.
/* Copyright © 2014 Apex Software. All rights reserved. */

using UnityEngine;
using Apex;
using Apex.Steering;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Apex Path")]
	[Tooltip("Sends the Resume command to the Unit.")]
	public class ApexPath_IMovable_Resume : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Unit GameObject to control.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Optionally send an event. Useful in conditional or sequenced action stacks.")]
		public FsmEvent finishEvent;

		private IMovable mover;
		private GameObject go;

		public override void Reset()
		{
			gameObject = null;
		}
		
		public override void OnEnter()
		{
			go = Fsm.GetOwnerDefaultTarget(gameObject);
			mover = go.As<IMovable>();

			if (go == null){
				LogError("missing GameObject");
				return;
			}
			
			if (mover==null){
				LogError("GameObject missing IMovable Interface");
				return;
			}

			mover.Resume ();
			Finish();

			if (finishEvent != null)
			{
				Fsm.Event(finishEvent);
			}
		}
	}
}