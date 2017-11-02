using System;
using System.Collections.Generic;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Utils;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Units
{
    public class PlatformUnit : Unit {
        public Aliens AliensOwner { get; set; }
        public List<GameObject> Units { get; set; }
        public Province TargetProvince { get; set; }
        public Province CurrentProvince { get; set; }
        public int TerraformingTime;
        public bool IsTerraforming { get; set; }
        private int _terraformingStatus;
        public override void Start()
        {
            TargetProvince = null;
            Target = transform.position;
            Units = new List<GameObject>();
            Target = transform.position;
            AliensOwner = FindObjectOfType<Aliens>();
            if (AliensOwner == null)
            {
                throw new UnityException("No Aliens on scene!");
            }
            InvokeRepeating("DecideNextMove", 1f, 1f);
            SetupTimeValues();
            GetComponent<SpriteRenderer>().color = AliensOwner.UnitColor;
            _terraformingStatus = 0;
            IsTerraforming = false;
        }

        private void DecideNextMove()
        {
            if (IsTerraforming)
            {
                if (CurrentProvince.Owner.Equals(AliensOwner))
                {
                    _terraformingStatus++;
                    if (_terraformingStatus >= TerraformingTime)
                    {
                        CurrentProvince.IsTerraformed = true;
                        IsTerraforming = false;
                        _terraformingStatus = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    IsTerraforming = false;
                    _terraformingStatus = 0;
                }
            }
            if(TargetProvince == null)return;

            if (UtilsAndTools.GetDistance(this, TargetProvince) > 6f)
            {
                var target = UtilsAndTools.FindNearestProvince(TargetProvince);
                foreach (var unit in Units)
                {
                    try
                    {
                        unit.GetComponent<Unit>().SetNewTarget(target.transform.position);
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
                SetNewTarget(target.transform.position);
            }

            if (ShouldBuildMoreUnits())
            {
                Units.Add(AliensOwner.ProduceUnit(transform.position));
            }
            else if(!ShouldMove())
            {
                for (var i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    if (unit != null)
                    {
                        unit.GetComponent<Unit>().SetNewTarget(TargetProvince.gameObject.transform.position);
                    }
                    else
                    {
                        Units.RemoveAt(i);
                    }
                }
            }
        }



        private bool ShouldBuildMoreUnits()
        {
            var sum = 0f;
            for (var i = 0; i < Units.Count; i++)
            {
                var unit = Units[i];
                if (unit != null)
                {
                    sum += unit.GetComponent<Unit>().AttackValue;
                }
                else
                {
                    Units.RemoveAt(i);
                }
            }

            return TargetProvince.DefenseValue + 1 > sum && !ShouldMove();
        }

        public override void ModifyStatus(float value)
        {
            Status += value;
            if (Status <= 0)
            {
                var units = AliensOwner.GetPlayerControllableUnits();
                foreach (var unit in units)
                {
                    var platform = (PlatformUnit) unit;
                    if (platform != this)
                    {
                        platform.Units.AddRange(Units);
                    }
                }
                Destroy(gameObject);
            }
            var propertyModifier = Status / InitialStatus;
            AttackValue *= propertyModifier;
            DefenceValue *= propertyModifier;
        }
    }
}
