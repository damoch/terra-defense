using Assets.TerraDefense.Abstractions.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.IO
{
    public class SaveLoadManager : MonoBehaviour
    {
        public void SaveGame()
        {
            var allObjects = FindObjectsOfType(typeof(MonoBehaviour));

            foreach(var obj in allObjects)
            {
                var monoBehaviour = (MonoBehaviour)obj;
                var saveLoad = monoBehaviour.GetComponent<ISaveLoad>();
                if (saveLoad != null) Debug.Log(saveLoad.GetSavableData());
            }
        }
    }
}
