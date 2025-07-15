//=========== Copyright (c) GameBuilders, All rights reserved. ================
//
// Purpose: The Gameplay Manager Script is a reference for the player settings and input bindings.
//          It is mainly used to set the behaviour of the input keys, like holding a button or simply tapping it to do an action.
//
//=============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0649

namespace GameBuilders.FPSBuilder.Core.Managers
{
    /// <summary>
    /// Reference for the player settings and input behaviours.
    /// </summary>
    [AddComponentMenu("FPS Builder/Managers/Game Manager"), DisallowMultipleComponent]
    public sealed class GameplayManager : Singleton<GameplayManager>
    {
        /// <summary>
        /// Provides the behaviour settings of several actions, such Running, Aiming and mouse axes.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Provides the behaviour settings of several actions, such Running, Aiming and mouse axes.")]
        private GameplaySettings m_GameplaySettings;

        /// <summary>
        /// Provides all buttons and axes used by the character.
        /// </summary>
        [SerializeField]
        [Required]
        [Tooltip("Provides all buttons and axes used by the character.")]
        private InputActionAsset m_InputBindings;

        #region PROPERTIES

        /// <summary>
        /// Is the character dead?
        /// </summary>
        public bool IsDead
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the input behaviour of the crouching action.
        /// </summary>
        public ActionMode CrouchStyle => m_GameplaySettings.CrouchStyle;

        /// <summary>
        /// Returns the input behaviour of the aiming action.
        /// </summary>
        public ActionMode AimStyle => m_GameplaySettings.AimStyle;

        /// <summary>
        /// Returns the input behaviour of the running action.
        /// </summary>
        public ActionMode SprintStyle => m_GameplaySettings.SprintStyle;

        /// <summary>
        /// Returns the input behaviour of the leaning action.
        /// </summary>
        public ActionMode LeanStyle => m_GameplaySettings.LeanStyle;

        /// <summary>
        /// Returns the overall mouse sensitivity.
        /// </summary>
        public float OverallMouseSensitivity => m_GameplaySettings.OverallMouseSensitivity;

        /// <summary>
        /// Is the horizontal mouse input reversed?
        /// </summary>
        public bool InvertHorizontalAxis => m_GameplaySettings.InvertHorizontalAxis;

        /// <summary>
        /// Is the vertical mouse input reversed?
        /// </summary>
        public bool InvertVerticalAxis => m_GameplaySettings.InvertVerticalAxis;

        /// <summary>
        /// Returns the main camera field of view used by this character.
        /// </summary>
        public float FieldOfView => m_GameplaySettings.FieldOfView;
        
        /// <summary>
        /// Find an InputActionMap by its name in the InputActionAsset.
        /// </summary>
        /// <param name="mapName"></param>
        public InputActionMap GetActionMap(string mapName)
        {
            return m_InputBindings.FindActionMap(mapName);
        }

        #endregion

        public void SetFOV(float fov)
        {
            m_GameplaySettings.FieldOfView = fov;
        }

        public void ChangeAimMode()
        {
            switch (m_GameplaySettings.AimStyle)
            {
                case ActionMode.Hold:
                    m_GameplaySettings.AimStyle = ActionMode.Toggle;
                    return;
                case ActionMode.Toggle:
                    m_GameplaySettings.AimStyle = ActionMode.Hold;
                    break;
            }
        }
        
        public void ChangeCrouchMode()
        {
            switch (m_GameplaySettings.CrouchStyle)
            {
                case ActionMode.Hold:
                    m_GameplaySettings.CrouchStyle = ActionMode.Toggle;
                    return;
                case ActionMode.Toggle:
                    m_GameplaySettings.CrouchStyle = ActionMode.Hold;
                    break;
            }
        }
        
        public void ChangeSprintMode()
        {
            switch (m_GameplaySettings.SprintStyle)
            {
                case ActionMode.Hold:
                    m_GameplaySettings.SprintStyle = ActionMode.Toggle;
                    return;
                case ActionMode.Toggle:
                    m_GameplaySettings.SprintStyle = ActionMode.Hold;
                    break;
            }
        }
        
        public void ChangeLeanMode()
        {
            switch (m_GameplaySettings.LeanStyle)
            {
                case ActionMode.Hold:
                    m_GameplaySettings.LeanStyle = ActionMode.Toggle;
                    return;
                case ActionMode.Toggle:
                    m_GameplaySettings.LeanStyle = ActionMode.Hold;
                    break;
            }
        }
        
        public void SetMouseSensitivity(float sensitivity)
        {
            m_GameplaySettings.OverallMouseSensitivity = sensitivity;
        }
        
        public void SetInvertHorizontalAxis(bool invert)
        {
            m_GameplaySettings.InvertHorizontalAxis = invert;
        }
        
        public void SetInvertVerticalAxis(bool invert)
        {
            m_GameplaySettings.InvertVerticalAxis = invert;
        }
    }
}
