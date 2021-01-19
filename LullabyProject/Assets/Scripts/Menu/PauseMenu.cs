using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

using UnityEngine.Events;

namespace Menu 
{
    /// <summary>
    /// 
    /// </summary>
    public class OnPauseMenuEvent : UnityEvent<bool> { }
    public class PauseMenu : MonoBehaviour
    {
        #region Serialised data

        public GameObject pauseMenu;

        #endregion

        #region MonoBehaviour events

        void Awake()
        {
            m_events = new Events();
        }

        // Start is called before the first frame update
        void Start()
        {
            Assert.IsNotNull(pauseMenu);

            PauseGame(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(!m_isPaused);
            }  
        }
        
        #endregion

        #region Utility

        public void PauseGame(bool pause) 
        {
            pauseMenu.SetActive(pause);
            Time.timeScale = pause ? 0f : 1f;
            m_isPaused = pause;
            
            m_events.onPauseMenuEvent.Invoke(pause);
        }

        public bool IsPaused()
        {
            return m_isPaused;
        }
        

        public void ReturnToMainMenu() 
        {
            SceneManager.LoadScene("MainMenu");
        }

        #endregion

        #region Events

        class Events
        {
            public readonly OnPauseMenuEvent onPauseMenuEvent;

            public Events()
            {
               onPauseMenuEvent = new OnPauseMenuEvent();
            }
        }

        public void AddOnPauseMenuListener(UnityAction<bool> onPauseMenu)
        {
            m_events.onPauseMenuEvent.AddListener(onPauseMenu);
        }
        
        public void RemoveOnPauseMenuListener(UnityAction<bool> onPauseMenu)
        {
            m_events.onPauseMenuEvent.RemoveListener(onPauseMenu);
        }

        #endregion

        #region Private data

        bool m_isPaused;

        Events m_events;

        #endregion
    }

}
