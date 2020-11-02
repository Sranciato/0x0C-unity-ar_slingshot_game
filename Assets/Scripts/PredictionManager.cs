using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PredictionManager : MonoBehaviour
{

    // physics scene
    public int maxIterations = 20;
    private Scene currentScene;
    private Scene predictionScene;
    private PhysicsScene predictionPhysicsScene;
    private PhysicsScene currentPhysicsScene;
    GameObject dummy;

    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();

    }

    void FixedUpdate()
    {
        if (currentPhysicsScene.IsValid())
        {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CopyPlane()
    {
        GameObject plane = Instantiate(FloorPlacementController.savePlane.gameObject);
        plane.transform.position = FloorPlacementController.savePlane.transform.position;
        plane.transform.rotation = FloorPlacementController.savePlane.transform.rotation;
        Renderer planeR = plane.GetComponent<Renderer>();
        if (planeR)
        {
            planeR.enabled = false;
        }
        SceneManager.MoveGameObjectToScene(plane, predictionScene);
    }

    public void Predict(GameObject subject, Vector3 currentPosition, Vector3 force, LineRenderer lineRenderer)
    {
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid())
        {
            if (dummy == null)
            {
                dummy = Instantiate(subject);
                SceneManager.MoveGameObjectToScene(dummy, predictionScene);
                dummy.GetComponent<Slingshot>().enabled = false;
            }
            dummy.transform.position = currentPosition;
            dummy.GetComponent<Rigidbody>().isKinematic = false;
            dummy.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            lineRenderer.positionCount = maxIterations;

            for (int i = 0; i < maxIterations; i++)
            {
                lineRenderer.SetPosition(i, dummy.transform.position);
                predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
            }
            Destroy(dummy);
        }
    }
}
