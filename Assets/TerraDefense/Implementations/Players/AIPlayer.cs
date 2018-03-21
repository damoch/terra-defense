using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.Utils;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Players
{
    // ReSharper disable once InconsistentNaming
    public class AIPlayer : MonoBehaviour, ISaveLoad {
        public Aliens Aliens { get; set; }

        public int Priority => 0;

        private List<Province> _provinces;
        private Dictionary<PlatformUnit, Province> _orders;
        private void Start ()
        {
            _orders = new Dictionary<PlatformUnit, Province>();
            Aliens = FindObjectOfType<Aliens>();
            _provinces = FindObjectsOfType<Province>().ToList();
            InvokeRepeating("MakeNextMove", 5f,1f);
        }

        private void MakeNextMove()
        {
            #region AI
            if(Aliens.UnitsInReserve.Count > 0) Aliens.UnitsInReserve?.RemoveAll(x => x == null || !x.gameObject.activeInHierarchy);
            var units = Aliens.GetPlayerControllableUnits();
            if (units.Count == 0) {
                FinalAttack();
                Aliens.CallingAnotherAliensWave = true;
                return;
            }

            Aliens.CallingAnotherAliensWave = false;
            var platforms = new List<PlatformUnit>();
            foreach (var unit in units)
            {
                platforms.Add((PlatformUnit)unit);
            }

            FixOrders(platforms);
            foreach (var platformUnit in platforms)
            {
                if (!_orders.ContainsKey(platformUnit)) _orders.Add(platformUnit, null);

                var target = _orders[platformUnit];
                if (target == null || target.Owner.Equals(Aliens))
                {
                    FindTargetFor(platformUnit);
                }
            }
        }

        private void FinalAttack()
        {
            var units = FindObjectsOfType<Unit>().Where(x => x.Owner == Aliens);
            if (units.Count() == 0) return;
            var targets = GetValidTargets();
            foreach (var unit in units)
            {
                unit.SetNewTarget(targets[UnityEngine.Random.Range(0, targets.Count)].transform.position);
            }
        }

        private void FixOrders(List<PlatformUnit> platforms)
        {

            for (var i = 0; i < _orders.Count(); i++)
            {
                var ordersKey = _orders.Keys.ToList()[i];
                if (!platforms.Contains(ordersKey) || !ordersKey.CurrentProvince)


                    try
                    {
                        _orders.Remove(ordersKey);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                        //
                    }
            }
        }

        private void FindTargetFor(PlatformUnit platformUnit)
        {
            var validTargets = GetValidTargets();

            if (validTargets.Count == 0) return;

            var currentTarget = UtilsAndTools.FindNearestProvince(platformUnit, validTargets);
            platformUnit.TargetProvince = currentTarget;
            _orders[platformUnit] = currentTarget;
        }

        private List<Province> GetValidTargets()
        {
            var alreadyAttacked = _orders.Values.ToList();
            var validTargets = _provinces.Where(p => !p.Owner.Equals(Aliens) && !alreadyAttacked.Contains(p)).ToList();
            return validTargets;
        }

        public Dictionary<string, string> GetSavableData()
        {
            return new Dictionary<string, string>
            {
                { "name", gameObject.name },
            };
        }

        public void SetSavableData(Dictionary<string, string> json)
        {
        }
        #endregion

    }
}
