using System.Collections;
using System.Collections.Generic;
using Framework.DataStruct;
using Framework.SQLite3;
using UnityEngine;

public enum PlayerEnum
{
    ID,
    Name,
    Skill,
    AchieveMent,
    Max
}
public class Player : Base
{
    [Constraint(SQLite3Constraint.PrimaryKey | SQLite3Constraint.AutoIncrement)]
    [Sync((int)PlayerEnum.ID)]
    public int ID { get; private set; }

    [Constraint(SQLite3Constraint.NotNull)]
    [Sync((int)PlayerEnum.Name)]
    public string Name { get; private set; }

    [Sync((int)PlayerEnum.Skill)]
    public int[,,,] Skill { get; private set; }

    [Sync((int)PlayerEnum.AchieveMent)]
    public int[][][] Achievement { get; private set; }

    public Player()
    {

    }

    public Player(int InID, string InName, int[,,,] InSkill, int[][][] InAchievement)
    {
        ID = InID;
        Name = InName;
        Skill = InSkill;
        Achievement = InAchievement;
    }

    public override string ToString()
    {
        string str = string.Empty;
        if (null != Skill)
        {
            //int length = Skill.GetLength(0), length1 = Skill.GetLength(1), length2 = Skill.GetLength(2), length3 = Skill.GetLength(3);
            //for (int i = 0; i < length; i++)
            //{
            //    for (int j = 0; j < length1; j++)
            //    {
            //        for (int k = 0; k < length2; k++)
            //        {
            //            for (int m = 0; m < length3; m++)
            //            {
            //                str += Skill[i, j, k, m] + ",";
            //            }
            //        }
            //    }

            //}
        }

        string achStr = string.Empty;
        if (null != Achievement)
        {
            int length = Achievement.Length;
            for (int i = 0; i < length; i++)
            {
                int length1 = Achievement[i].Length;
                for (int j = 0; j < length1; j++)
                {
                    str += Achievement[i][j] + ",";
                }

            }
        }
        return string.Format("[Player: ID={0}, Name={1}, Skill={2}], Achievement={3}", ID, Name, str, achStr);
    }
}

