using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public GameObject BuildPanel;
    public BuildingPlacement BuildingPlacementScript;

    Building selectedBuildingType;

	void Start () {
		
	}

    public void OpenCloseBuildUI() {
            BuildPanel.SetActive(!BuildPanel.activeInHierarchy);
    }
    public void OpenBuildUI() {
        if (!BuildPanel.activeInHierarchy)
            BuildPanel.SetActive(true);
    }
    public void CloseBuildUI() {
        if (BuildPanel.activeInHierarchy)
            BuildPanel.SetActive(false);
    }
    public void BuildTownHall() {
        selectedBuildingType = Building.TownHall;
        print("Click");
        Build();
    }
    public void BuildTower() {
        selectedBuildingType = Building.Tower;
        Build();
    }
    public void BuildHouse() {
        selectedBuildingType = Building.House;
        Build();
    }

    void Build() {
        BuildingPlacementScript.InitiateBuildMode(selectedBuildingType);
        selectedBuildingType = Building.None;
    }
}
