using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;


internal class BundleInfo
{
    public string name;
    public string md5;
    public uint size;
    public string[] include;
    public string[] dependency;

    public BundleInfo(string name)
    {
        this.name = name;
        Update();
    }

    public void Update()
    {
        //UpdateSize();
        //UpdateCRC();
        UpdateInclude();
        UpdateDependency();
    }

    void UpdateSize()
    {
        string path = Config.bundlePoolRelativePath + "/" + name;
        if (File.Exists(path))
        {
            FileInfo fi = new FileInfo(path);
            size = (uint)fi.Length;
        }
        else
        {
            size = 0;
        }

    }

    void UpdateCRC()
    {
        //BuildPipeline.GetCRCForAssetBundle(Config.bundlePoolRelativePath + "/" + name, out md5);
    }

    void UpdateInclude()
    {
        include = AssetDatabase.GetAssetPathsFromAssetBundle(name);
    }

    void UpdateDependency()
    {
        dependency = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPathsFromAssetBundle(name));
    }
}


