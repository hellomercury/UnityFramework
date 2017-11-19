/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum LeaderBoardEnum
    {
        ID,
        boardInfo,
        Max
    }

    public class LeaderBoard : Base
    {
        private readonly int hashCode;

        [Sync((int)LeaderBoardEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)LeaderBoardEnum.boardInfo)]
        public string boardInfo { get; set; }  //排行榜信息

        public LeaderBoard()
        {
        }

        public LeaderBoard(int InID, string InboardInfo)
        {
            hashCode = InID;
            ID = InID;
            boardInfo = InboardInfo;
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
            return "LeaderBoard : ID = " + ID+ ", boardInfo = " + boardInfo;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is LeaderBoard && (InObj as LeaderBoard).ID == ID;
        }
    }
