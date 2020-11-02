using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class FloorPlacementController : MonoBehaviour
{
    public GameManager gameManager;
    public bool hasStarted;
    public Text selectPlane;
    public GameObject startButton;
    private List<GameObject> spawnNew = new List<GameObject>();
    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    [SerializeField]
    public List<GameObject> objectToSpawn = new List<GameObject>();
    static public ARPlane savePlane;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Pose hitPose;
    private bool surfaceFound = false;

    public int numberOfTargets = 5;

    void Awake()
    {
        savePlane = null;
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;

        return false;
    }
    void Update()
    {
        if (selectPlane.enabled && aRPlaneManager.trackables.count > 0)
            selectPlane.text = "Select A Plane";

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (!surfaceFound && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                savePlane = aRPlaneManager.GetPlane(hits[0].trackableId);
                hitPose = hits[0].pose;

                foreach (var plane in aRPlaneManager.trackables)
                {
                    if (hits[0].trackableId != plane.trackableId)
                        plane.gameObject.SetActive(false);
                }
                aRPlaneManager.enabled = false;

                selectPlane.enabled = false;
                startButton.SetActive(true);
                surfaceFound = true;
            }
        }
    }

    public void StartButton()
    {
        hasStarted = true;
        GameObject plane = GameObject.FindGameObjectWithTag("Plane");
        plane.GetComponent<MeshRenderer>().enabled = false;
        plane.GetComponent<LineRenderer>().enabled = false;
        plane.GetComponent<ARPlaneMeshVisualizer>().enabled = false;

        foreach (var target in spawnNew)
        {
            Destroy(target);
        }
        GameObject[] objectArray = objectToSpawn.ToArray();
        for (int i = 0; i < numberOfTargets; i++)
        {
            spawnNew.Add(Instantiate(objectArray[Random.Range(0, objectArray.Length)], new Vector3(savePlane.transform.position.x, savePlane.transform.position.y + 0.05f, savePlane.transform.position.z), hitPose.rotation));
        }
        startButton.SetActive(false);
    }
}
