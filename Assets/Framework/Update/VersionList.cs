using UnityEngine;

namespace Framework.Update
{
    [System.Serializable]
    public class Version
    {
        public string AssetFileName;
        public string LoaclFileName;
        public bool IsNeedCheck;
        public string Md5;
        public bool IsUnzip;
        public float FileSize;

        public Version()
        {

        }
        public Version(string InAssetFileName, string InLoaclFileName, string InMd5,
            bool InIsNeedCheck, bool InIsUnzip, float InFileSize)
        {
            AssetFileName = InAssetFileName;
            LoaclFileName = InLoaclFileName;
            IsNeedCheck = InIsNeedCheck;
            Md5 = InMd5;
            IsUnzip = InIsUnzip;
            FileSize = InFileSize;
        }

        public override string ToString()
        {
            return "AssetFileName = " + AssetFileName
                + "\nLoaclFileName = " + LoaclFileName
                + "\nMd5 = " + Md5
                + "\nIsUnzip = " + IsUnzip +
                   "\nFileSize = " + FileSize;
        }
    }

    //[CreateAssetMenu]
    public class VersionList : ScriptableObject
    {
        public string CurrentVersion;
        public Version[] Files, FilesNeedCheck;
    }
}

