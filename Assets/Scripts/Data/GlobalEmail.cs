/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum GlobalEmailEnum
    {
        ID,
        Name,
        Description,
        ItemID,
        ItemNum,
        SendTime,
        EndTime,
        StartTime,
        EmailKey,
        Max
    }

    public class GlobalEmail : Base
    {
        private readonly int hashCode;

        [Sync((int)GlobalEmailEnum.ID)]
        public int ID { get; private set; }  //只用来存储，不上传

        [Sync((int)GlobalEmailEnum.Name)]
        public string Name { get; set; }

        [Sync((int)GlobalEmailEnum.Description)]
        public string Description { get; set; }

        [Sync((int)GlobalEmailEnum.ItemID)]
        public string ItemID { get; set; }

        [Sync((int)GlobalEmailEnum.ItemNum)]
        public string ItemNum { get; set; }

        [Sync((int)GlobalEmailEnum.SendTime)]
        public string SendTime { get; set; }

        [Sync((int)GlobalEmailEnum.EndTime)]
        public string EndTime { get; set; }

        [Sync((int)GlobalEmailEnum.StartTime)]
        public string StartTime { get; set; }

        [Sync((int)GlobalEmailEnum.EmailKey)]
        public string EmailKey { get; set; }

        public GlobalEmail()
        {
        }

        public GlobalEmail(int InID, string InName, string InDescription, string InItemID, string InItemNum, string InSendTime, string InEndTime, string InStartTime, string InEmailKey)
        {
            hashCode = InID;
            ID = InID;
            Name = InName;
            Description = InDescription;
            ItemID = InItemID;
            ItemNum = InItemNum;
            SendTime = InSendTime;
            EndTime = InEndTime;
            StartTime = InStartTime;
            EmailKey = InEmailKey;
        }

    //-------------------------------*Self Code Begin*-------------------------------
    //自定义代码.
    //-------------------------------*Self Code End*   -------------------------------
        

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return "GlobalEmail : ID = " + ID+ ", Name = " + Name+ ", Description = " + Description+ ", ItemID = " + ItemID+ ", ItemNum = " + ItemNum+ ", SendTime = " + SendTime+ ", EndTime = " + EndTime+ ", StartTime = " + StartTime+ ", EmailKey = " + EmailKey;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is GlobalEmail && (InObj as GlobalEmail).ID == ID;
        }
    }
