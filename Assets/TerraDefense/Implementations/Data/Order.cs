using Assets.TerraDefense.Enums;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Data
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
