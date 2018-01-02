using System;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject NewGameOptions;
        public GameObject Options;
        public AudioSource MusicSource;

        private void Start()
        {
            int resolutionX = 800, resolutionY = 600;//lowest resolution
            var isFullscreen = false;
            if (PlayerPrefs.HasKey(OptionsMenuController.ResolutionXKey))
            {
                resolutionX = PlayerPrefs.GetInt(OptionsMenuController.ResolutionXKey);
            }

            if (PlayerPrefs.HasKey(OptionsMenuController.ResolutionYKey))
            {
                resolutionY = PlayerPrefs.GetInt(OptionsMenuController.ResolutionYKey);
            }

            if (PlayerPrefs.HasKey(OptionsMenuController.IsFullscreenKey))
            {
                isFullscreen = Convert.ToBoolean(PlayerPrefs.GetString(OptionsMenuController.IsFullscreenKey));
            }

            if (PlayerPrefs.HasKey(OptionsMenuController.AudioVolumeKey))
            {
                MusicSource.volume = PlayerPrefs.GetFloat(OptionsMenuController.AudioVolumeKey);
            }

            Screen.SetResolution(resolutionX, resolutionY, isFullscreen);
        }

        public void NewGameButtonClicked()
        {
            NewGameOptions.SetActive(true);
            gameObject.SetActive(false);
        }
        public void OptionsButtonClicked()
        {
            Options.SetActive(true);
            gameObject.SetActive(false);
        }
        public void LoadGameButtonClicked()
        {

        }
        public void QuitButtonClicked()
        {

        }
    }
}

