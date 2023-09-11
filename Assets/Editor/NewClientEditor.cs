using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace NewSetting
{
    [CustomEditor(typeof(NewClient))]
    class NewClientEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var newClient = target as NewClient;

            if (GUILayout.Button("Day"))
            {
                newClient.SetDay();
            }
            if (GUILayout.Button("Night"))
            {
                newClient.SetNight();
            }
            if (GUILayout.Button("Rain"))
            {
                newClient.SetRainy();
            }
            if (GUILayout.Button("NoRain"))
            {
                newClient.SetSunny();
            }
        }
    }
}