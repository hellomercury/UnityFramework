using System;

namespace Framework.DataStruct
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SyncAttribute : Attribute
    {
        public int SyncID { get; set; }

        public SyncAttribute(int InSyncID)
        {
            SyncID = InSyncID;
        }
    }
}