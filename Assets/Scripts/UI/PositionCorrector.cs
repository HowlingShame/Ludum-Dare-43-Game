using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCorrector : MonoBehaviour 
{
	public Vector3				m_WorldOffset;
	public IWorldPosition		m_WorldPosition;

	//////////////////////////////////////////////////////////////////////////
	void Update () 
	{
		if(m_WorldPosition != null)
			transform.position = m_WorldPosition.GetWorldPosition() + m_WorldOffset;
	}
}
