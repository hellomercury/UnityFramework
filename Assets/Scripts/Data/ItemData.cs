/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum ItemDataEnum
    {
        ID,
        itemID,
        Count,
        Max
    }

    public class ItemData : Base
    {
        private readonly int hashCode;

        [Sync((int)ItemDataEnum.ID)]
        public int ID { get; private set; }

        [Sync((int)ItemDataEnum.itemID)]
        public int itemID { get; set; }  //物品ID

        [Sync((int)ItemDataEnum.Count)]
        public int Count { get; set; }  //数量

        public ItemData()
        {
        }

        public ItemData(int InID, int InitemID, int InCount)
        {
            hashCode = InID;
            ID = InID;
            itemID = InitemID;
            Count = InCount;
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
            return "ItemData : ID = " + ID+ ", itemID = " + itemID+ ", Count = " + Count;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is ItemData && (InObj as ItemData).ID == ID;
        }
    }
