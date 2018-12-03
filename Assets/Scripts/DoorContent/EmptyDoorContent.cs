using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EmptyDoorContent : DoorContentExecution 
{

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	public override void Connected(DoorContent doorContent)
	{
	}

	override public void DoorOpen()
	{
		Debug.Log("Door is open");
	}

	override public void ActivateContent(Human human)
	{
		human.m_StateMachine.CurrentState = HumanState.Wait;
		Debug.Log("EmptyEnter");
	}

	override public void DeactivateContent(Human human)
	{
		Debug.Log("EmptyLeave");
	}

	//	override public void Click(Human human)
	//	{
	//	}
}
