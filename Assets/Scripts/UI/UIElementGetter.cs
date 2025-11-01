using Player;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIElementGetter : MonoBehaviour
{
    public Canvas _startCanvas;
    public Canvas _pauseCanvas;
    public Canvas _audioSettingsCanvas;
    public CinemachineCamera _StartuiCamera;
    public CinemachineCamera _pickupCamera;
    public PlayerInput _playerInput;
    public ThirdPersonController _thirdPersonController;
    public CheckForPickables _checkForPickables;
    public StarterAssetsInputs _starterAssetsInputs;
    public PlayAudioOnStart playAudioOnStart;
    public EventSystem _startEventSystem;
    public EventSystem _pauseEventSystem;
    public Animator _animator;

    private void Awake()
    {

        _startCanvas = ValidateComponent<Canvas>(GameObject.FindGameObjectWithTag("StartCanvas"), "StartCanvas");
        _pauseCanvas = ValidateComponent<Canvas>(GameObject.FindGameObjectWithTag("PauseCanvas"), "Pause Canvas");
        _audioSettingsCanvas = ValidateComponent<Canvas>(GameObject.FindGameObjectWithTag("AudioSettingsCanvas"), "Audio Settings Canvas");
        _StartuiCamera = ValidateComponent<CinemachineCamera>(GameObject.Find("Start_UI_Camera"), "UI Camera");
        _pickupCamera = ValidateComponent<CinemachineCamera>(GameObject.Find("pickup_Cam"), "Pause Camera");
        _playerInput = ValidateComponent<PlayerInput>(GameObject.FindGameObjectWithTag("Player"), "PlayerInput");
        _startEventSystem = ValidateComponent<EventSystem>(GameObject.Find("StartUI_EventSystem"), "Start UI Event System");
        _pauseEventSystem = ValidateComponent<EventSystem>(GameObject.Find("PauseUI_EventSystem"), "Pause UI Event System");
        playAudioOnStart = ValidateComponent<PlayAudioOnStart>(GameObject.Find("AudioOnStart_ScriptHolder"), "PlayAudioOnStart");
        _thirdPersonController = ValidateComponent<ThirdPersonController>(GameObject.FindWithTag("Player"), "Third Person Controller");
        _checkForPickables = ValidateComponent<CheckForPickables>(GameObject.Find("PlayerPickupController"), "Player Pickup Controller");
        _starterAssetsInputs = ValidateComponent<StarterAssetsInputs>(_thirdPersonController.gameObject, "Starter Assets Input");
        _animator = ValidateComponent<Animator>(_thirdPersonController.gameObject, "Animator");
        
    }

    private T ValidateComponent<T>(GameObject obj, string name) where T : Component
    {
        if (obj == null)
        {
            Debug.LogError($"{name} GameObject is missing or not found!");
            return null;
        }

        T component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"{name} component is missing in {obj.name}!");
        }

        return component;
    }

    private GameObject ValidateObject(GameObject obj, string name)
    {
        if (obj == null)
        {
            Debug.LogError($"{name} GameObject is missing or not found!");
        }
        return obj;
    }

    public bool HasMissingReferences()
    {
        return _startCanvas == null || _StartuiCamera == null || _playerInput == null || playAudioOnStart == null;
    }
}
