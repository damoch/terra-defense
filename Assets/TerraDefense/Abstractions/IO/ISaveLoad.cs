using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.TerraDefense.Abstractions.IO
{
    public interface ISaveLoad
    {
        string GetSavableData();
        void SetSavableData(string json);
    }
}
