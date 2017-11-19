/*
 * 数据库数据表结构类
 * --->次类为代码自动生成<---
 * --->如需进行修改，请将修改内容放置在"//自定义代码."，支持多行。
 *                                                                                    --szn
 */

using Framework.DataStruct;

    public enum HighscoreEnum
    {
        ID,
        stage_id,
        highscore,
        time_cost,
        Max
    }

    public class Highscore : Base
    {
        private readonly int hashCode;

        [Sync((int)HighscoreEnum.ID)]
        public int ID { get; private set; }  //索引

        [Sync((int)HighscoreEnum.stage_id)]
        public int stage_id { get; set; }  //关卡号

        [Sync((int)HighscoreEnum.highscore)]
        public int highscore { get; set; }  //最高分

        [Sync((int)HighscoreEnum.time_cost)]
        public float time_cost { get; set; }  //游戏时长

        public Highscore()
        {
        }

        public Highscore(int InID, int Instage_id, int Inhighscore, float Intime_cost)
        {
            hashCode = InID;
            ID = InID;
            stage_id = Instage_id;
            highscore = Inhighscore;
            time_cost = Intime_cost;
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
            return "Highscore : ID = " + ID+ ", stage_id = " + stage_id+ ", highscore = " + highscore+ ", time_cost = " + time_cost;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is Highscore && (InObj as Highscore).ID == ID;
        }
    }
