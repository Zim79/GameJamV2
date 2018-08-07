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
	List<CombineInstance>[] combine;
	private float thick;
	public Material mat;

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
		spawned.transform.LookAt(pos2);
		spawned.transform.Rotate(-90,0,0);
		Quaternion qua = spawned.transform.rotation;
		Destroy(spawned);
		twoDist = pos2 - pos1;
		combine = new List<CombineInstance>[Mathf.CeilToInt(dist / 20)];
		for (int i = 0; i < Mathf.CeilToInt(dist / 20); i++)
		{
			combine[i] = new List<CombineInstance>();
		}
		for (int i = 0; i < dist; i++)
		{
			if (i % 2 == 0)
			{
				spawned = Instantiate(wall, pos1 + (twoDist / dist) * i, qua);
			}
			else
			{
				spawned = Instantiate(wall1, pos1 + (twoDist / dist) * i, qua);
			}
			spawned.transform.Translate(0, -thick, 0, Space.Self);
			CombineInstance comb = new CombineInstance();
			comb.mesh = spawned.GetComponent<MeshFilter>().mesh;
			comb.transform = spawned.transform.localToWorldMatrix;
			combine[Mathf.CeilToInt(i / 20)].Add(comb);
			Destroy(spawned);
		}
		for (int i = 0; i < Mathf.CeilToInt(dist / 20); i++)
		{
			spawned = Instantiate(wall, Vector3.zero, Quaternion.identity);
			spawned.transform.localScale = new Vector3(1, 1, 1);
			spawned.GetComponent<MeshFilter>().mesh = new Mesh();
			spawned.GetComponent<MeshFilter>().mesh.CombineMeshes(combine[i].ToArray());
			spawned.GetComponent<Renderer>().material = mat;
			spawned.AddComponent<MeshCollider>();
		}
	}
}
