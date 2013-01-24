using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour
{
	public WallData data;
	
	void Update ()
	{
		renderer.material.color = data.color;
		transform.position = data.position;
		transform.localScale = data.size;
	}
}
