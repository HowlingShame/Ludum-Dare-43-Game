using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableObject : GLMonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
	public DragableObjectType			m_ObjectTtype;
	[NonSerialized]
	public Rect							m_Rect;
	[NonSerialized]
	public Vector3						m_DragOffset;
	[NonSerialized]
	public DropPlace					m_LastDropPlace;

	public Action						m_BeginDrag;

	public Action						m_EndDrag;

	static public DragableObject		s_CurrentlyDragged;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		if(m_ObjectTtype == DragableObjectType.None)
			Debug.LogWarning($"m_ObjectTtype is none, object name: {gameObject.name}");

		m_Rect = GetComponent<Image>().rectTransform.rect;
		
	}

	//////////////////////////////////////////////////////////////////////////
	public void OnBeginDrag(PointerEventData eventData)
	{
		if(s_CurrentlyDragged != null)
			if(s_CurrentlyDragged != this)
				s_CurrentlyDragged.OnEndDrag(null);

		s_CurrentlyDragged = this;

		m_DragOffset = transform.position - Core.Instance.m_MouseWorldPosition;
		transform.position = Core.Instance.m_MouseWorldPosition + m_DragOffset;

		Disconnect();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if(s_CurrentlyDragged != this)
			return;

		transform.position = Core.Instance.m_MouseWorldPosition + m_DragOffset;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Core.Instance.Drop(this);
		m_EndDrag?.Invoke();

		if(s_CurrentlyDragged != this)
			return;

		s_CurrentlyDragged = null;
	}

	public void Disconnect()
	{
		m_LastDropPlace?.Leave(this);
		m_BeginDrag?.Invoke();
	}

	//////////////////////////////////////////////////////////////////////////
	public void RestorePosition()
	{
		m_LastDropPlace?.Enter(this);
	}
}



public enum DragableObjectType
{
	None,
	Unit,
	Item
}