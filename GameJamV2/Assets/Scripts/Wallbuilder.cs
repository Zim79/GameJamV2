using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallbuilder : MonoBehaviour {

	public GameObject wall;
	public GameObject wall1;
	private GameObject spawned;
	public Vector3 pos1;
	public Vector3 pos2;
	private float rot;
	private Vector3 twoDist;
	public float dist;
	List<CombineInstance>[] combine;
	private float thick;
	public Material mat;
	public Material mat1;
	public bool build;
	public List<Vector3> wPos;
	public List<Quaternion> wRot;
	private Vector3[] av;
	private int num;
	private Vector3[] vert;
	private int[] tri;
	private int[] tri1;
	private Mesh mesh;
	private Vector2[] uv;
	private Vector3[] norm;
	private int nah;
	private int mNum;
	private List<Vector3> dam;

	void Start ()
	{
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
		mNum = -1;
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
				mNum = i;
			}
		}
		spawned = Instantiate(wall, pos1, Quaternion.Euler(-90, 0, 0));
		thick = spawned.GetComponent<Renderer>().bounds.size.z / 2;
		dist = (Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z)) / (spawned.GetComponent<Renderer>().bounds.size.z));
		spawned.transform.LookAt(pos2);
		spawned.transform.Rotate(-90,0,0);
		Quaternion qua = spawned.transform.rotation;
		if (num != -1 && qua.eulerAngles.y != wRot[num].eulerAngles.y)
		{
			spawned.transform.Translate(0, 10, 0, Space.Self);
			vert = new Vector3[5];
			Vector3 right = Vector3.Cross(Vector3.up, wRot[num] * Vector3.forward);        // right vector
			float dir = Vector3.Dot(right, spawned.transform.position);
			if (dir > 0f)
			{
				 nah = 1;
			}
			else if (dir < 0f)
			{
				 nah = -1;
			}
			else
			{
				 nah = 0;
			}
			//if (spawned.transform.InverseTransformDirection(qua.eulerAngles).y - wRot[num].eulerAngles.y > -80f && spawned.transform.InverseTransformDirection(qua.eulerAngles).y - wRot[num].eulerAngles.y < 180)
			if (nah < 0)
			{
				vert[0] = pos1 - wRot[num] * new Vector3(1.624999f, -3.437351f, 0);
				vert[1] = pos1 + wRot[num] * new Vector3(0, 3.437351f, 0);
				vert[2] = pos1 - Quaternion.Euler(0, qua.eulerAngles.y, 0) * new Vector3(1.624999f, -3.437351f, 0);
				vert[3] = vert[0] - new Vector3(0, 3.437351f * 2, 0);
				vert[4] = vert[2] - new Vector3(0, 3.437351f * 2, 0);
				tri = new int[3];
				tri[0] = 0;
				tri[1] = 2;
				tri[2] = 1;
				tri1 = new int[6];
				tri1[0] = 0;
				tri1[1] = 3;
				tri1[2] = 2;
				tri1[3] = 3;
				tri1[4] = 4;
				tri1[5] = 2;
			}
			else
			{
				vert[0] = pos1 - wRot[num] * new Vector3(-1.624999f, -3.437351f, 0);
				vert[1] = pos1 + wRot[num] * new Vector3(0, 3.437351f, 0);
				vert[2] = pos1 - Quaternion.Euler(0, qua.eulerAngles.y, 0) * new Vector3(-1.624999f, -3.437351f, 0);
				vert[3] = vert[0] - new Vector3(0, 3.437351f * 2, 0);
				vert[4] = vert[2] - new Vector3(0, 3.437351f * 2, 0);
				tri = new int[3];
				tri[0] = 0;
				tri[1] = 1;
				tri[2] = 2;
				tri1 = new int[6];
				tri1[0] = 0;
				tri1[1] = 2;
				tri1[2] = 3;
				tri1[3] = 3;
				tri1[4] = 2;
				tri1[5] = 4;
			}
			mesh = new Mesh();
			mesh.vertices = vert;
			mesh.triangles = tri;
			uv = new Vector2[5];
			norm = new Vector3[5];
			uv[0] = new Vector2(1, 1);
			uv[1] = new Vector2(0.5f, 0);
			uv[2] = new Vector2(1, 0);
			uv[3] = new Vector2(0, 1);
			uv[4] = new Vector2(0, 0);
			for (int i = 0; i < 5; i++)
			{
				//uv[i] = new Vector2(0.5f, 0.5f);
				norm[i] = new Vector3(32, 32, 32);
			}
			mesh.uv = uv;
			mesh.subMeshCount = 2;
			mesh.SetTriangles(tri, 0);
			mesh.SetTriangles(tri1, 1);
			spawned.transform.position = Vector3.zero;
			spawned.transform.eulerAngles = Vector3.zero;
			spawned.transform.localScale = new Vector3(1, 1, 1);
			spawned.GetComponent<MeshFilter>().mesh = mesh;
			Material[] mats = new Material[2];
			mats[0] = mat1;
			mats[1] = mat;
			spawned.GetComponent<Renderer>().materials = mats;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.RecalculateTangents();
		}
		else
		{
			Destroy(spawned);
		}
		
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
			wRot.Add(Quaternion.Euler(0, -qua.eulerAngles.y, 0));
			wPos.Add(Quaternion.Euler(0, qua.eulerAngles.y, 0) * (combine[i][combine[i].Count - 1].transform.MultiplyPoint3x4(Vector3.zero) + new Vector3(0, 0, thick)));
			wRot.Add(Quaternion.Euler(0, qua.eulerAngles.y, 0));
			mesh.CombineMeshes(combine[i].ToArray(), true, true);
			//mesh.ClearBlendShapes();
			spawned.GetComponent<MeshFilter>().mesh = mesh;
			spawned.GetComponent<Renderer>().material = mat;
			spawned.AddComponent<MeshCollider>();
		}
		if (mNum != -1)
		{
			float goal = Vector2.Distance(new Vector2(wPos[wPos.Count - 1].x, wPos[wPos.Count - 1].z), new Vector2(pos1.x, pos1.z)) - Vector2.Distance(new Vector2(pos2.x, pos2.z), new Vector2(pos1.x, pos1.z));
			Vector3[] vectors = spawned.GetComponent<MeshFilter>().mesh.vertices;
			dam = new List<Vector3>();
			for (int i = 0; i < vectors.Length; i++)
			{
				if (dam.Count == 0)
				{
					dam.Add(vectors[i]);
				}
				else if (vectors[i].x == dam[0].x)
				{
					dam.Add(vectors[i]);
				}
				else if (vectors[i].x > dam[0].x)
				{
					dam.RemoveRange(0, dam.Count - 1);
					dam.Add(vectors[i]);
				}
			}
			for (int i = 0; i < dam.Count; i++)
			{
				dam[i] = new Vector3(dam[i].x + goal, dam[i].y, dam[i].z);
			}
		}
		pos1 = pos2;
	}

	public void Width()
	{
		Mesh m = spawned.GetComponent<MeshFilter>().mesh;
		Vector3[] v = m.vertices;
		float n = 10;
		float s = 10;
		for (int i = 0; i < v.Length; i++)
		{
			if (v[i].y < s)
			{
				s = v[i].y;
			}
			else if (v[i].y < n)
			{
				n = v[i].y;
			}
		}
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