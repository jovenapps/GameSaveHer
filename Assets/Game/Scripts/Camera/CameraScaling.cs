using UnityEngine;
using System.Collections;



/// <summary>
/// Camera scaling.
/// This adjusts camera size to maintain a fixed base height width among different screen sizes
/// 
///  by: Carl Joven
/// </summary>
public class CameraScaling : MonoBehaviour 
{
	private const float BASE_WIDTH	= 1366.0f;
	private const float BASE_HEIGHT = 768.0f;

	[SerializeField] private float minSize = 4f;	// For very wide screens
	[SerializeField] private float maxSize = 5f; 	// For near square screens like 4:3

	private float scale = 1f;

	void Awake () 
	{
		scale = (BASE_WIDTH / BASE_HEIGHT * (float)Screen.height / (float)Screen.width);
		Camera cam = this.GetComponent<Camera>();
		float orthoScale = cam.orthographicSize * scale;
		cam.orthographicSize = Mathf.Clamp (orthoScale, minSize, maxSize);
	}


	public float Scale 
	{
		get { return scale; }
	}


	void Start () {
	
	}


	void Update () {
	
	}
}
