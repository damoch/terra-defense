using System.Collections.Generic;
using Assets.Scripts.Implementations.Units;
using UnityEngine;

namespace Assets.Scripts.Abstractions.Factions
{
    public abstract class UnitOwner : MonoBehaviour
    {
        public int Credits;
        public List<Unit> AvaibleUnits;

        public abstract bool IsEnemy(Unit unit);


        public abstract List<Unit> GetPlayerControllableUnits();

        public abstract GameObject ProduceUnit(Vector2 spawnPosition);

        public abstract void EnemyIsAttackingProperty(GameObject caller);

        public abstract void EnemyIsRetreatingFromProperty(GameObject caller);

        public abstract void EnemyIsCloseToProperty(GameObject caller);
    }
}
