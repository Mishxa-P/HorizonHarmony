using System.Collections;
using UnityEngine;

public class ForegroundEventManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [Header("Desert")]
    [SerializeField] private GameObject desertSmallForeground;
    [SerializeField] private GameObject desertBigForeground;
    [Header("Ocean")]
    [SerializeField] private GameObject oceanForeground;
    [Header("SnowMountains")]
    [SerializeField] private GameObject snowMountainsForeground;

    public static ForegroundEventManager Singleton { get; private set; }

    private PlayerMovement playerMovement;
    private PlayerEvents playerEvents;

    //--desert--
    private Vector3 DESERT_SCALE_FOR_LESS_2_5_SCREEN_RATIO = new Vector3(1.25f, 1.25f, 1.25f);
    private Vector3 DESERT_SCALE_FOR_LESS_3_0_SCREEN_RATIO = new Vector3(1.35f, 1.35f, 1.35f);
    private const float DESERT_DELAY_BEFORE_DUNES_DESTROY = 7.0f;
    //--ocean--
    private Vector3 oceanOffset = Vector3.zero;
    private Vector3 OCEAN_OFFSET_FOR_LESS_1_5_SCREEN_RATIO = new Vector3(-10.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_2_0_SCREEN_RATIO = new Vector3(-13.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_FIRST = new Vector3(-25.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_SECOND = new Vector3(0.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_FIRST = new Vector3(-40.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_SECOND = new Vector3(-15.0f, -26.0f, 5.0f);
    private Vector3 OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_THIRD = new Vector3(10.0f, -26.0f, 5.0f);
    private const float OCEAN_DELAY_BEFORE_WAVES_DESTROY = 2.5f;
    //--snow mountains--
    private Vector3 snowMountainsOffset = Vector3.zero;
    private Vector3 SNOW_OFFSET = new Vector3(0.0f, 7.0f, 0.0f);
    private Vector3 SNOW_SCALE_FOR_LESS_2_5_SCREEN_RATIO = new Vector3(1.05f, 1.05f, 1.05f);
    private Vector3 SNOW_SCALE_FOR_LESS_3_0_SCREEN_RATIO = new Vector3(1.15f, 1.15f, 1.15f);
    private const float SNOW_DELAY_BEFORE_BILLOWS_DESTROY = 3.0f;
    private const float SNOW_CAMERA_X = 0.375f;
    private const float SNOW_CAMERA_CHANGE_TIME = 1.0f;

    private void Awake()
    {
        if (Singleton != null)
        {
            Debug.LogError("Foreground events manager is already exist!");
        }
        else
        {
            Singleton = this;
        }
    }
    private void Start()
    {
        if (!cameraTransform)
        {
            cameraTransform = Camera.main.transform;
        }
        playerMovement = player.GetComponent<PlayerMovement>();
        playerEvents = player.GetComponent<PlayerEvents>();
    }
    public void ActivateSmallForegroundEvent()
    {
        switch (MapGenerationManager.Singleton.NextLocationState)
        {
            case MapGenerationManager.LocationState.Desert:
                PlayDesertForegroundSmallEvent();
                break;
            case MapGenerationManager.LocationState.Ocean:
                PlayOceanForegroundSmallEvent();
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                PlaySnowMountainsForegroundSmallEvent();
                break;
        }
    }
    public void ActivateBigForegroundEvent()
    {
        switch (MapGenerationManager.Singleton.NextLocationState)
        {
            case MapGenerationManager.LocationState.Desert:
                PlayDesertForegroundBigEvent();
                break;
            case MapGenerationManager.LocationState.Ocean:
                PlayOceanForegroundBigEvent();
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                PlaySnowMountainsForegroundBigEvent();
                break;
        }
        playerEvents.ActivateAccelerationEvent(2.0f);
    }
    public void PlayDesertForegroundSmallEvent()
    {
        GameObject desertSmallForegroundEvent = Instantiate(desertSmallForeground, Vector3.zero, Quaternion.identity);
        Parallax[] parallaxes = desertSmallForegroundEvent.GetComponentsInChildren<Parallax>();
        foreach (Parallax parallax in parallaxes)
        {
            parallax.ActivateBackgroundEvent();
        }
        StartCoroutine(DestroyEvent(desertSmallForegroundEvent, DESERT_DELAY_BEFORE_DUNES_DESTROY));
    }
    public void PlayDesertForegroundBigEvent()
    {
        GameObject desertBigForegroundEvent = Instantiate(desertBigForeground, Vector3.zero, Quaternion.identity);
        float screenRatio = (float)Screen.width / Screen.height;
        if (screenRatio > 1.5f && screenRatio <= 2.5f)
        {
            desertBigForegroundEvent.transform.localScale = DESERT_SCALE_FOR_LESS_2_5_SCREEN_RATIO;
        }
        else
        {
            desertBigForegroundEvent.transform.localScale = DESERT_SCALE_FOR_LESS_3_0_SCREEN_RATIO;
        }

        Parallax[] parallaxes = desertBigForegroundEvent.GetComponentsInChildren<Parallax>();
        foreach (Parallax parallax in parallaxes)
        {
            parallax.ActivateBackgroundEvent();
        }
        StartCoroutine(DestroyEvent(desertBigForegroundEvent, DESERT_DELAY_BEFORE_DUNES_DESTROY));
    }
    public void PlayOceanForegroundSmallEvent()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        if (screenRatio < 1.5f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_1_5_SCREEN_RATIO;
            GameObject oceanForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanForegroundEvent);
        }
        else if (screenRatio <= 2.0f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_0_SCREEN_RATIO;
            GameObject oceanForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanForegroundEvent);
        }
        else if (screenRatio <= 2.5f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_FIRST;
            GameObject oceanFirstForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanFirstForegroundEvent);
            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_SECOND;
            GameObject oceanSecondForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanSecondForegroundEvent);
        }
        else
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_FIRST;
            GameObject oceanFirstForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanFirstForegroundEvent);
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_SECOND;
            GameObject oceanSecondForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanSecondForegroundEvent);
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_THIRD;
            GameObject oceanThirdForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanThirdForegroundEvent);
        }
    }
    public void PlayOceanForegroundBigEvent()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        if (screenRatio < 1.5f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_1_5_SCREEN_RATIO;
            GameObject oceanForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanForegroundEvent, false);
        }
        else if (screenRatio <= 2.0f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_0_SCREEN_RATIO;
            GameObject oceanForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            OceanForegroundEvent(oceanForegroundEvent, false);
        }
        else if (screenRatio <= 2.5f)
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_FIRST;
            GameObject oceanFirstForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            SpriteRenderer[] spriteRenderers = oceanFirstForegroundEvent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = 0;
            }
            OceanForegroundEvent(oceanFirstForegroundEvent, false);

            oceanOffset = OCEAN_OFFSET_FOR_LESS_2_5_SCREEN_RATIO_SECOND;
            GameObject oceanSecondForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            spriteRenderers = oceanSecondForegroundEvent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = 1;
            }
            OceanForegroundEvent(oceanSecondForegroundEvent, false);
        }
        else
        {
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_FIRST;
            GameObject oceanFirstForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            SpriteRenderer[] spriteRenderers = oceanFirstForegroundEvent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = 1;
            }
            OceanForegroundEvent(oceanFirstForegroundEvent, false);
         
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_SECOND;
            GameObject oceanSecondForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            spriteRenderers = oceanSecondForegroundEvent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = 2;
            }
            OceanForegroundEvent(oceanSecondForegroundEvent, false);
           
            oceanOffset = OCEAN_OFFSET_FOR_LESS_3_0_SCREEN_RATIO_THIRD;
            GameObject oceanThirdForegroundEvent = Instantiate(oceanForeground, cameraTransform.position + oceanOffset, Quaternion.identity);
            spriteRenderers = oceanThirdForegroundEvent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = 0;
            }
            OceanForegroundEvent(oceanThirdForegroundEvent, false);
           
        }
    }
    public void PlaySnowMountainsForegroundSmallEvent()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        GameObject snowMountainsForegroundEvent;
        if (screenRatio < 1.5f)
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
        }
        else if (screenRatio <= 2.5f)
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
            snowMountainsForegroundEvent.transform.localScale = SNOW_SCALE_FOR_LESS_2_5_SCREEN_RATIO;
        }
        else
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
            snowMountainsForegroundEvent.transform.localScale = SNOW_SCALE_FOR_LESS_3_0_SCREEN_RATIO;
        }
        SnowMountainsForegroundEvent(snowMountainsForegroundEvent);
    }
    public void PlaySnowMountainsForegroundBigEvent()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        GameObject snowMountainsForegroundEvent;
        if (screenRatio < 1.5f)
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
        }
        else if (screenRatio <= 2.5f)
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
            snowMountainsForegroundEvent.transform.localScale = SNOW_SCALE_FOR_LESS_2_5_SCREEN_RATIO;
        }
        else
        {
            snowMountainsOffset = SNOW_OFFSET;
            snowMountainsForegroundEvent = Instantiate(snowMountainsForeground, cameraTransform.position + snowMountainsOffset, Quaternion.identity);
            snowMountainsForegroundEvent.transform.localScale = SNOW_SCALE_FOR_LESS_3_0_SCREEN_RATIO;
        }

        if (playerMovement != null)
        {
            CameraManager.Singleton.ChangeCameraScreenX(SNOW_CAMERA_X, SNOW_CAMERA_CHANGE_TIME);
        }

        SnowMountainsForegroundEvent(snowMountainsForegroundEvent, false);
    }
    private void OceanForegroundEvent(GameObject oceanEvent, bool isSmallEvent = true)
    {
        Parallax parallax = oceanEvent.GetComponent<Parallax>();
        parallax.offset = oceanOffset;
        parallax.followingTarget = cameraTransform;
        Animator oceanAnimator = oceanEvent.GetComponent<Animator>();
        if (playerMovement != null)
        {
            oceanAnimator.SetFloat("AnimationSpeed", Mathf.Max(playerMovement.CurrentSpeed / playerMovement.InitialSpeed * 0.2f, 1.0f));
        }
        else
        {
            oceanAnimator.SetFloat("AnimationSpeed", 0.9f);
        }

        if (isSmallEvent)
        {
            oceanAnimator.SetTrigger("SmallWave");
        }
        else
        {
            oceanAnimator.SetTrigger("BigWave");
        }
        StartCoroutine(DestroyEvent(oceanEvent, OCEAN_DELAY_BEFORE_WAVES_DESTROY));
    }
    private void SnowMountainsForegroundEvent(GameObject snowEvent, bool isSmallEvent = true)
    {
        Parallax parallax = snowEvent.GetComponent<Parallax>();
        parallax.offset = snowMountainsOffset;
        parallax.followingTarget = cameraTransform;
        Animator snowMountainsAnimator = snowEvent.GetComponent<Animator>();
        if (playerMovement != null)
        {
            snowMountainsAnimator.SetFloat("AnimationSpeed", Mathf.Max(playerMovement.CurrentSpeed / playerMovement.InitialSpeed * 0.2f, 1.0f));
        }
        else
        {
            snowMountainsAnimator.SetFloat("AnimationSpeed", 0.9f);
        }
        if (isSmallEvent)
        {
            snowMountainsAnimator.SetTrigger("SmallBillow");
        }
        else
        {
            snowMountainsAnimator.SetTrigger("BigBillow");
        }
        StartCoroutine(DestroyEvent(snowEvent, SNOW_DELAY_BEFORE_BILLOWS_DESTROY));
    }
    private IEnumerator DestroyEvent(GameObject eventObj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(eventObj);
    }
}
