using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode;
using ZXing;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject StartFields;
    [SerializeField] private TMP_InputField PlayerNameInput;

    [SerializeField] private GameObject RoomFields;

    [SerializeField] private RawImage _rawImageReceiver;

    [SerializeField] private RawImage _rawImageBackground;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
    [SerializeField] private TextMeshProUGUI _textOut;
    [SerializeField] private RectTransform _scanZone;

    [SerializeField] private TMP_InputField RoomCodeInput;
    [SerializeField] private TMP_Text RoomCodeText;


    private Texture2D _storeEncodedTexture; // to store the generated encoded texture
    private bool _isCamAvailable; // to check for camera avilability
    private WebCamTexture _cameraTexture; // to store camera textures

    private bool joinRoomPressed = false;
    private bool joinedRoom = false;

    // Start is called before the first frame update
    void Start()
    {
        _storeEncodedTexture = new Texture2D(256, 256);
    }

    // Update is called once per frame
    void Update()
    {
        if (joinRoomPressed && !joinedRoom)
        {
            UpdateCameraRender();
        }
    }
    public void OnStartButtonClicked()
    {
        string playername = PlayerNameInput.text;
        
        if (!string.IsNullOrEmpty(playername) )
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playername;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public void CreateRoom()
    {
        string roomName = UnityEngine.Random.Range(0, 1000).ToString("D3");
        RoomCodeText.text = roomName;

        // Encode the room name to create QR Code
        Color32[] _convertPixelToTexture = Encode(roomName, _storeEncodedTexture.width, _storeEncodedTexture.height);
        _storeEncodedTexture.SetPixels32(_convertPixelToTexture);
        _storeEncodedTexture.Apply();

        _rawImageReceiver.texture = _storeEncodedTexture;

        // Create room with room name
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void JoinRoom()
    {
        joinRoomPressed = true;

        SetUpCamera();

        if (PhotonNetwork.InRoom)
            joinedRoom = true;
    }

    public void OnClickScanBtn()
    {
        Scan();
    }

    public void OnClickEnterRoomBtn()
    {
        string roomname = RoomCodeInput.text;
        PhotonNetwork.JoinRoom(roomname);
        SceneManager.LoadScene("ARGameScene");
    }

    #region PRIVATE METHODS
    private Color32[] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
            }
        };
        return writer.Write(textForEncoding);
    }
    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            _isCamAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
                _cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
        }
        _cameraTexture.Play();
        _rawImageBackground.texture = _cameraTexture;
        _isCamAvailable = true;
    }
    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if (result != null)
            {
                _textOut.text = result.Text;
                PhotonNetwork.JoinRoom(result.Text);
                SceneManager.LoadScene("ARGameScene");
            }
            else
            {
                _textOut.text = "FAILED TO READ QR CODE";
            }
        }
        catch
        {
            _textOut.text = "FAILED IN TRY";
        }
    }
    private void UpdateCameraRender()
    {
        if (_isCamAvailable == false)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / _cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_cameraTexture.videoRotationAngle;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("We connected to Internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("We connected to Photon Server.");

        StartFields.SetActive(false); // Disabling Start Fields
        RoomFields.SetActive(true); // Enabling the Room Fields
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        SceneManager.LoadScene("ARGameScene");
    }
    #endregion
}
