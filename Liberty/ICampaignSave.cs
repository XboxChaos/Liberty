using System;
using System.IO;

namespace Liberty
{
    public interface ICampaignSave
    {
        void Update(string path);
        void Update(Stream stream);
    }
}
