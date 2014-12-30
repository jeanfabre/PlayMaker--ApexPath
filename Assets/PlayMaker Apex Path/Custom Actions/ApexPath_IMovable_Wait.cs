// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.
/* Copyright © 2014 Apex Software. All rights reserved. */

using UnityEngine;
using Apex;
using Apex.Steering;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Apex Path")]
	[Tooltip("Sends the Wait command to the Unit.")]
	public class ApexPath_IMovable_Wait : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Unit GameObject to control.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Seconds to wait. Use zero here to stop the agent *completely* until a Resume command is sent to it.")]
		public FsmFloat seconds;

		public FsmEvent finishEvent;

		private IMovable mover;
		private float timer;
		private GameObject go;

		public override void Reset()
		{
			gameObject = null;
			seconds = 0;
		}
		
		public override void OnEnter()
		{
			go = Fsm.GetOwnerDefaultTarget(gameObject);
			mover = go.As<IMovable>();
			timer = 0f;

			DoWait();
		}

		public override void OnExit()
		{
			Finish();
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			
			if (timer >= seconds.Value)
			{
				if (seconds.Value > 0)
				{
					mover.Resume ();
				}

				Finish();

				if (finishEvent != null)
				{
					Fsm.Event(finishEvent);
				}
			}
		}

		public void DoWait()
		{

			if (go == null){
				LogError("missing GameObject");
				return;
			}
			 
			if (mover==null){
				LogError("GameObject missing IMovable Interface");
				return;
			}

			if (seconds.Value > 0){
				mover.Wait(seconds.Value);
			}

			else{
				mover.Wait(null);
			}
		}
	}
}