using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Province : MonoBehaviour
    {
        public float BattleDelay;
        public List<Unit> EnemyUnits { get; set; }
        public List<Unit> AlliedUnits { get; set; }
        public int DefenseValue { get; set; }
        public bool IsBattle { get; set; }
        public UnitOwner Owner;

        private void Start ()
        {
           
            IsBattle = false;
            EnemyUnits = new List<Unit>();
            AlliedUnits = new List<Unit>();
            DefenseValue = 0;

            #if UNITY_EDITOR
            if (BattleDelay == 0)
            {
                BattleDelay = 1f;
            }
            #endif
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitEnter(unitComponent);
            }
        }

        private void HandleUnitEnter(Unit unitComponent)
        {
            if(AlliedUnits.Contains(unitComponent) || EnemyUnits.Contains(unitComponent))return;

            if (Owner.IsEnemy(unitComponent))
            {
                EnemyUnits.Add(unitComponent);
                if (!IsBattle)
                {
                    StartCoroutine("CommenceBattle");
                }
            }
            else 
            {
                AlliedUnits.Add(unitComponent);
                DefenseValue += unitComponent.DefenceValue;
            }
        }

        public IEnumerator CommenceBattle()
        {
            IsBattle = true;
            yield return new WaitForSeconds(BattleDelay);
            SetSkirmishResult();

            if (IsBattle)
            {
                Debug.Log("Battle rages on");
                StartCoroutine("CommenceBattle");
            }


        }

        private void SetSkirmishResult()
        {
            var totalAttack = EnemyUnits.Sum(unit => unit.AttackValue);
            var totalDefense = AlliedUnits.Sum(unit => unit.DefenceValue);
            var damageValue = Math.Abs(totalDefense - totalAttack);

            var losingArmy = totalAttack > totalDefense ? AlliedUnits : EnemyUnits;
            var winningArmy = totalAttack <= totalDefense ? AlliedUnits : EnemyUnits;

            for (var index = 0; index < losingArmy.Count; index++)
            {
                var unit = losingArmy[index];
                unit.AddDamage(damageValue);
            }

            if (losingArmy.Count == 0)
            {
                Debug.Log("Battle is over");
                Owner = winningArmy[0].Owner;
                AlliedUnits = winningArmy;
                EnemyUnits = new List<Unit>();
                IsBattle = false;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitExit(unitComponent);
            }
        }

        private void HandleUnitExit(Unit unitComponent)
        {
            if (Owner.IsEnemy(unitComponent))
            {
                EnemyUnits.Remove(unitComponent);
            }
            else
            {
                AlliedUnits.Remove(unitComponent);
                DefenseValue -= unitComponent.DefenceValue;
            }
        }
    }
}
