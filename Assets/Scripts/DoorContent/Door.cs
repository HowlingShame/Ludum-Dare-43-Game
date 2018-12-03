using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : GLMonoBehaviour , IPointerClickHandler
{
	[NonSerialized]
	public DropPlace		m_DropPlace;
	[NonSerialized]
	public Transform		m_UnitPos;
	
	[NonSerialized]
	public Human			m_CurrentVisitor;

	public ParticleSystem	m_BreakPrefab;

	public AnimationCurve		m_UnitSetPositionTimeFromDistance = new AnimationCurve(new Keyframe(1, 0.6f), new Keyframe(10, 1.2f));

	public const float					c_ShakeTime = 0.1f;
	public static readonly Vector3		c_ShakeDirection = new Vector3(0.2f, 0.4f);
	public float						m_ClickShakeScal = 1.0f;
	public float						m_DestroyShakeScal = 1.4f;
	public const float					c_ChickDamage = 0.3f;
	
	public StateMachine<DoorState>		m_StateMachine = new StateMachine<DoorState>();

	[NonSerialized]
	public DoorContent			m_DoorContent;

	public float				m_Durability;

	public DoorType				m_DoorType;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_UnitPos = transform.Find("UnitPos").transform;
		m_DropPlace = GetComponent<DropPlace>();

		m_DropPlace.m_ValidDropped += OnlyHumanDrops;
		m_DropPlace.m_OnEnter += SetupUnit;
		
		m_DropPlace.m_OnLeave += ReleaseUnit;

		
		m_StateMachine.AddState(DoorState.Closed,
			() =>				// start
			{
			},
			() =>				// update 
			{
			},
			() => 				// stop
			{
			}
			);

		m_StateMachine.AddState(DoorState.TryToOpen,
			() =>				// start
			{
			},
			() =>				// update 
			{
				Core.Instance.m_SoundManager.PlayEvent(AudioEvent.KnokingDoor);
				if(m_CurrentVisitor != null)
				{
					m_Durability -= m_CurrentVisitor.m_BreakForce * Time.deltaTime;
					m_DoorContent.UpdateDamage(m_Durability);

					if(m_Durability <= 0.0f)
					{
						m_DoorContent.DoorOpen();
						m_DoorContent.ActivateContent(m_CurrentVisitor);
						m_StateMachine.CurrentState = DoorState.Open;
					}
				}
			},
			() => 				// stop
			{
			}
			);

		m_StateMachine.AddState(DoorState.Open,
			() =>				// start
			{
				GameObject.Instantiate(m_BreakPrefab).transform.position = transform.position;
				m_DoorContent.DestroyDoor();
			},
			() =>				// update 
			{
			},
			() => 				// stop
			{
			}
			);

		m_StateMachine.CurrentState = DoorState.Closed;
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_StateMachine.Update();
	}


	//////////////////////////////////////////////////////////////////////////
	public bool OnlyHumanDrops(DragableObject obj)
	{
		if(m_CurrentVisitor == null && obj.m_ObjectTtype == DragableObjectType.Unit)
			return true;

		return false;
	}


	public void SetupUnit(DragableObject obj)	// unit dropped
	{
		m_CurrentVisitor = obj.GetComponent<Human>();

		float time = m_UnitSetPositionTimeFromDistance.Evaluate(Vector3.Distance(obj.gameObject.transform.position, m_UnitPos.position));
		iTween.MoveTo(obj.gameObject, iTween.Hash("position", m_UnitPos.position, "time", time, "easeType", "easeInOutBack", 
			"oncomplete", "onComplete", "oncompletetarget", gameObject));
	}

	public void onComplete()		// end time animation function for SetupUnit, unit dropped
	{
		switch(m_StateMachine.CurrentState)	
		{
			case DoorState.Open:
			{
				// TODO: open door usage behaviour
				m_DoorContent.ActivateContent(m_CurrentVisitor);
			}
			break;
			case DoorState.Closed:
			{
				m_CurrentVisitor.m_StateMachine.CurrentState = HumanState.BreakingDoor;
				m_StateMachine.CurrentState = DoorState.TryToOpen;
			}
			break;
			case DoorState.TryToOpen:
			{
				Debug.LogWarning("onCompleet DoorState.TryToOpen, not gonna happen");
			}
			break;
		}
	}

	public void ReleaseUnit(DragableObject obj)	// unit taken
	{

		switch(m_StateMachine.CurrentState)	
		{
			case DoorState.TryToOpen:
				m_StateMachine.CurrentState = DoorState.Closed;
			break;
			case DoorState.Open:
				m_DoorContent.DeactivateContent(m_CurrentVisitor);
			break;
			case DoorState.Closed:
				Debug.LogWarning("ReleaseUnit DoorState.Closed, not gonna happen");
			break;
		}

		m_CurrentVisitor = null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(m_Durability > 0.0f)
		{
			m_Durability -= c_ChickDamage;
			
			Core.Instance.m_PlayerInfo.m_ClickCount ++;
			Core.Instance.m_SoundManager.PlayEvent(AudioEvent.Punch);
			m_DoorContent.UpdateDamage(m_Durability);

			if(m_Durability <= 0.0f)
			{
				m_DoorContent.DoorOpen();
				m_StateMachine.CurrentState = DoorState.Open;

				if(m_CurrentVisitor != null)
						m_DoorContent.ActivateContent(m_CurrentVisitor);
			}
			else
			{
				iTween.ShakePosition(gameObject, iTween.Hash("amount", c_ShakeDirection * m_ClickShakeScal, "time", 0.1f));
			}
		}
	}
}


[Serializable]
public enum DoorState
{
	Closed,
	Open,
	TryToOpen
}

[Serializable]
public enum DoorType
{
	Escape,
	Loot,
	Danger,
	Human
}