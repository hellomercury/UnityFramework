using System;
using System.Collections.Generic;
using Framework.SQLite3;
using Framework.Tools;
using UnityEngine;

namespace Framework.Test
{
    public class Test : MonoBehaviour
    {
        private SQLite3Handle handle;

        void Awake()
        {
            handle = new SQLite3Handle(Application.dataPath + "/Test.db", SQLite3OpenFlags.ReadWrite | SQLite3OpenFlags.Create);
        }

        void OnApplicationQuit()
        {
            handle.CloseDB();
        }

        void OnGUI()
        {
            GUI.skin.button.fontSize = 64;
            if (GUILayout.Button("W"))
            {
                Player player = new Player();
                handle.CreateTable<Player>();
            }

            if (GUILayout.Button("R"))
            {

            }
        }

	}
}