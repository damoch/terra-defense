using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Assets.TerraDefense.Implementations.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject NewGameOptions;
        public GameObject Options;
        public AudioSource MusicSource;
        public GameObject SaveLoadPanel;
        public bool IsMenuActive { get { return Time.timeScale == 1; }}

        public List<Texture2D> Backgrounds;
        private int _currentBackgroundIndex = 0;
        private List<GameObject> _itemsWithBackgrounds;

        private void Start()
        {
            _itemsWithBackgrounds = new List<GameObject>
            {
                NewGameOptions, Options, SaveLoadPanel, gameObject
            };
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

            if (SceneManager.GetActiveScene().name == "mainMenu")InvokeRepeating("SetNewBackground", 0, 10f);
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
            SaveLoadPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        public void QuitButtonClicked()
        {
            Application.Quit();
        }

        public void BringUpMainMenu()
        {
            if (!IsMenuActive)
            {
                gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }

        public void TurnOffMenu()
        {
            if (IsMenuActive)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }

        public void ExitToMenu()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            SceneManager.LoadScene("mainMenu");
        }

        private void SetNewBackground()
        {
            if (_currentBackgroundIndex > Backgrounds.Count - 1) _currentBackgroundIndex = 0;
            foreach(var item in _itemsWithBackgrounds)
            {
                var newBackground = Backgrounds[_currentBackgroundIndex];
                var comp = item.GetComponent<Image>();
                var oldPosition = comp.sprite.rect.position;
                var oldRect = new Rect(oldPosition, new Vector2(newBackground.width, newBackground.height));


                comp.sprite = Sprite.Create(Backgrounds[_currentBackgroundIndex], oldRect, comp.sprite.pivot);
            }
            _currentBackgroundIndex++;
        }
    }
}

