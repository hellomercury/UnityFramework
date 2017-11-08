/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum ItemEnum
    {
        ID,
        Name,
        Icon,
        Des,
        PileUpperLimit,
        Skill,
        ItemBuyInfo,
        UnlockInfo,
        Max
    }

    public class Item : Base
    {
        private readonly int hashCode;

        [Sync((int)ItemEnum.ID)]
        public int ID { get; private set; }  //物品ID

        [Sync((int)ItemEnum.Name)]
        public string Name { get; set; }  //物品名称

        [Sync((int)ItemEnum.Icon)]
        public string Icon { get; set; }  //物品图标

        [Sync((int)ItemEnum.Des)]
        public string Des { get; set; }  //物品描述

        [Sync((int)ItemEnum.PileUpperLimit)]
        public int PileUpperLimit { get; set; }  //最大堆叠数量

        [Sync((int)ItemEnum.Skill)]
        public int Skill { get; set; }  //技能

        [Sync((int)ItemEnum.ItemBuyInfo)]
        public int ItemBuyInfo { get; set; }  //物品购买信息

        [Sync((int)ItemEnum.UnlockInfo)]
        public int UnlockInfo { get; set; }  //解锁信息

        public Item()
        {
        }

        public Item(int InID, string InName, string InIcon, string InDes, int InPileUpperLimit, int InSkill, int InItemBuyInfo, int InUnlockInfo)
        {
            hashCode = InID;
            ID = InID;
            Name = InName;
            Icon = InIcon;
            Des = InDes;
            PileUpperLimit = InPileUpperLimit;
            Skill = InSkill;
            ItemBuyInfo = InItemBuyInfo;
            UnlockInfo = InUnlockInfo;
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
            return "Item : ID = " + ID+ ", Name = " + Name+ ", Icon = " + Icon+ ", Des = " + Des+ ", PileUpperLimit = " + PileUpperLimit+ ", Skill = " + Skill+ ", ItemBuyInfo = " + ItemBuyInfo+ ", UnlockInfo = " + UnlockInfo;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is Item && (InObj as Item).ID == ID;
        }
    }
