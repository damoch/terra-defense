using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.Utils;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Players
{
    // ReSharper disable once InconsistentNaming
    public class AIPlayer : MonoBehaviour {
        public Aliens Aliens { get; set; }
        private List<Province> _provinces;
        private Dictionary<PlatformUnit, Province> _orders;
        private void Start ()
        {
            _orders = new Dictionary<PlatformUnit, Province>();
            Aliens = FindObjectOfType<Aliens>();
            _provinces = FindObjectsOfType<Province>().ToList();


            InvokeRepeating("MakeNextMove",1f,1f);
        }

        private void MakeNextMove()
        {
            #region AI
            var units = Aliens.GetPlayerControllableUnits();
            var platforms = new List<PlatformUnit>();
            foreach (var unit in units)
            {
                platforms.Add((PlatformUnit)unit);
            }
            
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

        private void FindTargetFor(PlatformUnit platformUnit)
        {
            var alreadyAttacked = _orders.Values.ToList();
            var validTargets = _provinces.Where(p => !p.Owner.Equals(Aliens) && !alreadyAttacked.Contains(p)).ToList();
            
            if(validTargets.Count == 0)return;

            var currentTarget = UtilsAndTools.FindNearestProvince(platformUnit, validTargets);
            platformUnit.TargetProvince = currentTarget;
            _orders[platformUnit] = currentTarget;
        }
        #endregion
      
    }
}
