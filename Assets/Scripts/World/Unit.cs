using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Unit : MonoBehaviour
    {
        private Vector3 _target;
        public UnitOwner Owner;
        public float UnitSpeed;
        public int AttackValue;
        public int DefenceValue;
        public int Status;
        
        private void Start ()
        {
            _target = transform.position;
        }
	
        private void Update () {
            if (ShouldMove())
            {
                MoveTowardsTarget();
            }
		
        }
        
        private void MoveTowardsTarget()
        {
            var step = UnitSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target, step);
        }

        private bool ShouldMove()
        {
            return !_target.Equals(transform.position);
        }

        /// <summary>
        /// Gives new target to Unit
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetNewTarget(Vector3 newTarget)
        {
            _target = newTarget;
        }

        /// <summary>
        /// Adds damage points to unit HP, if HP fells below zero, unit is destroyed
        /// </summary>
        /// <param name="value"></param>
        public void AddDamage(int value)
        {
            Status -= value;
            if (Status <= 0)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
