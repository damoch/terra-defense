using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.World;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.TerraDefense.Implementations.Controllers;

namespace Assets.TerraDefense.Implementations.Units
{
    public class Unit : MonoBehaviour, ITimeAffected, ISaveLoad
    {
        protected Vector3 Target;
        public string UnitName;
        public UnitOwner Owner;
        public float Status;
        public float AttackValue;
        public float AirAttackValue;
        public float DefenceValue;
        public float UnitSpeed;
        protected float InitialStatus;
        public int Cost;
        public UnitType UnitType;
        private string _ownerName;
        public int Priority
        {
            get
            {
                return 3;
            }
        }
        public delegate void StatusDelegate(Unit unit);
        public StatusDelegate OnStatusUpdate;
        public bool IsHurt { get { return Status < InitialStatus; } }

        public virtual void Start ()
        {
            if (Owner == null) Owner = UnitOwner.GetByName(_ownerName);
            if (Cost == 0)
            {
                Cost = 5;
            }
            Target = transform.position;
            InitialStatus = Status;
            GetComponent<SpriteRenderer>().color = Owner.UnitColor;
        }
	
        public virtual void Update () {
            if (ShouldMove())
            {
                MoveTowardsTarget();
            }
		
        }

        protected void MoveTowardsTarget()
        {
            var step = UnitSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Target, step);
        }

        protected bool ShouldMove()
        {
            return !Target.Equals(transform.position);
        }
        
        public void SetNewTarget(Vector3 newTarget)
        {
            Target = newTarget;
        }
        
        public virtual bool ModifyStatus(float value)
        {
            Status += value;
            if (Status <= 0/* && !Application.isEditor*/)
            {
                GameController.RemoveUnit(gameObject);//Destroy(gameObject);
                return false;
            }
            var propertyModifier = Status / (float)InitialStatus;
            AttackValue  *= propertyModifier;
            DefenceValue *= propertyModifier;
            OnStatusUpdate?.Invoke(this);
            return true;
        }

        public void HourEvent()
        {
            Debug.Log("Test");
        }

        public void SetupTimeValues(float seconds)
        {
            UnitSpeed = UnitSpeed / seconds;
        }

        public virtual Dictionary<string, string> GetSavableData()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "name", gameObject.name },
                { "position", JsonConvert.SerializeObject(transform.position) },
                { "unitSpeed", UnitSpeed.ToString() },
                { "ownerName", Owner.Name }
            };
            return dictionary;
        }

        public virtual void SetSavableData(Dictionary<string, string> json)
        {
            transform.position = JsonConvert.DeserializeObject<Vector3>( json["position"]);
            UnitSpeed = float.Parse(json["unitSpeed"]);

            //Owner = UnitOwner.GetByName((string)json["ownerName"]);
            _ownerName = json["ownerName"];
            //transform.position = position;

        }
        public void ChangeOwner(UnitOwner newOwner)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            Owner = newOwner;
            GetComponent<SpriteRenderer>().color = Owner.UnitColor;
        }
    }
}
