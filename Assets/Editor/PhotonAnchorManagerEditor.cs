using NewSetting;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhotonAnchorManager))]
public class PhotonAnchorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var photon = target as PhotonAnchorManager;

        if (GUILayout.Button("Create room"))
        {
            photon.OnCreateRoomButtonPressed();
        }
        if (GUILayout.Button("Find room"))
        {
            photon.OnFindRoomButtonPressed();
        }
        if (GUILayout.Button("Join first"))
        {
            photon.OnJoinRoomButtonPressed();
        }
    }
}
