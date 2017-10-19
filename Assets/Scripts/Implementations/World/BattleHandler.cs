using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Abstractions.Factions;
using Assets.Scripts.Enums;
using Assets.Scripts.Implementations.Units;
using UnityEngine;

namespace Assets.Scripts.Implementations.World
{
    internal class BattleHandler
    {
        public UnitOwner SetSkirmishResult(List<Unit> alliedUnits, List<Unit> enemyUnits)
        {
            var totalAttack = enemyUnits.Sum(unit => unit.AttackValue);
            var totalDefense = alliedUnits.Sum(unit => unit.DefenceValue);

            var damageValue = Math.Abs(totalDefense - totalAttack);
            var losingArmy = totalAttack > totalDefense ? alliedUnits : enemyUnits;
            var winningArmy = totalAttack <= totalDefense ? alliedUnits : enemyUnits;

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
                    Debug.Log(e.Message);
                }
            }
            damageValue *= 0.33f;

            for (var i = 0; i < winningArmy.Count; i++)
            {
                try
                {
                    var unit = winningArmy[i];
                    unit.ModifyStatus(-damageValue);
                }
                catch
                {
                    //
                }
            }


            if (losingArmy.Count != 0) return null;
            return  winningArmy.Count > 0 ? winningArmy[0].Owner : null;
        }
    }
}
