using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ConnectToServer : MonoBehaviour
{
    [SerializeField] private Button _buttonConnect;
    private const string HOST_ADDRESS = "127.0.0.1";
    private const int PORT = 9933;
    private const string NAME_ZONE = "TestZone";
    private SmartFox _sfs;

    [Inject]
    private void Construct(SmartFox sfs) => _sfs = sfs;

    private void Start()
    {
        _buttonConnect.OnClickAsObservable().Subscribe(_ => Connection()).AddTo(this);
    }

    private void Update()
    {
        if(_sfs != null) _sfs.ProcessEvents();
    }
    
    private void OnApplicationQuit()
    {
        if(_sfs != null && _sfs.IsConnected) 
            _sfs.Disconnect();
    }

    private void Connection()
    {
        _buttonConnect.interactable = false;
        Debug.Log("Now connecting...");
        
        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        ConfigData configData = new ConfigData();
        configData.Host = HOST_ADDRESS;
        configData.Port = PORT;
        configData.Zone = NAME_ZONE;
        
        _sfs.Connect(configData);
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        Debug.LogWarning("Connection Lost; is reason is: " + (string)evt.Params["reason"]);
        ResetConnect();
    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("Connection established successfully");
            Debug.Log("SFS2X API version: " + _sfs.Version);
        }
        else
        {
            Debug.LogError("Connection failed; is the server running at all?");
            ResetConnect();
        }
    }

    private void ResetConnect()
    {
        _sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        _sfs = null;
        _buttonConnect.interactable = true;
    }
}
