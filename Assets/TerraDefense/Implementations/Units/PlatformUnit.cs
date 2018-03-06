﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Utils;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Units
{
    public class PlatformUnit : Unit {
        public Aliens AliensOwner { get; set; }
        public List<Unit> Units { get; set; }
        public Province TargetProvince { get; set; }
        public Province CurrentProvince { get; set; }
        public Action OnDestroyed { get; internal set; }

        public override void Start()
        {
            TargetProvince = null;
            Target = transform.position;
            Units = new List<Unit>();
            Target = transform.position;
            AliensOwner = FindObjectOfType<Aliens>();
            Owner = AliensOwner;
            if (AliensOwner == null)
            {
                throw new UnityException("No Aliens on scene!");
            }
            InvokeRepeating("DecideNextMove", 1f, 1f);
            GetComponent<SpriteRenderer>().color = AliensOwner.UnitColor;
            InitialStatus = Status;
        }

        private void DecideNextMove()
        {
            if (CurrentProvince != null && CurrentProvince.Owner == AliensOwner)
            {
                var units = CurrentProvince.AlliedUnits.Where(x => x != this).ToList();
                if (units.Count == 0)
                {
                    AliensOwner.ProduceUnit(this);
                    return;
                }
            }
            if (TargetProvince == null || TargetProvince.Owner == AliensOwner) return;

            if (UtilsAndTools.GetDistance(this, TargetProvince) > 6f)
            {
                var target = UtilsAndTools.FindNearestProvince(TargetProvince);
                foreach (var unit in Units)
                {
                    try
                    {
                        unit.SetNewTarget(target.transform.position);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
                SetNewTarget(target.transform.position);
            }

            if (ShouldBuildMoreUnits())
            {
                Units.Add(AliensOwner.ProduceUnit(this)?.GetComponent<Unit>());
            }
            else if (!ShouldMove())
            {
                for (var i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    if (unit != null)
                    {
                        unit.SetNewTarget(TargetProvince.gameObject.transform.position);
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
                    sum += unit.AttackValue;
                }
                else
                {
                    Units.RemoveAt(i);
                }
            }

            return TargetProvince.DefenseValue + 1 > sum && !ShouldMove();
        }

        public override bool ModifyStatus(float value)
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
                Units.Clear();
                OnDestroyed?.Invoke();
                GameController.RemoveUnit(gameObject);
                return false;
            }
            var propertyModifier = Status / (float)InitialStatus;
            AttackValue *= propertyModifier;
            DefenceValue *= propertyModifier;
            return true;
        }
    }
}
