using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EscapeDoorContent : DoorContentExecution
{
	public Sprite			m_EscapeContent;
	public float			m_EscapeTime = 6.0f;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	public override void Connected(DoorContent doorContent)
	{
		doorContent.m_ContentImage = m_EscapeContent;
		doorContent.SetImages(false, false, true, false);
	}

	override public void DoorOpen()
	{
		// TODO: Propagate say call
	}

	override public void ActivateContent(Human human)
	{
		if(Core.Instance.PushToNextLevel(human))
		{
			iTween.MoveTo(human.gameObject, iTween.Hash("position" , transform.Find("UnitEscapePos").transform.position, "time", m_EscapeTime, "easeType", "linear"));
			iTween.ScaleTo(human.gameObject, iTween.Hash("scale", new Vector3(0.6f, 0.6f, 1.0f), "time" , m_EscapeTime, "delay", m_EscapeTime * 0.2f, "easeType", "easeOutQuad")); 
			human.StartCoroutine(Core.WaitAndDo(m_EscapeTime, () => human.GetComponent<Image>().enabled = false));
		}
		else
			human.m_StateMachine.CurrentState = HumanState.Wait;
	}

	override public void DeactivateContent(Human human)
	{
	}
}
