/*
 * --->SQLite3 database table structure.<---
 * --->This class code is automatically generated。<---
 * --->If you need to modify, please place the custom code between <Self Code Begin> and <Self Code End>.
 *                                                                                    --szn
 */

namespace Framework.DataStruct
{
    public enum PlayerDataEnum
    {
        ID,
        Name,
        CurrentLevel,
        Life,
        Gold,
        Max
    }

    public class PlayerData : Base
    {
        private readonly int hashCode;

        [Sync((int)PlayerDataEnum.ID)]
        public int ID { get; private set; }  //The first column name must be ID and type must be an int.

        [Sync((int)PlayerDataEnum.Name)]
        public string Name { get; set; }  //Player name.

        [Sync((int)PlayerDataEnum.CurrentLevel)]
        public string CurrentLevel { get; set; }  //The level the player is playing

        [Sync((int)PlayerDataEnum.Life)]
        public int Life { get; set; }  //The player's current health value

        [Sync((int)PlayerDataEnum.Gold)]
        public int Gold { get; set; }  //Player's gold coin

        public PlayerData()
        {
        }

        public PlayerData(int InID, string InName, string InCurrentLevel, int InLife, int InGold)
        {
            hashCode = InID;
            ID = InID;
            Name = InName;
            CurrentLevel = InCurrentLevel;
            Life = InLife;
            Gold = InGold;
        }

        //-------------------------------*Self Code Begin*-------------------------------
        //Custom code.
        //-------------------------------*Self Code End*   -------------------------------
        

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return "PlayerData : ID = " + ID+ ", Name = " + Name+ ", CurrentLevel = " + CurrentLevel+ ", Life = " + Life+ ", Gold = " + Gold;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is PlayerData && (InObj as PlayerData).ID == ID;
        }
    }
}
