﻿using System;
using System.Collections.Generic;
using System.Text;
using Framework.SQLite3;
using Framework.Tools;
using UnityEngine;

namespace Framework.Test
{
    public class Test : MonoBehaviour
    {
        //private SQLite3Handle handle;

        //void Awake()
        //{
        //    handle = new SQLite3Handle(Application.dataPath + "/Test.db", SQLite3OpenFlags.ReadWrite | SQLite3OpenFlags.Create);
        //}

        //void OnApplicationQuit()
        //{
        //    handle.CloseDB();
        //}

        void OnGUI()
        {
            GUI.skin.button.fontSize = 64;
            if (GUILayout.Button("W"))
            {
                //Player player = new Player();
                //handle.CreateTable<Player>();
                //int[,,] skill = new int[2, 3, 4];
                //for (int i = 0; i < 2; i++)
                //{
                //    for (int j = 0; j < 3; j++)
                //    {
                //        for (int k = 0; k < 4; k++)
                //        {
                //            skill[i, j, k] = i * j * k;
                //        }
                //    }
                //}
                int[,,,] skill = new int[4,1,2,2]
                //{{0, 1},{2,3}};
                  {{{{0, 1},{2, 3}}},{{{4, 5},{6, 7}}},{{{8, 9},{10, 11}}},{{{12, 13},{14, 15}}}};
                //{{{{0, 1},{2, 3}}},{{{4, 5},{6, 7}}},{{{8, 9},{10, 11}}},{{{12, 13},{14, 15}}}}
                //{
                //  {
                //      {
                //          {0, 1},
                //          {2, 3}
                //      }
                //  },
                //  {
                //      {
                //          {4, 5},
                //          {6, 7}
                //      }
                //  },
                //  {
                //      {
                //          {8, 9},
                //          {10, 11}
                //      }
                //  },
                //  {
                //      {
                //          {12, 13},
                //          {14, 15}
                //      }
                //  }
                //}
                //
                int[][][] achievement = new int[2][][];
                achievement[0] = new int[2][];
                achievement[0][0] = new int[] { 0, 1};
                achievement[0][1] = new int[] { 2, 3, 4};
                achievement[1] = new int[2][];
                achievement[1][0] = new int[] { 5};
                achievement[1][1] = new int[] { 6, 7 };


                //0|szn|{0,1},{2,3},}|
                //[[0,1],[0,1,2]]
                Player player = new Player(0, "szn", skill, achievement);
                PlayerPrefsUtility.SetT(player);

                string str = "{{{{0,1},{2,3},{2,3}},{{0,1},{2,3},{2,3}}},{{{4,5},{6,7},{2,3}},{{0,1},{2,3},{2,3}}},{{{8,9},{10,11},{2,3}},{{0,1},{2,3},{2,3}}},{{{12,13},{14,15},{2,3}},{{0,1},{2,3},{2,3}}}}";
               
                GetArrayByString(str, typeof(int));
            }

            if (GUILayout.Button("R"))
            {
                Debug.LogError(PlayerPrefsUtility.GetT<Player>());
            }
        }

        public Array GetArrayByString(string InString, Type InType)
        {
            int length = InString.Length;
            List<int> rankList = new List<int>(16);
            char[] contentCharArray = new char[InString.Length];
            int index = -1, minIndex = -1;
            for (int i = 0, j = 0; i < length; i++)
            {
                if ('{' == InString[i])
                {
                    ++index;
                    if (rankList.Count == index) rankList.Add(0);
                }
                else if ('}' == InString[i])
                {
                    --index;
                    if (-1 == index) break;
                    if (-2 == minIndex) minIndex = index;
                    if (minIndex >= index)
                    {
                        ++rankList[index];
                        minIndex = index;
                    }
                }
                else if (',' == InString[i])
                {
                    if (-1 == minIndex)
                    {
                        rankList[rankList.Count - 1] = 2;
                        minIndex = -2;
                    }
                    else if (-2 == minIndex)
                    {
                        ++rankList[rankList.Count - 1];
                    }
                    contentCharArray[j] = InString[i];
                    ++j;
                }
                else if (' ' != InString[i])
                {
                    contentCharArray[j] = InString[i];
                    ++j;
                }

            }

            int[] rankArray = rankList.ToArray();
            length = rankArray.Length;
            int[] arrayIndexes = new int[length];
            minIndex = length - 1;
            arrayIndexes[minIndex] = -1;

            string[] contentStringArray = (new string(contentCharArray)).Split(',');
            length = contentStringArray.Length;

            Array array = Array.CreateInstance(InType, rankArray);

            for (int i = 0; i < length; i++)
            {
                ++arrayIndexes[minIndex];
                for (int j = minIndex; j > -1; j--)
                {
                    if (arrayIndexes[j] == rankList[j])
                    {
                        arrayIndexes[j] = 0;
                        ++arrayIndexes[j - 1];
                    }
                    else break;
                }

                array.SetValue(Convert.ToInt32(contentStringArray[i]), arrayIndexes);
            }

            int ranka = array.Rank;
            StringBuilder sb = new StringBuilder();
            int[] ranksa = new int[ranka];
            int starta = ranka - 1;
            int sum = 1;
            for (int j = 0; j < ranka; j++)
            {
                sb.Append("{");
                ranksa[j] = array.GetLength(j);
                sum *= ranksa[j];
            }

            int[] indexesa = new int[ranka];
            for (int j = 0; j < sum; j++)
            {
                int m = 0;
                for (int k = starta; k > -1; k--)
                {
                    if (indexesa[k] == ranksa[k])
                    {
                        indexesa[k] = 0;
                        ++indexesa[k - 1];
                        ++m;
                    }
                }
                if (0 != m)
                {
                    sb.Remove(sb.Length - 1, 1);
                    for (int k = 0; k < m; k++)
                    {
                        sb.Append("}");
                    }
                    sb.Append(",");
                    for (int k = 0; k < m; k++)
                    {
                        sb.Append("{");
                    }
                }

                sb.Append(array.GetValue(indexesa)).Append(",");

                ++indexesa[starta];
            }

            sb.Remove(sb.Length - 1, 1);
            for (int j = 0; j < ranka; j++)
            {
                sb.Append("}");
            }
            sb.Append("|");
            Debug.LogError(sb);


            return null;
        }

    }
}