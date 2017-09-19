using Assets.Scripts.Factions;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Unit : MonoBehaviour, ITimeAffected
    {
        protected Vector3 Target;
        public UnitOwner Owner;
        public float UnitSpeed;
        public int AttackValue;
        public int DefenceValue;
        public int Status;
        public int Cost;
        
        public virtual void Start ()
        {
            if (Cost == 0)
            {
                Cost = 5;
            }
            Target = transform.position;
            SetupTimeValues();
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
        
        public void ModifyStatus(int value)
        {
            Status += value;
            if (Status <= 0)
            {
                Destroy(gameObject);
            }
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
