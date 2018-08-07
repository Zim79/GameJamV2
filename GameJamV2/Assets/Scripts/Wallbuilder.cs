using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallbuilder : MonoBehaviour {

	private List<Mesh> mesh;
	public GameObject wall;
	public GameObject wall1;
	private GameObject spawned;
	public Vector3 pos1;
	public Vector3 pos2;
	public float rot;
	public Vector3 twoDist;
	public float dist;
	CombineInstance[] combine;
	private float thick;

	void Start ()
	{
		mesh = new List<Mesh>();
		Build();
	}
	
	void Build ()
	{
		spawned = Instantiate(wall, pos1, Quaternion.Euler(-90, 0, 0));
		thick = spawned.GetComponent<Renderer>().bounds.size.z / 2;
		dist = (Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z)) / (spawned.GetComponent<Renderer>().bounds.size.z));
		Destroy(spawned);
		twoDist = pos2 - pos1;
		combine = new CombineInstance[Mathf.CeilToInt(dist)];
		for (int i = 0; i < dist; i++)
		{
			if (i % 2 == 0)
			{
				spawned = Instantiate(wall, pos1 + (twoDist / dist) * i, Quaternion.Euler(-90, 0, 0));
			}
			else
			{
				spawned = Instantiate(wall1, pos1 + (twoDist / dist) * i, Quaternion.Euler(-90, 0, 0));
			}
			spawned.transform.LookAt(pos2);
			spawned.transform.Rotate(-90,0,0);
			spawned.transform.Translate(0, -thick, 0, Space.Self);
			combine[i].mesh = spawned.GetComponent<MeshFilter>().mesh;
			combine[i].transform = spawned.transform.localToWorldMatrix;
			Destroy(spawned);
		}
		spawned = Instantiate(wall, pos1, Quaternion.identity);
		spawned.transform.localScale = new Vector3(1, 1, 1);
		spawned.GetComponent<MeshFilter>().mesh = new Mesh();
		spawned.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
	}
}
