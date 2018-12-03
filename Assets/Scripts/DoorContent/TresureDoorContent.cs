using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureDoorContent : DoorContentExecution 
{
	public List<Item>		m_RandomItemPrefabs = new List<Item>();
	public static Vector2	s_SpawnOffsetMin = new Vector2(-0.2f, -1.2f);
	public static Vector2	s_SpawnOffsetMax = new Vector2(0.2f, -0.2f);

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
		if(m_RandomItemPrefabs.Count != 0)
		{
			var selectedPosition = m_RandomItemPrefabs[UnityEngine.Random.Range(0, m_RandomItemPrefabs.Count)];
			if(selectedPosition != null)
			{
				var item = Instantiate(selectedPosition, Core.Instance.m_Canvas.transform);
				item.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(s_SpawnOffsetMin.x, s_SpawnOffsetMax.x), UnityEngine.Random.Range(s_SpawnOffsetMin.y, s_SpawnOffsetMax.y));
			}
		}
	}

	override public void ActivateContent(Human human)
	{
		human.m_StateMachine.CurrentState = HumanState.Wait;
	}

	override public void DeactivateContent(Human human)
	{

	}

}
