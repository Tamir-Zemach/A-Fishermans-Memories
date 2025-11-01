using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class CheckForPickables : MonoBehaviour
    {
        [Header("Ray Params")]
        public float RayLength = 5.5f;
        public RaycastHit HitInfo;
        public LayerMask IgnoreRaycastLayer;

        [Header("Input Text")]
        [SerializeField] private TextMeshPro _textToAppear;


        private OutlineHandler _outlineHandler;
        private PlayerPickupController _pickupController;
        private Camera _camera;

        private Vector3 _screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        [HideInInspector] public bool IsHoldingObj;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
    
        
        [Header("Input Buffer")]
        [SerializeField] private float _bufferTime = 0.5f;
        private bool _canPressDrop = true;

    

        private void Awake()
        {
            _camera = Camera.main;
            _pickupController = gameObject.GetComponent<PlayerPickupController>();
            _outlineHandler = gameObject.GetComponent<OutlineHandler>();
            _textToAppear = gameObject.transform.GetComponentInChildren<TextMeshPro>();
        }


        private void Update()
        {
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                _screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }

            CheckingForPickupsWithRay();
            HandleDropPickup();
        }

        private void CheckingForPickupsWithRay()
        {
            if (IsObjectPickable() && !IsHoldingObj)
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

        private bool IsObjectPickable()
        {

            Ray ray = _camera.ScreenPointToRay(_screenCenter);
            Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.red);
            if (Physics.Raycast(ray, out HitInfo, RayLength, ~IgnoreRaycastLayer))
            {
                return HitInfo.transform.GetComponent<LightScript>() != null;
            }
            return false;

        }

        private void HandlePickup()
        {
            if (!Input.GetKeyDown(KeyCode.E) || IsHoldingObj || !_canPressDrop) return;
            _pickupController.Pickup();
            _outlineHandler.ResetHighlight();
            IsHoldingObj = true;
            StartCoroutine(InputBuffer());

        }
        private void HandleDropPickup()
        {
            if (!IsHoldingObj || !Input.GetKeyDown(KeyCode.E) || !_canPressDrop) return;
            _pickupController.DropPickup();
            _outlineHandler.ResetHighlight();
            IsHoldingObj = false;
            StartCoroutine(InputBuffer());
        }

        private IEnumerator InputBuffer()
        {
            _canPressDrop = false;
            yield return  new WaitForSeconds(_bufferTime);
            _canPressDrop = true;
        }

    }
}