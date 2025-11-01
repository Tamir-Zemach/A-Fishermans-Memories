using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerPickupController : MonoBehaviour
    {
        private GameObject _player;
        private Rigidbody _objectRigidbody;
        private GameObject _objectThatGotPickedUp;

        [SerializeField] private CinemachineCamera _pickupCam;
        private float _defaultCameraTopClamp;
        private float _defaultCameraBottomClamp;
        [SerializeField] private GameObject _playerCameraRoot;
        [SerializeField] private Transform _handBone;

        private CheckForPickables _checkForPickables;
        private ThirdPersonController _playerController;

        private bool _lockRotation;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerController = gameObject.GetComponentInParent<ThirdPersonController>();
            _checkForPickables = gameObject.GetComponentInParent<CheckForPickables>();
            _defaultCameraTopClamp = _playerController.TopClamp;
            _defaultCameraBottomClamp = _playerController.BottomClamp;

        }

        private void LateUpdate()
        {
            if (_lockRotation)
            {
                _objectThatGotPickedUp.transform.rotation = transform.rotation;
            }
        }
        public void Pickup()
        {
            AudioManager.instance.playOneShot(FmodEvents.instance.playerPickup, transform.position);

            AttachPickedUpObject();

            SetPickupCameraMode();

        }
        private void AttachPickedUpObject()
        {
            _lockRotation = true;
            _objectThatGotPickedUp = _checkForPickables.HitInfo.transform.gameObject;
            _objectRigidbody = _objectThatGotPickedUp.GetComponent<Rigidbody>();
            _objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Transform objectTransform = _objectThatGotPickedUp.transform;
            objectTransform.position = _handBone.position;
            objectTransform.SetParent(_handBone);


        }

        private void SetPickupCameraMode()
        {
            _pickupCam.enabled = true;
            _playerController.TopClamp = 0;
            _playerController.BottomClamp = 0;
            _playerController.CinemachineCameraTarget = _player;
        }

        public void DropPickup()
        {
            AudioManager.instance.playOneShot(FmodEvents.instance.playerDrop, transform.position);

            DetachPickedUpObject(_objectRigidbody);

            ResetCameraDropMode();

            _objectThatGotPickedUp = null;
        }

        private void DetachPickedUpObject(Rigidbody objectRigidbody)
        {
            _lockRotation = false;
            objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _objectThatGotPickedUp.transform.SetParent(null);
            objectRigidbody.linearVelocity = Vector3.zero;
        }

        private void ResetCameraDropMode()
        {
            _pickupCam.enabled = false;
            _playerController.TopClamp = _defaultCameraTopClamp;
            _playerController.BottomClamp = _defaultCameraBottomClamp;
            _playerController.CinemachineCameraTarget = _playerCameraRoot;
        }

        public void DropPickupInFrontOfWall(Vector3 dropPoint)
        {

            DetachPickedUpObject(_objectRigidbody);
            _objectThatGotPickedUp.transform.position = dropPoint;
            _objectThatGotPickedUp = null;
            ResetCameraDropMode();

        }


    }
}
