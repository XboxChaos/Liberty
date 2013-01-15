using System;
using System.IO;

namespace Liberty.SaveManager.Games
{
    public interface ICampaignSave
    {
        void Update(string path);
        void Update(Stream stream);
    }
}
