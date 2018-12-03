using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SacrificePlace : GLMonoBehaviour, IPointerClickHandler
{
	[HideInInspector]
	public DropPlace		m_DropPlace;
	public Transform		m_BowlBottom;
	//public Vector2			m_RandomPosMin;
	//public Vector2			m_RandomPosMax;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_DropPlace = GetComponent<DropPlace>();

		m_DropPlace.m_ValidDropped = Any;
		m_DropPlace.m_OnEnter = Sacrifice;
		
		//m_DropPlace.m_OnLeave = ;
	}

	//////////////////////////////////////////////////////////////////////////
	public bool Any(DragableObject obj)
	{
		return true;
	}

	public void RandomzePlace()
	{
		//transform.localPosition = new Vector3(UnityEngine.Random.Range(m_RandomPosMin.x, m_RandomPosMax.x), UnityEngine.Random.Range(m_RandomPosMin.y, m_RandomPosMax.y), 0.0f);
	}

	public void Sacrifice(DragableObject obj)
	{
		switch(obj.m_ObjectTtype)
		{
			case DragableObjectType.Unit:
			{
				Core.Instance.m_PlayerInfo.m_SacrificeCount ++;
				
				var humanSacrifice = obj.GetComponent<Human>();
				humanSacrifice.transform.SetParent(m_BowlBottom);
				Core.Instance.SacrificeValue(humanSacrifice.m_SacrificeValue);
				humanSacrifice.Destroy();

				iTween.MoveTo(obj.gameObject, iTween.Hash("position", m_BowlBottom.position, "time", 0.4f, "easeType", "easeInOutBack"));
			}
			break;
			
			case DragableObjectType.Item:
			{
				Core.Instance.m_PlayerInfo.m_SacrificeCount ++;

				var itemSacrifice = obj.GetComponent<Item>();
				itemSacrifice.transform.SetParent(m_BowlBottom);
				Core.Instance.SacrificeValue(itemSacrifice.m_SacrificeValue);
				itemSacrifice.Destroy();

				iTween.MoveTo(obj.gameObject, iTween.Hash("position", m_BowlBottom.position, "time", 0.4f, "easeType", "easeInOutBack"));
			}
			break;

			default:
				Debug.Log("unknwn type");
			return;
		}
		
		Core.Instance.m_SoundManager.PlayEvent(AudioEvent.Sacrifice);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Core.Instance.m_SoundManager.PlayEvent(AudioEvent.Punch);
		Core.Instance.m_ProgressBar.Apply(-8.0f);
		if(Core.Instance.m_ProgressBar.m_ProgressValue <= 0.0f)
			Core.Instance.m_PlayerInfo.m_LoseReson = LoseReson.OverclickedSucrificeBowl;

		iTween.ShakeRotation(transform.parent.gameObject, new Vector3(0, 0, 20.0f), 0.4f);
	}
}
