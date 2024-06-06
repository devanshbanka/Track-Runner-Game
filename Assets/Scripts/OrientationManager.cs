using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public Canvas canvasLandscape;
    public Canvas canvasPortrait;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.MyInstance;
        UpdateOrientation();
    }

    void Update()
    {
        //if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
        //    Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        //{
        //    SetLandscape();
        //}
        //else if (Input.deviceOrientation == DeviceOrientation.Portrait ||
        //         Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        //{
        //    SetPortrait();
        //}
    }

    void SetLandscape()
    {
        canvasLandscape.gameObject.SetActive(true);
        canvasPortrait.gameObject.SetActive(false);
        gameManager.SetLandscapeReferences();
    }

    void SetPortrait()
    {
        canvasLandscape.gameObject.SetActive(false);
        canvasPortrait.gameObject.SetActive(true);
        gameManager.SetPortraitReferences();
    }

    void UpdateOrientation()
    {
        if (Screen.width > Screen.height)
        {
            SetLandscape();
        }
        else
        {
            SetPortrait();
        }
    }
}
