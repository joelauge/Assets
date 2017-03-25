using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using System.Security.Cryptography;
using System.Text;
using Encoder = SevenZip.Compression.LZMA.Encoder;
using Xxtea;

internal enum TargetPlatform{
	StandaloneWindows,
	Android,
	iOS,
	StandaloneOSXIntel
}

internal class BundleManager
{
    public List<BundleInfo> bundles = new List<BundleInfo>();

    public BundleManager()
    {
        GameUpdatePoster.bm = this;
    }

    public void BuildAll()
    {
        if (!Directory.Exists(Config.bundlePoolRelativePath + "/" + Config.platform))
        {
            Directory.CreateDirectory(Config.bundlePoolRelativePath + "/" + Config.platform);
        }
        BuildPipeline.BuildAssetBundles(Config.bundlePoolRelativePath + "/" + Config.platform, BuildAssetBundleOptions.UncompressedAssetBundle, GetUnityBuildTarget(Config.platform));
    }

    public void CopyAll(bool copyToResources = false)
    {
        string destPath = copyToResources
			? Config.resourcesPath + "/" + Config.bundleRelativePath
            : "AssetBundles/" + Config.platform + "/" + Config.bundleRelativePath;
        if (Directory.Exists(destPath))
        {
            Directory.Delete(destPath, true);
        }
        Directory.CreateDirectory(destPath);
        
        foreach (var bundle in bundles)
        {
            int childFolder = bundle.name.LastIndexOf("/");
            if (childFolder > 0)
            {
                string childFolderPath = destPath + "/" + bundle.name.Substring(0, childFolder);
                if (!Directory.Exists(childFolderPath))
                {
                    Directory.CreateDirectory(childFolderPath);
                }
            }
            CompressAndEncryptLZMA(Config.bundlePoolRelativePath + "/" + Config.platform + "/" + bundle.name, destPath + "/" + bundle.name + Config.suffix);

            bundle.size = (uint)new FileInfo(destPath + "/" + bundle.name + Config.suffix).Length;
            bundle.md5 = GetFileMD5(destPath + "/" + bundle.name + Config.suffix);
        }
    }

    public void UpdateAll()
    {
        bundles.Clear();
        string[] unusedBundle = AssetDatabase.GetUnusedAssetBundleNames();
        var existBundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundleName in existBundleNames)
        {
            if (unusedBundle.Contains(bundleName))
            {
                continue;
            }
            BundleInfo bundle = new BundleInfo(bundleName);
            bundles.Add(bundle);
        }
    }

    public void CompressAndEncryptLZMA(string inputPath, string outputPath)
    {
        using (MemoryStream inputStream = new MemoryStream(File.ReadAllBytes(inputPath)))
        {
            Encoder coder = new Encoder();
            using (MemoryStream compressStream = new MemoryStream())
            {
                coder.WriteCoderProperties(compressStream);
                compressStream.Write(BitConverter.GetBytes(inputStream.Length), 0, 8);
                coder.Code(inputStream, compressStream, inputStream.Length, -1, null);
                if (string.IsNullOrEmpty(Config.password))
                {
                    File.WriteAllBytes(outputPath, compressStream.ToArray());
                }
                else
                {
					File.WriteAllBytes(outputPath, XXTEA.Encrypt(compressStream.ToArray(), Config.password));
                }       
            }
        }      
    }

    string GetFileMD5(string path)
    {
        byte[] md5Result;
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5Result = md5.ComputeHash(fs);
        }
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < md5Result.Length; i++)
        {
            sb.Append(md5Result[i].ToString("x2"));
        }
        return sb.ToString();

    }

	BuildTarget GetUnityBuildTarget(TargetPlatform platform){
		BuildTarget bt = BuildTarget.StandaloneWindows;
		switch (platform) {
		case TargetPlatform.StandaloneWindows:
			bt = BuildTarget.StandaloneWindows;
			break;
		case TargetPlatform.StandaloneOSXIntel:
			bt = BuildTarget.StandaloneOSXIntel;
			break;
		case TargetPlatform.iOS:
			bt = BuildTarget.iOS;
			break;
		case TargetPlatform.Android:
			bt = BuildTarget.Android;
			break;
		}
		return bt;
	}
}


