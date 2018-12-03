using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Human : GLMonoBehaviour, IPointerClickHandler, IWorldPosition
{
	public DragableObject				m_DragableObject;
	public StateMachine<HumanState>		m_StateMachine = new StateMachine<HumanState>();
	public float						m_BreakForce = 1.0f;
	public int							m_Halth;
	public bool							m_Sayng;
	public float						m_SacrificeValue;

	public ParticleSystem			m_HumanDiedEffect;

	static public List<string>			s_LastWords = new List<string>(){ "For what?!", "Aghhh..", "Oh no...", "This dying is boring", "A dying man can do nothing easy", "Bring me a bullet-proof vest"};

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_DragableObject = GetComponent<DragableObject>();
		m_DragableObject.m_BeginDrag = OnDrag;

		m_StateMachine.AddState(HumanState.None);
		m_StateMachine.CurrentState = HumanState.None;

		m_StateMachine.AddState(HumanState.Wait,
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

		m_StateMachine.AddState(HumanState.Drag,
			() =>				// start
			{
				//GetComponent<Image>().color = Color.red;
			},
			() =>				// update 
			{
			},
			() => 				// stop
			{
				
				Core.Instance.m_SoundManager.PlayEvent(AudioEvent.UnitMove);
				//GetComponent<Image>().color = Color.white;
			}
			);

		m_StateMachine.AddState(HumanState.BreakingDoor,
			() =>				// start
			{
				//GetComponent<Image>().color = Color.blue;
			},
			() =>				// update 
			{
			},
			() => 				// stop
			{
				//GetComponent<Image>().color = Color.white;
			}
			);

		m_StateMachine.AddState(HumanState.MissingOut,
			() =>				// start
			{
				//GetComponent<Image>().color = Color.blue;
			},
			() =>				// update 
			{
			},
			() => 				// stop
			{
				GetComponent<DragableObject>().enabled = true;

				GetComponent<Image>().enabled = true;
				GetComponent<Image>().color = Color.white;
			}
			);
	}

	private void Start()
	{
		Core.Instance.m_CurrentLevelHumans.Add(this);
	}

	private void Update()
	{
		m_StateMachine.Update();
	}

	//////////////////////////////////////////////////////////////////////////
	public void OnDrag()
	{
		m_StateMachine.CurrentState = HumanState.Drag;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(m_StateMachine.CurrentState == HumanState.BreakingDoor || m_StateMachine.CurrentState == HumanState.Wait)
		{
			Core.Instance.m_SoundManager.PlayEvent(AudioEvent.Punch);
			Core.Instance.m_PlayerInfo.m_ClickCount ++;
			m_Halth --;

			if(m_Halth <= 0)
			{
				Core.Instance.m_PlayerInfo.m_LoseReson = LoseReson.Suicide;
				Destroy();
			}
		}
	}

	public void LeaveScene()
	{
		Core.Instance.m_CurrentLevelHumans.Remove(this);
		m_DragableObject.m_LastDropPlace?.Leave(m_DragableObject);
		m_StateMachine.CurrentState = HumanState.MissingOut;
		m_DragableObject.enabled = false;
	}

	public void Restore()
	{
		transform.SetScaleXY(1.0f, 1.0f);
		StopAllCoroutines();
		iTween.Stop(gameObject);
		
		m_StateMachine.CurrentState = HumanState.None;

		transform.position = new Vector3(UnityEngine.Random.Range(-5, 0), -5, 0);
		m_DragableObject.OnEndDrag(null);
	}

	public Vector3 GetWorldPosition()
	{
		return transform.position;
	}
	
	public void Destroy()
	{
		Core.Instance.m_PlayerInfo.m_HumansDied ++;
		GameObject.Instantiate(m_HumanDiedEffect).transform.position = transform.position;
		Say(s_LastWords[UnityEngine.Random.Range(0, s_LastWords.Count)]);
		Core.Instance.DestroyHuman(this);
		Core.Instance.m_SoundManager.PlayEvent(AudioEvent.UnitDied);
		
		m_DragableObject.Disconnect();
		m_DragableObject.enabled = false;
		GetComponent<Image>().enabled = false;
		iTween.FadeTo(gameObject, 0.0f, 10.0f);
	}

	public void Say(string text)
	{
		Core.CreateSpeechBalloon(this, text);
	}
	
	[InspectorButton]
	public void SayTest()
	{
		Core.CreateSpeechBalloon(this, "Hello World!");
	}
}


public enum HumanState
{
	None,
	Sacrifice,
	Drag,
	Wait,
	BreakingDoor,
	MissingOut
}