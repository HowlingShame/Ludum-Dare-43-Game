using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gamelogic.Extensions;
using UnityEngine.UI;

public class SpeechBalloon : GLMonoBehaviour 
{
	public float		m_CharacterDelay;
	public Action		m_FinishAction;
	public Action		m_EndedAction;
	public string		m_Text;
	public Text			m_SpeechBaloonText;
	public	int			m_CurrentCharacter;
	public const string			c_SpeechBaloonTextElementName = "SpeechBaloonText";
	public const float			c_DefaultPerCharacterFadeTime = 0.2f;
	private SingleCoroutine		m_TextOutputCoroutine;
	public float				m_BallonFadeTime;
	public PositionCorrector	m_PositionCorrector;
	public AudioSource			m_AudioSource;

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		Init();
	}

	private void OnDestroy()
	{
		m_TextOutputCoroutine?.Stop();
		m_EndedAction?.Invoke();
	}



	//////////////////////////////////////////////////////////////////////////
	public void PushText(IWorldPosition worldPos, string text, Color color, float delay, Vector3 worldOffset, float ballonFadeTime, Action onFinish, Action onEnded)
	{
		m_CharacterDelay = delay;
		m_FinishAction = onFinish;
		m_EndedAction = onEnded;
		m_Text = text;
		m_BallonFadeTime = ballonFadeTime;

		m_PositionCorrector.m_WorldPosition = worldPos;
		m_PositionCorrector.m_WorldOffset = worldOffset;

		m_SpeechBaloonText.color = color;

		gameObject.SetActive(true);
		m_TextOutputCoroutine.Restart();
		
	}

	public void Init()
	{
		if(m_SpeechBaloonText == null)
		{
			m_SpeechBaloonText = GetComponent<Text>();
			if(m_SpeechBaloonText == null)
				m_SpeechBaloonText = transform.Find(c_SpeechBaloonTextElementName)?.GetComponent<Text>();
		}

		if(m_SpeechBaloonText == null)
			Debug.LogError("Object don't have SpeechBaloonText element");

		
		if(m_PositionCorrector == null)
		{
			m_PositionCorrector = GetComponent<PositionCorrector>();
			if(m_PositionCorrector == null)
				m_PositionCorrector = transform.GetComponentInChildren<PositionCorrector>();
		}

		if(m_PositionCorrector == null)
			Debug.LogError("Object don't have PositionCorrector element");

		m_TextOutputCoroutine = new SingleCoroutine(this, implTextOutput);
	}

	private IEnumerator implTextOutput() 
	{
		for(m_CurrentCharacter = 0; m_CurrentCharacter != m_Text.Length; ++ m_CurrentCharacter)
		{
			m_SpeechBaloonText.text = m_Text.Substring(0, m_CurrentCharacter);
			yield return new WaitForSeconds(m_CharacterDelay);
			m_AudioSource.Play();
		}

		m_FinishAction?.Invoke();

		yield return new WaitForSeconds(m_BallonFadeTime);
		m_PositionCorrector.m_WorldPosition = null;
		m_EndedAction?.Invoke();
		gameObject.SetActive(false);
	}

	//////////////////////////////////////////////////////////////////////////
	[InspectorButton]
	public void PushText()
	{
		var unit = FindObjectOfType<Human>();
		if(unit != null)
		{
			//var marker = Marker.Create(Vector3.zero, Vector3.up, unit.transform);
			PushText(unit, "Hello speech baloon World!!", Color.white, 0.1f, Vector3.up, 5.0f, 
				() =>
				{
					Debug.Log("Finish");
					
				}, null/*() => marker.Release()*/);
		}
	}
}
