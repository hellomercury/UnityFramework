using System;
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
                ////Player player = new Player();
                ////handle.CreateTable<Player>();
                ////int[,,] skill = new int[2, 3, 4];
                ////for (int i = 0; i < 2; i++)
                ////{
                ////    for (int j = 0; j < 3; j++)
                ////    {
                ////        for (int k = 0; k < 4; k++)
                ////        {
                ////            skill[i, j, k] = i * j * k;
                ////        }
                ////    }
                ////}
                //int[,,,] skill = new int[4,1,2,2]
                ////{{0, 1},{2,3}};
                //  {{{{0, 1},{2, 3}}},{{{4, 5},{6, 7}}},{{{8, 9},{10, 11}}},{{{12, 13},{14, 15}}}};
                ////{{{{0, 1},{2, 3}}},{{{4, 5},{6, 7}}},{{{8, 9},{10, 11}}},{{{12, 13},{14, 15}}}}
                ////{
                ////  {
                ////      {
                ////          {0, 1},
                ////          {2, 3}
                ////      }
                ////  },
                ////  {
                ////      {
                ////          {4, 5},
                ////          {6, 7}
                ////      }
                ////  },
                ////  {
                ////      {
                ////          {8, 9},
                ////          {10, 11}
                ////      }
                ////  },
                ////  {
                ////      {
                ////          {12, 13},
                ////          {14, 15}
                ////      }
                ////  }
                ////}
                ////
                int[][][] achievement = new int[2][][];
                achievement[0] = new int[2][];
                achievement[0][0] = new int[] { 0, 1};
                achievement[0][1] = new int[] { 2, 3, 4};
                achievement[1] = new int[2][];
                achievement[1][0] = new int[] { 5};
                achievement[1][1] = new int[] { 6, 7 };


                ////0|szn|{0,1},{2,3},}|
                ////[[0,1],[0,1,2]]
                //Player player = new Player(0, "szn", skill, achievement);
                //PlayerPrefsUtility.SetT(player);

                //string str = "{{{{0,1},{2,3},{2,3}},{{0,1},{2,3},{2,3}}},{{{4,5},{6,7},{2,3}},{{0,1},{2,3},{2,3}}},{{{8,9},{10,11},{2,3}},{{0,1},{2,3},{2,3}}},{{{12,13},{14,15},{2,3}},{{0,1},{2,3},{2,3}}}}";
                //            //{{{{0,1},{2,3},{2,3}},{{0,1},{2,3},{2,3}}},{{{4,5},{6,7},{2,3}},{{0,1},{2,3},{2,3}}},{{{8,9},{10,11},{2,3}},{{0,1},{2,3},{2,3}}},{{{12,13},{14,15},{2,3}},{{0,1},{2,3},{2,3}}}}|
                //GetArrayByString(str, typeof(int));

                Utility.ChangeStringToArray("{{{{0, 1},{2, 3}}},{{{4, 5},{6, 7}}},{{{8, 9},{10, 11}}},{{{12, 13},{14, 15}}}}", typeof(int));
                //GetArrayByString("[[[0,1],[2,3,4]],[[5],[6,7]]]", typeof(int));
                //[
                //  [
                //      [0,1],
                //      [2,3,4]
                //  ],
                //  [
                //      [5],
                //      [6,7]
                //  ]
                //]
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
            int index = -1, minIndex = -2, nodeState = 0;
            for (int i = 0, j = 0; i < length; i++)
            {
                if ('[' == InString[i])
                {
                    nodeState = -1;

                    ++index;
                    if (rankList.Count == index) rankList.Add(0);
                }
                else if (']' == InString[i])
                {
                    nodeState = 1;
                    --index;
                    if (-1 == index) break;
                    if (-1 == minIndex) minIndex = index;
                    if (minIndex >= index)
                    {
                        ++rankList[index];
                        minIndex = index;
                    }
                }
                else if (',' == InString[i])
                {
                    
                    contentCharArray[j] = InString[i];
                    ++j;
                }
                else if (' ' != InString[i])
                {

                    if (-2 == minIndex)
                    {
                        rankList[rankList.Count - 1] = 1;
                        minIndex = -1;
                        nodeState = 0;
                    }
                    else
                    {
                        if (-1 == nodeState)
                        {
                            rankList.Add(1);
                            nodeState = 0;
                        }
                        else ++rankList[rankList.Count - 1];

                    }

                    contentCharArray[j] = InString[i];
                    ++j;
                }

            }

            int[] rankArray = rankList.ToArray();
            length = rankArray.Length;

            string log = string.Empty;
            for (int i = 0; i < length; i++)
            {
                log += rankArray[i] + ",";
            }
            Debug.LogError(log);

            return null;
            //int[] arrayIndexes = new int[length];
            //minIndex = length - 1;
            //arrayIndexes[minIndex] = -1;

            //string[] contentStringArray = (new string(contentCharArray)).Split(',');
            //length = contentStringArray.Length;

            //Array array = Array.CreateInstance(InType, rankArray);

            //for (int i = 0; i < length; i++)
            //{
            //    ++arrayIndexes[minIndex];
            //    for (int j = minIndex; j > -1; j--)
            //    {
            //        if (arrayIndexes[j] == rankList[j])
            //        {
            //            arrayIndexes[j] = 0;
            //            ++arrayIndexes[j - 1];
            //        }
            //        else break;
            //    }

            //    array.SetValue(Convert.ToInt32(contentStringArray[i]), arrayIndexes);
            //}

            //return array;
        }

    }
}