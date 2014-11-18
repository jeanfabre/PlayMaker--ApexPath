// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.
/* Copyright © 2014 Apex Software. All rights reserved. */

using UnityEngine;
using Apex;
using Apex.Steering;
using Apex.Messages;
using Apex.Services;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Apex Path")]
	[Tooltip("Asks the unit to move to the specified position.")]
	public class ApexPath_IMovable_MoveTo : FsmStateAction , IHandleMessage<UnitNavigationEventMessage>
	{
		[RequiredField]
		[Tooltip("The Unit GameObject to control.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Move Unit To a transform position.")]
		public FsmGameObject transformDestination;
		
		[Tooltip("Position the Unit will move to. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorDestination;
		
		[Tooltip("if set to true the destination is added as a way point.")]
		public FsmBool addAsWaypoint; 

		// the line below is an example of how to insert a section,but since we have a custom action editor, we don't need it.
		//[ActionSection("Result")] // this insert a separator on the action UI for more redability.

		[Tooltip("the event of the 'Unit Navigation Event Message' system. All the event below reflect 'navigationEvent'")]
		[UIHint(UIHint.Variable)] // force the property to point to a variable since it's a "GETTER"
		[ObjectType(typeof(UnitNavigationEventMessage.Event))]
		public FsmEnum navigationMessage;

		[Tooltip("Event sent when unit reached a way point")]
		public FsmEvent waypointReachedEvent;
		[Tooltip("Event sent when unit reached destination")]
		public FsmEvent destinationReachedEvent;
		[Tooltip("Event sent when unit stopped as no route exists to its proposed destination")]
		public FsmEvent stoppedNoRouteExistsEvent;
		[Tooltip("Event sent when unit stopped as its destination is blocked")]
		public FsmEvent stoppedDestinationBlockedEvent;
		[Tooltip("Event sent when unit stopped as its path request decayed")]
		public FsmEvent stoppedRequestDecayedEvent;
		[Tooltip("Event sent when unit got stuck")]
		public FsmEvent stuckEvent;
		[Tooltip("Event sent when unit reached a node along the path")]
		public FsmEvent nodeReachedEvent;
		[Tooltip("Event sent when unit message is 'none'")]
		public FsmEvent noEvent;


		// EDITOR variable
		// if there is no custom action editor, then it will be shown, I am using this only for convenient serialization instead of hosting this value in the action editor class itself
		// if you know a way to have the action editor serializing properly so that the toggle is kept as the user wants between plays and recompilation, then I'd like to see how you do it :)
		public bool result_UIFoldout = false;

		private GameObject go;

		public override void Reset() // reset is called by the user ( from the action menu bar) or when the action is dropped on a state.
		{
			gameObject = null;
			transformDestination = null;
			vectorDestination = null;
			addAsWaypoint = null;
			destinationReachedEvent = null;

			navigationMessage = null;
			waypointReachedEvent = null;
			destinationReachedEvent = null;
			stoppedNoRouteExistsEvent = null;
			stoppedDestinationBlockedEvent = null;
			stoppedRequestDecayedEvent = null;
			stuckEvent = null;
			nodeReachedEvent = null;
			noEvent = null;
		}

		public override string ErrorCheck()
		{
			// you can perform here error check ( during editing), for example validating that gameObject has an iMovable Interface.
			return "";
		}

		
		public override void OnEnter() // performed once when the state is activated
		{
			DoGameObjectMoveTo(); // I always tend to create a specific method so that t

			if (destinationReachedEvent!=null)
			{
				GameServices.messageBus.Subscribe(this);
			}else{
				Finish(); // this is VERY important as it tells the fsm state that this action was performed, the fsm state can transit automatically if all actions have finished.
			}

		}

		public override void OnExit() // performed once when the state is activated
		{
			GameServices.messageBus.Unsubscribe(this);

		}

		public void DoGameObjectMoveTo()
		{
			go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) 
			{
				LogError("missing GameObject");
				return;
			}
			
			IMovable mover = go.As<IMovable>(); 
			if (mover==null)
			{
				LogError("GameObject missing IMovable Interface");
				return;
			}


			Vector3 _destination = vectorDestination.IsNone ? Vector3.zero : vectorDestination.Value;
			if(! transformDestination.IsNone)
			{
				if(transformDestination.Value)
				{
					_destination = transformDestination.Value.transform.position + _destination;
				}
			}

			mover.MoveTo(_destination, addAsWaypoint.Value);

		}

		void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
		{
			if  (! message.entity.Equals(go) )
			{
				return;
			}

			if (!navigationMessage.IsNone) // the user actually selected a fsm variable, so it wants this information
			{
				navigationMessage.Value = message.eventCode;
			}

		
			switch(message.eventCode)
			{
				case UnitNavigationEventMessage.Event.DestinationReached:
					Fsm.Event(destinationReachedEvent);
					break;
				case UnitNavigationEventMessage.Event.NodeReached:
					Fsm.Event(nodeReachedEvent);
					break;
				case UnitNavigationEventMessage.Event.None:
					Fsm.Event(noEvent);
					break;
				case UnitNavigationEventMessage.Event.StoppedDestinationBlocked:
					Fsm.Event(stoppedDestinationBlockedEvent);
					break;
				case UnitNavigationEventMessage.Event.StoppedNoRouteExists:
					Fsm.Event(stoppedNoRouteExistsEvent);
					break;
				case UnitNavigationEventMessage.Event.StoppedRequestDecayed:
					Fsm.Event(stoppedRequestDecayedEvent);
					break;
				case UnitNavigationEventMessage.Event.Stuck:
					Fsm.Event(stuckEvent);
					break;
				case UnitNavigationEventMessage.Event.WaypointReached:
					Fsm.Event(waypointReachedEvent);
					break;
			}

			Finish(); // very important to finish all cases, event could be sent to a different fsm.

			//UnityEngine.Debug.Log(string.Format("Unit '{0}' ({1}) reports: {2} at position: {3}.", message.entity.name, message.entity.transform.position, message.eventCode, message.destination));
		}



	}
}