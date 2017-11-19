/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum PlayerInfoEnum
    {
        ID,
        name,
        iconid,
        card_front,
        card_back,
        stage,
        Life,
        MaxLifeTime,
        MaxLife,
        Infinite_time,
        Gold,
        device_id,
        player_update,
        daily_id,
        daily_time,
        emailKey,
        Max
    }

    public class PlayerInfo : Base
    {
        private readonly int hashCode;

        [Sync((int)PlayerInfoEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)PlayerInfoEnum.name)]
        public string name { get; set; }  //名字

        [Sync((int)PlayerInfoEnum.iconid)]
        public int iconid { get; set; }  //当前头像

        [Sync((int)PlayerInfoEnum.card_front)]
        public int card_front { get; set; }  //当前卡片front

        [Sync((int)PlayerInfoEnum.card_back)]
        public int card_back { get; set; }  //当前卡片back

        [Sync((int)PlayerInfoEnum.stage)]
        public int stage { get; set; }  //关卡进度

        [Sync((int)PlayerInfoEnum.Life)]
        public int Life { get; set; }  //生命

        [Sync((int)PlayerInfoEnum.MaxLifeTime)]
        public string MaxLifeTime { get; set; }  //体力完全回复时间

        [Sync((int)PlayerInfoEnum.MaxLife)]
        public int MaxLife { get; set; }  //生命上限

        [Sync((int)PlayerInfoEnum.Infinite_time)]
        public string Infinite_time { get; set; }  //无限生命结束时间

        [Sync((int)PlayerInfoEnum.Gold)]
        public int Gold { get; set; }  //金币

        [Sync((int)PlayerInfoEnum.device_id)]
        public string device_id { get; set; }  //最后一次登录的设备ID

        [Sync((int)PlayerInfoEnum.player_update)]
        public string player_update { get; set; }  //表更新时间戳

        [Sync((int)PlayerInfoEnum.daily_id)]
        public int daily_id { get; set; }  //每日登陆进度

        [Sync((int)PlayerInfoEnum.daily_time)]
        public string daily_time { get; set; }  //领取时间

        [Sync((int)PlayerInfoEnum.emailKey)]
        public string emailKey { get; set; }  //全局邮件key

        public PlayerInfo()
        {
        }

        public PlayerInfo(int InID, string Inname, int Iniconid, int Incard_front, int Incard_back, int Instage, int InLife, string InMaxLifeTime, int InMaxLife, string InInfinite_time, int InGold, string Indevice_id, string Inplayer_update, int Indaily_id, string Indaily_time, string InemailKey)
        {
            hashCode = InID;
            ID = InID;
            name = Inname;
            iconid = Iniconid;
            card_front = Incard_front;
            card_back = Incard_back;
            stage = Instage;
            Life = InLife;
            MaxLifeTime = InMaxLifeTime;
            MaxLife = InMaxLife;
            Infinite_time = InInfinite_time;
            Gold = InGold;
            device_id = Indevice_id;
            player_update = Inplayer_update;
            daily_id = Indaily_id;
            daily_time = Indaily_time;
            emailKey = InemailKey;
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
            return "PlayerInfo : ID = " + ID+ ", name = " + name+ ", iconid = " + iconid+ ", card_front = " + card_front+ ", card_back = " + card_back+ ", stage = " + stage+ ", Life = " + Life+ ", MaxLifeTime = " + MaxLifeTime+ ", MaxLife = " + MaxLife+ ", Infinite_time = " + Infinite_time+ ", Gold = " + Gold+ ", device_id = " + device_id+ ", player_update = " + player_update+ ", daily_id = " + daily_id+ ", daily_time = " + daily_time+ ", emailKey = " + emailKey;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is PlayerInfo && (InObj as PlayerInfo).ID == ID;
        }
    }
