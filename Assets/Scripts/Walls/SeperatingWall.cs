using Player;
using UnityEngine;

public class SeperatingWall : MonoBehaviour
{
    /* Check for pickups, if detect pickup drop it in front of wall */

    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private PlayerPickupController pickupController;
    private CheckForPickables _checkForPickables;
    private Vector3 _dropPoint;
    [SerializeField] private float _dropDistance;

    private void Awake()
    {
        pickupController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerPickupController>();
        _checkForPickables = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CheckForPickables>();
        _dropPoint = transform.position + transform.forward * _dropDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (((1 << other.gameObject.layer) & pickupLayer) == 1 << other.gameObject.layer)
            {
                pickupController.DropPickupInFrontOfWall(_dropPoint);
                _checkForPickables.IsHoldingObj = false;
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_dropPoint, 0.1f);
    }
}
