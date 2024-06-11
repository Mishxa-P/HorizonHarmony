using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayTime: MonoBehaviour
{
    [SerializeField] private float duration = 25.0f;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float nightLightIntesity = 0.4f;
    [SerializeField] private float dayLightIntesity = 1.0f;

    [Header("Sun")]
    [SerializeField] private GameObject sun;
    [SerializeField] private float daySunOffsetY;
    [SerializeField] private float nightSunOffsetY;
    [SerializeField] private float sunOffsetX;

    [Header("Night Post-Process")]
    [SerializeField] private Volume volume;
   

    private State currentState;
    private float startTime = 0.0f;
    public enum State
    {
        Night = 0,
        Day = 1
    }
    private void Start()
    {
        currentState = GameData.dayTimeState;
        switch (currentState)
        {
            case State.Night:
                sun.GetComponent<Parallax>().offset = new Vector3(sunOffsetX, nightSunOffsetY, 0.0f);
                volume.weight = 1.0f;
                break;
            case State.Day:
                sun.GetComponent<Parallax>().offset = new Vector3(sunOffsetX, daySunOffsetY, 0.0f);
                volume.weight = 0.0f;
                break;
            default:
                break;
        }
        startTime = GameData.dayTimeStartTime;
    }
    private void Update()
    {
        float t = (Time.time - startTime) / duration;
        switch (currentState)
        {
            case State.Night:
                globalLight.intensity = Mathf.SmoothStep(nightLightIntesity, dayLightIntesity, t);
                volume.weight = Mathf.SmoothStep(1.0f, 0.0f, t);
                sun.GetComponent<Parallax>().offset = new Vector3(sunOffsetX, Mathf.SmoothStep(nightSunOffsetY, daySunOffsetY, t), 0.0f);
                break;
            case State.Day:
                globalLight.intensity = Mathf.SmoothStep(dayLightIntesity, nightLightIntesity, t);
                volume.weight = Mathf.SmoothStep(0.0f, 1.0f, t);
                sun.GetComponent<Parallax>().offset = new Vector3(sunOffsetX, Mathf.SmoothStep(daySunOffsetY, nightSunOffsetY, t), 0.0f);
                break;
            default:
                break;
        }
        if (t >= 1.0f)
        {
            if(currentState == State.Night) 
            {
                currentState = State.Day;
            }
            else
            {
                currentState = State.Night;
            }
            startTime = Time.time;
            GameData.dayTimeStartTime = startTime;
        }
    }
}
