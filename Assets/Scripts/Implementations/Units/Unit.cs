﻿using Assets.Scripts.Abstractions.Factions;
using Assets.Scripts.Implementations.World;
using Assets.Scripts.Interfaces.World;
using UnityEngine;

namespace Assets.Scripts.Implementations.Units
{
    public class Unit : MonoBehaviour, ITimeAffected
    {
        protected Vector3 Target;
        public UnitOwner Owner;
        public float UnitSpeed;
        public float AttackValue;
        public float DefenceValue;
        private float _initialStatus;
        public float Status;
        public int Cost;
        
        public virtual void Start ()
        {
            if (Cost == 0)
            {
                Cost = 5;
            }
            Target = transform.position;
            SetupTimeValues();
            _initialStatus = Status;
            GetComponent<SpriteRenderer>().color = Owner.UnitColor;
        }
	
        public virtual void Update () {
            if (ShouldMove())
            {
                MoveTowardsTarget();
            }
		
        }

        protected void MoveTowardsTarget()
        {
            var step = UnitSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Target, step);
        }

        protected bool ShouldMove()
        {
            return !Target.Equals(transform.position);
        }
        
        public void SetNewTarget(Vector3 newTarget)
        {
            Target = newTarget;
        }
        
        public void ModifyStatus(float value)
        {
            Status += value;
            if (Status <= 0)
            {
                Destroy(gameObject);
            }
            var propertyModifier = Status / (float)_initialStatus;
            AttackValue  *= propertyModifier;
            DefenceValue *= propertyModifier;
        }

        public void HourEvent()
        {
            Debug.Log("Test");
        }

        public void SetupTimeValues()
        {
            var clock = FindObjectOfType<Clock>();
            UnitSpeed = UnitSpeed / clock.LengthOfHour;
        }
    }
}
