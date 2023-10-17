using UnityEngine.EventSystems;

public class FpStandaloneInputModule : StandaloneInputModule
{
	public PointerEventData CurrentData => (PointerEventData)(((PointerInputModule)this).m_PointerData.ContainsKey(-1) ? ((object)((PointerInputModule)this).m_PointerData[-1]) : ((object)new PointerEventData(EventSystem.current)));
}
