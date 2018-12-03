using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DropPlace : GLMonoBehaviour//, IDropHandler		// IDropHandler showed very bad results
{
	public DragableObject	m_Visitor;
	public PlaceType		m_PlaceType;
	[HideInInspector]
	public Rect			m_Rect;
	public float		m_DroprectScale = 1.0f;

	public Action<DragableObject>		m_OnEnter;
	public Action<DragableObject>		m_OnLeave;
	public Func<DragableObject, bool>	m_ValidDropped;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		m_Rect = GetComponent<Image>().rectTransform.rect;
		m_Rect.size *= m_DroprectScale;
		m_Rect.position += transform.position.To2DXY();

		Register();

		if(m_Visitor != null)
			Enter(m_Visitor);
	}

	private void OnDestroy()
	{
		Release();
	}

	//////////////////////////////////////////////////////////////////////////
	public void Register()
	{
		Core.Instance.m_DropPlaces.Add(this);
	}

	public void Release()
	{
		Core.Instance.m_DropPlaces.Remove(this);
	}

	public bool	IsValid(DragableObject dropped)
	{
		if(m_ValidDropped != null)
			return m_ValidDropped(dropped);

		return false;
	}

	public void Enter(DragableObject dropped)
	{
		dropped.m_LastDropPlace?.Leave(dropped);

		m_Visitor = dropped;
		m_OnEnter?.Invoke(dropped);
		dropped.m_LastDropPlace = this;
		//Debug.Log("Enter");
	}

	public void Leave(DragableObject dropped)
	{
		if(m_Visitor == dropped)
			m_Visitor = null;

		m_OnLeave?.Invoke(dropped);

		//Debug.Log("Leave");
	}

	/*
	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("drop");
	}*/
}

public enum PlaceType
{
	Door,
	Sacrifice,
	UnitPlace
}