using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransferOwnership : MonoBehaviourPun, IPunOwnershipCallbacks
{
    [SerializeField] private GameObject RequestOwnershipBtn;
    [SerializeField] private GameObject TransferOwnershipBtn;
    [SerializeField] private TMP_Text ownershipRequestText;

    private Player OriginalMasterPlayer;

    private Player requestingPlayer;

    PhotonView photonView;
    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (photonView == targetView && requestingPlayer.IsMasterClient)
            photonView.TransferOwnership(requestingPlayer);

        if (photonView == targetView && PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            this.requestingPlayer = requestingPlayer;
            ownershipRequestText.text = "Player " + requestingPlayer.NickName + " has requested ownership transfer.";
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (photonView == targetView && previousOwner == PhotonNetwork.LocalPlayer)
        {
            ownershipRequestText.text = "";
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            RequestOwnershipBtn.SetActive(false);
            TransferOwnershipBtn.SetActive(true);
        }
        else
        {
            RequestOwnershipBtn.SetActive(true);
            TransferOwnershipBtn.SetActive(false);
        }
    }
    public void OnRequestOwnershipBtnClicked()
    {
        photonView.RequestOwnership();
        ownershipRequestText.text = "Player " + PhotonNetwork.LocalPlayer.NickName + " has requested ownership transfer";
    }
    public void OnTransferOwnershipBtnClicked()
    {
        photonView.TransferOwnership(requestingPlayer);
        ownershipRequestText.text = "";
    }
}
