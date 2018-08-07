using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour {

    public GameObject[] Buildings = new GameObject[4];

    Vector3 clickPosInWorldCoordinates;
    Coroutine waitForClick;
    GameObject buildingToPlace;
    bool buildMode;

	void Start () 
    {
		
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
            if(buildingToPlace == null)
            {
                buildingToPlace = GameObject.CreatePrimitive(PrimitiveType.Cube);
                buildingToPlace.layer=2;
            }

            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1);
            if (hit.collider != null)
            {
                buildingToPlace.transform.position = hit.point;
            }

            if (Input.touchCount != 0 || Input.GetAxis("RightClick") == 1)
            {
                buildMode = false;
                buildingToPlace = null;
            }


            yield return null;
        }
        if (!buildMode)
            StopCoroutine(waitForClick);
    }
}

public enum Building {
    None,
    TownHall,
    House,
    Tower
}