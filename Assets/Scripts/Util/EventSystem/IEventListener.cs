using UnityEngine;

namespace Util.EventSystem
{
	public interface IEventListener
	{
		void OnEvent(EEventType eventType, Component sender, object param = null);
	}
}