using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour {
	private bool two = false;
	private bool three;
	public Bounds[] bounds;
	public int[] sce;
	private bool[] scen;




	// Use this for initialization
	void Awake ()
	{
		scen = new bool[sce.Length];
		for (int i = 0; i < bounds.Length; i++)
		{
			scen[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < sce.Length; i++)
		{
			if (Vector3.Distance(bounds[i].ClosestPoint(transform.position), transform.position) <= 1000)
			{
				if (!scen[i])
				{
					SceneManager.LoadSceneAsync(sce[i], LoadSceneMode.Additive);
					scen[i] = true;
				}
			}
			else if (scen[i])
			{
				SceneManager.UnloadSceneAsync(sce[i]);
				scen[i] = false;
			}
		}
	}
}
