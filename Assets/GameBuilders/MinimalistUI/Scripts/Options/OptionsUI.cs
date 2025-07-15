using System.Collections.Generic;

using GameBuilders.FPSBuilder.Core.Managers;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace GameBuilders.MinimalistUI.Scripts.Options
{
    public class OptionsUI : MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField]
        private TMP_Text m_FieldOfViewLabel;
        
        [SerializeField]
        private TMP_Text m_AimModeLabel;
        
        [SerializeField]
        private TMP_Text m_CrouchModeLabel;
        
        [SerializeField]
        private TMP_Text m_SprintModeLabel;
        
        [SerializeField]
        private TMP_Text m_LeanModeLabel;
        
        [SerializeField]
        private TMP_Text m_MouseSensitivityLabel;
        
        [Header("Audio")]
        [SerializeField]
        private TMP_Text m_MasterAudioLabel;
        
        [SerializeField]
        private TMP_Text m_SFxAudioLabel;
        
        [SerializeField]
        private TMP_Text m_MusicAudioLabel;

        [SerializeField]
        private TMP_Text m_VoiceAudioLabel;
        
        [Header("Video")]
        
        [SerializeField]
        private TMP_Text m_UiScaleLabel;
        
        [SerializeField]
        private float m_UiScale = 0.5f;
        
        [SerializeField]
        private CanvasScaler[] m_CanvasScalers;
        
        [SerializeField]
        private TMP_Text m_QualityLevelLabel;
        
        [SerializeField]
        private TMP_Text m_TextureQualityLabel;
        
        [SerializeField]
        private TMP_Text m_AnisoLevelLabel;
        
        [SerializeField]
        private TMP_Text m_ShadowResolutionLabel;
        
        [SerializeField]
        private TMP_Text m_MeshQualityLabel;
        
        [SerializeField]
        private TMP_Text m_VSyncLabel;
        
        [SerializeField]
        private TMP_Text m_FullScreenModeLabel;
        
        [SerializeField]
        private TMP_Dropdown m_ResolutionDropdown;
        
        private int m_QualityLevel;
        private int m_LastQualityLevel;
        private int m_MaxQualityLevel;
        
        private int m_TextureQuality;

        private ShadowResolution m_ShadowResolution;

        private SkinWeights m_SkinWeights;

        private int m_VSyncCount;

        private FullScreenMode m_FullScreenMode;
        
        private Resolution m_Resolution;

        private readonly string[] m_TextureQualities =
            {"Full Resolution", "Half Resolution", "Quarter Resolution", "Eighth Resolution"};

        private AnisotropicFiltering m_AnisoLevel;
        
        private readonly string[] m_ShadowResolutions =
            {"Low Resolution", "Medium Resolution", "High Resolution", "Very High Resolution"};
        
        private readonly string[] m_MeshQualities =
            {"Low", "Medium", "High", "Very High"};
        
        private readonly string[] m_VSyncOptions =
            {"Don't Sync", "Every V Blank", "Every Second V Blank"};
        
        private readonly string[] m_FullScreenModes =
            {"Exclusive Full Screen", "Full Screen Window", "Maximized Window", "Windowed"};

        private void Start()
        {
            foreach (CanvasScaler canvasScaler in m_CanvasScalers)
            {
                Vector2 minResolution = new Vector2(1280, 720);
                Vector2 maxResolution = new Vector2(2560, 1440);
                canvasScaler.referenceResolution = Vector2.Lerp(minResolution, maxResolution, 1 - m_UiScale / 2);
            }

            m_QualityLevel = QualitySettings.GetQualityLevel();
            m_LastQualityLevel = m_QualityLevel;
            m_MaxQualityLevel = QualitySettings.names.Length - 1;
            m_TextureQuality = QualitySettings.globalTextureMipmapLimit;
            m_AnisoLevel = QualitySettings.anisotropicFiltering;
            m_ShadowResolution = QualitySettings.shadowResolution;
            m_SkinWeights = QualitySettings.skinWeights;
            m_VSyncCount = QualitySettings.vSyncCount;
            m_FullScreenMode = Screen.fullScreenMode;
            m_Resolution = Screen.currentResolution;
            
            m_ResolutionDropdown.options = new List<TMP_Dropdown.OptionData>();

            for(int i = 0; i < Screen.resolutions.Length; i++)
            {
                m_ResolutionDropdown.options.Add(new TMP_Dropdown.OptionData(Screen.resolutions[i].ToString()));

                if (m_Resolution.width == Screen.resolutions[i].width &&
                    m_Resolution.height == Screen.resolutions[i].height &&
                    (m_Resolution.refreshRate - 1 == Screen.resolutions[i].refreshRate ||
                     m_Resolution.refreshRate == Screen.resolutions[i].refreshRate ||
                     m_Resolution.refreshRate + 1 == Screen.resolutions[i].refreshRate))
                    m_ResolutionDropdown.value = i;
            }
        }

        private void Update()
        {
            m_FieldOfViewLabel.text = GameplayManager.Instance.FieldOfView.ToString("F0");
            m_AimModeLabel.text = GameplayManager.Instance.AimStyle.ToString();
            m_CrouchModeLabel.text = GameplayManager.Instance.CrouchStyle.ToString();
            m_SprintModeLabel.text = GameplayManager.Instance.SprintStyle.ToString();
            m_LeanModeLabel.text = GameplayManager.Instance.LeanStyle.ToString();
            m_MouseSensitivityLabel.text = GameplayManager.Instance.OverallMouseSensitivity.ToString("F1");

            m_MasterAudioLabel.text = (AudioListener.volume * 100).ToString("F0");
            m_SFxAudioLabel.text = (AudioManager.Instance.SFxVolume * 100).ToString("F0");
            m_MusicAudioLabel.text = (AudioManager.Instance.MusicVolume * 100).ToString("F0");
            m_VoiceAudioLabel.text = (AudioManager.Instance.VoiceVolume * 100).ToString("F0");

            m_UiScaleLabel.text = m_UiScale.ToString("F1");
            m_QualityLevelLabel.text = QualitySettings.names[m_QualityLevel];
            m_TextureQualityLabel.text = m_TextureQualities[m_TextureQuality];
            m_AnisoLevelLabel.text = m_AnisoLevel == AnisotropicFiltering.Enable ? "Disabled" : "Enabled";
            m_ShadowResolutionLabel.text = m_ShadowResolutions[(int) m_ShadowResolution];

            switch ((int)m_SkinWeights)
            {
                case 1:
                    m_MeshQualityLabel.text = m_MeshQualities[0];
                    break;
                case 2:
                    m_MeshQualityLabel.text = m_MeshQualities[1];
                    break;
                case 4:
                    m_MeshQualityLabel.text = m_MeshQualities[2];
                    break;
                case 255:
                    m_MeshQualityLabel.text = m_MeshQualities[3];
                    break;
            }
            
            m_VSyncLabel.text = m_VSyncOptions[m_VSyncCount];
            m_FullScreenModeLabel.text = m_FullScreenModes[(int) m_FullScreenMode];
        }
        
        public void SetFieldOfView(float fov)
        {
            GameplayManager.Instance.SetFOV(fov);
        }
        
        public void ChangeAimMode()
        {
            GameplayManager.Instance.ChangeAimMode();
        }
        
        public void ChangeCrouchMode()
        {
            GameplayManager.Instance.ChangeCrouchMode();
        }
        
        public void ChangeSprintMode()
        {
            GameplayManager.Instance.ChangeSprintMode();
        }
        
        public void ChangeLeanMode()
        {
            GameplayManager.Instance.ChangeLeanMode();
        }
        
        public void SetMouseSensitivity(float sensitivity)
        {
            GameplayManager.Instance.SetMouseSensitivity(sensitivity);
        }

        public void SetHorizontalAxis(bool invert)
        {
            GameplayManager.Instance.SetInvertHorizontalAxis(invert);
        }
        
        public void SetVerticalAxis(bool invert)
        {
            GameplayManager.Instance.SetInvertVerticalAxis(invert);
        }

        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = Mathf.Clamp01(volume);
        }
        
        public void SetSFxVolume(float volume)
        {
            AudioManager.Instance.SFxVolume = volume;
        }
        
        public void SetMusicVolume(float volume)
        {
            AudioManager.Instance.MusicVolume = volume;
        }
        
        public void SetVoiceVolume(float volume)
        {
            AudioManager.Instance.VoiceVolume = volume;
        }

        public void SetUiScale(float scale)
        {
            m_UiScale = scale;
        }

        public void DecreaseQualityLevel()
        {
            m_QualityLevel = Mathf.Clamp(m_QualityLevel - 1, 0, m_MaxQualityLevel);
        }
        
        public void IncreaseQualityLevel()
        {
            m_QualityLevel = Mathf.Clamp(m_QualityLevel + 1, 0, m_MaxQualityLevel);
        }
        
        public void DecreaseTextureLevel()
        {
            m_TextureQuality = Mathf.Clamp(m_TextureQuality - 1, 0, 3);
        }
        
        public void IncreaseTextureLevel()
        { 
            m_TextureQuality = Mathf.Clamp(m_TextureQuality + 1, 0, 3);
        }
        
        public void DecreaseAnisoLevel()
        {
            m_AnisoLevel = AnisotropicFiltering.Enable;
        }
        
        public void IncreaseAnisoLevel()
        { 
            m_AnisoLevel = AnisotropicFiltering.ForceEnable;
        }
        
        public void DecreaseShadowResolution()
        {
            m_ShadowResolution = (ShadowResolution)Mathf.Clamp((int)m_ShadowResolution - 1, 0, 3);
        }
        
        public void IncreaseShadowResolution()
        { 
            m_ShadowResolution = (ShadowResolution)Mathf.Clamp((int)m_ShadowResolution + 1, 0, 3);
        }
        
        public void DecreaseSkinWeights()
        {
            switch ((int)m_SkinWeights)
            {
                case 1:
                    m_SkinWeights = m_SkinWeights = SkinWeights.OneBone;
                    break;
                case 2:
                    m_SkinWeights = m_SkinWeights = SkinWeights.OneBone;
                    break;
                case 4:
                    m_SkinWeights = m_SkinWeights = SkinWeights.TwoBones;
                    break;
                case 255:
                    m_SkinWeights = m_SkinWeights = SkinWeights.FourBones;
                    break;
            }
        }
        
        public void IncreaseSkinWeights()
        { 
            switch ((int)m_SkinWeights)
            {
                case 1:
                    m_SkinWeights = m_SkinWeights = SkinWeights.TwoBones;
                    break;
                case 2:
                    m_SkinWeights = m_SkinWeights = SkinWeights.FourBones;
                    break;
                case 4:
                    m_SkinWeights = m_SkinWeights = SkinWeights.Unlimited;
                    break;
                case 255:
                    m_SkinWeights = m_SkinWeights = SkinWeights.Unlimited;
                    break;
            }
        }
        
        public void DecreaseVSyncLevel()
        {
            m_VSyncCount = Mathf.Clamp(m_VSyncCount - 1, 0, 2);
        }
        
        public void IncreaseVSyncLevel()
        { 
            m_VSyncCount = Mathf.Clamp(m_VSyncCount + 1, 0, 2);
        }
        
        public void DecreaseFullScreenMode()
        {
            m_FullScreenMode = (FullScreenMode)Mathf.Clamp((int)m_FullScreenMode - 1, 0, 3);
        }
        
        public void IncreaseFullScreenMode()
        { 
            m_FullScreenMode = (FullScreenMode)Mathf.Clamp((int)m_FullScreenMode + 1, 0, 3);
        }

        public void SetResolution(int index)
        {
            m_Resolution = Screen.resolutions[index];
        }

        public void ApplyVideoSettings()
        {
            if (Screen.currentResolution.width != m_Resolution.width ||
                Screen.currentResolution.height != m_Resolution.height || 
                Screen.currentResolution.refreshRate != m_Resolution.refreshRate ||
                Screen.fullScreenMode != m_FullScreenMode)
                Screen.SetResolution(m_Resolution.width, m_Resolution.height, m_FullScreenMode);
            
            foreach (CanvasScaler canvasScaler in m_CanvasScalers)
            {
                Vector2 minResolution = new Vector2(1280, 720);
                Vector2 maxResolution = new Vector2(2560, 1440);
                canvasScaler.referenceResolution = Vector2.Lerp(minResolution, maxResolution, 1 - m_UiScale / 2);
            }
            
            if (m_QualityLevel != m_LastQualityLevel)
            {
                QualitySettings.SetQualityLevel(m_QualityLevel, true);
                m_LastQualityLevel = m_QualityLevel;
                
                m_TextureQuality = QualitySettings.globalTextureMipmapLimit;
                m_AnisoLevel = QualitySettings.anisotropicFiltering;
                m_ShadowResolution = QualitySettings.shadowResolution;
                m_SkinWeights = QualitySettings.skinWeights;
                m_VSyncCount = QualitySettings.vSyncCount;
            }
            
            if (QualitySettings.globalTextureMipmapLimit != m_TextureQuality)
                QualitySettings.globalTextureMipmapLimit = m_TextureQuality;

            if (QualitySettings.anisotropicFiltering != m_AnisoLevel)
                QualitySettings.anisotropicFiltering = m_AnisoLevel;

            if (QualitySettings.shadowResolution != m_ShadowResolution)
                QualitySettings.shadowResolution = m_ShadowResolution;

            if (QualitySettings.skinWeights != m_SkinWeights)
                QualitySettings.skinWeights = m_SkinWeights;
            
            if (QualitySettings.vSyncCount != m_VSyncCount)
                QualitySettings.vSyncCount = m_VSyncCount;
        }
    }
}
