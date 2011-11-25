using System;

namespace Liberty
{
    public interface ICampaignSave
    {
        void Update(System.IO.Stream stream);
        void Update(string path);
    }
}
