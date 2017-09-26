﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Implementations.Factions;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Implementations.World;
using Assets.Scripts.Implementations.Utils;
using UnityEngine;

namespace Assets.Scripts.Implementations.Players
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
            #region AI
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

            var currentTarget = UtilsAndTools.FindNearestProvince(platformUnit, validTargets);
            platformUnit.TargetProvince = currentTarget;

            var attackPosition = UtilsAndTools.FindNearestProvince(platformUnit, Aliens);
            if(attackPosition != null)
                platformUnit.SetNewTarget(attackPosition.transform.position);

        }

        #endregion
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