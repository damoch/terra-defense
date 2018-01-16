using Assets.TerraDefense.Abstractions.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Assets.TerraDefense.Enums;

namespace Assets.TerraDefense.Implementations.IO
{
    public class SaveLoadManager : MonoBehaviour
    {
        public string AutoSaveKey;
        public void SaveGame()
        {
            var allObjects = FindObjectsOfType(typeof(MonoBehaviour));
            var resultsList = new Dictionary<int, List<Dictionary<string, object>>>();
            foreach(var obj in allObjects)
            {
                var monoBehaviour = (MonoBehaviour)obj;
                var saveLoad = monoBehaviour.GetComponent<ISaveLoad>();
                if (saveLoad != null)
                {
                    var priority = saveLoad.Priority;
                    if (!resultsList.ContainsKey(priority))
                        resultsList.Add(priority, new List<Dictionary<string, object>>());

                    resultsList[priority].Add(saveLoad.GetSavableData());
                }
            }
            var saveData = JsonConvert.SerializeObject(resultsList);
            Debug.Log(resultsList.Count);
            PlayerPrefs.SetString("autosave", saveData);
        }


        public void LoadGame()
        {
            var gObjects = new List<GameObject>();
            var json = PlayerPrefs.GetString("autosave");

            var list = JsonConvert.DeserializeObject<Dictionary<int, List<Dictionary<string, object>>>>(json);
            var sortedPriorities = list.Keys.ToList();
            sortedPriorities.Sort();

            foreach(var priority in sortedPriorities)
            {
                var group = list[priority];
                foreach(var saved in group)
                {
                    var gObject = new GameObject();
                    gObject.SetActive(false);
                    gObject.name = (string)saved["name"];
                    var typeName = Type.GetType((string)saved["type"]);
                    var behaviour = gObject.AddComponent(typeName);
                    behaviour.GetComponent<ISaveLoad>().SetSavableData(saved);
                    gObjects.Add(gObject);
                }
            }


            gObjects.ForEach(x => x.SetActive(true));
            PlayerPrefs.SetString("StartInstruction", StartInstruction.NewGame.ToString());
        }
    }



}
