using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace NewSetting
{
    public enum Settings
    {
        Day,
        Night,
        City,
        Ocean,
        Rainy,
        Sunny,
        Begin
    }

    public class NewServer : MonoBehaviourPun
    {
        PhotonView view;
        public GameObject shaker;
        bool shakerOn = true;

        private void Start()
        {
            view = PhotonView.Get(this);
            view.RPC("SetDay", RpcTarget.Others);
            SetShaker();
        }

        public void SetShaker()
        {
            shaker.SetActive(!shakerOn);
            shakerOn = !shakerOn;
        }
        public void Day()
        {
            view.RPC("SetDay", RpcTarget.Others);
        }
        public void Night()
        {
            view.RPC("SetNight", RpcTarget.Others);
        }
        public void City()
        {
            view.RPC("SetCity", RpcTarget.Others);
        }
        public void Ocean()
        {
            view.RPC("SetOcean", RpcTarget.Others);
        }
        public void Rainy()
        {
            view.RPC("SetRainy", RpcTarget.Others);
        }
        public void Sunny()
        {
            view.RPC("SetSunny", RpcTarget.Others);
        }
    }
}
