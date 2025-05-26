using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public float _speed;
    public float _startDelay;
    private float _defaultSpeed;
    public float _speedFaster;
    public float _speedSlower;
    private bool _pressedUp;
    private bool _pressedDown;
    public bool _startMooving = false;

    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine(StartDelay());
        _defaultSpeed = _speed;
    }
    private void Update()
    {
        GetInput();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_startMooving)
        {
            transform.Translate(Vector2.up * Time.deltaTime * _speed);
            if (_pressedUp)
            {
                _speed = _speedFaster;
            }
            else if (_pressedDown)
            {
                _speed = _speedSlower;
            }
            else
            {
                _speed = _defaultSpeed;
            }

        }
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _pressedUp = true;
        }
        else
        {
            _pressedUp = false;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _pressedDown = true;
        }
        else
        {
            _pressedDown = false;
        }
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(_startDelay);
        _startMooving = true;
    }

}