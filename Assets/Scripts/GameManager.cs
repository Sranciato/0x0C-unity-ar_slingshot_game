using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public ARSession arSession;
    public GameObject sessionOrigin;
    private FloorPlacementController placementController;
    private bool alreadySpawned = false;
    public bool canGrabAmmo = true;
    public GameObject ammo;
    private GameObject _ammo;
    public GameObject explosionPrefab;
    public SoundManagerScript soundManagerScript;

    // UI Objects
    public GameObject uiCanvas;
    public GameObject playAgainButton;
    public int amountOfAmmo = 7;
    public Text ammoText;
    public Text scoreText;
    int amountOfScore = 0, holdAmountOfAmmo = 0, targetsDestroyed = 0, numOfTargets = 0;
    void Start()
    {
        canGrabAmmo = true;
        holdAmountOfAmmo = amountOfAmmo;
        placementController = sessionOrigin.GetComponent<FloorPlacementController>();
        ammoText.text = amountOfAmmo.ToString();
        scoreText.text = amountOfScore.ToString();
        numOfTargets = placementController.numberOfTargets;

    }

    // Update is called once per frame
    void Update()
    {
        if (!alreadySpawned && placementController.hasStarted)
        {
            _ammo = Instantiate(ammo, Camera.main.ScreenToWorldPoint( new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane + .2f)), Quaternion.identity);
            uiCanvas.SetActive(true);
            alreadySpawned = true;
        }
        if (targetsDestroyed == numOfTargets && amountOfAmmo > 0)
        {
            targetsDestroyed = 0;
            NextLevel();
        }
    }

    public void DestroyAmmo()
    {
        Destroy(_ammo);
        soundManagerScript.PlaySound("missedTarget");
        amountOfAmmo -= 1;
        ammoText.text = amountOfAmmo.ToString();
        if (amountOfAmmo < 1)
        {
            canGrabAmmo = false;
            soundManagerScript.PlaySound("gameOver");
            playAgainButton.SetActive(true);
        }
        alreadySpawned = false;
    }

    public void IncreaseScore()
    {
        amountOfScore += 10;
        scoreText.text = amountOfScore.ToString();
    }
    public void Restart()
    {
        arSession.Reset();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        canGrabAmmo = true;
        placementController.StartButton();
        amountOfAmmo = holdAmountOfAmmo;
        ammoText.text = amountOfAmmo.ToString();
        amountOfScore = 0;
        scoreText.text = amountOfScore.ToString();
        playAgainButton.SetActive(false);
        targetsDestroyed = 0;
    }

    private void NextLevel()
    {
        placementController.StartButton();
        amountOfAmmo = holdAmountOfAmmo;
        ammoText.text = amountOfAmmo.ToString();
    }

    public void DoExplosion(Transform explosionTransform)
    {
        targetsDestroyed += 1;
        GameObject explosion = Instantiate(explosionPrefab, explosionTransform.position, Quaternion.identity);
        StartCoroutine(DestroyExplosion(explosion));
        soundManagerScript.PlaySound("hitTarget");
    }

    IEnumerator DestroyExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(5f);
        Destroy(explosion);
    }

    public void launchAmmo()
    {
        soundManagerScript.PlaySound("launched");
    }

    public void PressButton()
    {
        soundManagerScript.PlaySound("buttonPress");
    }
}
