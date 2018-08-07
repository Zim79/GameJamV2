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
    bool spaceAvailable;

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
                buildingToPlace.transform.position = hit.point + Vector3.up/2;

                LayerMask layerMask = 1 << 9;
                
                Collider[] checkSpaceAvailable = Physics.OverlapBox(buildingToPlace.transform.position, new Vector3(buildingToPlace.transform.localScale.x, buildingToPlace.transform.localScale.y/2, buildingToPlace.transform.localScale.z/2), Quaternion.identity, layerMask);
                if (checkSpaceAvailable.Length != 0)
                {
                    spaceAvailable = false;
                    print("Can't build; " + checkSpaceAvailable[0] + " in the way!");
                }
                else
                {
                    spaceAvailable = true;
                }
            }

            if (Input.touchCount != 0 || Input.GetMouseButtonDown (0) && spaceAvailable)
            {
                buildMode = false;
                buildingToPlace.layer = 9;
                buildingToPlace = null;
            }


            yield return null;
        }
        if (!buildMode)
            StopCoroutine(waitForClick);
    }
    private void OnDrawGizmos() {
        if (buildMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(buildingToPlace.transform.position, new Vector3(buildingToPlace.transform.localScale.x, buildingToPlace.transform.localScale.y, buildingToPlace.transform.localScale.z));
        }
        
    }
}

public enum Building {
    None,
    TownHall,
    House,
    Tower
}