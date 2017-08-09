using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        private Vector3 _target;
        
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
            transform.Translate(_target);
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
