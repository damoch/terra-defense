using Assets.TerraDefense.Implementations.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.TerraDefense.Implementations.World
{
    public class BattleGroup
    {
        public Province TargetProvince { get; set; }
        public Province RallyProvince { get; set; }
        public List<Unit> BattleGroupUnits { get; set; }
        public float GroupAttackStrength
        {
            get
            {
                return BattleGroupUnits.Sum(x => x.AttackValue);
            }
        }

        public BattleGroup()
        {
            BattleGroupUnits = new List<Unit>();
        }

        public bool IsGroupReadyForAttack()
        {
            return AreUnitsReallied() && IsGroupStrengthSufficient();
        }

        private bool AreUnitsReallied()
        {
            foreach (var unit in BattleGroupUnits)
            {
                if (!RallyProvince.AlliedUnits.Contains(unit))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsGroupStrengthSufficient()
        {
            return GroupAttackStrength > TargetProvince.DefenseValue;
        }

        internal void CommenceAttack()
        {
            foreach (var unit in BattleGroupUnits)
            {
                unit.SetNewTarget(TargetProvince.transform.position);
            }
        }
    }
}
