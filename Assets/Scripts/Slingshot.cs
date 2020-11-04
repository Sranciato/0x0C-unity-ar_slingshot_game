using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Slingshot : MonoBehaviour
{
    // Ammo components
    [HideInInspector]
    public GameObject predictionManagerObject;
    [HideInInspector]
    public PredictionManager predictionManager;
    [HideInInspector]
    public GameObject gameManagerObject;
    [HideInInspector]
    public GameManager gameManager;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private Vector3 directionOfForce;
    private bool holdTouch, shotAmmo;
    private float distance;
    Vector3 ammoOrigin;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GameObject _other = GameObject.Find("PredictionManager");
        predictionManager = _other.GetComponent<PredictionManager>();
        GameObject other = GameObject.Find("GameManager");
        gameManager = other.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        transform.position = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane + .2f) );
    }

    // Update is called once per frame
    void Update()
    { 
        if (shotAmmo)
        {
            if (transform.position.y < FloorPlacementController.savePlane.transform.position.y)
                gameManager.DestroyAmmo();
            return;
        }
            
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            transform.position = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane + .2f));
            return;
        }
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hitObject;
            if (Physics.Raycast(ray, out hitObject) && gameManager.canGrabAmmo)
            {
                if (hitObject.transform.tag == "Ammo")
                {
                    holdTouch = true;
                }
            }
        }
        if (holdTouch && TouchPhase.Ended == Input.touches[0].phase)
        {
            ShootAmmo();
            holdTouch = false;
        }
        if (holdTouch)
        {
            lineRenderer.enabled = true;
            // as y goes down, z comes closer
            float pullBallOnZAxis = touchPosition.y * (Camera.main.nearClipPlane + .2f) / (Screen.height / 2);
            transform.position = Camera.main.ScreenToWorldPoint( new Vector3(touchPosition.x, touchPosition.y, pullBallOnZAxis));
            ammoOrigin = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane + .2f));
            directionOfForce = (ammoOrigin - transform.position);
            distance = Vector3.Distance(ammoOrigin, transform.position);
            predictionManager.Predict(gameObject, transform.position, directionOfForce * (distance * 200f), lineRenderer);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Target")
        {
            gameManager.IncreaseScore();
            gameManager.DoExplosion(transform);
            Destroy(collider.gameObject);
        }
    }

    void ShootAmmo()
    {
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        lineRenderer.enabled = false;
        rb.AddForce(directionOfForce * (distance * 200f), ForceMode.Impulse);
        gameManager.launchAmmo();
        shotAmmo = true;
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
}
