using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HumansRoot : GLMonoBehaviour
{
	public List<HumanPlace>		m_HumanPlaces = new List<HumanPlace>(4);

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			var humanPlace = m_HumanPlaces[0];
			if(humanPlace.IsFree == false)
			{
				humanPlace.m_CurrentVisitor.gameObject.AddComponent<TakeExecution>().Execute(new Vector3(0,-0.5f,0));
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			var humanPlace = m_HumanPlaces[1];
			if(humanPlace.IsFree == false)
			{
				humanPlace.m_CurrentVisitor.gameObject.AddComponent<TakeExecution>().Execute(new Vector3(0,-0.5f,0));
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			var humanPlace = m_HumanPlaces[2];
			if(humanPlace.IsFree == false)
			{
				humanPlace.m_CurrentVisitor.gameObject.AddComponent<TakeExecution>().Execute(new Vector3(0,-0.5f,0));
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			var humanPlace = m_HumanPlaces[3];
			if(humanPlace.IsFree == false)
			{
				humanPlace.m_CurrentVisitor.gameObject.AddComponent<TakeExecution>().Execute(new Vector3(0,-0.5f,0));
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public void Init()
	{
		m_HumanPlaces.Clear();
		m_HumanPlaces.Add(transform.Find("HumanPlace (1)").GetComponent<HumanPlace>());
		m_HumanPlaces.Add(transform.Find("HumanPlace (2)").GetComponent<HumanPlace>());
		m_HumanPlaces.Add(transform.Find("HumanPlace (3)").GetComponent<HumanPlace>());
		m_HumanPlaces.Add(transform.Find("HumanPlace (4)").GetComponent<HumanPlace>());
	}
}
