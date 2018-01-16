using System.Collections.Generic;

namespace Assets.TerraDefense.Abstractions.IO
{
    public interface ISaveLoad
    {
        Dictionary<string, object> GetSavableData();
        void SetSavableData(Dictionary<string, object> json);
        int Priority { get; }
    }
}
