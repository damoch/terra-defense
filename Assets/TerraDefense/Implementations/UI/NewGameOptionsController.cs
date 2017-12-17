using Assets.TerraDefense.Implementations.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.TerraDefense.Implementations.UI
{
    public class NewGameOptionsController : MonoBehaviour
    {
        public InputField NumberOfInvadersInput;
        public string NewGameSceneName;

        public void StartGame()
        {
            CollectDataFromInputs();
            SceneManager.LoadScene(NewGameSceneName);
        }

        private void CollectDataFromInputs()
        {
            NewGameData.NumberOfInvaders = Int32.Parse(NumberOfInvadersInput.text);
           
        }
    }
}

