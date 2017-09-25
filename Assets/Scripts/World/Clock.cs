﻿using System;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Clock : MonoBehaviour
    {
        public DateTime GameDateTime { get; set; }
        public float LengthOfHour;
        
        private void Start () {
		    GameDateTime = new DateTime(2075,4,5,6,0,0);
            InvokeRepeating("HourEvent",LengthOfHour,LengthOfHour);
        }

        private void HourEvent()
        {
            GameDateTime = GameDateTime.AddHours(1);
            Debug.Log(GameDateTime);
            
            var objectsWithTag = GameObject.FindGameObjectsWithTag("TimeAffected");
            for (var index = 0; index < objectsWithTag.Length; index++)
            {
                try
                {
                    var affected = objectsWithTag[index];
                    affected.GetComponent<ITimeAffected>().HourEvent();
                }
                catch
                {
                    //Obiekt nie istnieje
                }
            }
        }
    }
}
