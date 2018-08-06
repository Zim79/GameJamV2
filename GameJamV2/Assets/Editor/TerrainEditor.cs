using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LowPolyTerrain))]
public class TerrainEditor : Editor
{

	private LowPolyTerrain myscript;
	private Vector3[] vert;
	private int[] tri;
	private Vector2[] gen;
	public int resolution;
	public int siz;
	private Vector2[] uvs;
	private int vent;
	private string[] Brush;
	public int index;
	public int matIndex;
	private bool show;
	private Vector3 treeRot;
	private GameObject stree;
	private int childNum;
	private Vector3 treeSiz;
	private int nResolution;
	public bool cliff;
	private List<GameObject> spawnedTrees = new List<GameObject>();
	private Vector3[] norms;
	private Texture2D tex;
	public Color col;

	public override void OnInspectorGUI()
	{
		Brush = new string[] { "Settings", "Raise", "Lower", "Flatten", "Paint", "Trees" };
		myscript = (LowPolyTerrain)target;
		serializedObject.Update();
		if (nResolution == 0)
		{
			nResolution = Mathf.RoundToInt(Mathf.Sqrt(myscript.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3 / 2));
		}

		index = GUILayout.Toolbar(index, Brush, GUILayout.Height(25));
		if (index == 0)
		{
			if (myscript.GetComponent<MeshFilter>().sharedMesh.triangles != null)
			{
				nResolution = EditorGUILayout.IntField("Resolution:", nResolution);

			}
			if (siz == 0)
			{
				siz = Mathf.RoundToInt(myscript.GetComponent<MeshFilter>().sharedMesh.bounds.size.x);
			}
			siz = EditorGUILayout.IntField("Size:", siz);
			vert = new Vector3[0];
			tri = new int[0];

			if (GUILayout.Button("Reset Terrain"))
			{
				Reset();
			}
			if (GUILayout.Button("Smooth edges"))
			{
				Smooth();
			}
			if (GUILayout.Button(new GUIContent("Create LOD", "Creates a 16x16 version of the terrain to use for LOD")))
			{
				Lod();
			}
			if (index == 0)
			{
				myscript.show = false;
			}
			else
			{
				myscript.show = true;
			}
			EditorGUILayout.HelpBox("High resolutions (=> 128) along with a big brush size can cause performance issues, when modifying the terrain! \nYou can, if neccesary, place several terrains alongside eachother. You will then have to remove this script from the terrains you don't edit, but you can always add it again", MessageType.Warning);
			myscript.show = false;
		}
		else
		{
			myscript.show = true;
		}
		if (index != 0 && index != 4)
		{
			myscript.brushSize = EditorGUILayout.IntSlider("Brush Size:", Mathf.CeilToInt(myscript.brushSize), 1, 200);
			myscript.opacity = EditorGUILayout.IntSlider("Brush Opacity:", Mathf.CeilToInt(myscript.opacity), 1, 500);
		}

		if (index == 3)
		{
			myscript.destHeight = EditorGUILayout.FloatField("Destination height", myscript.destHeight);
		}
		if (index == 4)
		{
			myscript.brushSize = EditorGUILayout.IntSlider("Brush Size:", Mathf.CeilToInt(myscript.brushSize), 0, 500);
			col = EditorGUILayout.ColorField("Brush Color:", col);
		}
		if (index == 5)
		{
			myscript.tree = EditorGUILayout.ObjectField("Tree:",myscript.tree, typeof(GameObject), false) as GameObject;
			treeRot = EditorGUILayout.Vector3Field("Tree rotation:", new Vector3(-90, 0, 0));
			treeSiz.x = EditorGUILayout.IntSlider("Random tree x scale:", Mathf.CeilToInt(treeSiz.x), 0, 99);
			treeSiz.y = EditorGUILayout.IntSlider("Random tree y scale:", Mathf.CeilToInt(treeSiz.y), 0, 99);
			treeSiz.z = EditorGUILayout.IntSlider("Random tree z scale:", Mathf.CeilToInt(treeSiz.z), 0, 99);
		}
	}

	private void OnSceneGUI()
	{
		if (Event.current.button == 1 && Event.current.isMouse && vent <= 0)
		{
			Action();
			vent = 10;
		}
		vent--;
	}
	public void Lod()
	{
		Mesh mesh = new Mesh();
		Vector3[] mVert = new Vector3[289];
		Vector2[] mUV = new Vector2[289];
		Vector3[] tNorms = new Vector3[289];
		for (int i = 0; i < 17; i++)
		{
			for (int p = 0; p < 17; p++)
			{
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(new Vector3(i * (siz / 16) + myscript.transform.position.x, myscript.gameObject.GetComponent<Renderer>().bounds.size.y + myscript.transform.position.y, p * (siz / 16) + myscript.transform.position.z), Vector3.down, out hit);
					mVert[i * 17 + p] = hit.point;
					mUV[i * 17 + p] = new Vector2(mVert[i * 17 + p].x / siz, mVert[i * 17 + p].z / siz);
					tNorms[i * 17 + p] = new Vector3(64, mVert[i * 17 + p].y, 0);
			}
		}
		mesh.vertices = mVert;
		mesh.uv = mUV;
		int[] mTri = new int[16 * 16 * 6];
		for (int i = 0; i < 16; i++)
		{
			if (i % 2 == 0)
			{
				for (int p = 0; p < 16; p++)
				{
					if (p % 2 == 0)
					{
						mTri[(i * 16 + p) * 6] = i * 17 + p;
						mTri[(i * 16 + p) * 6 + 1] = i * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 2] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 3] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 4] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 5] = i * 17 + p;
					}
					else
					{
						mTri[(i * 16 + p) * 6] = i * 17 + p;
						mTri[(i * 16 + p) * 6 + 1] = i * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 2] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 3] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 4] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 5] = i * 17 + p + 1;
					}
				}
			}
			else
			{
				for (int p = 0; p < 16; p++)
				{
					if (p % 2 == 0)
					{
						mTri[(i * 16 + p) * 6] = i * 17 + p;
						mTri[(i * 16 + p) * 6 + 1] = i * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 2] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 3] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 4] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 5] = i * 17 + p + 1;
					}
					else
					{
						mTri[(i * 16 + p) * 6] = i * 17 + p;
						mTri[(i * 16 + p) * 6 + 1] = i * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 2] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 3] = (i + 1) * 17 + p + 1;
						mTri[(i * 16 + p) * 6 + 4] = (i + 1) * 17 + p;
						mTri[(i * 16 + p) * 6 + 5] = i * 17 + p;
					}
				}
			}
		}
		mesh.triangles = mTri;
		mesh.normals = tNorms;
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
		GameObject game = new GameObject();
		game.transform.parent = myscript.transform;
		game.AddComponent<MeshFilter>();
		game.GetComponent<MeshFilter>().sharedMesh = mesh;
		game.AddComponent<MeshRenderer>();
		game.GetComponent<Renderer>().sharedMaterial = myscript.GetComponent<Renderer>().sharedMaterial;
	}
	public void Smooth()
	{
		vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
		for (int i = 0; i < (resolution + 1); i++)
		{
			for (int p = 0; p < (resolution + 1); p++)
			{
				if (i == 0 || p == 0 || i == resolution || p == resolution)
				{
					
						vert[i * (resolution + 1) + p] = new Vector3(vert[i * (resolution + 1) + p].x, 0, vert[i * (resolution + 1) + p].z);
				}
			}
		}
		myscript.GetComponent<MeshFilter>().sharedMesh.vertices = vert;
		myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
	}
	public void Reset()
	{
		if (vert != null && nResolution != 0)
		{
			myscript.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			spawnedTrees = new List<GameObject>();
			resolution = nResolution;
			childNum = myscript.transform.childCount;
			for (int i = 0; i < (childNum); i++)
			{
				DestroyImmediate(myscript.transform.GetChild(childNum - i - 1).gameObject);
			}
			System.Array.Resize(ref vert, (resolution + 1) * (resolution + 1));
			tex = new Texture2D(resolution + 1, resolution + 1);
			for (int i = 0; i < (resolution + 1); i++)
			{
				for (int p = 0; p < (resolution + 1); p++)
				{
					vert[i * (resolution + 1) + p] = new Vector3(i * siz / resolution, Random.Range(-0.04f, 0.04f) * siz * 0.01f, p * siz / resolution);
					tex.SetPixel(i, p, Color.green);
				}
				if (i > 0)
				{
					EditorUtility.DisplayProgressBar("Reset progress","Resetting terrain...", (resolution * 4) / i);
				}
			}
			myscript.GetComponent<MeshFilter>().sharedMesh.vertices = vert;
			tri = new int[resolution * resolution * 6];
			for (int i = 0; i < resolution; i++)
			{
				if (i % 2 == 0)
				{
					for (int p = 0; p < resolution; p++)
					{
						if (p % 2 == 0)
						{
							tri[(i * (resolution) + p) * 6] = i * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 1] = i * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 2] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 3] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 4] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 5] = i * (resolution + 1) + p;
						}
						else
						{
							tri[(i * (resolution) + p) * 6] = i * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 1] = i * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 2] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 3] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 4] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 5] = i * (resolution + 1) + p + 1;
						}
					}
				}
				else
				{
					for (int p = 0; p < resolution; p++)
					{
						if (p % 2 == 0)
						{
							tri[(i * (resolution) + p) * 6] = i * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 1] = i * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 2] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 3] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 4] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 5] = i * (resolution + 1) + p + 1;
						}
						else
						{
							tri[(i * (resolution) + p) * 6] = i * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 1] = i * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 2] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 3] = (i + 1) * (resolution + 1) + p + 1;
							tri[(i * (resolution) + p) * 6 + 4] = (i + 1) * (resolution + 1) + p;
							tri[(i * (resolution) + p) * 6 + 5] = i * (resolution + 1) + p;
						}
					}
				}
				if (i > 0)
				{
					EditorUtility.DisplayProgressBar("Reset progress","Resetting terrain...", (resolution * 4) / (i + resolution));
				}
			}
			myscript.GetComponent<MeshFilter>().sharedMesh.triangles = tri;
			myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
			myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
			uvs = new Vector2[vert.Length];
			Material mat = new Material(Shader.Find("Standard"));
			
			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i] = new Vector2(vert[i].x / siz, vert[i].z / siz);
				if (i > 0)
				{
					EditorUtility.DisplayProgressBar("Reset progress","Resetting terrain...", (resolution * 4) / (i + resolution * 2));
				}
			}
			tex.Apply();
			mat.mainTexture = tex;
			myscript.GetComponent<Renderer>().sharedMaterial = mat;
			myscript.GetComponent<MeshFilter>().sharedMesh.uv = uvs;
			myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
			norms = new Vector3[vert.Length];
			for (int i = 0; i < uvs.Length; i++)
			{
				norms[i] = new Vector3(64, vert[i].y, 0);
				if (i > 0)
				{
					EditorUtility.DisplayProgressBar("Reset progress", "Resetting terrain...", (resolution * 4) / (i + resolution * 3));
				}
			}
			myscript.GetComponent<MeshFilter>().sharedMesh.normals = norms;
			myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
			DestroyImmediate(myscript.GetComponent<MeshCollider>());
			myscript.gameObject.AddComponent<MeshCollider>();
			EditorUtility.ClearProgressBar();
		}
	}
	public void Action()
	{
		resolution = Mathf.RoundToInt(Mathf.Sqrt(myscript.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3 / 2));
		switch (index)
		{
			case 0:
				break;
			case 1:
				vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
				uvs = myscript.GetComponent<MeshFilter>().sharedMesh.uv;
				norms = myscript.GetComponent<MeshFilter>().sharedMesh.normals;
				for (int i = (Mathf.RoundToInt(myscript.brushPos.x) - myscript.brushSize / 2) * resolution / siz; i <= (Mathf.RoundToInt(myscript.brushPos.x) + myscript.brushSize / 2) * resolution / siz; i++)
				{
					if (i >= 0)
					{
						for (int p = (Mathf.RoundToInt(myscript.brushPos.z) - myscript.brushSize / 2) * resolution / siz; p <= (Mathf.RoundToInt(myscript.brushPos.z) + myscript.brushSize / 2) * resolution / siz; p++)
						{
							if (i * (resolution + 1) + p < vert.Length && p >= 0)
							{
								float dist = Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].z));
								if (dist <= myscript.brushSize / 2)
								{
									vert[i * (resolution + 1) + p] = new Vector3(myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].x, myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].y + myscript.opacity * 0.02f, myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].z);
								}
							}
						}
					}
				}
				myscript.GetComponent<MeshFilter>().sharedMesh.uv = uvs;
				myscript.GetComponent<MeshFilter>().sharedMesh.vertices = vert;
				myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
				myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
				childNum = myscript.transform.childCount;
				myscript.gameObject.GetComponent<MeshCollider>().sharedMesh = myscript.gameObject.GetComponent<MeshFilter>().sharedMesh;
				for (int i = 0; i < (childNum); i++)
				{
					RaycastHit hit;
					if (Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(myscript.transform.GetChild(i).position.x, myscript.transform.GetChild(i).position.z)) <= myscript.brushSize / 2)
					{
						if (Physics.Raycast(myscript.transform.GetChild(i).position + Vector3.up * 100, -Vector3.up, out hit, Mathf.Infinity, 1 << 9))
						{
							myscript.transform.GetChild(i).position = hit.point;
						}
					}
				}
				break;
			case 2:
				norms = myscript.GetComponent<MeshFilter>().sharedMesh.normals;
				vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
				for (int i = (Mathf.RoundToInt(myscript.brushPos.x) - myscript.brushSize / 2) * resolution / siz; i <= (Mathf.RoundToInt(myscript.brushPos.x) + myscript.brushSize / 2) * resolution / siz; i++)
				{
					if (i >= 0)
					{
						for (int p = (Mathf.RoundToInt(myscript.brushPos.z) - myscript.brushSize / 2) * resolution / siz; p <= (Mathf.RoundToInt(myscript.brushPos.z) + myscript.brushSize / 2) * resolution / siz; p++)
						{
							if (i * (resolution + 1) + p < vert.Length && p >= 0)
							{
								float dist = Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].z));
								if (dist <= myscript.brushSize / 2)
								{
									vert[i * (resolution + 1) + p] = new Vector3(myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].x, myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].y - myscript.opacity * 0.02f, myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].z);
								}
							}
						}
					}
				}
				myscript.GetComponent<MeshFilter>().sharedMesh.vertices = vert;
				myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
				childNum = myscript.transform.childCount;
				myscript.gameObject.GetComponent<MeshCollider>().sharedMesh = myscript.gameObject.GetComponent<MeshFilter>().sharedMesh;
				for (int i = 0; i < (childNum); i++)
				{
					RaycastHit hit;
					if (Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(myscript.transform.GetChild(i).position.x, myscript.transform.GetChild(i).position.z)) <= myscript.brushSize / 2)
					{
						if (Physics.Raycast(myscript.transform.GetChild(i).position - Vector3.up * 100, Vector3.up, out hit, Mathf.Infinity, 1 << 9))
						{
							myscript.transform.GetChild(i).position = hit.point;
						}
					}
				}
				break;
			case 3:
				norms = myscript.GetComponent<MeshFilter>().sharedMesh.normals;
				vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
				for (int i = (Mathf.RoundToInt(myscript.brushPos.x) - myscript.brushSize / 2) * resolution / siz; i <= (Mathf.RoundToInt(myscript.brushPos.x) + myscript.brushSize / 2) * resolution / siz; i++)
				{
					if (i >= 0)
					{
						for (int p = (Mathf.RoundToInt(myscript.brushPos.z) - myscript.brushSize / 2) * resolution / siz; p <= (Mathf.RoundToInt(myscript.brushPos.z) + myscript.brushSize / 2) * resolution / siz; p++)
						{
							if (i * (resolution + 1) + p < vert.Length && p >= 0)
							{
								float dist = Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].z));
								if (dist <= myscript.brushSize / 2)
								{

									vert[i * (resolution + 1) + p] = new Vector3(myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].x, myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].y + Mathf.Clamp(myscript.opacity * -(vert[i * (resolution + 1) + p].y - myscript.destHeight) * 0.002f, -Mathf.Abs((vert[i * (resolution + 1) + p].y - myscript.destHeight)), Mathf.Abs((vert[i * (resolution + 1) + p].y - myscript.destHeight))), myscript.GetComponent<MeshFilter>().sharedMesh.vertices[i * (resolution + 1) + p].z);
								}
							}
						}
					}
				}
				childNum = myscript.transform.childCount;
				myscript.GetComponent<MeshFilter>().sharedMesh.vertices = vert;
				myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
				childNum = myscript.transform.childCount;
				myscript.gameObject.GetComponent<MeshCollider>().sharedMesh = myscript.gameObject.GetComponent<MeshFilter>().sharedMesh;
				for (int i = 0; i < (childNum); i++)
				{
					RaycastHit hit;
					if (Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(myscript.transform.GetChild(i).position.x, myscript.transform.GetChild(i).position.z)) <= myscript.brushSize / 2)
					{
						if (Physics.Raycast(myscript.transform.GetChild(i).position + Vector3.up * 100, -Vector3.up, out hit, Mathf.Infinity, 1 << 9))
						{
							myscript.transform.GetChild(i).position = hit.point;
						}
					}
				}
				break;
			case 4:
				uvs = myscript.GetComponent<MeshFilter>().sharedMesh.uv;
				vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
				tex = myscript.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
				for (int i = (Mathf.RoundToInt(myscript.brushPos.x) - myscript.brushSize / 2) * resolution / siz; i <= (Mathf.RoundToInt(myscript.brushPos.x) + myscript.brushSize / 2) * resolution / siz; i++)
				{
					if (i >= 0)
					{
						for (int p = (Mathf.RoundToInt(myscript.brushPos.z) - myscript.brushSize / 2) * resolution / siz; p <= (Mathf.RoundToInt(myscript.brushPos.z) + myscript.brushSize / 2) * resolution / siz; p++)
						{
							if (i * (resolution + 1) + p < uvs.Length && p >= 0)
							{
								float dist = Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].z));
								if (dist <= myscript.brushSize / 2)
								{
									tex.SetPixel(i, p, col);
								}
							}
						}
					}
				}
				tex.Apply();
				myscript.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
				myscript.GetComponent<MeshFilter>().sharedMesh.uv = uvs;
				myscript.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
				break;
			case 5:
				vert = myscript.GetComponent<MeshFilter>().sharedMesh.vertices;
				for (int i = (Mathf.RoundToInt(myscript.brushPos.x) - myscript.brushSize / 2) * resolution / siz; i <= (Mathf.RoundToInt(myscript.brushPos.x) + myscript.brushSize / 2) * resolution / siz; i++)
				{
					if (i >= 0)
					{
						for (int p = (Mathf.RoundToInt(myscript.brushPos.z) - myscript.brushSize / 2) * resolution / siz; p <= (Mathf.RoundToInt(myscript.brushPos.z) + myscript.brushSize / 2) * resolution / siz; p++)
						{
							if (i * (resolution + 1) + p < vert.Length && p >= 0)
							{
								float dist = Vector2.Distance(new Vector2(myscript.brushPos.x, myscript.brushPos.z), new Vector2(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].z));
								if (dist <= myscript.brushSize / 2 && Random.Range(1, 5000) <= myscript.opacity)
								{
									stree = Instantiate(myscript.tree, new Vector3(vert[i * (resolution + 1) + p].x, vert[i * (resolution + 1) + p].y, vert[i * (resolution + 1) + p].z), Quaternion.Euler(new Vector3(treeRot.x, treeRot.y + Random.Range(1, 360), treeRot.z)));
									stree.transform.parent = myscript.transform;
									stree.transform.localScale = new Vector3(stree.transform.localScale.x * Random.Range(1 - treeSiz.x * 0.01f, 1 + treeSiz.x * 0.01f), stree.transform.localScale.y * Random.Range(1 - treeSiz.y * 0.01f, 1 + treeSiz.y * 0.01f), stree.transform.localScale.z * Random.Range(1 - treeSiz.z * 0.01f, 1 + treeSiz.z * 0.01f));
									foreach(GameObject spawnedTree in spawnedTrees)
									{
										if(Vector3.Distance(stree.transform.position, spawnedTree.transform.position) <= 1)
										{
												DestroyImmediate(stree);
												break;
										}
									}
									if (stree != null)
									{
										spawnedTrees.Add(stree);
									}
								}
							}
						}
					}
				}
				break;
		}
	}
}