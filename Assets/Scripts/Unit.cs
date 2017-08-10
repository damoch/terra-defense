using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        private Vector3 _target;
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

        public void SetNewTarget(Vector3 newTarget)
        {
            _target = newTarget;
        }
        
    }
}
