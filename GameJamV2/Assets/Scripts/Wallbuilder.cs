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
	public bool build = false;
	public List<Vector3> wPos;
	public List<Quaternion> wRot;
	public Vector3[] av;
	private int num;
	private Vector3[] vert;

	void Start ()
	{
		mesh = new List<Mesh>();
	}

	private void Update()
	{
		if (build)
		{
			build = false;
			Build();
			Width();
		}
	}

	public void Build ()
	{
		num = -1;
		for (int i = 0; i < wPos.Count; i++)
		{
			if (Vector3.Distance(pos1, wPos[i]) <= 2)
			{
				pos1 = wPos[i];
				num = i;
			}
			else if (Vector3.Distance(pos2, wPos[i]) <= 2)
			{
				pos2 = wPos[i];
			}
		}
		spawned = Instantiate(wall, pos1, Quaternion.Euler(-90, 0, 0));
		thick = spawned.GetComponent<Renderer>().bounds.size.z / 2;
		spawned.transform.LookAt(pos2);
		spawned.transform.Rotate(-90,0,0);
		Quaternion qua = spawned.transform.rotation;
		if (num != -1)
		{
			vert = new Vector3[5];
			vert[0] = pos1 - wRot[num] * new Vector3(1.624999f, 0, 0);
		}
		dist = (Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z)) / (spawned.GetComponent<Renderer>().bounds.size.z));
		Destroy(spawned);
		twoDist = pos2 - pos1;
		combine = new List<CombineInstance>[Mathf.CeilToInt(dist / 20)];
		av = new Vector3[Mathf.CeilToInt(dist / 20)];
		for (int i = 0; i < Mathf.CeilToInt(dist / 20); i++)
		{
			combine[i] = new List<CombineInstance>();
			av[i] = Vector3.zero;
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
			Matrix4x4 matrix = spawned.transform.localToWorldMatrix;
			spawned.transform.RotateAround(Vector3.zero, Vector3.up, -qua.eulerAngles.y);
			matrix.SetTRS(spawned.transform.position, spawned.transform.rotation, spawned.transform.localScale);
			comb.transform = matrix;
			combine[Mathf.CeilToInt(i / 20)].Add(comb);

			Destroy(spawned);
		}
		for (int i = 0; i < Mathf.CeilToInt(dist / 20); i++)
		{
			spawned = Instantiate(wall, Vector3.zero, Quaternion.Euler(0, qua.eulerAngles.y, 0));
			spawned.transform.localScale = new Vector3(1, 1, 1);
			Mesh mesh = new Mesh();
			wPos.Add(Quaternion.Euler(0, qua.eulerAngles.y, 0) * (combine[i][0].transform.MultiplyPoint3x4(Vector3.zero) - new Vector3(0, 0, thick)));
			wRot.Add(Quaternion.Euler(0, qua.eulerAngles.y, 0));
			wPos.Add(Quaternion.Euler(0, qua.eulerAngles.y, 0) * (combine[i][combine[i].Count - 1].transform.MultiplyPoint3x4(Vector3.zero) + new Vector3(0, 0, thick)));
			wRot.Add(Quaternion.Euler(0, -qua.eulerAngles.y, 0));
			mesh.CombineMeshes(combine[i].ToArray(), true, true);
			//mesh.ClearBlendShapes();
			spawned.GetComponent<MeshFilter>().mesh = mesh;
			spawned.GetComponent<Renderer>().material = mat;
			spawned.AddComponent<MeshCollider>();
		}
	}

	public void Width()
	{
		Mesh m = spawned.GetComponent<MeshFilter>().mesh;
		Vector3[] v = m.vertices;
		float n = 0;
		float s = 1000;
		for (int i = 0; i < v.Length; i++)
		{
			if (v[i].x < s && v[i].y < 0)
			{
				s = v[i].x;
			}
			else if (v[i].x > n && v[i].y < 0)
			{
				n = v[i].x;
			}
		}
		print(n);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		foreach (Vector3 wPo in wPos)
		{
			Gizmos.DrawSphere(wPo, 0.5f);
		}
		if (vert != null)
		{
			foreach (Vector3 ver in vert)
			{
				Gizmos.DrawSphere(ver, 0.2f);
			}
		}
	}
}
