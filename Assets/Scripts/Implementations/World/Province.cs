using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abstractions.Factions;
using Assets.Scripts.Implementations.Factions;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Interfaces.World;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Implementations.World
{
    public class Province : MonoBehaviour, ITimeAffected
    {
        public float BattleDelay;
        public float AlertDelay { get; set; }
        public List<Unit> EnemyUnits { get; set; }
        public List<Unit> AlliedUnits { get; set; }

        public float DefenseValue
        {
            get
            {
                return AlliedUnits != null ? AlliedUnits.Sum(a => a.DefenceValue) : 0;
            }
        }

        public bool IsBattle { get; set; }
        public UnitOwner Owner;
        private UnitOwner _originalOwner;
        public int CreditsPerHour;
        private SpriteRenderer _spriteRenderer;
        private void Start ()
        {
            SetupTimeValues();
           
            IsBattle = false;
            EnemyUnits = new List<Unit>();
            AlliedUnits = new List<Unit>();
            #if UNITY_EDITOR
            if (BattleDelay == 0)
            {
                BattleDelay = 1f;
            }
            #endif

            _originalOwner = Owner;
            InvokeRepeating("CheckSurrondings", AlertDelay, AlertDelay);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = Owner.Color;
        }

        private void CheckSurrondings()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, 100);

            foreach (var hitCollider in hitColliders)
            {
                var unit = hitCollider.gameObject.GetComponent<Unit>();
                try
                {
                    if (unit != null && Owner.IsEnemy(unit))
                    {
                        Owner.EnemyIsCloseToProperty(gameObject);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
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

            if (unitComponent.GetType() == typeof(PlatformUnit))
            {
                var unit = (PlatformUnit) unitComponent;
                unit.CurrentProvince = this;
            }

            if (Owner.IsEnemy(unitComponent))
            {
                EnemyUnits.Add(unitComponent);
                Owner.EnemyIsAttackingProperty(gameObject);
                if (!AlliedUnits.Any())
                {
                    SetSkirmishResult();
                    return;
                }
                if (!IsBattle)
                {
                    StartCoroutine("CommenceBattle");
                }
            }
            else 
            {
                AlliedUnits.Add(unitComponent);
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
            List<Unit> losingArmy;
            List<Unit> winningArmy;
            float damageValue;

            if (Math.Abs(totalDefense - totalAttack) < 0.01)
            {
                losingArmy = EnemyUnits;
                winningArmy = AlliedUnits;
                damageValue = UnityEngine.Random.Range(0.01f, totalDefense / 2);
            }
            else
            {
                damageValue = Math.Abs(totalDefense - totalAttack);

                losingArmy = totalAttack > totalDefense ? AlliedUnits : EnemyUnits;
                winningArmy = totalAttack <= totalDefense ? AlliedUnits : EnemyUnits;
            }

            for (var i = 0; i < losingArmy.Count; i++)
            {
                try
                {
                    var unit = losingArmy[i];
                    unit.ModifyStatus(-damageValue);
                }
                catch(Exception e)
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
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            //damageValue *= 0.3f;

            //for (var i = 0; i < winningArmy.Count; i++)
            //{
            //    var unit = losingArmy[i];
            //    unit.ModifyStatus(-damageValue);
            //}


            if (losingArmy.Count != 0) return;
            var proposedOwner = winningArmy[0].Owner;//obsłużyć nullowanie
            Owner = proposedOwner.GetType() == typeof(Aliens) ? proposedOwner : _originalOwner ;

            if (Owner != _originalOwner)
            {
                try
                {
                    _originalOwner.LostProvinces.Add(this);
                }
                catch (Exception e)
                {
                    _originalOwner.LostProvinces = new List<Province> {this};
                }
            }
            else
            {
                Owner.LostProvinces.Remove(this);
            }

            _spriteRenderer.color = Owner.Color;
            AlliedUnits = winningArmy;
            EnemyUnits = new List<Unit>();
            IsBattle = false;
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
            if (unitComponent.GetType() == typeof(PlatformUnit))
            {
                var unit = (PlatformUnit)unitComponent;
                unit.CurrentProvince = null;
            }

            if (Owner.IsEnemy(unitComponent))
            {
                EnemyUnits.Remove(unitComponent);
            }
            else
            {
                AlliedUnits.Remove(unitComponent);
            }
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
