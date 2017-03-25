using System.Collections.Generic;

namespace newx
{
    public class VersionConfig
    {
        public string versionNum;
        public string bundleRelativePath;
        public List<BundleInfo> bundles;
    }


    public class BundleInfo
    {
        public string name;
        public string md5;
        public uint size;
        public string[] include;
        public string[] dependency;
    }

}
