using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Units;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class BattleHandler
    {
        public UnitOwner CurrentWinner { get; set; }

        public BattleHandler()
        {
        }

        public IEnumerator SetSkirmishResult(List<Unit> alliedUnits, List<Unit> enemyUnits)
        {
            var totalAttack = enemyUnits.Sum(unit => unit.AttackValue);
            var totalDefense = alliedUnits.Sum(unit => unit.DefenceValue);
            yield return null;

            var damageValue = Math.Abs(totalDefense - totalAttack);

            List<Unit> losingArmy;
            List<Unit> winningArmy;
            var attackersWon = totalAttack > totalDefense;
            if (attackersWon)
            {
                winningArmy = enemyUnits;
                losingArmy = alliedUnits;
            }
            else
            {
                winningArmy = alliedUnits;
                losingArmy = enemyUnits;
            }

            damageValue += winningArmy.Average(x => x.AttackValue);
            if (winningArmy.Count > losingArmy.Count) damageValue *= 1.1f;

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

            losingArmy.RemoveAll(x => !x.gameObject.activeInHierarchy);

            if (losingArmy.Count > 0)
            {
                float counterValue;
                if (attackersWon)
                    counterValue = losingArmy.Average(x => x.DefenceValue);
                else
                    counterValue = losingArmy.Average(x => x.AttackValue);


                if (losingArmy.Count >= winningArmy.Count) counterValue *= 1.15f;
                var counterAirValue = losingArmy.Average(x => x.AirAttackValue);

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


                winningArmy.RemoveAll(x => !x.gameObject.activeInHierarchy);

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

