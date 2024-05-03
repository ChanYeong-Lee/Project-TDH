using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;


public enum CameraType
{
    QuaterView  = 0,
    TopView     = 10,
}

public enum CameraZoomType
{
    Default     = 0,
    Full        = 1,
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("설정")]
    public CinemachineVirtualCamera quaterViewDefaultCam;
    public CinemachineVirtualCamera quaterViewFullCam;
    public CinemachineVirtualCamera topViewDefaultCam;
    public CinemachineVirtualCamera topViewFullCam;

    [Header("상태")]
    public CameraType cameraType;
    public CameraZoomType zoomType;

    private Dictionary<int, CinemachineVirtualCamera> vCamDictionary;
    private CinemachineVirtualCamera currentCam;

    private void Awake()
    {
        Instance = this;

        vCamDictionary = new Dictionary<int, CinemachineVirtualCamera>
        {
            {(int)CameraType.QuaterView + (int)CameraZoomType.Default,  quaterViewDefaultCam },
            {(int)CameraType.QuaterView + (int)CameraZoomType.Full,     quaterViewFullCam },
            {(int)CameraType.TopView    + (int)CameraZoomType.Default,  topViewDefaultCam },
            {(int)CameraType.TopView    + (int)CameraZoomType.Full,     topViewFullCam },
        };

        foreach (CinemachineVirtualCamera vCam in vCamDictionary.Values)
        {
            vCam.Priority = 10;
        }

        cameraType = CameraType.QuaterView;
        zoomType = CameraZoomType.Default;

        currentCam = vCamDictionary[TranslateTypeToInt(cameraType, zoomType)];
        currentCam.Priority = 20;
    }

    public void ChangeCameraType()
    {
        currentCam.Priority = 10;

        switch (cameraType)
        {
            case CameraType.QuaterView:
                cameraType = CameraType.TopView;
                break;
            case CameraType.TopView:
                cameraType = CameraType.QuaterView;
                break;
        }

        currentCam = vCamDictionary[TranslateTypeToInt(this.cameraType, zoomType)];
        currentCam.Priority = 20;
    }

    public void ChangeCameraZoom()
    {
        currentCam.Priority = 10;

        switch (zoomType)
        {
            case CameraZoomType.Default:
                zoomType = CameraZoomType.Full;
                break;
            case CameraZoomType.Full:
                zoomType = CameraZoomType.Default;
                break;
        }

        currentCam = vCamDictionary[TranslateTypeToInt(cameraType, this.zoomType)];
        currentCam.Priority = 20;
    }

    private int TranslateTypeToInt(CameraType cameraType, CameraZoomType zoomType)
    {
        return (int)cameraType + (int)zoomType; 
    }
}
