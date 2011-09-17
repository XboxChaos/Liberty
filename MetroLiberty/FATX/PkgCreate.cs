using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FATX_Browser.FATX
{
    class PkgCreate
    {
        int SPC, Blocks, PID;
        //long PartitionOffset;
        string Path;
        public PkgCreate(int SectorsPerCluster, int Clusters, int PartitionID, string FilePath, long partitionOffset)
        {
            SPC = SectorsPerCluster;
            Blocks = Clusters;
            PID = PartitionID;
            Path = FilePath;
        }

        public void Createpackage()
        {

        }

        void WriteHeader()
        {

        }

        void WriteFAT()
        {

        }

        void WriteRoot()
        {

        }
    }
}
