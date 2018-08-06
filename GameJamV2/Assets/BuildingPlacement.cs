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

            yield return null;
        }
    }
}

public enum Building {
    Townhall,
    House,
    Tower,
}