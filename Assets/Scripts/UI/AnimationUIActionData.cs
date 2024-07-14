using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UI
{
	[CreateAssetMenu(fileName = "AnimationUIActionData", menuName = "Scriptable Object/AnimationUI Action Data", order = int.MaxValue)]
	public class AnimationUIActionData : ScriptableObject
	{
		[Header("UI Transform")]
		public bool doChangeActive;
		public bool doChangeTransform;
		public Vector2 showPosition;
		public Vector2 showSize;
		public Vector3 showScale;
		public Vector2 hidePosition;
		public Vector2 hideSize;
		public Vector3 hideScale;

		[Header("UI StateAction")] 
		public List<StateAction> uiStateActions;

		public UIVisualState GetUIVisualState(EuiState uiState)
		{
			foreach (var action in uiStateActions.Where(action => action.uiState == uiState))
			{
				return action.visualState;
			}
			return UIVisualState.Hide;
		}
	}
	
	[Serializable]
	public class StateAction
	{
		public EuiState uiState;
		public UIVisualState visualState;
	}

	public enum UIVisualState
	{
		Show,
		Hide
	}
}