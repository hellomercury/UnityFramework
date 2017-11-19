/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum FriendDataEnum
    {
        ID,
        fb_id,
        gs_id,
        name,
        head_LocPath,
        head_WebPath,
        stage,
        Max
    }

    public class FriendData : Base
    {
        private readonly int hashCode;

        [Sync((int)FriendDataEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)FriendDataEnum.fb_id)]
        public string fb_id { get; set; }  //facebook帐号id

        [Sync((int)FriendDataEnum.gs_id)]
        public string gs_id { get; set; }  //gamespark帐号id

        [Sync((int)FriendDataEnum.name)]
        public string name { get; set; }  //好友名称

        [Sync((int)FriendDataEnum.head_LocPath)]
        public string head_LocPath { get; set; }  //好友本地头像地址

        [Sync((int)FriendDataEnum.head_WebPath)]
        public string head_WebPath { get; set; }  //好友网络头像地址

        [Sync((int)FriendDataEnum.stage)]
        public int stage { get; set; }  //关卡进度

        public FriendData()
        {
        }

        public FriendData(int InID, string Infb_id, string Ings_id, string Inname, string Inhead_LocPath, string Inhead_WebPath, int Instage)
        {
            hashCode = InID;
            ID = InID;
            fb_id = Infb_id;
            gs_id = Ings_id;
            name = Inname;
            head_LocPath = Inhead_LocPath;
            head_WebPath = Inhead_WebPath;
            stage = Instage;
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
            return "FriendData : ID = " + ID+ ", fb_id = " + fb_id+ ", gs_id = " + gs_id+ ", name = " + name+ ", head_LocPath = " + head_LocPath+ ", head_WebPath = " + head_WebPath+ ", stage = " + stage;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is FriendData && (InObj as FriendData).ID == ID;
        }
    }
