using System;
using System.Text;
using Framework.DataStruct;
using UnityEngine;

namespace Framework.Tools
{
    public static class PlayerPrefsUtility
    {
        public static int GetInt(string InKey, int InDeffaultValue = 0)
        {
            return PlayerPrefs.GetInt(InKey, InDeffaultValue);
        }

        public static float GetFloat(string InKey, float InDeffaultValue = 0)
        {
            return PlayerPrefs.GetFloat(InKey, InDeffaultValue);
        }

        public static string GetString(string InKey, string InDeffaultValue = null)
        {
            return PlayerPrefs.GetString(InKey, InDeffaultValue);
        }

        public static void SetInt(string InKey, int InValue)
        {
            AddKey(InKey, "_i");
            PlayerPrefs.SetInt(InKey, InValue);
        }

        public static void SetFloat(string InKey, float InValue)
        {
            AddKey(InKey, "_f");
            PlayerPrefs.SetFloat(InKey, InValue);
        }

        public static void SetString(string InKey, string InValue)
        {
            AddKey(InKey, "_s");
            PlayerPrefs.SetString(InKey, InValue);
        }

        public static void SetT<T>(T InT) where T : Base
        {
            //sb.Remove(0, sb.Length);
            //ClassProperty property = InT.ClassPropertyInfos;
            //for (int i = 0; i < property.Infos.Length; i++)
            //{
            //    if (property.Infos[i].PropertyType.IsArray)
            //        sb.Append(Utility.ChangeArrayToString((Array)property.Infos[i].GetValue(InT, null))).Append("|");
            //    else
            //        sb.Append(property.Infos[i].GetValue(InT, null)).Append("|");
            //}
            //sb.Remove(sb.Length - 1, 1);
            //Debug.LogError(sb);
            //SetString(property.ClassName, sb.ToString());
        }

        public static T GetT<T>() where T : Base, new()
        {
            //ClassProperty property = Base.GetPropertyInfos(typeof(T));
            //temp = GetString(property.ClassName);

            //if (string.IsNullOrEmpty(temp)) return default(T);

            T t = new T();
            t.OnSyncAll(InObject: temp.Split('|'));

            return t;
        }

        public static void HasKey(string InKey)
        {
            PlayerPrefs.HasKey(InKey);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static void DeleteKey(string InKey)
        {
            SubKey(InKey);
            PlayerPrefs.DeleteKey(InKey);
        }

        public static void DeleteAll()
        {
            keys = string.Empty;
            PlayerPrefs.DeleteAll();
        }

        private static string keys = string.Empty;
        private static string temp = string.Empty;
        private const string allKey = "PLAYER_PREFS_ALL_KEY";
        private static StringBuilder sb = new StringBuilder(256);
        private static void AddKey(string InKey, string InType)
        {
            if (string.Empty == keys) keys = PlayerPrefs.GetString(allKey, string.Empty);
            sb.Remove(0, sb.Length);
            temp = sb.Append(InKey).Append(InType).Append("|").ToString();
            if (!keys.Contains(temp))
            {
                keys = keys + temp;
                PlayerPrefs.SetString(allKey, keys);
            }
        }

        private static void SubKey(string InKey)
        {
            if (string.Empty == keys) keys = PlayerPrefs.GetString(allKey, string.Empty);
            if (keys.Contains(InKey + "_i|"))
            {
                keys = keys.Replace(InKey + "_i|", "|");
                PlayerPrefs.SetString(allKey, keys);
            }
            else if (keys.Contains(InKey + "_f|"))
            {
                keys = keys.Replace(InKey + "_f|", "|");
                PlayerPrefs.SetString(allKey, keys);
            }
            else if (keys.Contains(InKey + "_s|"))
            {
                keys = keys.Replace(InKey + "_s|", "|");
                PlayerPrefs.SetString(allKey, keys);
            }
        }

        public static string GetAll()
        {
            if (string.Empty == keys) keys = PlayerPrefs.GetString(allKey, string.Empty);

            string[] keyArray = keys.Split('|');
            int length;
            if ((length = keyArray.Length) > 0)
            {
                sb.Remove(0, sb.Length);
                for (int i = 0; i < length; ++i)
                {
                    temp = keyArray[i];
                    sb.Append(temp);
                    if (temp.Contains("_i"))
                        sb.Append(",").Append(PlayerPrefs.GetInt(temp.Remove(temp.Length - 2, 2))).Append("|");
                    else if (temp.Contains("_f"))
                        sb.Append(",").Append(PlayerPrefs.GetFloat(temp.Remove(temp.Length - 2, 2))).Append("|");
                    else if (temp.Contains("_s"))
                        sb.Append(",").Append(PlayerPrefs.GetString(temp.Replace("_s", ""))).Append("|");
                }

                return sb.ToString();
            }

            return "";
        }

        public static void SetAll(string InValue)
        {
            string[] data, keyArray = InValue.Split('|');
            int length;
            if ((length = keyArray.Length) > 0)
            {
                sb.Remove(0, sb.Length);
                for (int i = 0; i < length; ++i)
                {
                    data = keyArray[i].Split(',');
                    if (2 == data.Length)
                    {
                        temp = data[0];
                        sb.Append(temp).Append("|");
                        if (temp.Contains("_i"))
                            PlayerPrefs.SetInt(temp.Remove(temp.Length - 2, 2), Convert.ToInt32(data[1]));
                        else if (temp.Contains("_f"))
                            PlayerPrefs.SetFloat(temp.Remove(temp.Length - 2, 2), Convert.ToSingle(data[1]));
                        else if (temp.Contains("_s"))
                            PlayerPrefs.SetString(temp.Remove(temp.Length - 2, 2), data[1]);
                    }
                }

                keys = sb.ToString();
                PlayerPrefs.SetString(allKey, keys);
            }
        }
    }
}