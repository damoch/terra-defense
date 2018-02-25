using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.TerraDefense.Implementations.UI
{
    public class SaveLoadUIController : MonoBehaviour
    {
        public SaveLoadManager SaveLoadManager;
        public RectTransform ScrollViewContent;
        public Button OriginalObj;
        public InputField SaveName;
        private void Awake()
        {
            OnActivation();
        }
        public void OnActivation()
        {
            var filenames = SaveLoadManager.GetAllSaveNames();
            var itemsCount = ScrollViewContent.childCount;
            var enumerator = 0;
            foreach(var fileName in filenames)
            {
                Button button;
                if (itemsCount > 0) button = ScrollViewContent.GetChild(enumerator++).GetComponent<Button>();
                else  button = Instantiate(OriginalObj);
                button.name = fileName;
                button.GetComponentInChildren<Text>().text = fileName;
                button.onClick.AddListener(delegate
                {
                    OnLoadButtonClicked(fileName);
                }
                );
                if(button.transform.parent == null)button.transform.SetParent(ScrollViewContent, false);
                button.gameObject.SetActive(true);
                itemsCount--;
            }
            if (itemsCount <= 0) return;
            for (var i = enumerator; i < ScrollViewContent.childCount; i++)
            {
                 ScrollViewContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnLoadButtonClicked(string filename)
        {                    
            PlayerPrefs.SetString(SaveLoadManager.LoadGameNameKey, filename);
            PlayerPrefs.SetString("StartInstruction", StartInstruction.LoadGame.ToString());
            SceneManager.LoadScene("generated");
        }

        public void OnSaveGameClicked()
        {
            SaveLoadManager.SaveGame(SaveName.text);
            OnActivation();
        }
    }
}
