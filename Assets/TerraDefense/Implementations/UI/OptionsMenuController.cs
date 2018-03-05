using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TerraDefense.Implementations.UI
{

    public class OptionsMenuController : MonoBehaviour
    {
        public static string SelectedResolutionKey = "selectedResolution";
        public static string ResolutionXKey = "resolutionX";
        public static string ResolutionYKey = "resolutionY";
        public static string IsFullscreenKey = "isFullscreen";
        public static string AudioVolumeKey = "audioVolume";

        public GameObject MainMenuObject;
        public Dropdown ResolutionDropdown;
        public Toggle FullscreentToggle;
        public Slider AudioVolumeSlider;
        public AudioSource MusicSource;

        private int _defaultResolution = 0;
        private bool _defaultIsFullscreen = false;
        private float _defaultAudioValue = 0.5f;
        private bool _changesSaved;

        public void OnEnable()
        {
            if (PlayerPrefs.HasKey(SelectedResolutionKey))
            {
                _defaultResolution = PlayerPrefs.GetInt(SelectedResolutionKey);
            }

            if (PlayerPrefs.HasKey(IsFullscreenKey))
            {
                _defaultIsFullscreen = Convert.ToBoolean(PlayerPrefs.GetString(IsFullscreenKey));
            }

            if (PlayerPrefs.HasKey(AudioVolumeKey))
            {
                _defaultAudioValue = PlayerPrefs.GetFloat(AudioVolumeKey);
            }

            ResetControls();
            _changesSaved = false;
        }

        public void ReturnToMainMenuClicked()
        {
            if (!_changesSaved) ResetControls();
            MainMenuObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ResetControls()
        {
            ResolutionDropdown.value = _defaultResolution;
            FullscreentToggle.isOn = _defaultIsFullscreen;
            AudioVolumeSlider.value = _defaultAudioValue;
        }

        public void ApplyChanges()
        {
            var selected = ResolutionDropdown.options[ResolutionDropdown.value].text;
            var resolution = selected.Split('x');

            PlayerPrefs.SetInt(SelectedResolutionKey, ResolutionDropdown.value);

            var resolutionX = Convert.ToInt32(resolution[0]);
            PlayerPrefs.SetInt(ResolutionXKey, resolutionX);

            var resolutionY = Convert.ToInt32(resolution[1]);
            PlayerPrefs.SetInt(ResolutionYKey, resolutionY);

            var isFullscreen = FullscreentToggle.isOn;
            PlayerPrefs.SetString(IsFullscreenKey, isFullscreen.ToString());

            var audioVolume = AudioVolumeSlider.value;
            PlayerPrefs.SetFloat(AudioVolumeKey, audioVolume);

            Screen.SetResolution(resolutionX, resolutionY, isFullscreen);
            _changesSaved = true;
        }

        public void OnAudioSliderValueChange(Single value)
        {
            MusicSource.volume = AudioVolumeSlider.value;
        }
    }

}