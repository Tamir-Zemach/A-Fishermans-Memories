using Player;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private Animator _animator;
    private CheckForPickables _checkForPickables;
    private const int MaxWeight = 1; // Using PascalCase for constants
    private const int MinWeight = 0;
    private int _handLayerIndex;

    private void Awake()
    {
        // Cache components efficiently
        _animator = GetComponent<Animator>();
        _checkForPickables = GetComponentInChildren<CheckForPickables>();
        _handLayerIndex = _animator.GetLayerIndex("Hands_Layer");
    }

    private void Update()
    {
        UpdateAnimationParameters();
        ManageLayerWeight();
    }

    private void UpdateAnimationParameters()
    {
        _animator.SetBool("isHolding", _checkForPickables.IsHoldingObj);
    }

    private void ManageLayerWeight()
    {
        _animator.SetLayerWeight(_handLayerIndex, _checkForPickables.IsHoldingObj ? MaxWeight : MinWeight);
    }
}
