
using UnityEngine;
using System.Collections;
using Walls;

public class LightScript : MonoBehaviour
{

    [SerializeField] float RayCastRange = 15f;
    [SerializeField] float sphereCastRadius = 2f;
    [SerializeField] float rayYOffset = -1.2f;
    [SerializeField] LayerMask IgnoredLayer;
    [SerializeField] LanternColor lanternColor;

    private Material[] materials;
    private Color defaultEmission;

    private ColorReactiveWall currentHitLightReactionScript;
    //private ColorReactiveWall lightReactionScriptHitBySphere;
    private Light[] VisualLights;

    private SphereCollider OverlapSphere;

    private Color VisualColor; //must be a better way to do this

    private void OnValidate()
    {
        OverlapSphere = GetComponent<SphereCollider>();
    }

    private void Awake()
    {

        VisualLights = GetComponentsInChildren<Light>();

        OverlapSphere = GetComponent<SphereCollider>();
        OverlapSphere.isTrigger = true;

        switch (lanternColor)
        {
            case LanternColor.Red:
                foreach (var light in VisualLights)
                {
                    light.color = Color.red;
                    VisualColor = Color.red;
                }
                break;

            case LanternColor.Yellow:
                foreach (var light in VisualLights)
                {
                    light.color = Color.yellow;
                    VisualColor = Color.yellow;
                }
                break;

            case LanternColor.Blue:
                foreach (var light in VisualLights)
                {
                    light.color = Color.blue;
                    VisualColor = Color.blue;
                }
                break;
        }

        OverlapSphere.radius = sphereCastRadius;
    }

    private void Start()
    {
        materials = GetComponentInChildren<Renderer>().materials;
        defaultEmission = VisualColor * 2.2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled)
        {
            var lightReactionScript = other.GetComponent<ColorReactiveWall>();
            lightReactionScript?.AddColorToList(lanternColor);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.enabled)
        {
            var lightReactionScript = other.GetComponent<ColorReactiveWall>();
            lightReactionScript?.AddColorToList(lanternColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.enabled)
        {
            var lightReactionScript = other.GetComponent<ColorReactiveWall>();
            lightReactionScript?.RemoveColorFromList(lanternColor);
        }

    }


    void Update()
    {
        RaycastForLightReaction();
    }

    private void OnDrawGizmos()
    {
        if (this.enabled)
        {
            Debug.DrawRay(transform.position + new Vector3(0, rayYOffset, 0), transform.forward * RayCastRange, VisualColor);
            Gizmos.color = VisualColor;
            Gizmos.DrawWireSphere(transform.TransformPoint(OverlapSphere.center), sphereCastRadius/2); // make sure lantern scale is 1,1,1 so that dividing by 2 isnt necessery
        }

    }



    private void RaycastForLightReaction()
    {
        Physics.queriesHitTriggers = true;

        /* before checking for light Reaction script, make it null and remove color tag from the list - 
         * so that if it hits a different target it still removes colortag from the first hit*/
        if (currentHitLightReactionScript != null)
        {
            currentHitLightReactionScript.RemoveColorFromList(lanternColor);
            currentHitLightReactionScript = null;
        }

        bool isRaycastHitting = Physics.Raycast(transform.position + new Vector3(0, rayYOffset, 0), transform.forward, out RaycastHit hitInfo, RayCastRange, ~IgnoredLayer);


        if (isRaycastHitting)
        {
            var hitObject = hitInfo.transform;

            hitObject.TryGetComponent<ColorReactiveWall>(out currentHitLightReactionScript);

            currentHitLightReactionScript?.AddColorToList(lanternColor);
        }
    }

    private IEnumerator TurnLightOffForTimer(float toggleDuration, int flashCount, float flashDuration)
    {
        this.enabled = false;
        ToggleEmission(false);
        yield return new WaitForSeconds(toggleDuration);

        // Flashing effect
        for (int i = 0; i < flashCount; i++)
        {
            ToggleEmission(true);
            yield return new WaitForSeconds(flashDuration);
            ToggleEmission(false);
            yield return new WaitForSeconds(flashDuration);
        }

        ToggleEmission(true); // Turn emission back on
        this.enabled = true;
    }

    private void ToggleEmission(bool state)
    {

        foreach (Material material in materials)
        {
            if (state)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");
        }
    }
    public void StartToggleLightCoroutine(float toggleDuration, int flashCount, float flashDuration)
    {
        StopAllCoroutines();
        StartCoroutine(TurnLightOffForTimer(toggleDuration, flashCount, flashDuration));
    }

    private void OnEnable()
    {
        foreach (var light in VisualLights)
        {
            light.enabled = true;
        }
    }

    private void OnDisable()
    {
        foreach (var light in VisualLights)
        {
            light.enabled = false;
        }
        if (currentHitLightReactionScript != null)
        {
            currentHitLightReactionScript.RemoveColorFromList(lanternColor);
            currentHitLightReactionScript = null;
        }
    }
}
