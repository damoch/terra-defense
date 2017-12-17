using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject NewGameOptions;
        public void NewGameButtonClicked()
        {
            NewGameOptions.SetActive(true);
            gameObject.SetActive(false);
        }
        public void OptionsButtonClicked()
        {

        }
        public void LoadGameButtonClicked()
        {

        }
        public void QuitButtonClicked()
        {

        }
    }
}

