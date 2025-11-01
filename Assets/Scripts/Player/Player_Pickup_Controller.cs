
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

public class Player_Pickup_Controller : MonoBehaviour
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
    private ThirdPersonController _playerContoller;

    private bool lockRotation;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerContoller = gameObject.GetComponentInParent<ThirdPersonController>();
        _checkForPickables = gameObject.GetComponentInParent<CheckForPickables>();
        _defaultCameraTopClamp = _playerContoller.TopClamp;
        _defaultCameraBottomClamp = _playerContoller.BottomClamp;

    }

    private void LateUpdate()
    {
        if (lockRotation)
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
        lockRotation = true;
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
        _playerContoller.TopClamp = 0;
        _playerContoller.BottomClamp = 0;
        _playerContoller.CinemachineCameraTarget = _player;
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
        lockRotation = false;
        objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _objectThatGotPickedUp.transform.SetParent(null);
        objectRigidbody.linearVelocity = Vector3.zero;
    }

    private void ResetCameraDropMode()
    {
        _pickupCam.enabled = false;
        _playerContoller.TopClamp = _defaultCameraTopClamp;
        _playerContoller.BottomClamp = _defaultCameraBottomClamp;
        _playerContoller.CinemachineCameraTarget = _playerCameraRoot;
    }

    public void DropPickupInFrontOfWall(Vector3 dropPoint)
    {

        DetachPickedUpObject(_objectRigidbody);
        _objectThatGotPickedUp.transform.position = dropPoint;
        _objectThatGotPickedUp = null;
        ResetCameraDropMode();

    }


}
