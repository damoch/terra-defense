using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Fractions
{
    public abstract class UnitOwner : MonoBehaviour
    {
        public List<Unit> AvaibleUnits;
        /// <summary>
        /// Checks, if unit entering province is friendly
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract bool IsEnemy(Unit unit);

        /// <summary>
        /// Returns Units, taht can be controlled by player (either AI or Human)
        /// </summary>
        /// <returns></returns>
        public abstract List<Unit> GetPlayerControllableUnits();

        public abstract GameObject ProduceUnit(Vector2 spawnPosition);
    }
}
