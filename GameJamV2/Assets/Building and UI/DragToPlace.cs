using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToPlace : MonoBehaviour {

    public GameObject[] DragButtons = new GameObject[4];
    public GameObject[] BuildingPrefabs = new GameObject[4];
    public List<GameObject> PlacedBuildings = new List<GameObject>();
    public Mesh[] ProbeMeshes = new Mesh[4];
    public GameObject Probe;
    public Coroutine Probing;
    public bool isBuyMode;
    public Building selectedForPurchase;
    public bool PlacementAvailable = true;

    RaycastHit hit;


    void Start() {
        int i = 0;
        foreach (GameObject dragButton in DragButtons)
        {
            DragButton db = dragButton.AddComponent<DragButton>();
            db.dtp = this;
            db.ShopItem = (Building)i;
            i++;
        }
    }

    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            print("Finger lifted");
            StopAllCoroutines();
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && isBuyMode)
        {
            StartCoroutine("SelectPlacement");
        }
    }

    public IEnumerator SelectPlacement() {
        while (true)
        {
            if (Input.touchCount != 0)
            {
                if (Probe.activeInHierarchy == false)
                    Probe.SetActive(true);

                Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit, 1000f, 1);

                Probe.transform.position = hit.point;
            }

            yield return null;
        }
    }

    public void PlacementModeInitiated(Building _shopItem) {
        isBuyMode = true;
        Destroy(Probe);
        Probe = Instantiate(BuildingPrefabs[(int)_shopItem]);
        Probing = StartCoroutine("SelectPlacement");
        selectedForPurchase = _shopItem;
    }

    public void EndBuyMode() {
        isBuyMode = false;
        Destroy(Probe);
        Probe.SetActive(false);
    }

    public void Buy() {

        PlacementAvailable = true;

        foreach (GameObject placedBuilding in PlacedBuildings)
        {
            if (Probe.GetComponentInChildren<MeshCollider>().bounds.Intersects(placedBuilding.GetComponentInChildren<MeshCollider>().bounds))
            {
                PlacementAvailable = false;
                print("UNAVAILABLE");
            }
        }

        if(PlacementAvailable)
            PlacedBuildings.Add(Instantiate(BuildingPrefabs[(int)selectedForPurchase], hit.point, Quaternion.identity));

        EndBuyMode();
    }
}
public class DragButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    [HideInInspector]
    public DragToPlace dtp;
    public Building ShopItem;

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        dtp.PlacementModeInitiated(ShopItem);        
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
    }

}

public class ProbeObject : MonoBehaviour {

    public Mesh[] meshes = new Mesh[4];
    MeshFilter mf;
    MeshCollider mc;

    private void Start() {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
    }

}

public enum Building {
    Townhall,
    House,
    Farm
}
















