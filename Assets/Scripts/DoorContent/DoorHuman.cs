using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHuman  : DoorContentExecution 
{
	public List<Human>		m_RandomHumanPrefabs = new List<Human>();
	public static Vector2	s_SpawnOffsetMin = new Vector2(-0.2f, -1.2f);
	public static Vector2	s_SpawnOffsetMax = new Vector2(0.2f, -0.2f);

	public static List<string>		s_RandomHello = new List<string>(){ "Hello World!", "Hi", "Hice day", "Oh", "My live is starting!!"};

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
		if(m_RandomHumanPrefabs.Count != 0)
		{
			var selectedPosition = m_RandomHumanPrefabs[UnityEngine.Random.Range(0, m_RandomHumanPrefabs.Count)];
			if(selectedPosition != null)
			{
				var human = Instantiate(selectedPosition, Core.Instance.m_Canvas.transform);
				human.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(s_SpawnOffsetMin.x, s_SpawnOffsetMax.x), UnityEngine.Random.Range(s_SpawnOffsetMin.y, s_SpawnOffsetMax.y));
				
				human.Say(s_RandomHello[UnityEngine.Random.Range(0, s_RandomHello.Count)]);
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
