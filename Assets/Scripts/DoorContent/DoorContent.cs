using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DoorContent : GLMonoBehaviour 
{
	public Sprite						m_DamageNone;
	public Sprite						m_DamageLowImage;
	public Sprite						m_DamageMediumImage;
	public Sprite						m_DamageHightImage;
	
	public Sprite						m_DoorImage;
	public Sprite						m_ArcImage;
	public Sprite						m_ContentImage;
	
	[NonSerialized]
	public GameObject						m_Acr;
	[NonSerialized]
	public GameObject						m_Door;
	[NonSerialized]
	public GameObject						m_Damage;
	[NonSerialized]
	public GameObject						m_Content;

	[NonSerialized]
	public DoorContentExecution				m_ContentExecution;
	
	public const float			c_DemageNone = 1.0f;
	public const float			c_DemageLow = 0.75f;
	public const float			c_DemageMedium = 0.55f;
	public const float			c_DemageHight = 0.25f;

	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
		Init();
	}

	//////////////////////////////////////////////////////////////////////////
	public void Init()
	{
		m_Door = transform.Find("Door").gameObject;
		m_Damage = transform.Find("Damage").gameObject;
		m_Content = transform.Find("Content").gameObject;
	
		m_Acr = transform.parent?.gameObject;
		if(m_Acr != null)		// prefab error fix
		{
			m_Acr.GetComponent<Door>().m_DoorContent = GetComponentInChildren<DoorContent>();
			m_ContentExecution = m_Acr.GetComponent<DoorContentExecution>();
			m_ContentExecution?.Connected(this);
		}
	}
	public void UpdateDamage(float durabiltyScal)
	{
		if(durabiltyScal <= c_DemageHight)
		{
			m_Damage.GetComponent<Image>().sprite = m_DamageHightImage;
			return;
		}
		if(durabiltyScal <= c_DemageMedium)
		{
			m_Damage.GetComponent<Image>().sprite = m_DamageMediumImage;
			return;
		}
		if(durabiltyScal <= c_DemageLow)
		{
			m_Damage.GetComponent<Image>().sprite = m_DamageLowImage;
			return;
		}
		if(durabiltyScal <= c_DemageNone)
		{
			m_Damage.GetComponent<Image>().sprite = m_DamageNone;
			return;
		}
	}

	public void DestroyDoor()
	{
		m_Damage.GetComponent<Image>().enabled = false;
		m_Door.GetComponent<Image>().enabled = false;
		
		
		iTween.ShakePosition(m_Acr.gameObject, iTween.Hash("amount", Door.c_ShakeDirection * m_Acr.GetComponent<Door>().m_DestroyShakeScal, "time", 0.2f));
	}

	public void SetImages(bool arc, bool door, bool content, bool damage)
	{
		var mainImage = m_Acr.GetComponent<Image>();
		
		if(door)
		{
			Image imageDoor = m_Door.GetComponent<Image>();
			imageDoor.sprite = m_DoorImage;
			imageDoor.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
		}
		
		if(content)
		{
			Image imageContent = m_Content.GetComponent<Image>();
			imageContent.sprite = m_ContentImage;
			imageContent.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
		}

		if(damage)
		{
			Image imageDamage = m_Damage.GetComponent<Image>();
			imageDamage.sprite = m_DamageNone;
			imageDamage.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
		}

		if(arc)
			mainImage.sprite = m_ArcImage;
	}

	public void UpdateSizes()
	{
		var mainImage = m_Acr.GetComponent<Image>();
		
		Image imageDoor = m_Door?.GetComponent<Image>();
		if(imageDoor != null)
			imageDoor.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
		
		Image imageContent = m_Content?.GetComponent<Image>();
		if(imageContent != null)
			imageContent.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;

		Image imageDamage = m_Damage?.GetComponent<Image>();
		if(imageDamage != null)
			imageDamage.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
	}

	virtual public void DoorOpen()
	{
		Core.Instance.m_PlayerInfo.m_DoorsOpen ++;
		Core.Instance.m_SoundManager.PlayEvent(AudioEvent.DoorOpen);
		m_ContentExecution.DoorOpen();
	}

	virtual public void ActivateContent(Human human)
	{
		m_ContentExecution.ActivateContent(human);
	}

	virtual public void DeactivateContent(Human human)
	{
		m_ContentExecution.DeactivateContent(human);
	}

	//////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
	private void OnValidate()
	{
		Validate();
	}

	public void Validate()
	{
		Init();
		try
		{
			SetImages(true, true, true, true);
			EditorUtility.SetDirty(m_Door);
			EditorUtility.SetDirty(m_ContentImage);
			EditorUtility.SetDirty(m_Damage);
			EditorUtility.SetDirty(m_Acr);
		}
		catch (System.Exception ex)	{ /*Debug.Log(ex);*/ }
	}
#endif
}
