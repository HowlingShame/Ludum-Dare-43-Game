using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeExecution : GLMonoBehaviour 
{
	public void Execute(Vector3 offset) 
	{
		iTween.MoveTo(gameObject, iTween.Hash("position", Core.Instance.m_MouseWorldPosition + offset, "time", 0.4f, "easeType", "easeInOutQuad", "oncomplete", "TakeExecutionFinish" , "oncompletetarget", gameObject));
	}

	public void TakeExecutionFinish()
	{
		DragableObject dragableObject = GetComponent<DragableObject>();
		dragableObject.OnBeginDrag(null);
		dragableObject.OnEndDrag(null);

		Destroy(this);
	}
}
