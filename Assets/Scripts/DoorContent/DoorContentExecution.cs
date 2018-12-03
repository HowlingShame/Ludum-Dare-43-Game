using Gamelogic.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorContentExecution : GLMonoBehaviour 
{
	abstract public void Connected(DoorContent doorContent);

	abstract public void DoorOpen();

	abstract public void ActivateContent(Human human);
	
	abstract public void DeactivateContent(Human human);
}
