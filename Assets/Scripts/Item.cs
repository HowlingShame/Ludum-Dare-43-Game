using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : GLMonoBehaviour, IPointerClickHandler
{
	public DragableObject				m_DragableObject;

	public float						m_SacrificeValue;
	public float						m_Durability;
	public ParticleSystem				m_DestroyEffect;
	private const float					c_ChickDamage = 1.0f;
	public static readonly Vector3		c_ShakeDirection = new Vector3(0.02f, 0.10f);
	[ReadOnly]
	public bool							m_Dragged = false;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		m_DragableObject = GetComponent<DragableObject>();
		m_DragableObject.m_BeginDrag = OnDrag;
		m_DragableObject.m_EndDrag = OnDragEnd;
	}
	
	public void OnDrag()
	{
		m_Dragged = true;
	}

	public void OnDragEnd()
	{
		m_Dragged = false;
	}

	public void Destroy()
	{
		if(m_DestroyEffect != null)
			Instantiate(m_DestroyEffect).transform.position = transform.position;

		Core.Instance.m_PlayerInfo.m_ItemsDestroyed ++;

		Core.Instance.m_SoundManager.PlayEvent(AudioEvent.UnitDied);
		Destroy(gameObject);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(m_Dragged)
			return;

		if(m_Durability > 0.0f)
		{
			m_Durability -= c_ChickDamage;
			
			Core.Instance.m_PlayerInfo.m_ClickCount ++;
			Core.Instance.m_SoundManager.PlayEvent(AudioEvent.Punch);

			if(m_Durability <= 0.0f)
			{
				Destroy();
			}
			else	
				iTween.ShakePosition(gameObject, iTween.Hash("amount", c_ShakeDirection, "time", 0.1f));
		}
	}
}
