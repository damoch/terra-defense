using System;
using System.Collections;
using System.Collections.Generic;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class Clock : MonoBehaviour, ISaveLoad
    {
        public DateTime GameDateTime { get; set; }

        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public float LengthOfHour;
        
        private void Start () {
		    GameDateTime = new DateTime(2075,4,5,6,0,0);
            if(LengthOfHour > 0)
                InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);
        }

        private void HourEvent()
        {
            GameDateTime = GameDateTime.AddHours(1);
            Debug.Log(GameDateTime);
            var objectsWithTag = GameObject.FindGameObjectsWithTag("TimeAffected");
            StartCoroutine(FinishHourTasks(objectsWithTag));

        }

        private IEnumerator FinishHourTasks(GameObject[] tasks)
        {
            for (var index = 0; index < tasks.Length; index++)
            {
                var affected = tasks[index].GetComponent<ITimeAffected>();
                try
                {
                    affected.HourEvent();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                yield return null;
            }
        }

        public Dictionary<string, object> GetSavableData()
        {
            var result = new Dictionary<string, object>();

            result.Add("lengthOfHour", LengthOfHour.ToString());
            result.Add("gameDate", GameDateTime.ToString());
            result.Add("name", gameObject.name);
            result.Add("type", GetType().FullName);

            return result;
        }

        public void SetSavableData(Dictionary<string, object> json)
        {
            GameDateTime = DateTime.Parse((string)json["gameDate"]);
            LengthOfHour = float.Parse((string)json["lengthOfHour"]);
            InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);

        }
    }
}
