using UnityEngine;
using Walls;

public class Collision_Checker : MonoBehaviour
{
    private ColorReactiveWall _colorReactiveWall;

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.TryGetComponent<ColorReactiveWall>(out var component))
        {
            _colorReactiveWall = component;
            component.ObjectInCollider = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<ColorReactiveWall>(out var component))
        {
            _colorReactiveWall = component;
            component.ObjectInCollider = false;
            _colorReactiveWall = null;
        }
    }

}
