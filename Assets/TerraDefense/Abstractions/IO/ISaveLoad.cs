using System.Collections.Generic;

namespace Assets.TerraDefense.Abstractions.IO
{
    public interface ISaveLoad
    {
        Dictionary<string, string> GetSavableData();
        void SetSavableData(Dictionary<string, string> json);
        int Priority { get; }
    }
}
