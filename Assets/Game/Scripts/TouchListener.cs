using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Touch listener: Listens for touch/tap events and notifies listeners.
/// - Carl Joven
/// </summary>
public class TouchListener : MonoBehaviour
{
	private static TouchListener instance;
	public static TouchListener GetInstance ()
	{
		return instance;
	}


	[SerializeField]
	private Camera cam;


	private List<GameObject> listeners = new List<GameObject> ();


	// First function call for the object
	void Awake ()
	{
		instance = this;
	}

	void OnDestroy ()
	{
		instance = null;
		if (listeners != null)
		{
			listeners.Clear ();
			listeners = null;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown (0))
		{
			OnTouch ();
		}
	}

	private void OnTouch ()
	{
		if (cam == null)
			return;
		
		Vector3 mPos = Input.mousePosition;
		Vector3 wPos = cam.ScreenToWorldPoint (mPos);
		BroadcastOnTouch (wPos);
		                    
	}

	#region Register Listeners

	public static void AddTouchListener (GameObject go)
	{
		if (instance == null)
			return;

		if(!instance.listeners.Contains(go))
			instance.listeners.Add (go);
	}

	public static void RemoveTouchListener (GameObject go)
	{
		if (instance == null)
			return;

		instance.listeners.Remove (go);
	}

	public static void BroadcastOnTouch (Vector3 pos)
	{
		if (instance == null)
			return;

		if (instance.listeners == null || instance.listeners.Count <= 0)
			return;

		string msg = "OnTouch";
		for (int i = 0; i < instance.listeners.Count; i++)
		{
			GameObject go = instance.listeners [i];
			if (go == null)
				continue;

			go.SendMessage (msg, pos, SendMessageOptions.DontRequireReceiver);
		}
	}

	#endregion
}
