using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlace : GLMonoBehaviour 
{
	[HideInInspector]
	public DropPlace		m_DropPlace;

	public Human			m_CurrentVisitor;

	public bool	IsFree{ get{ return m_CurrentVisitor == false; } }

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_DropPlace = GetComponent<DropPlace>();

		m_DropPlace.m_ValidDropped += OnlyHumanDrops;
		m_DropPlace.m_OnEnter += SetupUnit;
		
		m_DropPlace.m_OnLeave += ReleaseUnit;
	}

	void Start()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	public bool OnlyHumanDrops(DragableObject obj)
	{
		if(IsFree && obj.m_ObjectTtype == DragableObjectType.Unit)
			return true;

		return false;
	}

	public void SetupUnit(DragableObject obj)
	{		
		m_CurrentVisitor = obj.GetComponent<Human>();
		m_CurrentVisitor.m_StateMachine.CurrentState = HumanState.Wait;
		iTween.MoveTo(obj.gameObject, iTween.Hash("position", transform.position, "time", 0.4f, "easeType", "easeInOutBack"));
	}

	public void ReleaseUnit(DragableObject obj)
	{
		m_CurrentVisitor = null;
	}
}
