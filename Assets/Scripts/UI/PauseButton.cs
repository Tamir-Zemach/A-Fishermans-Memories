using UnityEngine;

namespace UI

{
    public class PauseButton : MonoBehaviour
    {
        private UIFunctions _uIFunctions;

        public KeyCode PauseKey = KeyCode.P;

        public bool _pressedPauseButton;
        public bool _inStartCanvas;
        private void Awake()
        {
            _uIFunctions = GetComponent<UIFunctions>();
            _pressedPauseButton = true;
            _inStartCanvas = true;
        }

        void Update()
        {

            if (!_inStartCanvas)
            {
                if (!_pressedPauseButton)
                {
                    GetInput();
                }
                else
                {
                    GetInputWhilePaused();
                }
            }

        }


        private void GetInput()
        {
            if (Input.GetKeyDown(PauseKey))
            {
                _uIFunctions.PauseGame();
            }
        }

        private void GetInputWhilePaused()
        {
            if (Input.GetKeyDown(PauseKey) )
            {
                _uIFunctions.Continue();
            }
        }



    }
}
