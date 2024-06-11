using Cinemachine;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float initialScreenSizeInUnits = 20.0f;
    [SerializeField] private float maxScreenSizeInUnits = 25.0f;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool mainMenuCamera = false;
   
    public Constraint constraint = Constraint.Landscape;
    public enum Constraint { Landscape, Portrait }
    public static CameraManager Singleton;

    private CinemachineBrain brain;
    private CinemachineFramingTransposer transposer;

    private float previousScreenSize = 20.0f;
    private float previousAspect = 1.0f;
    private float screenSizeInUnits;
    private float initialScreenX;

    private float vel;
    private void Awake()
    {
        if (Singleton != null)
        {
            Debug.LogError("Camera manager is already exist!");
        }
        else
        {
            Singleton = this;
            brain = GetComponent<CinemachineBrain>();
            transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            initialScreenX = transposer.m_ScreenX;
            screenSizeInUnits = initialScreenSizeInUnits;
            previousScreenSize = screenSizeInUnits;
            ComputeResolution();
        }
    }
    private void Update()
    {
        if (!mainMenuCamera)
        {
            float targetSscreenSizeInUnits = initialScreenSizeInUnits + Math.Min(playerMovement.CurrentSpeed / playerMovement.MaxSpeedWithoutEvents, 1.0f) * (maxScreenSizeInUnits - initialScreenSizeInUnits);
            screenSizeInUnits = Mathf.SmoothDamp(screenSizeInUnits, targetSscreenSizeInUnits, ref vel, smoothTime);
            if (previousScreenSize != screenSizeInUnits || previousAspect != virtualCamera.m_Lens.Aspect)
            {
                previousScreenSize = screenSizeInUnits;
                ComputeResolution();
            }
        }
        brain.ManualUpdate();
    }

    public void ComputeResolution()
    {
        if (constraint == Constraint.Landscape)
        {
            virtualCamera.m_Lens.OrthographicSize = 1.0f / virtualCamera.m_Lens.Aspect * screenSizeInUnits / 2.0f;
        }
        else
        {
            virtualCamera.m_Lens.OrthographicSize = screenSizeInUnits / 2.0f;
        }
    }
    public void ChangeCameraScreenX(float x, float duration)
    {
        StartCoroutine(ChangeCameraScreenXCoroutine(x, duration));
    }
    private IEnumerator ChangeCameraScreenXCoroutine(float x, float duration)
    {
        transposer.m_ScreenX = x;
        yield return new WaitForSeconds(duration);
        transposer.m_ScreenX = initialScreenX;
    }
}
