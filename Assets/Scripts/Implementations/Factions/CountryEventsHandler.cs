using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Implementations.Utils;
using Assets.Scripts.Implementations.World;
using UnityEngine;

namespace Assets.Scripts.Implementations.Factions
{
    public class CountryEventsHandler
    {
        private readonly Country _country;

        public CountryEventsHandler(Country country)
        {
            _country = country;
        }

        public void HandleProvinceWithEnemiesNear(Province provinceWithEnemiesNear)
        {
            var supporter = UtilsAndTools.FindNearestProvince(provinceWithEnemiesNear, _country);
            foreach (var supporterAlliedUnit in supporter.AlliedUnits)
            {
                supporterAlliedUnit.SetNewTarget(provinceWithEnemiesNear.transform.position);
            }
            var unit = _country.ProduceUnit(provinceWithEnemiesNear.transform.position);

            if (provinceWithEnemiesNear.Owner != _country)
            {
                unit.transform.position = UtilsAndTools.FindNearestProvince(provinceWithEnemiesNear, _country).transform.position;
            }
        }

        public void HandleProvinceUnderAttack(Province provinceUnderAttack, List<Unit> playerUnits)
        {
            var attackStrength = provinceUnderAttack.EnemyUnits.Sum(a => a.AttackValue);
            if (provinceUnderAttack.DefenseValue <= attackStrength &&
                playerUnits.Sum(a => a.DefenceValue) < attackStrength)
            {
                var retreatProvince = UtilsAndTools.FindNearestProvince(provinceUnderAttack, _country);
                foreach (var alliedUnit in provinceUnderAttack.AlliedUnits)
                {
                    alliedUnit.SetNewTarget(retreatProvince.transform.position);
                }
                var unit = _country.ProduceUnit(retreatProvince.transform.position);

                if (retreatProvince.Owner != _country)
                {
                    unit.transform.position = UtilsAndTools.FindNearestProvince(retreatProvince, _country).transform.position;
                }
            }
            var avgDist = UtilsAndTools.FindAverageDistance(provinceUnderAttack, playerUnits);
            var hitColliders = Physics2D.OverlapCircleAll(provinceUnderAttack.transform.position, avgDist);

            foreach (var hitCollider in hitColliders)
            {
                try
                {
                    var unit = hitCollider.gameObject.GetComponent<Unit>();
                    if (!_country.IsEnemy(unit))
                        unit.SetNewTarget(provinceUnderAttack.transform.position);
                }
                catch
                {
                    //
                }
            }
        }

        public void HandleLostProvince(Province lostProvince, List<Unit> playerUnits)
        {
            var occupationStrength = lostProvince.DefenseValue;
            
            Debug.Log("Alies" + playerUnits.Sum(a => a.AttackValue));
            Debug.Log("Enemy " + occupationStrength);

            if (playerUnits.Sum(a => a.AttackValue) <= occupationStrength+1)
            {
                var retreatProvince = UtilsAndTools.FindNearestProvince(lostProvince, _country);
                if(retreatProvince != null)
                    _country.ProduceUnit(retreatProvince.transform.position);
                return;
            }
            var attackStrength = 0f;

            foreach (var alliedUnit in playerUnits)
            {
                alliedUnit.SetNewTarget(lostProvince.transform.position);
                attackStrength += alliedUnit.AttackValue;

                if (attackStrength > occupationStrength) break;
            }
        }
    }
}