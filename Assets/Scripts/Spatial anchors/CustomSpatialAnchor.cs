using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSpatialAnchor : MonoBehaviour
{
    private string _oculusUsername;
    private ulong _oculusUserId;

    [SerializeField] GameObject anchorPrefab;
    [SerializeField] Transform pointReference;

    public SharedAnchor colocationAnchor;


    // Start is called before the first frame update
    void Start()
    {
        Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetLoggedInUserCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("GetLoggedInUserCallback: failed with error: " + msg.GetError());
            return;
        }

        Debug.Log("GetLoggedInUserCallback: success with message: " + msg + " type: " + msg.Type);

        var isLoggedInUserMessage = msg.Type == Message.MessageType.User_GetLoggedInUser;

        if (!isLoggedInUserMessage)
        {
            return;
        }

        _oculusUsername = msg.GetUser().OculusID;
        _oculusUserId = msg.GetUser().ID;

        Debug.Log("GetLoggedInUserCallback: oculus user name: " + _oculusUsername + " oculus id: " + _oculusUserId);

        if (_oculusUserId == 0)
            Debug.LogError("You are not authenticated to use this app. Shared Spatial Anchors will not work.");

        Photon.Pun.PhotonNetwork.LocalPlayer.NickName = _oculusUsername;
    }

    public void ShareAnchor()
    {
        colocationAnchor = Instantiate(anchorPrefab, pointReference.position, pointReference.rotation).GetComponent<SharedAnchor>();
    }


    //private void SaveToCloudThenShare()
    //{
    //    OVRSpatialAnchor.SaveOptions saveOptions;
    //    saveOptions.Storage = OVRSpace.StorageLocation.Cloud;
    //    _spatialAnchor.Save(saveOptions, (spatialAnchor, isSuccessful) =>
    //    {
    //        if (isSuccessful)
    //        {
    //            SampleController.Instance.Log("Successfully saved anchor(s) to the cloud");

    //            var userIds = PhotonAnchorManager.GetUserList().Select(userId => userId.ToString()).ToArray();
    //            ICollection<OVRSpaceUser> spaceUserList = new List<OVRSpaceUser>();
    //            foreach (string strUsername in userIds)
    //            {
    //                Debug.Log("Share users: " + strUsername);
    //                spaceUserList.Add(new OVRSpaceUser(ulong.Parse(strUsername)));
    //            }

    //            OVRSpatialAnchor.Share(new List<OVRSpatialAnchor> { spatialAnchor }, spaceUserList, OnShareComplete);

    //            SampleController.Instance.AddSharedAnchorToLocalPlayer(this);
    //        }
    //        else
    //        {
    //            SampleController.Instance.Log("Saving anchor(s) failed. Retrying...");
    //            SaveToCloudThenShare();
    //        }
    //    });
    //}

    //private static void OnShareComplete(ICollection<OVRSpatialAnchor> spatialAnchors, OVRSpatialAnchor.OperationResult result)
    //{
    //    SampleController.Instance.Log(nameof(OnShareComplete) + " Result: " + result);

    //    if (result != OVRSpatialAnchor.OperationResult.Success)
    //    {
    //        foreach (var spatialAnchor in spatialAnchors)
    //        {
    //            spatialAnchor.GetComponent<SharedAnchor>().shareIcon.color = Color.red;
    //        }
    //        return;
    //    }

    //    var uuids = new Guid[spatialAnchors.Count];
    //    var uuidIndex = 0;

    //    foreach (var spatialAnchor in spatialAnchors)
    //    {
    //        SampleController.Instance.Log("OnShareComplete: space: " + spatialAnchor.Space.Handle + ", uuid: " + spatialAnchor.Uuid);

    //        uuids[uuidIndex] = spatialAnchor.Uuid;
    //        ++uuidIndex;
    //    }

    //    PhotonAnchorManager.Instance.PublishAnchorUuids(uuids, (uint)uuids.Length, true);
    //}
}
