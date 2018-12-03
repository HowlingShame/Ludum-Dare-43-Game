using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour 
{
	public AudioSource		m_Music;
	public AudioSource		m_KnockingDoor;

	public AudioSource		m_DoorOpen;
	public AudioSource		m_Sacrifice;
	public AudioSource		m_Punch;
	public AudioSource		m_UnitMove;
	public AudioSource		m_UnitDied;

	//////////////////////////////////////////////////////////////////////////
	public void MuteAll()
	{
		//m_Music.Stop();

		m_KnockingDoor.Stop();

		m_DoorOpen.Stop();
		m_Sacrifice.Stop();
		m_Punch.Stop();
		m_UnitMove.Stop();
		m_UnitDied.Stop();
	}

	public void PlayEvent(AudioEvent audioEvent)
	{
		switch(audioEvent)
		{
			case AudioEvent.None:
			break;
			case AudioEvent.KnokingDoor:
				if(m_KnockingDoor.isPlaying == false)
					m_KnockingDoor.Play();
			break;
			case AudioEvent.DoorOpen:
				m_DoorOpen.Play();
			break;
			case AudioEvent.Sacrifice:
				m_Sacrifice.Play();
			break;
			case AudioEvent.Punch:
				m_Punch.Play();
			break;
			case AudioEvent.UnitMove:
				m_UnitMove.Play();
			break;
			case AudioEvent.UnitDied:
				m_UnitDied.Play();
			break;
		}
	}
}


public enum AudioEvent
{
	None,
	KnokingDoor,
	DoorOpen,
	Sacrifice,
	Punch,
	UnitMove,
	UnitDied
}