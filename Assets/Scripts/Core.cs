using Gamelogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : GLMonoBehaviour 
{
	public static Core		Instance;
	public Camera			m_MainCamera;

	public Vector3			m_MouseWorldPosition;

	public List<DropPlace>	m_DropPlaces = new List<DropPlace>();

	public string			m_StartGameScene;
	public string			m_LoseGameScene;

	public DoorGenerator	m_DoorGenerator;

	public List<Human>		m_CurrentLevelHumans = new List<Human>();

	[NonSerialized]
	public GameObject		m_Doors;
	[NonSerialized]
	public GameObject		m_Canvas;
	[NonSerialized]
	public HumansRoot		m_HumansRoot;

	[Serializable]
	public class NextLevelData
	{
		//public List<Thing>		m_Things = new List<Thing>();
		public List<Human>		m_Humans = new List<Human>();
		public int				m_Depth = 0;
	}
	public NextLevelData	m_NextLevelData = new NextLevelData();

	public SpeechBalloon		m_SpeechBalloonPrefab;

	public GameObject			m_SwapPlane;
	public GameObject			m_UI;
	public ProgressBar			m_ProgressBar;

	public float				m_TimeLosePersecond;
	
	public Toggle				m_MusicToggle;

	public SoundManager			m_SoundManager;

	public PlayerInfo			m_PlayerInfo = new PlayerInfo();
	public const int			c_HumanPlaceCount = 4;
	public const float			c_SacrificaValueMultiplyer = 10.0f;

	public bool					m_IsPlaing;

	public static Dictionary<LoseReson, string>		g_LoseResonStrings = new Dictionary<LoseReson, string>(){ 
		{ LoseReson.BreakTheBox, "you're break pandora box don't do this next time)." }, 
		{ LoseReson.FireIsOver, "you're didn't keep the fire, to fill up the fire throw everything you see there" },
		{ LoseReson.NoHumansLeft, "all you humans died's (" },
		{ LoseReson.OverclickedSucrificeBowl, "don't click on fire so much, next time" },
		{ LoseReson.Suicide, "you killed all your humans, don't click on it to much, they are to fragile for this world" }
		};

	public class PlayerInfo
	{
		public LoseReson			m_LoseReson = LoseReson.OverclickedSucrificeBowl;
		public float				m_PlayTime;
		public int					m_Score;
		public int					m_ClickCount;
		public int					m_DoorsOpen;
		public int					m_HumansDied;
		public int					m_ItemsDestroyed;
		public int					m_SacrificeCount;
	}

	//////////////////////////////////////////////////////////////////////////
	void Update () 
	{
		m_MouseWorldPosition = m_MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MainCamera.nearClipPlane));
		
		// primal bug fixes
		if(m_Doors == null)
			m_Doors = GameObject.Find("Doors");
		if(m_Canvas == null)
			m_Canvas = GameObject.Find("Canvas");
		if(m_HumansRoot == null)
			m_HumansRoot = m_Canvas?.GetComponentInChildren<HumansRoot>();

		if(m_IsPlaing)
		{
			m_ProgressBar.Apply(-m_TimeLosePersecond * Time.deltaTime);

			m_PlayerInfo.m_PlayTime += Time.deltaTime;

			if(m_ProgressBar.m_ProgressValue <= 0.0f)
			{
				GameOver();
			}
		}
	}

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
//#if UNITY_EDITOR
//		if(SceneManager.GetSceneByName(m_StartGameScene) != null)
//		{	// for debug purposes
//			m_UI.gameObject.SetActive(false);
//			m_Doors = GameObject.Find("Doors");
//		}
//#endif

		m_ProgressBar.gameObject.SetActive(false);
		DontDestroyOnLoad(gameObject);
	}

	//////////////////////////////////////////////////////////////////////////
	public void Drop(DragableObject dropped)
	{
		Rect worldRect = dropped.m_Rect;

		worldRect.position += dropped.transform.position.To2DXY();

		foreach(var n in m_DropPlaces)
		{
			if(n.m_Rect.Overlaps(worldRect) && n.IsValid(dropped))
			{
				n.Enter(dropped);
				return;
			}
		}

		// if dropped place are empty or unvalid
		dropped.RestorePosition();
	}

	public bool PushToNextLevel(Human human)
	{
		try
		{
			if(m_NextLevelData.m_Humans.Count < c_HumanPlaceCount)
			{
				human.LeaveScene();

				m_NextLevelData.m_Humans.Add(human);

				if(m_CurrentLevelHumans.Count == 0)
				{
					PlayLevelTransiton();
				}
				return true;
			}
			else
			{
				human.Say("No more slots");
				//human.Destroy();
				return false;
			}
		}
		catch (System.Exception ex)	{ Debug.Log(ex); return false;}

	}

	public void PlayLevelTransiton()
	{
		m_SwapPlane.SetActive(true);
		m_SwapPlane.transform.position = new Vector3(0, 460);
		iTween.MoveTo(m_SwapPlane.gameObject, iTween.Hash("position", new Vector3(0, 0), "easeType", "easeInOutSine", "oncomplete", "GoToNextLevel" , "oncompletetarget", gameObject));
	}

	public void GoToNextLevel()
	{
		iTween.MoveTo(m_SwapPlane.gameObject, iTween.Hash("position", new Vector3(0, -460), "easeType", "easeOutSine"));

		foreach(var n in m_CurrentLevelHumans)
			GameObject.Destroy(n);

		m_CurrentLevelHumans.Clear();
		
		implCreateNextLevel();
	}

	private void implCreateNextLevel()
	{
		if(m_NextLevelData.m_Humans.Count != 0)		// run next level
		{
			foreach(var n in m_Doors.transform.GetChildren())		// clear
				n.DestroyChildren();
			
			foreach(var n in m_CurrentLevelHumans)
				n.Destroy();
			m_CurrentLevelHumans.Clear();

			var items = GameObject.FindObjectsOfType<Item>();
			foreach(var n in items)
				n.Destroy();

			//////////////////////////////////////////////////////////////////////////
			var doors = m_DoorGenerator.GenerateDoors(m_NextLevelData);

			for(int n = 0; n < DoorGenerator.c_DoorsCount; ++ n)	// set new doors
				doors[n].transform.SetParent(m_Doors.transform.GetChild(n), false);



			m_CurrentLevelHumans.AddRange(m_NextLevelData.m_Humans);	// fill human array
			for(int n = 0; n < m_CurrentLevelHumans.Count; ++ n)
			{
				var currentHuman = m_CurrentLevelHumans[n];

				Core.StartDo(() => currentHuman.Restore());		// restore state

				var humanPlace = m_HumansRoot.m_HumanPlaces[n];
				currentHuman.m_DragableObject.m_LastDropPlace = humanPlace.m_DropPlace;
				m_HumansRoot.m_HumanPlaces[n].m_CurrentVisitor = currentHuman;	// set to place
			}

			m_NextLevelData = new NextLevelData(){ m_Depth = m_NextLevelData.m_Depth + 1 };
			m_ProgressBar.GetComponentInChildren<Text>().text = "Level " + m_NextLevelData.m_Depth;
			
		}
		else
		{
			m_PlayerInfo.m_LoseReson = LoseReson.NoHumansLeft;
			GameOver();
		}
	}

	public void GameOver()
	{
		if(m_IsPlaing == true)
		{
			Core.Instance.m_SoundManager.m_Music.Stop();
			m_IsPlaing = false;
			m_ProgressBar.gameObject.SetActive(false);
			SceneManager.UnloadSceneAsync(m_StartGameScene);
			SceneManager.LoadScene(m_LoseGameScene, LoadSceneMode.Single);
		}
	}
	
	public void SacrificeValue(float sacrificeValue)
	{
		m_ProgressBar.Apply(sacrificeValue * c_SacrificaValueMultiplyer);
	}

	public void MusicChange(Toggle value)
	{
		if(value.isOn)
			m_SoundManager.m_Music.Play();
		else
			m_SoundManager.m_Music.Stop();
	}

	public void StartNewGame()
	{
		if(m_MusicToggle.isOn)		m_SoundManager.m_Music.Play();
		else						m_SoundManager.m_Music.Stop();

		m_IsPlaing = true;
		m_PlayerInfo = new PlayerInfo();
		Debug.Log("start new game");
		SceneManager.LoadScene(m_StartGameScene, LoadSceneMode.Single);

		m_ProgressBar.gameObject.SetActive(true);
		m_ProgressBar.Apply(1000.0f, true);
		m_Doors = GameObject.Find("Doors");
		m_NextLevelData = new NextLevelData(){ m_Depth = 0 };
		m_ProgressBar.GetComponentInChildren<Text>().text = "Level " + m_NextLevelData.m_Depth;

		//transform.Find("UI").Find("Start").gameObject.SetActive(false);
		m_UI.gameObject.SetActive(false);
	}

	public void DestroyHuman(Human human)
	{
		m_CurrentLevelHumans.Remove(human);
		if(m_CurrentLevelHumans.Count == 0)
		{
			m_PlayerInfo.m_LoseReson = LoseReson.NoHumansLeft;
			PlayLevelTransiton();
		}

	}

	//////////////////////////////////////////////////////////////////////////
	static public SpeechBalloon CreateSpeechBalloon(IWorldPosition sayer, string text, float timePerCharacter = 0.1f)
	{
		Vector3 worldOffset = new Vector3(0.0f, 1.0f, 0.0f);
		Color color = Color.white;
		return CreateSpeechBalloon(sayer, text, timePerCharacter, worldOffset, color);
	}

	static public SpeechBalloon CreateSpeechBalloon(IWorldPosition sayer, string text, float timePerCharacter, Vector3 worldOffset , Color color )
	{
		var human = (sayer as Human); 
		if(human.m_Sayng == true)
			return null;

		if(human != null)
			human.m_Sayng = true;

		var result = GameObject.Instantiate(Instance.m_SpeechBalloonPrefab, Core.Instance.m_Canvas.transform);

		result.Init();
		StartDo(() => result.PushText(sayer, text, color, timePerCharacter, worldOffset, 5.0f, null, () => 
		{
			GameObject.Destroy(result);
			if(human != null)
				human.m_Sayng = false; }));

		return result;
	}
	
	static public Coroutine StartWaitAndDo(float time, Action action)
	{
		return Instance.StartCoroutine(WaitAndDo(time, action));
	}
	static public IEnumerator WaitAndDo(float time, Action action) 
	{
		yield return new WaitForSeconds(time);
		action();
	}
	static public Coroutine StartDo(Action action)
	{
		return Instance.StartCoroutine(Do(action));
	}
	static public IEnumerator Do(Action action) 
	{
		yield return null;
		action();
	}
}


[Serializable]
public class DoorGenerator
{
	public List<Door>			m_BehaviourPrefabs;
	public List<DoorContent>	m_ViewPrefabs;
	public const int			c_DoorsCount = 5;
	public float			m_RandomSizeMix = 0.9f;
	public float			m_RandomSizeMax = 1.1f;

	public Vector2			m_RandomPosMin;
	public Vector2			m_RandomPosMax;
	public Vector2			m_DoorDurabilityRange = new Vector2(-1, 1);
	

	//////////////////////////////////////////////////////////////////////////
	private List<T> implSelect<T>(List<T> input, Func<T, bool> condition)
	{
		var result = new List<T>();
		foreach(var n in input)
			if(condition(n))
				result.Add(n);

		return result;
	}

	private Door implRandomEscapeDoor()
	{
		var escapeBehaviourCandidates = implSelect(m_BehaviourPrefabs, (s) => s.m_DoorType == DoorType.Escape);
		var candidatIndex = UnityEngine.Random.Range(0, escapeBehaviourCandidates.Count);

		return escapeBehaviourCandidates[candidatIndex];
	}
	
	private Door implRandomLootDoor()
	{
		var behaviourCandidates = implSelect(m_BehaviourPrefabs, (s) => s.m_DoorType == DoorType.Loot);
		var candidatIndex = UnityEngine.Random.Range(0, behaviourCandidates.Count);

		return behaviourCandidates[candidatIndex];
	}

	private Door implRandomHumanDoor()
	{
		var behaviourCandidates = implSelect(m_BehaviourPrefabs, (s) => s.m_DoorType == DoorType.Human);
		var candidatIndex = UnityEngine.Random.Range(0, behaviourCandidates.Count);

		return behaviourCandidates[candidatIndex];
	}

	private Door implRandomDangerDoor()
	{
		var behaviourCandidates = implSelect(m_BehaviourPrefabs, (s) => s.m_DoorType == DoorType.Danger);
		var candidatIndex = UnityEngine.Random.Range(0, behaviourCandidates.Count);

		return behaviourCandidates[candidatIndex];
	}


	private DoorContent	implRandomView()
	{
		var candidatIndex = UnityEngine.Random.Range(0, m_ViewPrefabs.Count);
		return m_ViewPrefabs[candidatIndex];
	}

	private Door implInstantiate(Door behaviour, DoorContent view, Core.NextLevelData levelData)
	{
		var result = GameObject.Instantiate(behaviour);


		var transform = result.GetComponent<Image>().rectTransform;
		// randomize size and offset
		transform.localPosition = new Vector3(UnityEngine.Random.Range(m_RandomPosMin.x, m_RandomPosMax.x), UnityEngine.Random.Range(m_RandomPosMin.y, m_RandomPosMax.y), 0.0f);
		transform.sizeDelta *= UnityEngine.Random.Range(m_RandomSizeMix, m_RandomSizeMax);
		result.m_Durability += UnityEngine.Random.Range(m_DoorDurabilityRange.x, m_DoorDurabilityRange.y) * m_ClickPower.Evaluate(levelData.m_Depth * m_DifficultyScale);

		var doorContent = GameObject.Instantiate(view, result.transform);
		doorContent.UpdateSizes();

		return result;
	}

	public AnimationCurve		m_EscapeDoorChanse = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public AnimationCurve		m_DangerDoorChanse = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public AnimationCurve		m_LootDoorChanse = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public AnimationCurve		m_HumanDoorChanse = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public AnimationCurve		m_ClickPower = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public float				m_DifficultyScale = 0.1f;

	private DoorType implVote(Core.NextLevelData levelData)
	{
		float difficultyScale = (float)levelData.m_Depth * m_DifficultyScale;
		
		float escapeVote = UnityEngine.Random.Range(0.0f, m_EscapeDoorChanse.Evaluate(difficultyScale));
		float dangerVote = UnityEngine.Random.Range(0.0f, m_DangerDoorChanse.Evaluate(difficultyScale));
		float lootVote = UnityEngine.Random.Range(0.0f, m_LootDoorChanse.Evaluate(difficultyScale));
		float humanVote = UnityEngine.Random.Range(0.0f, m_HumanDoorChanse.Evaluate(difficultyScale));
		
		float maxValue = Mathf.Max(escapeVote, dangerVote, lootVote, humanVote);

		if(maxValue == humanVote)
			return DoorType.Human;
		if(maxValue == dangerVote)
			return DoorType.Danger;
		if(maxValue == lootVote)
			return DoorType.Loot;
		if(maxValue == escapeVote)
			return DoorType.Escape;

		return DoorType.Human;
	}

	private Door implGetDoorInstance(DoorType doorType)
	{
		switch(doorType)
		{
			case DoorType.Escape:
				return implRandomEscapeDoor();
			case DoorType.Loot:
				return implRandomLootDoor();
			case DoorType.Danger:
				return implRandomDangerDoor();
			case DoorType.Human:
				return implRandomHumanDoor();
			default:
				return implRandomHumanDoor();
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public List<Door> GenerateDoors(Core.NextLevelData levelData)
	{
		var result = new List<Door>(c_DoorsCount);

		{	// atleast one escape door
			if(levelData.m_Depth % 4 == 0)
				result.Add(implInstantiate(implRandomEscapeDoor(), implRandomView(), levelData));

			result.Add(implInstantiate(implGetDoorInstance(implVote(levelData)), implRandomView(), levelData));
			
			if(levelData.m_Depth % 4 == 1)		// no time make good random
				result.Add(implInstantiate(implRandomEscapeDoor(), implRandomView(), levelData));

			result.Add(implInstantiate(implGetDoorInstance(implVote(levelData)), implRandomView(), levelData));
			
			if(levelData.m_Depth % 4 == 2)
				result.Add(implInstantiate(implRandomEscapeDoor(), implRandomView(), levelData));

			result.Add(implInstantiate(implGetDoorInstance(implVote(levelData)), implRandomView(), levelData));
			
			if(levelData.m_Depth % 4 == 3)
				result.Add(implInstantiate(implRandomEscapeDoor(), implRandomView(), levelData));

			result.Add(implInstantiate(implGetDoorInstance(implVote(levelData)), implRandomView(), levelData));
		}

		return result;
	}
}

public enum LoseReson
{
	NoHumansLeft,
	FireIsOver,
	BreakTheBox,
	OverclickedSucrificeBowl,
	Suicide
}

public class SingleCoroutine		// coroutine wrapper
{
	Coroutine			m_pCoroutine = null;
	Func<IEnumerator>	m_pFunction;
	MonoBehaviour		m_pGameObject = null;
	
	//////////////////////////////////////////////////////////////////////////
	public void Restart()
	{
		Stop();
		Start();
	}

	public void Ended()	// helper for optimization purpouse
	{
		m_pCoroutine = null;
	}

	public void Start()
	{
		if(m_pCoroutine == null)
			m_pCoroutine = m_pGameObject.StartCoroutine(m_pFunction());
	}

	public void Stop()
	{
		if(m_pCoroutine != null)
		{
			m_pGameObject.StopCoroutine(m_pCoroutine);
			m_pCoroutine = null;
		}
	}

	public void SetFunction(Action func)
	{
		Stop();
		m_pFunction = () => Core.Do(func);
	}
	public void SetFunction(Func<IEnumerator> func)
	{
		Stop();
		m_pFunction = func;
	}
	
	public SingleCoroutine(MonoBehaviour gameObject, Action func)
	{
		m_pGameObject = gameObject;
		m_pFunction = () => Core.Do(func);
	}
	public SingleCoroutine(MonoBehaviour gameObject, Func<IEnumerator> func)
	{
		m_pGameObject = gameObject;
		m_pFunction = func;
	}
}

public interface IWorldPosition
{
	Vector3 GetWorldPosition();
}

[Serializable]
public class ColorAnimationCurve
{
	public AnimationCurve	m_Red = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
	public AnimationCurve	m_Green = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
	public AnimationCurve	m_Blue = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
	public AnimationCurve	m_Alpha = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));

	public Color Evaluate(float scale)
	{
		return new Color(m_Red.Evaluate(scale), m_Green.Evaluate(scale), m_Blue.Evaluate(scale), m_Alpha.Evaluate(scale));
	}
}



//iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
//iTween.RotateBy(gameObject, iTween.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));

// copy paste code for conflicts where exists few drop targets
    //public static float DistancePointToRectangle(Vector2 point, Rect rect) {
    //        //  Calculate a distance between a point and a rectangle.
    //        //  The area around/in the rectangle is defined in terms of
    //        //  several regions:
    //        //
    //        //  O--x
    //        //  |
    //        //  y
    //        //
    //        //
    //        //        I   |    II    |  III
    //        //      ======+==========+======   --yMin
    //        //       VIII |  IX (in) |  IV
    //        //      ======+==========+======   --yMax
    //        //       VII  |    VI    |   V
    //        //
    //        //
    //        //  Note that the +y direction is down because of Unity's GUI coordinates.
    // 
    //        if (point.x < rect.xMin) { // Region I, VIII, or VII
    //            if (point.y < rect.yMin) { // I
    //                Vector2 diff = point - new Vector2(rect.xMin, rect.yMin);
    //                return diff.magnitude;
    //            }
    //            else if (point.y > rect.yMax) { // VII
    //                Vector2 diff = point - new Vector2(rect.xMin, rect.yMax);
    //                return diff.magnitude;
    //            }
    //            else { // VIII
    //                return rect.xMin - point.x;
    //            }
    //        }
    //        else if (point.x > rect.xMax) { // Region III, IV, or V
    //            if (point.y < rect.yMin) { // III
    //                Vector2 diff = point - new Vector2(rect.xMax, rect.yMin);
    //                return diff.magnitude;
    //            }
    //            else if (point.y > rect.yMax) { // V
    //                Vector2 diff = point - new Vector2(rect.xMax, rect.yMax);
    //                return diff.magnitude;
    //            }
    //            else { // IV
    //                return point.x - rect.xMax;
    //            }
    //        }
    //        else { // Region II, IX, or VI
    //            if (point.y < rect.yMin) { // II
    //                return rect.yMin - point.y;
    //            }
    //            else if (point.y > rect.yMax) { // VI
    //                return point.y - rect.yMax;
    //            }
    //            else { // IX
    //                return 0f;
    //            }
    //        }
    //    }