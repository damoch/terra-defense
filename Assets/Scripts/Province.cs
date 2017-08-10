using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Province : MonoBehaviour {
        public List<Unit> Units { get; set; }
        public List<Unit> EnemyUnits { get; set; }
        
        private void Start () {
		    Units = new List<Unit>();
            EnemyUnits = new List<Unit>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitEnter(unitComponent);
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

        private void HandleUnitEnter(Unit unit)
        {
            Units.Add(unit);
        }

        private void HandleUnitExit(Unit unit)
        {
            Units.Remove(unit);
        }

        public int GetProvinceDefenceValue()
        {
            return Units.Sum(unit => unit.DefenceValue);
        }
    }
}
