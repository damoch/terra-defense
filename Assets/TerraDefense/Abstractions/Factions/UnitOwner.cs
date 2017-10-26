using System.Collections.Generic;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Abstractions.Factions
{
    public abstract class UnitOwner : MonoBehaviour
    {
        public Color Color;
        public Color UnitColor;
        public int Credits;
        public List<Unit> AvaibleUnits;

        public List<Province> LostProvinces { get; set; }

        public abstract bool IsEnemy(Unit unit);


        public abstract List<Unit> GetPlayerControllableUnits();

        public abstract GameObject ProduceUnit(Vector2 spawnPosition);

        public abstract void EnemyIsAttackingProperty(GameObject caller);

        public abstract void EnemyIsRetreatingFromProperty(GameObject caller);

        public abstract void EnemyIsCloseToProperty(GameObject caller);

        public abstract void PropertyChangesOwner(Province province, bool isLost);
    }
}
