using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.Utils;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class BattleHandler
    {
        private readonly Province _province;
        public UnitOwner CurrentWinner { get; set; }

        public BattleHandler(Province province)
        {
            _province = province;
        }

        public IEnumerator SetSkirmishResult(List<Unit> alliedUnits, List<Unit> enemyUnits)
        {
            var totalAttack = enemyUnits.Sum(unit => unit.AttackValue);
            var totalDefense = alliedUnits.Sum(unit => unit.DefenceValue);
            yield return null;

            var damageValue = Math.Abs(totalDefense - totalAttack);
            var losingArmy = totalAttack > totalDefense ? alliedUnits : enemyUnits;
            var winningArmy = totalAttack <= totalDefense ? alliedUnits : enemyUnits;
            yield return null;
            var totalAirAttack = enemyUnits.Sum(unit => unit.AirAttackValue);
            var totalAirDefense = alliedUnits.Sum(unit => unit.AirAttackValue);

            var airDamageValue = Math.Abs(totalAirDefense - totalAirAttack);

            for (var i = 0; i < losingArmy.Count; i++)
            {
                try
                {
                    var unit = losingArmy[i];
                    unit.ModifyStatus(unit.UnitType == UnitType.Ground ? -damageValue : -airDamageValue);

                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                yield return null;
            }
#if UNITY_EDITOR
            losingArmy.RemoveAll(x => x.Status <= 0);
#endif
            if (losingArmy.Count > 0)
            {
                var counterValue = losingArmy.Sum(x => x.AttackValue);
                var counterAirValue = losingArmy.Sum(x => x.AirAttackValue);

                for (var i = 0; i < winningArmy.Count; i++)
                {
                    try
                    {
                        var unit = winningArmy[i];
                        unit.ModifyStatus(unit.UnitType == UnitType.Ground ? -counterValue : -counterAirValue);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                    yield return null;
                }

#if UNITY_EDITOR
                winningArmy.RemoveAll(x => x.Status <= 0);
#endif
            }


            if (losingArmy.Count != 0)
            {
                CurrentWinner = null;
            }
            else
            {
                CurrentWinner = winningArmy.Count > 0 ? winningArmy[0].Owner : null;
                yield return CurrentWinner;
            }
        }

    }
}
