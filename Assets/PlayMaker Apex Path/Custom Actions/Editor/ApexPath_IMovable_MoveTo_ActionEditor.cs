using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;
using System.Collections;

using UnityEditor.AnimatedValues;

namespace HutongGames.PlayMakerEditor
{
	[CustomActionEditor(typeof (ApexPath_IMovable_MoveTo))]
	public class ApexPath_IMovable_MoveTo_ActionEditor : CustomActionEditor
	{
		AnimBool _resultFoldOutFadeAnim;

		public override bool OnGUI()
		{
			if (_resultFoldOutFadeAnim==null)
			{
				_resultFoldOutFadeAnim = new AnimBool(true);
			}

			ApexPath_IMovable_MoveTo _target = target as ApexPath_IMovable_MoveTo;

			EditField("gameObject");
			EditField("transformDestination");
			EditField("vectorDestination");
			EditField("addAsWaypoint");

			_target.result_UIFoldout = EditorGUILayout.Foldout(_target.result_UIFoldout,"Result");
			_resultFoldOutFadeAnim.target = _target.result_UIFoldout;

			if (EditorGUILayout.BeginFadeGroup(_resultFoldOutFadeAnim.faded))
			{
				EditField("navigationMessage");
				GUILayout.Space(10f);
				EditField("waypointReachedEvent");
				EditField("destinationReachedEvent");
				EditField("stoppedNoRouteExistsEvent");
				EditField("stoppedDestinationBlockedEvent");
				EditField("stoppedRequestDecayedEvent");
				EditField("stuckEvent");
				EditField("nodeReachedEvent");
				EditField("noEvent");

			}

			EditorGUILayout.EndFadeGroup();


			return GUI.changed || _resultFoldOutFadeAnim.isAnimating;
		}
		
		public override void OnSceneGUI()
		{
			// here you can show things in the scene for this action
		}
	}
}
