using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Factions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.World
{
    public class Province : MonoBehaviour, ITimeAffected
    {
        public float BattleDelay;
        public float AlertDelay { get; set; }
        public List<Unit> EnemyUnits { get; set; }
        public List<Unit> AlliedUnits { get; set; }
        public int DefenseValue { get; set; }
        public bool IsBattle { get; set; }
        public UnitOwner Owner;
        public int CreditsPerHour;
        private void Start ()
        {
            SetupTimeValues();
           
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

            InvokeRepeating("CheckSurrondings", AlertDelay, AlertDelay);
        }

        private void CheckSurrondings()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, 100);
            var units = new List<Unit>();

            foreach (var hitCollider in hitColliders)
            {
                var unit = hitCollider.gameObject.GetComponent<Unit>();
                if (unit != null && Owner.IsEnemy(unit))
                {
                    Owner.EnemyIsCloseToProperty(gameObject);
                    return;
                }
            }
            
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
                Owner.EnemyIsAttackingProperty(gameObject);
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

            for (var i = 0; i < losingArmy.Count; i++)
            {
                var unit = losingArmy[i];
                unit.ModifyStatus(-damageValue);
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

        public Vector2 GetRandomPosition()
        {
            var center = transform.position;
            var size = GetComponent<SpriteRenderer>().sprite.bounds.size;
            var x = Random.Range(center.x - size.x / 2, center.x + size.y / 2);
            var y = Random.Range(center.y - size.y / 2, center.y - size.y / 2);
            return new Vector2(x,y);
        }

        public void HourEvent()
        {
            Owner.Credits += CreditsPerHour;
        }

        public void SetupTimeValues()
        {
            AlertDelay = FindObjectOfType<Clock>().LengthOfHour / 4;
        }
    }
}
