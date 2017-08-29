using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Factions;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Players
{
    // ReSharper disable once InconsistentNaming
    public class AIPlayer : MonoBehaviour {
        public Aliens Aliens { get; set; }
        public List<Province> Provinces { get; set; }
        private void Start ()
        {
            Aliens = FindObjectOfType<Aliens>();
            Provinces = FindObjectsOfType<Province>().ToList();
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
            foreach (var platformUnit in platforms)
            {
                if (platformUnit.TargetProvince == null || platformUnit.TargetProvince.Owner.Equals(Aliens))
                {
                    FindTargetFor(platformUnit);
                }
            }
        }

        private void FindTargetFor(PlatformUnit platformUnit)
        {
            var validTargets = Provinces.Where(p => !p.Owner.Equals(Aliens)).ToList();
            if(validTargets.Count == 0)return;

            var currentTarget = FindClosestProvince(validTargets, platformUnit);
            platformUnit.TargetProvince = currentTarget;

            var attackPositions = Provinces.Where(p => p.Owner.Equals(Aliens)).ToList();
            if (attackPositions.Count == 0) return;

            platformUnit.SetNewTarget(FindClosestProvince(attackPositions, platformUnit).transform.position);

        }

        private Province FindClosestProvince(List<Province> validTargets, PlatformUnit platformUnit)
        {
            var currentTarget = validTargets[0];
            var currentDist = Vector2.Distance(platformUnit.transform.position, currentTarget.transform.position);
            foreach (var targetOption in validTargets)
            {
                if (!(Vector2.Distance(transform.position, targetOption.transform.position) < currentDist)) continue;
                currentDist = Vector2.Distance(platformUnit.transform.position, targetOption.transform.position);
                currentTarget = targetOption;
            }
            return currentTarget;
        }
    }
}
