using GameBuilders.FPSBuilder.Core.Inventory;
using GameBuilders.FPSBuilder.Core.Managers;
using GameBuilders.FPSBuilder.Core.Player;
using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_GameMenuCanvas;

        [SerializeField]
        private GameObject m_HUDCanvas;
        public GameObject HUDCanvas => m_HUDCanvas;

        [SerializeField]
        private GameObject m_DeathScreenCanvas;

        [SerializeField]
        private BlackScreen m_PauseBlackScreen;

        [SerializeField]
        private BlackScreen m_DeathBlackScreen;

        [SerializeField]
        private TMP_Text m_SceneLabel;
        
        [SerializeField]
        private GameObject m_AboutView;

        [Header("References")]
        [SerializeField]
        [Required]
        private FirstPersonCharacterController m_FirstPersonCharacter;

        [SerializeField]
        [Required]
        private HealthController m_HealthController;

        [SerializeField]
        [Required]
        private WeaponManager m_InventoryManager;

        private bool m_Restarting;
        private bool m_About;
        private bool m_UpgradeSelecting;
        public bool UpgradeSelecting { get { return m_UpgradeSelecting; } set { m_UpgradeSelecting = value; } }

        private InputActionMap m_WeaponInputBindings;
        private InputActionMap m_MovementInputBindings;
        private InputActionMap m_MenuInputBindings;
        private InputAction m_PauseAction;

        public FirstPersonCharacterController FirstPersonCharacter => m_FirstPersonCharacter;

        public HealthController HealthController => m_HealthController;

        public WeaponManager InventoryManager => m_InventoryManager;

        private void Start()
        {
            m_WeaponInputBindings = GameplayManager.Instance.GetActionMap("Weapons");
            m_MovementInputBindings = GameplayManager.Instance.GetActionMap("Movement");

            m_MenuInputBindings = GameplayManager.Instance.GetActionMap("Menu");
            m_MenuInputBindings.Enable();
            m_PauseAction = m_MenuInputBindings.FindAction("Pause");

            m_SceneLabel.text = "Operation - " + SceneManager.GetActiveScene().name;

            Resume();
        }

        private void Update()
        {
            if (GameplayManager.Instance.IsDead && !m_Restarting)
            {
                DeathScreen();
            }
            else
            {
                if (m_PauseAction.triggered)
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            m_WeaponInputBindings.Enable();
            m_MovementInputBindings.Enable();

            Time.timeScale = 1;
            m_HUDCanvas.SetActive(true);
            m_GameMenuCanvas.SetActive(false);
            m_DeathScreenCanvas.SetActive(false);
            AudioListener.pause = false;
            HideCursor(true);
        }

        private void Pause()
        {
            if (m_UpgradeSelecting)
            {
                return;
            }
            m_WeaponInputBindings.Disable();
            m_MovementInputBindings.Disable();

            Time.timeScale = 0;
            m_HUDCanvas.SetActive(false);
            m_GameMenuCanvas.SetActive(true);
            m_DeathScreenCanvas.SetActive(false);
            AudioListener.pause = true;
            HideCursor(false);
        }

        public void DisableInputBindings()
        {
            m_WeaponInputBindings.Disable();
            m_MovementInputBindings.Disable();
        }

        public void EnableInputBindings()
        {
            m_WeaponInputBindings.Enable();
            m_MovementInputBindings.Enable();
        }

        private void DeathScreen()
        {
            m_WeaponInputBindings.Disable();
            m_MovementInputBindings.Disable();

            m_Restarting = true;
            Time.timeScale = 1;
            m_HUDCanvas.SetActive(false);
            m_GameMenuCanvas.SetActive(false);
            m_DeathScreenCanvas.SetActive(true);

            AudioListener.pause = false;

            m_DeathBlackScreen.Show = true;
            Invoke(nameof(LoadLastLevel), 4f);
        }

        public void Restart()
        {
            m_WeaponInputBindings.Disable();
            m_MovementInputBindings.Disable();

            m_Restarting = true;
            Time.timeScale = 1;
            m_PauseBlackScreen.Show = true;
            Invoke(nameof(LoadLastLevel), 1f);
        }

        public void About()
        {
            m_About = !m_About;
            m_AboutView.SetActive(m_About);
        }

        private void LoadLastLevel ()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void Quit ()
        {
            AudioListener.pause = false;
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void OnApplicationQuit ()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
        
        private static void HideCursor (bool hide)
        {
            if (hide)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
