using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour {

    public GameObject[] Buildings = new GameObject[4];

    Vector3 clickPosInWorldCoordinates;


	void Start () 
    {
		
	}

    private void Update() {
        if (Input.GetKeyUp(KeyCode.B))
        {
            StartCoroutine("SelectBuildingPlacement");
        }
    }

    /*void SelectBuildingPlacement()     
    {
        if (Input.touchCount != 0 || Input.GetAxis("RightClick") == 1)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPosInWorldCoordinates), out hit))
            {
                print(hit.collider.name);
            }
        }
    }*/

    IEnumerator SelectBuildingPlacement() 
    {
        while (true)
        {
            if (Input.touchCount != 0 || Input.GetAxis("RightClick") == 1)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f);
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
                if (hit.collider != null)
                {
                    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    g.transform.position = hit.point;
                }
            }
            yield return null;
        }
    }
}

public enum Building {
    Townhall,
    House,
    Tower,
}