using System.Collections.Generic;
using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Province : MonoBehaviour {
        public List<Unit> EnemyUnits { get; set; }
        public int DefenseValue { get; set; }
        public Country Owner;

        private void Start () {
            EnemyUnits = new List<Unit>();
            DefenseValue = 0;
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
            if (Owner.Alliance.IsUnitAllied(unitComponent)) DefenseValue += unitComponent.DefenceValue;
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
            if (Owner.Alliance.IsUnitAllied(unitComponent)) DefenseValue -= unitComponent.DefenceValue;
        }
    }
}
