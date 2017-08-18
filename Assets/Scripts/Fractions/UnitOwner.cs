using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Fractions
{
    public abstract class UnitOwner : MonoBehaviour
    {
        /// <summary>
        /// Checks, if unit entering province is friendly
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract bool IsEnemy(Unit unit);
    }
}
