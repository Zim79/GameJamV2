using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LowPolyTerrain : MonoBehaviour {

	[SerializeField]
	public int resolution = 32;
	[SerializeField]
	public Vector3[] siz;
	public int brushSize;
	public Vector3 brushPos;
	public int opacity;
	public float destHeight;
	public bool show;
	public Projector proj;
	public GameObject tree;

	private void Awake()
	{
		Destroy(GetComponent<MeshCollider>());
		gameObject.AddComponent<MeshCollider>();
	}

	[ExecuteInEditMode]
	private void OnDrawGizmosSelected()
	{
		if(!EditorApplication.isPlaying)
		{
			proj = GameObject.FindObjectOfType<Projector>() as Projector;
			RaycastHit hit;
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if (Physics.Raycast(ray, out hit) && show)
			{
				brushPos = hit.point - transform.position;
				proj.enabled = true;
				proj.transform.position = new Vector3 (hit.point.x, hit.point.y + 100, hit.point.z);
				proj.orthographicSize = brushSize / 2;
			}
			else
			{
				proj.enabled = false;
			}
		}
	}
}