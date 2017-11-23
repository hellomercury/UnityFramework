/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

namespace Framework.DataStruct
{
    public enum PlayerConfigEnum
    {
        ID,
        Name,
        IconID,
        Life,
        Gold,
        Max
    }

    public class PlayerConfig : Base
    {
        private readonly int hashCode;

        [Sync((int)PlayerConfigEnum.ID)]
        public int ID { get; private set; }  //The first column name must be ID and type must be an int.

        [Sync((int)PlayerConfigEnum.Name)]
        public string Name { get; set; }  //Player name.

        [Sync((int)PlayerConfigEnum.IconID)]
        public string IconID { get; set; }  //Player avatar icon.

        [Sync((int)PlayerConfigEnum.Life)]
        public int Life { get; set; }  //The player's maximum health value

        [Sync((int)PlayerConfigEnum.Gold)]
        public int Gold { get; set; }  //Initial gold

        public PlayerConfig()
        {
        }

        public PlayerConfig(int InID, string InName, string InIconID, int InLife, int InGold)
        {
            hashCode = InID;
            ID = InID;
            Name = InName;
            IconID = InIconID;
            Life = InLife;
            Gold = InGold;
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
            return "PlayerConfig : ID = " + ID+ ", Name = " + Name+ ", IconID = " + IconID+ ", Life = " + Life+ ", Gold = " + Gold;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is PlayerConfig && (InObj as PlayerConfig).ID == ID;
        }
    }
}
