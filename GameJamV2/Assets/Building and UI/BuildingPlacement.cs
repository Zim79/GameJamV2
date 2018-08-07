using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour {

    public GameObject[] Buildings = new GameObject[4];
    public List<GameObject> PlacedBuildings = new List<GameObject>();

    Vector3 clickPosInWorldCoordinates;
    Coroutine waitForBuild;
    GameObject buildingToPlace;
    bool buildMode;

	void Start () 
    {

	}

    public void InitiateBuildMode(Building _buildingType) 
    {
        buildMode = true;
        waitForBuild = StartCoroutine("SelectBuildingPlacement", _buildingType);
    }

    IEnumerator SelectBuildingPlacement(Building _buildingType) {
        while (buildMode)
        {
            if (buildingToPlace == null)
            {
                buildingToPlace = Instantiate(Buildings[(int)_buildingType - 1]);
                buildingToPlace.layer = 2;
            }


            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1);

            if(Input.touches.Length == 1)
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 1000f, 1);

            if (hit.collider != null)
            {
                buildingToPlace.transform.position = hit.point;
            }
            if (!buildMode)
                StopCoroutine(waitForBuild);
            yield return null;
        }
    }

    public void PlaceBuilding(Building _buildingType) {
        buildMode = false;
        Destroy(buildingToPlace);
        buildingToPlace = null;

        PlacedBuildings.Add(Instantiate(Buildings[(int)_buildingType - 1]));

    }

}

public enum Building {
    None,
    TownHall,
    House,
    Tower,
    Farm
}
                /*
                LayerMask layerMask = 1 << 9;
                Collider[] checkSpaceAvailable = Physics.OverlapBox(buildingToPlace.transform.position, new Vector3(5,2,5), Quaternion.identity, layerMask);
                if (checkSpaceAvailable.Length != 0)
                {
                    bool allPassed = false;
                    foreach (Collider col in checkSpaceAvailable)
                    {
                        print(Vector2.Distance(buildingToPlace.GetComponent<BoxCollider>().bounds.ClosestPoint(col.GetComponent<BoxCollider>().bounds.ClosestPoint(buildingToPlace.transform.position)), col.GetComponent<BoxCollider>().bounds.ClosestPoint(buildingToPlace.GetComponent<BoxCollider>().bounds.ClosestPoint(buildingToPlace.transform.position))));
                        if (Vector2.Distance(buildingToPlace.GetComponent<BoxCollider>().bounds.ClosestPoint(col.GetComponent<BoxCollider>().bounds.ClosestPoint(buildingToPlace.transform.position)), col.GetComponent<BoxCollider>().bounds.ClosestPoint(buildingToPlace.GetComponent<BoxCollider>().bounds.ClosestPoint(col.transform.position))) < 0.1f)
                        {
                            allPassed = false;
                            spaceAvailable = false;
                            break;
                        }
                        allPassed = true;
                    }
                    if (allPassed)
                    {
                        spaceAvailable = true;
                    }
                    else
                    {
                        print("Can't build; " + checkSpaceAvailable[0] + " in the way!");

                    }
                }
                else
                {
                    spaceAvailable = true;
                }
            }*/