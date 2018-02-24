using Assets.TerraDefense.Abstractions.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.World;
using System.IO;

namespace Assets.TerraDefense.Implementations.IO
{
    public class SaveLoadManager : MonoBehaviour
    {
        public string AutoSaveKey;
        public List<GameObject> LoadableObjects;
        public string FileExtension;
        private Component GetLoadableObject(Type type)
        {
            var result = LoadableObjects.FirstOrDefault(x => x.GetComponent(type) != null);
            return result != null ? Instantiate(result).GetComponent(type) : new GameObject().AddComponent(type);
        }

        public void SaveGame()
        {
            var allObjects = FindObjectsOfType(typeof(MonoBehaviour));
            var resultsList = new Dictionary<int, List<Dictionary<string, string>>>();
            foreach(var obj in allObjects)
            {
                var monoBehaviour = (MonoBehaviour)obj;
                var saveLoad = monoBehaviour.GetComponent<ISaveLoad>();
                if (saveLoad != null)
                {
                    var priority = saveLoad.Priority;
                    if (!resultsList.ContainsKey(priority))
                        resultsList.Add(priority, new List<Dictionary<string, string>>());

                    resultsList[priority].Add(saveLoad.GetSavableData());
                }
            }
            var saveData = JsonConvert.SerializeObject(resultsList);

            Debug.Log(resultsList.Count);
            //PlayerPrefs.SetString(AutoSaveKey, saveData);
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + Application.companyName + Path.DirectorySeparatorChar + Application.productName;

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            savePath += Path.DirectorySeparatorChar + AutoSaveKey + FileExtension;
            File.WriteAllText(savePath, saveData);

#if DEBUG || UNITY_EDITOR
            File.WriteAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + AutoSaveKey + ".txt", saveData);
            Debug.Log("File saved to " + System.Reflection.Assembly.GetExecutingAssembly().Location + AutoSaveKey + ".txt");
#endif
        }


        public void LoadGame()
        {
            var loadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + Application.companyName + Path.DirectorySeparatorChar + Application.productName + Path.DirectorySeparatorChar + AutoSaveKey + FileExtension;
            if (!File.Exists(loadPath)) return;
            var gObjects = new List<GameObject>();
            var json = File.ReadAllText(loadPath);

            var list = JsonConvert.DeserializeObject<Dictionary<int, List<Dictionary<string, string>>>>(json);
            var sortedPriorities = list.Keys.ToList();
            sortedPriorities.Sort();

            foreach(var priority in sortedPriorities)
            {
                var group = list[priority];
                foreach(var saved in group)
                {
                    var typeName = Type.GetType(saved["type"]);
                    var gObject = GetLoadableObject(typeName).gameObject;
                    gObject.SetActive(false);
                    gObject.name = saved["name"];
                    gObject.GetComponent<ISaveLoad>().SetSavableData(saved);
                    gObjects.Add(gObject);
                }
            }


            foreach (var item in gObjects)
            {
                item.SetActive(true);
                if (item.GetComponent<Province>())
                {
                    item.GetComponent<Province>().enabled = true;
                }
            }
            PlayerPrefs.SetString("StartInstruction", StartInstruction.NewGame.ToString());
        }
    }



}
