using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Imposter : EditorWindow {

	public Camera cam;
	public int vAngles;
	public int hAngles;
	public int lAngles;
	public GameObject target;
	private GameObject spawned;
	private GameObject sCam;
	private bool render;
	private Texture2D tTemp;
	private int[] hej;

	[MenuItem ("Window/Create Imposter")]

	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(Imposter), false, "Imposter", true);
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("");
		vAngles = EditorGUILayout.IntField(new GUIContent("Vertical angles:", "It is recommended to use between 8 and 32. Using more results in longer loading times."), vAngles);
		target = (EditorGUILayout.ObjectField("Gameobject:",target, typeof(GameObject), true)) as GameObject;
		if(GUILayout.Button("Bake"))
		{
			Create();
		}
	}
	public void Create()
	{
		spawned = Instantiate(target, new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, -90));
		spawned.layer = 10;
		sCam = new GameObject();
		sCam.transform.position = new Vector3(spawned.GetComponent<Renderer>().bounds.center.x + Mathf.Max(spawned.GetComponent<Renderer>().bounds.size.x, spawned.GetComponent<Renderer>().bounds.size.y, spawned.GetComponent<Renderer>().bounds.size.z), spawned.GetComponent<Renderer>().bounds.center.y, spawned.GetComponent<Renderer>().bounds.center.z);
		sCam.transform.LookAt(spawned.GetComponent<Renderer>().bounds.center);
		sCam.AddComponent(typeof(Camera));
		cam = sCam.GetComponent<Camera>();
		cam.cullingMask = 1 << 10;
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = Color.clear;
		cam.orthographic = true;
		cam.orthographicSize = Mathf.Max(spawned.GetComponent<Renderer>().bounds.size.x, spawned.GetComponent<Renderer>().bounds.size.y, spawned.GetComponent<Renderer>().bounds.size.z) * 0.51f;
		RenderTexture img = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
		if(Camera.main.transform.parent.gameObject.GetComponent<Shooter.Jobsystem.BillBoard>() == null)
		{
			Camera.main.transform.parent.gameObject.AddComponent<Shooter.Jobsystem.BillBoard>();
		}
		Camera.main.transform.parent.gameObject.GetComponent<Shooter.Jobsystem.BillBoard>().sprites = new Material[vAngles];
		img.antiAliasing = 4;
		img.Create();
		cam.targetTexture = img;
		RenderTexture.active = img;
		for (int i = 0; i < vAngles; i++)
		{
			cam.Render();
			tTemp = new Texture2D(512, 512);
			tTemp.ReadPixels(new Rect(0, 0, img.width, img.height), 0, 0, false);
			tTemp.Apply();
			tTemp.Compress(false);
			Material mat = new Material(Shader.Find("Unlit/Transparent"));
			mat.mainTexture = tTemp;
			Camera.main.transform.parent.gameObject.GetComponent<Shooter.Jobsystem.BillBoard>().sprites[i] = mat;
			spawned.transform.RotateAround(spawned.GetComponent<Renderer>().bounds.center, new Vector3(0, -1, 0), 360 / vAngles);
			if (i > 0)
			{
				EditorUtility.DisplayProgressBar("Imposter progress", "Creating imposters...", (vAngles) / (i));
			}
			if (i == 0)
			{
				if (target.transform.childCount > 0)
				{
					for (int c = 0; c < target.transform.childCount; c++)
					{
						if (target.transform.GetChild(c).tag == "Billboard")
						{
							target.transform.GetChild(c).GetComponent<Renderer>().material = mat;
						}
					}
				}
			}
		}
		RenderTexture.active = null;
		DestroyImmediate(sCam);
		DestroyImmediate(spawned);
		EditorUtility.ClearProgressBar();
	}
}
