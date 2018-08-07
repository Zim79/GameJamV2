using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour {

    public GameObject[] Buildings = new GameObject[4];

    Vector3 clickPosInWorldCoordinates;
    Coroutine waitForClick;
    bool buildMode;

	void Start () 
    {
		
	}

    private void Update() {
        if (Input.GetKeyUp(KeyCode.B))
        {
            InitiateBuildMode(Building.Townhall);
        }
    }

    public void InitiateBuildMode(Building buildingType) 
    {
        buildMode = true;
        waitForClick = StartCoroutine("SelectBuildingPlacement");
    }

    IEnumerator SelectBuildingPlacement() 
    {
        while (buildMode)
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
                    buildMode = false;
                }
            }
            yield return null;
        }
        if (!buildMode)
            StopCoroutine(waitForClick);
    }
}

public enum Building {
    Townhall,
    House,
    Tower,
}