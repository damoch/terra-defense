using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Implementations.Data
{
    public class Order : MonoBehaviour
    {
        public MonoBehaviour Subject { get; set; }
        public OrderType OrderType { get; set; }

        public object Arguments { get; set; }

        public override string ToString()
        {
            return "Order: " + OrderType + " " + Subject.name;
        }
    }
}
