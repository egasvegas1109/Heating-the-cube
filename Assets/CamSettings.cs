using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamSettings : MonoBehaviour
{
    [SerializeField] Main cube;
    [SerializeField] Camera camera;
    [SerializeField] FreeFlyCamera cameraMove;
    [SerializeField] public Toggle toggleCameraP;
    [SerializeField] public Toggle toggleActiveM;

    public void EnableMove()
    {
        cube = cube.GetComponent<Main>();
        cameraMove = cube.cam.GetComponent<FreeFlyCamera>();
        cameraMove.enabled = toggleActiveM.isOn;
    }

    public void ChangeSettings()
    {
        cube = cube.GetComponent<Main>();
        camera = cube.cam.GetComponent<Camera>();
        camera.orthographic = toggleCameraP.isOn;
        Debug.Log(toggleCameraP.isOn);
    }
}
