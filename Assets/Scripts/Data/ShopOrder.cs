/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum ShopOrderEnum
    {
        ID,
        ItemOrder,
        best,
        hot,
        Max
    }

    public class ShopOrder : Base
    {
        private readonly int hashCode;

        [Sync((int)ShopOrderEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)ShopOrderEnum.ItemOrder)]
        public int ItemOrder { get; set; }  //顺序

        [Sync((int)ShopOrderEnum.best)]
        public int best { get; set; }  //best标识

        [Sync((int)ShopOrderEnum.hot)]
        public int hot { get; set; }  //hot标识

        public ShopOrder()
        {
        }

        public ShopOrder(int InID, int InItemOrder, int Inbest, int Inhot)
        {
            hashCode = InID;
            ID = InID;
            ItemOrder = InItemOrder;
            best = Inbest;
            hot = Inhot;
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
            return "ShopOrder : ID = " + ID+ ", ItemOrder = " + ItemOrder+ ", best = " + best+ ", hot = " + hot;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is ShopOrder && (InObj as ShopOrder).ID == ID;
        }
    }
