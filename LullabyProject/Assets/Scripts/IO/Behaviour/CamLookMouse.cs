
using UnityEngine;
using UnityEngine.Assertions;

using Menu;

namespace IO.Behaviour
{
    public class CamLookMouse : UnityEngine.MonoBehaviour
    {

        public float lookSensitivity = 100f;

        public float maxUpAngle = 90f;
        public float minUpAngle = -90f;

        #region MonoBehaviour events

        void Start()
        {
            m_eulerAngles = transform.localEulerAngles;

            var gameManagerObject = GameObject.FindGameObjectWithTag(Utility.Tags.c_gameManagerTag);
            Assert.IsNotNull(gameManagerObject);
            m_pauseMenu = gameManagerObject.GetComponent<PauseMenu>();
            Assert.IsNotNull(m_pauseMenu);
            
            Subscribe();
        }

        void Update()
        {
            // Rotation
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

            // Clamp is to prevent the player from turning their head over their shoulders or under their legs, sorta.
            m_eulerAngles[0] = Mathf.Clamp( m_eulerAngles[0] - mouseY, minUpAngle, maxUpAngle);
            m_eulerAngles[1] += mouseX;

            // Rotate the camera around the x-axis
            transform.localEulerAngles = m_eulerAngles;
        }

        void OnDestroy() {
            UnHideCursor();
            Unsubscribe();
        }

        void OnEnable()
        {
            // Hide mouse cursor.
            HideCursor();
        }

        void OnDisable()
        {
            // Hide mouse cursor.
            UnHideCursor();
        }
        
        #endregion

        #region Pause menu events
        
        void OnPauseMenu(bool isPaused)
        {
            enabled = !isPaused;
        }

        void Subscribe()
        {
            m_pauseMenu.AddOnPauseMenuListener(OnPauseMenu);
        }

        void Unsubscribe()
        {
            m_pauseMenu.RemoveOnPauseMenuListener(OnPauseMenu);
        }
        #endregion

        #region Private utility
        
        // Save the previous state of the cursor
        void HideCursor()
        {
            m_previousMode = Cursor.lockState;
            m_previousVisibility = Cursor.visible;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void UnHideCursor()
        {
            Cursor.lockState = m_previousMode;
            Cursor.visible = m_previousVisibility;
        }
        
        #endregion

        #region Private data

        CursorLockMode m_previousMode;

        PauseMenu m_pauseMenu;
        bool m_previousVisibility;

        Vector3 m_eulerAngles;

        #endregion

    }
}