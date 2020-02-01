using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstarctions.Factions;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.Utils;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class CountryEventsHandler : ICountryEventsHandler
    {
        private readonly Country _country;
        private List<BattleGroup> _battleGroups;

        public CountryEventsHandler(Country country)
        {
            _country = country;
            _battleGroups = new List<BattleGroup>();
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

        public void FortifyProvince(Province province, List<Unit> playerUnits)
        {
            var unitCount = Random.Range(0, 6);

            foreach (var playerUnit in playerUnits)
            {
                unitCount--;
                playerUnit.SetNewTarget(province.transform.position);
                if(unitCount <= 0)return;
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

        public void CheckBattleGroups(List<Unit> playerUnits)
        {
            var groupsToRemove = new List<BattleGroup>();
            foreach (var battleGroup in _battleGroups)
            {
                if (battleGroup.IsGroupReadyForAttack())
                {
                    battleGroup.CommenceAttack();
                }

                while (!battleGroup.IsGroupStrengthSufficient())
                {
                    var unit = playerUnits.First();
                    battleGroup.BattleGroupUnits.Add(unit);

                    foreach (var u in battleGroup.BattleGroupUnits)
                    {
                        u.SetNewTarget(battleGroup.RallyProvince.transform.position);
                    }
                }

                if (!battleGroup.TargetProvince.Owner.IsEnemy(playerUnits.First()))
                {
                    groupsToRemove.Add(battleGroup);
                }


            }
            foreach (var group in groupsToRemove)
            {
                _battleGroups.Remove(group);
            }
        }

        public void AttackProvince(Province lostProvince, List<Unit> playerUnits)
        {
            var battleGroup = _battleGroups.Find(x => x.TargetProvince = lostProvince);
            if (battleGroup != null)
            {
                if (battleGroup.IsGroupReadyForAttack())
                {
                    battleGroup.CommenceAttack();
                }

                while (!battleGroup.IsGroupStrengthSufficient())
                {
                    var unit = playerUnits.First();
                    battleGroup.BattleGroupUnits.Add(unit);
                    unit.SetNewTarget(battleGroup.RallyProvince.transform.position);
                }
                return;
            }
            battleGroup = new BattleGroup();
            battleGroup.TargetProvince = lostProvince;
            battleGroup.RallyProvince = GameController.FindProvincesNear(lostProvince).First();
            battleGroup.BattleGroupUnits = battleGroup.RallyProvince.AlliedUnits;

            while (!battleGroup.IsGroupStrengthSufficient())
            {
                var unit = playerUnits.First(x => !IsUnitInBattleGroup(x) && !battleGroup.BattleGroupUnits.Contains(x));
                battleGroup.BattleGroupUnits.Add(unit);
                unit.SetNewTarget(battleGroup.RallyProvince.transform.position);
            }
            _battleGroups.Add(battleGroup);
        }

        private bool IsUnitInBattleGroup(Unit unit)
        {
            foreach (var battleGroup in _battleGroups)
            {
                if (battleGroup.BattleGroupUnits.Contains(unit))
                {
                    return true;
                }
            }
            return false;
        }

        public void DonateWithUnits(Country orderSubject, List<Unit> playerUnits)
        {
            var maxUnitsToSend = playerUnits.Count / 2;
            var panicPercentage = _country.PanicLevel / _country.Alliance.AveragePanic;
            if (panicPercentage > 1) return;
            var numberofUnitsToSend = maxUnitsToSend * panicPercentage;

            foreach (var playerUnit in playerUnits)
            {
                playerUnit.ChangeOwner(orderSubject);
                playerUnit.SetNewTarget(UtilsAndTools.FindNearestProvince(orderSubject).transform.position);
                orderSubject.PanicLevel -= (int)(playerUnit.DefenceValue / 10);
                if (numberofUnitsToSend-- < 0) return;
            }
        }
    }
}