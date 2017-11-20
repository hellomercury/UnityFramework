using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.DataStruct
{
    public class Base
    {
        public delegate void DlgtPropertyChanged(Base InObj, string InPropertyName, object InCurrentValue, object InOldValue);

        private Dictionary<string, DlgtPropertyChanged> propertyChangedDict;

        private SyncFactory factory;

        private Type type;


        protected Base()
        {
            type = GetType();

            factory = SyncFactory.GetOrCreateSyncFactory(type);

            propertyChangedDict = new Dictionary<string, DlgtPropertyChanged>();
        }

        public void OnSyncOne(int InIndex, object InValue)
        {
            factory.OnSyncOne(this, InIndex, InValue);
        }

        public void OnSyncAll(object[] InObject)
        {
            int length = InObject.Length;

            for (int i = 0; i < length; i++)
            {
                factory.OnSyncOne(this, i, InObject[i]);
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
