using System;
using System.Collections.Generic;
using System.Reflection;


namespace Framework.DataStruct
{
    public class SyncFactory
    {
        private Type type;
        private string name;
        private PropertyInfo[] propertyInfos;
        private int propertyInfoLength;

        public SyncFactory(Type InType)
        {
            type = InType;
            name = type.Name;
            PropertyInfo[] infos = InType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic);
            propertyInfoLength = infos.Length;
            int syncID;
            Type syncAttrType = typeof(SyncAttribute);
            for (int i = 0; i < propertyInfoLength; i++)
            {
                Object[] attrs = infos[i].GetCustomAttributes(syncAttrType, false);
                if (1 == attrs.Length && attrs[0] is SyncAttribute)
                {
                    syncID = (attrs[0] as SyncAttribute).SyncID;
                    if (-1 < syncID && syncID < propertyInfoLength) infos[syncID] = infos[i];
                    else throw new IndexOutOfRangeException("Please set SyncAttribute to the property according to the sequence.");
                }
            }
        }

        public void OnSyncOne(Object InObj, int InIndex, Object InValue)
        {
            if (null == InObj) throw new ArgumentNullException();
            if (0 > InIndex || InIndex >= propertyInfoLength) throw new IndexOutOfRangeException();
            if (InObj.GetType() != type) throw new ArgumentException("The input type not matched.");

            if(null != InValue)
            {
                PropertyInfo info = propertyInfos[InIndex];
                Object oldValue = info.GetValue(InObj, null);
                if(!oldValue.Equals(InValue))
                {
                    Object value = InValue;
                    if (null == oldValue) value = Convert.ChangeType(InValue, info.PropertyType);
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
                    UnityEngine.Debug.LogError(InType + " Create SyncFactory Error : " + ex.Message);
                }
            }

            return factory;
        }
    }
}

