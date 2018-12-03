using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoorContent : DoorContentExecution 
{
	public Sprite		m_TrapImage;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	public override void Connected(DoorContent doorContent)
	{
		doorContent.m_ContentImage = m_TrapImage;
		doorContent.SetImages(false, false, true, false);
	}

	override public void DoorOpen()
	{
	}

	override public void ActivateContent(Human human)
	{
		human.Destroy();
		//human.m_StateMachine.CurrentState = HumanState.Wait;
	}

	override public void DeactivateContent(Human human)
	{

	}
}
