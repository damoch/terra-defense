﻿using System;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Clock : MonoBehaviour
    {
        public DateTime GameDateTime { get; set; }
        public float LengthOfHour;

        // Use this for initialization
        private void Start () {
		    GameDateTime = new DateTime(2075,4,5,6,0,0);
            InvokeRepeating("HourEvent",LengthOfHour,LengthOfHour);
        }

        private void HourEvent()
        {
            GameDateTime = GameDateTime.AddHours(1);
            Debug.Log(GameDateTime);

            var objectsWithTag = GameObject.FindGameObjectsWithTag("TimeAffected");
            Debug.Log("sasasas");
            foreach (var affected in objectsWithTag)
            {
                affected.GetComponent<ITimeAffected>().HourEvent();
            }
        }




    }
}
