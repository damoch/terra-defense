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
        private float _currentTime;

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
            _currentTime = 0;
            //if(LengthOfHour > 0)
            //    InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if(_currentTime >= LengthOfHour)
            {
                HourEvent();
                _currentTime = 0;
            }
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
                { "currentTime", _currentTime.ToString() },
                { "lengthOfHour", LengthOfHour.ToString() },
                { "gameDate", GameDateTime.ToString() },
                { "name", gameObject.name },
            };

            return result;
        }

        public void SetSavableData(Dictionary<string, string> json)
        {
            _currentTime = (json.ContainsKey("currentTime") ? float.Parse(json["currentTime"]) : 0f);
            GameDateTime = DateTime.Parse(json["gameDate"]);
            LengthOfHour = float.Parse(json["lengthOfHour"]);
            InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);

        }
    }
}
