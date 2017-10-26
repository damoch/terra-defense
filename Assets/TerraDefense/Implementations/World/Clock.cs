using System;
using Assets.TerraDefense.Abstractions.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class Clock : MonoBehaviour
    {
        public DateTime GameDateTime { get; set; }
        public float LengthOfHour;
        
        private void Start () {
		    GameDateTime = new DateTime(2075,4,5,6,0,0);
            InvokeRepeating("HourEvent", LengthOfHour, LengthOfHour);
        }

        private void HourEvent()
        {
            GameDateTime = GameDateTime.AddHours(1);
            Debug.Log(GameDateTime);
            var target = new GameObject();
            var objectsWithTag = GameObject.FindGameObjectsWithTag("TimeAffected");
            for (var index = 0; index < objectsWithTag.Length; index++)
            {
                var affected = objectsWithTag[index].GetComponent<ITimeAffected>();
                try
                {
                    affected.HourEvent();
                }
                catch 
                {
                    //
                }
            }
        }
    }
}
