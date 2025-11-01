using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckForPickables : MonoBehaviour
{
    [Header("Ray Params")]
    public GameObject RayStartPos;
    public float RayLength = 5.5f;
    public RaycastHit HitInfo;
    [FormerlySerializedAs("ignoreRaycastLayer")] public LayerMask IgnoreRaycastLayer;

    [Header("Input Text")]
    [SerializeField] private TextMeshPro _textToAppear;


    private OutlineHandler _outlineHandler;
    private Player_Pickup_Controller _pickupController;
    private Camera _camera;

    private Vector3 _screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    [HideInInspector] public bool _isHoldingObj;
    private int lastScreenWidth;
    private int lastScreenHeight;
    

    [Header("Input Buffer")]
    [SerializeField] private float bufferTime = 0.5f;
    private bool canPressDrop = true;

    

    private void Awake()
    {
        _camera = Camera.main;
        _pickupController = gameObject.GetComponent<Player_Pickup_Controller>();
        _outlineHandler = gameObject.GetComponent<OutlineHandler>();
        _textToAppear = gameObject.transform.GetComponentInChildren<TextMeshPro>();
    }


    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            _screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }

        CheckingForPickupsWithRay();
        HandleDropPickup();
    }
    public void CheckingForPickupsWithRay()
    {
        if (IsObjectPickable() && !_isHoldingObj)
        {
            _outlineHandler.ShowPickupVisibleHint();
            _outlineHandler._currentOutLine = HitInfo.collider.GetComponent<Outline>();
            _textToAppear.enabled = true;
            HandlePickup();
        }
        else
        {
            _outlineHandler.ResetHighlight();
            _textToAppear.enabled = false;
        }
    }
    public bool IsObjectPickable()
    {

        Ray ray = _camera.ScreenPointToRay(_screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.red);
        if (Physics.Raycast(ray, out HitInfo, RayLength, ~IgnoreRaycastLayer))
        {
            if (HitInfo.transform.GetComponent<LightScript>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
    public void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_isHoldingObj && canPressDrop)
        {
            _pickupController.Pickup();
            _outlineHandler.ResetHighlight();
            _isHoldingObj = true;
            StartCoroutine(InputBuffer());
        }

    }
    private void HandleDropPickup()
    {
        if (_isHoldingObj && Input.GetKeyDown(KeyCode.E) && canPressDrop)
        {
            _pickupController.DropPickup();
            _outlineHandler.ResetHighlight();
            _isHoldingObj = false;
            StartCoroutine(InputBuffer());
        }
    }

    private IEnumerator InputBuffer()
    {
        canPressDrop = false;
        yield return  new WaitForSeconds(bufferTime);
        canPressDrop = true;
    }

}