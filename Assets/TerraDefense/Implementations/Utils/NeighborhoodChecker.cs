using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Utils
{
    public class NeighborhoodChecker : MonoBehaviour
    {
        public static NeighborhoodChecker Instance;
        public void Awake()
        {
            _isRunningJobs = false;
            Instance = this;
        }
        private static readonly Stack<Province> JobStack = new Stack<Province>();
        private static bool _isRunningJobs;
        public static void AddJobToQueue(Province prov)
        {
            if(!JobStack.Contains(prov))
                JobStack.Push(prov);
            if (!_isRunningJobs) Instance.StartCoroutine(ExecuteJobs());
        }

        private static IEnumerator ExecuteJobs()
        {
            _isRunningJobs = true;
            while (JobStack.Count > 0)
            {
                Debug.Log("Jobs in queue :" + JobStack.Count);
                var province = JobStack.Pop();
                var hitColliders = Physics2D.OverlapCircleAll(province.transform.position, 50);
                foreach (var hitCollider in hitColliders)
                {
                    var unit = hitCollider.gameObject.GetComponent<Unit>();
                    try
                    {
                        if (unit != null && province.Owner.IsEnemy(unit))
                        {
                            province.Owner.EnemyIsCloseToProperty(province.gameObject);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }

                }
                yield return null;
            }
            _isRunningJobs = false;
        }
    }
}
