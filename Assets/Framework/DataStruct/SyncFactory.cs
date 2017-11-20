using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace Framework.DataStruct
{
    public class SyncProperty
    {
        public Type ClassType { get; private set; }
        public string ClassName { get; private set; }
        public PropertyInfo[] Infos { get; private set; }
        public int InfosLength { get; private set; }

        public SyncProperty(Type InClassType, string InClassName, PropertyInfo[] InInfos, int InInfosLength)
        {
            ClassType = InClassType;
            ClassName = InClassName;
            Infos = InInfos;
            InfosLength = InInfosLength;
        }
    }

    public class SyncFactory
    {
        private Type classType;
        private PropertyInfo[] propertyInfos;
        private int propertyInfoLength;
        private SyncProperty Property;

        public SyncFactory(Type InType)
        {
            classType = InType;
            PropertyInfo[] infos = InType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfoLength = infos.Length;
            propertyInfos = new PropertyInfo[propertyInfoLength];
            int syncID;
            Type syncAttrType = typeof(SyncAttribute);
            for (int i = 0; i < propertyInfoLength; i++)
            {
                Object[] attrs = infos[i].GetCustomAttributes(syncAttrType, false);
                if (1 == attrs.Length && attrs[0] is SyncAttribute)
                {
                    syncID = (attrs[0] as SyncAttribute).SyncID;
                    if (-1 < syncID && syncID < propertyInfoLength) propertyInfos[syncID] = infos[i];
                    else throw new IndexOutOfRangeException("Please set SyncAttribute to the property according to the sequence.");
                }
            }

            Property = new SyncProperty(classType, classType.Name, propertyInfos, propertyInfoLength);
        }

        public void OnSyncOne(Object InObj, int InIndex, Object InValue)
        {
            if (null == InObj) throw new ArgumentNullException();
            if (0 > InIndex || InIndex >= propertyInfoLength) throw new IndexOutOfRangeException(InIndex +"< 0 || " + InIndex + " >= " + propertyInfoLength);
            if (InObj.GetType() != classType) throw new ArgumentException("The input type not matched.");

            if(null != InValue)
            {
                PropertyInfo info = propertyInfos[InIndex];
                Object oldValue = info.GetValue(InObj, null);
                if(!InValue.Equals(oldValue))
                {
                    Object value = InValue;
                    if (null == oldValue) value = ChangeType(InValue, info.PropertyType);//Convert.ChangeType(InValue, info.PropertyType);
                    info.SetValue(InObj, value, null);
                    if (InObj is Base) (InObj as Base).OnPropertyChanged(info.Name, oldValue, InValue);
                }
            }
        }

        private static Dictionary<Type, SyncFactory> factoriesDict = new Dictionary<Type, SyncFactory>();
        public static SyncFactory GetOrCreateSyncFactory(Type InType)
        {
            SyncFactory factory = null;
            if (!factoriesDict.TryGetValue(InType, out factory))
            {
                try
                {
                    factory = new SyncFactory(InType);
                    factoriesDict.Add(InType, factory);
                }
                catch (Exception ex)
                {
                    Debug.LogError(InType + " Create SyncFactory Error : " + ex.Message);
                }
            }

            return factory;
        }

        public static SyncProperty GetSyncProperty(Type InType)
        {
            return GetOrCreateSyncFactory(InType).Property;
        }

        private static Object ChangeType(Object InValue, Type InType)
        {
            if (InType.IsValueType) return Convert.ChangeType(InValue, InType);
            else if (InType.IsArray)
            {
                Type type = InType.GetElementType();
                int rank = InType.GetArrayRank();
                Array scoreArray = InValue as Array;
                Debug.LogError(rank+ ", " + type);

                if (1 == rank)
                {
                    int length = scoreArray.Length;
                    Array desArray = Array.CreateInstance(type, length);
                    for (int i = 0; i < length; i++)
                    {
                        desArray.SetValue(scoreArray.GetValue(i), i);
                    }
                    return desArray;
                }
                else
                {
                    int[] ranks = new int[rank];
                    int sum = 0;
                    for (int i = 0; i < rank; i++)
                    {
                        ranks[i] = scoreArray.GetLength(i);
                        sum += ranks[i];
                    }
                    Array desArray = Array.CreateInstance(type, ranks);
                    int[] indexes = new int[rank];
                    int startIndex = rank - 1;
                    for (int i = 0; i < sum; i++)
                    {
                        ++indexes[startIndex];
                        for (int j = startIndex; j > -1; --j)
                        {
                            if (indexes[j] == ranks[j])
                            {
                                indexes[j] = 0;
                                ++indexes[j - 1];
                            }
                            else break;
                        }

                        desArray.SetValue(scoreArray.GetValue(indexes), indexes);
                    }

                    return desArray;
                }
            }
            else 
            {
                throw new FormatException("Cannot convert dat format,please contact author to expand.");
            }
        }

        private static object ConvertObj(object InObj, Type InType)
        {
            if (InType.IsValueType) return Convert.ChangeType(InObj, InType);
            else if (InType.IsArray)
            {
                Type type = InType.GetElementType();
                string[] temp = InObj.ToString().Split('|');
                int length = temp.Length;
                Array array = Array.CreateInstance(type, length);
                for (int j = 0; j < length; j++)
                {
                    array.SetValue(Convert.ChangeType(temp[j], type), j);
                }
                return array;
            }
            else if (InType.IsClass) return Convert.ChangeType(InObj, InType);
            else
            {
                Debug.LogError("暂不支持此类型！" + InType + "," + InType.IsClass + "," + InType.IsPointer + "," + InType.IsCOMObject);
                return null;
            }
        }
    }
}

