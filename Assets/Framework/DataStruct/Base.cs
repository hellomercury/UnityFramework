using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.DataStruct
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SyncAttribute : Attribute
    {
        public int SyncID { get; set; }

        public SyncAttribute(int InSyncID)
        {
            SyncID = InSyncID;
        }
    }

    public struct ClassProperty
    {
        public string ClassName;
        public PropertyInfo[] Infos;

        public ClassProperty(string InClassName, PropertyInfo[] inInfos)
        {
            ClassName = InClassName;
            Infos = inInfos;
        }
    }

    public class Base
    {
        public delegate void DlgtPropertyChanged(Base InObj, string InPropertyName, object InCurrentValue, object InOldValue);

        private Dictionary<string, DlgtPropertyChanged> propertyChangedDict;

        //private static Base instance;

        //public static Base Instance
        //{
        //    get
        //    {
        //        if (null == instance) instance = new Base();
        //        return instance;
        //    }
        //}

        private static Dictionary<Type, ClassProperty> typeDict;

        private Type type;
        public ClassProperty ClassPropertyInfos
        {
            get { return typeDict[type]; }
        }

        public static ClassProperty GetPropertyInfos(Type InKey)
        {
            ClassProperty property;
            return null != typeDict && typeDict.TryGetValue(InKey, out property)
                ? property : InitProperty(InKey);
        }

        protected Base()
        {
            type = GetType();

            InitProperty(type);

            propertyChangedDict = new Dictionary<string, DlgtPropertyChanged>();
        }

        private static ClassProperty InitProperty(Type InType)
        {
            if (null == typeDict) typeDict = new Dictionary<Type, ClassProperty>();

            ClassProperty property;
            if (!typeDict.TryGetValue(InType, out property))
            {
                PropertyInfo[] properties = InType.GetProperties(BindingFlags.Public
                                                                    | BindingFlags.NonPublic | BindingFlags.Instance);
                int length = properties.Length;

                Dictionary<int, PropertyInfo> propertyInfoDict = new Dictionary<int, PropertyInfo>(length);
                Type attrType = typeof(SyncAttribute);
                for (int i = 0; i < length; ++i)
                {
                    object[] objs = properties[i].GetCustomAttributes(attrType, false);
                    if (objs.Length == 1 && objs[0] is SyncAttribute)
                        propertyInfoDict.Add(((SyncAttribute)objs[0]).SyncID, properties[i]);
                }

                length = propertyInfoDict.Count;
                PropertyInfo[] propertyInfos = new PropertyInfo[length];
                for (int i = 0; i < length; ++i)
                {
                    propertyInfos[i] = propertyInfoDict[i];
                }

                property = new ClassProperty(InType.Name, propertyInfos);
                typeDict.Add(InType, property);
            }

            return property;
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
                Debug.LogError("暂不支持此类型！" + InType +"," + InType.IsClass + "," + InType.IsPointer + "," + InType.IsCOMObject);
                return null;
            }
        }

        public void OnSyncOne(int InIndex, object InObj)
        {
            if (null != InObj)
            {
                PropertyInfo info = ClassPropertyInfos.Infos[InIndex];
                object oldObj = info.GetValue(this, null);

                if (null == oldObj) InObj = ConvertObj(InObj, info.PropertyType);
                if (!InObj.Equals(oldObj))
                {
                    info.SetValue(this, InObj, null);
                    OnPropertyChanged(info.Name, InObj, oldObj);
                }
            }
        }

        public void OnSyncAll(object[] InObject)
        {
            PropertyInfo[] infos = ClassPropertyInfos.Infos;
            int length = infos.Length;

            Assert.IsTrue(InObject != null && InObject.Length >= length);

            for (int i = 0; i < length; i++)
            {
                infos[i].SetValue(this, ConvertObj(InObject[i], infos[i].PropertyType), null);
            }
        }

        public void RegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
            {
                propertyChangedDict[InPropertyName] += InPropertyChangedFuc;
            }
            else
            {
                propertyChangedDict.Add(InPropertyName, InPropertyChangedFuc);
            }
        }

        public void UnRegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
            {
                propertyChangedDict[InPropertyName] -= InPropertyChangedFuc;
            }
        }

        public void OnPropertyChanged(string InPropertyName, object InPropertyValue, object InOldValue)
        {
            DlgtPropertyChanged del;
            if (propertyChangedDict.TryGetValue(InPropertyName, out del)
                    && del != null)
            {
                del(this, InPropertyName, InPropertyValue, InOldValue);
            }
        }
    }
}
