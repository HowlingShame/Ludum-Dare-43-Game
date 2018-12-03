using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ProgressBar : MonoBehaviour 
{
	public Image				m_Foreground;
	public Image				m_Background;
	public AnimationCurve		m_ProgressPosition = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));		// by scale progress bar position by scale

	public ColorAplification	m_ColorAplification;
	public ColorAnimationCurve	m_BackgroundColor = new ColorAnimationCurve();
	public ColorAnimationCurve	m_ForegroundColor = new ColorAnimationCurve();
	
	[NonSerialized]
	public Color			m_InitialForegroundColor;
	[NonSerialized]
	public Color			m_InitialBackgroundColor;
	[HideInInspector]
	public float			m_Scal;
	public float			m_Weight = 100.0f;
	public float			m_ProgressValue = 100.0f;

	public enum ColorAplification
	{
		None,
		Set,
		Multiply
	}

	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		Init();
	}

	private void OnValidate()
	{
		Init();
	}

	//////////////////////////////////////////////////////////////////////////
	public void Apply(float progress, bool clamp = true)
	{
		m_ProgressValue += progress;

		if(clamp)
			m_ProgressValue = Mathf.Clamp(m_ProgressValue, 0.0f, m_Weight);

		Apply();
	}

	public void Apply()
	{
		m_Scal = m_ProgressValue / m_Weight;
		m_Foreground.fillAmount = m_ProgressPosition.Evaluate(m_Scal);
		switch(m_ColorAplification)
		{
			case ColorAplification.None:
			break;
			case ColorAplification.Set:
				m_Foreground.color = m_ForegroundColor.Evaluate(m_Scal);
				m_Background.color = m_BackgroundColor.Evaluate(m_Scal);
			break;
			case ColorAplification.Multiply:
				m_Foreground.color = m_InitialForegroundColor * m_ForegroundColor.Evaluate(m_Scal);
				m_Background.color = m_InitialBackgroundColor * m_BackgroundColor.Evaluate(m_Scal);
			break;
		}
	}

	public void Init()
	{
		m_Foreground = transform.Find("ForeGround")?.GetComponent<Image>();
		m_Background = transform.Find("BackGround")?.GetComponent<Image>();
		
		switch(m_ColorAplification)
		{
			case ColorAplification.Multiply:
				m_InitialForegroundColor = m_Foreground.color;
				m_InitialBackgroundColor = m_Background.color;
			break;
		}
		Apply();
	}
}