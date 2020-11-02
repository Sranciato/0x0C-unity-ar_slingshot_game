using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MoveTarget : MonoBehaviour
{
    public LayerMask layer;
    private FloorPlacementController placementController;
    private ARPlane plane;
    private Vector3 normalizeDirection;
 
    private Vector3 randomTarget;
    public float speed = 2f;
    private bool offEdge;
    void Start()
    {
        plane = FloorPlacementController.savePlane;
        randomTarget = new Vector3(plane.center.x + Random.Range(-1f, 1f), transform.position.y, plane.center.z + Random.Range(-1f, 1f));
        normalizeDirection = (randomTarget - transform.position).normalized;
        transform.LookAt(randomTarget);
    }
    void Update()
    {
        if (!CheckEdge())
            return;
        transform.position += normalizeDirection * speed * Time.deltaTime;
        
    }

    private bool CheckEdge()
    {
        RaycastHit objectHit;
        // Shoot raycast
        if (Physics.Raycast(transform.position, transform.forward + (-transform.up), out objectHit, 50, layer))
        {
            if (offEdge)
            {
                normalizeDirection = (randomTarget - transform.position).normalized;
                offEdge = false;
            }
            return true;
        }
        else
        {
            offEdge = true;
            randomTarget = new Vector3(plane.center.x + Random.Range(-1f, 1f), transform.position.y, plane.center.z + Random.Range(-1f, 1f));
            transform.LookAt(randomTarget);
            return false;
        }
    }
}
