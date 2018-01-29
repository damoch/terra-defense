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
            FinishHourTasks(objectsWithTag);

        }

        private void FinishHourTasks(GameObject[] tasks)
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
            }
        }

        public Dictionary<string, string> GetSavableData()
        {
            var result = new Dictionary<string, string>
            {
                { "lengthOfHour", LengthOfHour.ToString() },
                { "gameDate", GameDateTime.ToString() },
                { "name", gameObject.name },
                { "type", GetType().FullName }
            };

            return result;
        }

        public void SetSavableData(Dictionary<string, string> json)
        {
            GameDateTime = DateTime.Parse(json["gameDate"]);
            LengthOfHour = float.Parse(json["lengthOfHour"]);
            InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);

        }
    }
}
