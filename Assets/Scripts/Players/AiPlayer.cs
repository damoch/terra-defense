using System.Collections.Generic;
using Assets.Scripts.Fractions;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Players
{
    public class AiPlayer : MonoBehaviour {
        public Aliens Aliens { get; set; }
        private void Start ()
        {
            Aliens = FindObjectOfType<Aliens>();
            Debug.Log(Aliens);
            InvokeRepeating("MakeNextMove",1f,1f);
        }

        private void MakeNextMove()
        {
            Debug.Log("Deciding move");
            var units = Aliens.GetPlayerControllableUnits();
            //Not sure if that is necesary...
            var platforms = new List<PlatformUnit>();
            foreach (var unit in units)
            {
                platforms.Add((PlatformUnit)unit);
            }
            Debug.Log(platforms);
        }

    }
}
