/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum FriendAllEnum
    {
        ID,
        fb_id,
        name,
        head_LocPath,
        head_WebPath,
        isSend,
        Max
    }

    public class FriendAll : Base
    {
        private readonly int hashCode;

        [Sync((int)FriendAllEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)FriendAllEnum.fb_id)]
        public string fb_id { get; set; }  //facebook帐号id

        [Sync((int)FriendAllEnum.name)]
        public string name { get; set; }  //好友名称

        [Sync((int)FriendAllEnum.head_LocPath)]
        public string head_LocPath { get; set; }  //好友本地头像地址

        [Sync((int)FriendAllEnum.head_WebPath)]
        public string head_WebPath { get; set; }  //好友网络头像地址

        [Sync((int)FriendAllEnum.isSend)]
        public int isSend { get; set; }

        public FriendAll()
        {
        }

        public FriendAll(int InID, string Infb_id, string Inname, string Inhead_LocPath, string Inhead_WebPath, int InisSend)
        {
            hashCode = InID;
            ID = InID;
            fb_id = Infb_id;
            name = Inname;
            head_LocPath = Inhead_LocPath;
            head_WebPath = Inhead_WebPath;
            isSend = InisSend;
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
            return "FriendAll : ID = " + ID+ ", fb_id = " + fb_id+ ", name = " + name+ ", head_LocPath = " + head_LocPath+ ", head_WebPath = " + head_WebPath+ ", isSend = " + isSend;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is FriendAll && (InObj as FriendAll).ID == ID;
        }
    }
