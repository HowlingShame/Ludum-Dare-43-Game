using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenu : GLMonoBehaviour
{
	public Vector3		m_ScaleAnimation;
	public float		m_ScaleTime;

	public Text			m_Score;
	public Text			m_Time;
	public Text			m_LoseReason;
	public Text			m_Level;
	public Text			m_DoorsOpen;
	public Text			m_ClickCount;
	public Text			m_HumansDied;
	public Text			m_ItemsDestroyed;
	public Text			m_SacrificeCount;
		 

/*	public const float c_TimeScoreMultiplier;
	public const float c_LevelScoreMultiplier;
	public const float c_DoorOpenScoreMultiplier;
	public const float c_ClickOpenScoreMultiplier;*/
	public Vector3		m_AnimationMove = new Vector3(0, 10.0f, 0);
	public float		m_AnimationTime = 2.0f;
	public const float	m_RandomDelay = 3.0f;

	//////////////////////////////////////////////////////////////////////////
	void Start() 
	{
		iTween.ScaleAdd(transform.Find("PresAnyKey").gameObject, iTween.Hash("amount", m_ScaleAnimation, "easetype", "easeInOutSine", "looptype", "pingPong"));

		iTween.MoveBy(m_Time.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_Level.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_DoorsOpen.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_ClickCount.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_HumansDied.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_ItemsDestroyed.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_SacrificeCount.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_LoseReason.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		iTween.MoveBy(m_Score.gameObject, iTween.Hash("amount", m_AnimationMove, "time", m_AnimationTime, "easetype", "easeInOutSine", "looptype", "pingPong", "delay", UnityEngine.Random.Range(0, m_RandomDelay)));
		
		//m_Score.text += ": " + Core.Instance.m_PlayerInfo.;
		m_Time.text += ": " + ((int)Core.Instance.m_PlayerInfo.m_PlayTime) + " sec";

		m_Level.text +=  ":" + Core.Instance.m_NextLevelData.m_Depth;
		m_DoorsOpen.text += ":" + Core.Instance.m_PlayerInfo.m_DoorsOpen;
		m_ClickCount.text += ":" + Core.Instance.m_PlayerInfo.m_ClickCount;
	
		m_HumansDied.text += ":" + Core.Instance.m_PlayerInfo.m_HumansDied;

		m_ItemsDestroyed.text += ": " + Core.Instance.m_PlayerInfo.m_ItemsDestroyed;
		m_SacrificeCount.text += ": " + Core.Instance.m_PlayerInfo.m_SacrificeCount;
		m_LoseReason.text += ": " + Core.g_LoseResonStrings[Core.Instance.m_PlayerInfo.m_LoseReson];

//		iTween.ScaleAdd(transform.Find("PresAnyKey").gameObject, iTween.Hash("amount", m_ScaleAnimation, "easetype", "easeInOutSine", "looptype", "pingPong"));
	}
	
	void Update() 
	{
		if(Input.anyKeyDown)
			Core.Instance.StartNewGame();
	}
}
