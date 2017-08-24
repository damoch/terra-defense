using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.World
{
    public interface ITimeAffected
    {
        void HourEvent();
        void SetupTimeValues();
    }
}
