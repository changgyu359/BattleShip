using UnityEngine;
using UnityEngine.SceneManagement;

using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Net;


[Serializable]
public class TurnSyncData
{
    public int FirstTurnId;

    public TurnSyncData(int _id)
    { FirstTurnId = _id; }

    public TurnSyncData() { }
}

[Serializable]
public class PacketData
{
    public string Name;
    public string Contents;
}

[Serializable]
public class ReadyData
{
    public int PlayerId;

    public ReadyData(int _id)
    { PlayerId = _id; }

    public ReadyData() { }
}


[Serializable]
public class BattleShipData
{
    public int X;
    public int Y;
    public int State;
    public int[] SunkX;
    public int[] SunkY;

    public BattleShipData(int _x, int _y, int _state=0, int[] _sunkX=null, int[] _sunkY=null)
    {
        X = _x; Y = _y; State = _state;
        SunkX = _sunkX; SunkY = _sunkY;
    }
    public BattleShipData() { }
}

public class TCPManager : MonoBehaviour
{
    private static TCPManager instance;
    public static TCPManager Instance
    { get { return instance; } }

    public const string ReadyDataStr = "ReadyData";
    public const string BattleshipDataStr = "BattleshipData";
    public const string VoteDataStr = "VoteData";
    public const string ForceRestartDataStr = "ForceRestartData";
    public const string TurnSyncDataStr = "TurnSyncData";

    [SerializeField]
    private string ipAddress = "127.0.0.1";
    [SerializeField]
    private int port = 7777;


    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;

    private StreamWriter writer;
    private StreamReader reader;

    private bool isRunning;

    private bool isServer = false;
    public bool IsServer
    { get { return isServer; } }

    private SemaphoreSlim sendLock = new SemaphoreSlim(1,1);


   


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public async Task StartServerAsync()
    {
        if (isRunning)
            return;
        isRunning = true;
        isServer = true;

        try
        {
            string str = ipAddress;

            listener = new TcpListener(IPAddress.Parse(str), port);
            listener.Start();
            Debug.Log("상대와의 연결을 대기중입니다.");

            client = await listener.AcceptTcpClientAsync();
            stream = client.GetStream();
            Debug.Log("상대방이 연결되었습니다.");

            UIManager.Instance.MultiParent.gameObject.SetActive(false);
            BattleshipManger.Instance.Player1Board.Tilemap.transform.parent.gameObject.SetActive(true);
            BattleshipManger.Instance.ShipParent.gameObject.SetActive(true);
            UIManager.Instance.ReadyBtn.gameObject.SetActive(true);
            

            writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            writer.AutoFlush = true;


            

            await RecieveDataAsync();

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task ConnectClientAsync()
    {
        if (isRunning)
            return;
        isRunning = true;
        isServer = false;

        try
        {

            client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);
            Debug.Log("접속이 되었습니다.");

            UIManager.Instance.MultiParent.gameObject.SetActive(false);
            BattleshipManger.Instance.Player1Board.Tilemap.transform.parent.gameObject.SetActive(true);
            BattleshipManger.Instance.ShipParent.gameObject.SetActive(true);
            UIManager.Instance.ReadyBtn.gameObject.SetActive(true);

            stream = client.GetStream();
            writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            writer.AutoFlush = true;
            

            await RecieveDataAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async Task RecieveDataAsync()
    {
        try
        {
            while (reader != null)
            {
                string json = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(json)) break;

                PacketData data = JsonUtility.FromJson<PacketData>(json);

                switch (data.Name)
                {
                    case ReadyDataStr:
                        ReadyData readyData = JsonUtility.FromJson<ReadyData>(data.Contents);
                        BattleshipManger.Instance.PressReadyButton(readyData.PlayerId);
                        break;
                    case BattleshipDataStr:
                        BattleShipData battleshipData = JsonUtility.FromJson<BattleShipData>(data.Contents);
                        InputManager.Instance.ExecuteNetworkAttack(battleshipData);
                        break;
                    case VoteDataStr:
                        BattleshipManger.Instance.VoteRetry();
                        break;
                    case ForceRestartDataStr:
                        BattleshipManger.Instance.Restart();
                        break;
                    case TurnSyncDataStr:
                        TurnSyncData turnData = JsonUtility.FromJson<TurnSyncData>(data.Contents);
                        
                        BattleshipManger.Instance.StartGame(turnData.FirstTurnId);
                        break;

                }
            }
        }
        catch(Exception e)
        {
            Debug.Log("상대가 떠났습니다." + e.Message);
        }
        finally
        {
            if(isRunning)
            {
                Disconnect();
                UIManager.Instance.ReturnToLobby();
            }
        }
       
    }

    public void SendReadyDataEvnet(int myPlayerID)
    {
        PacketData data = new PacketData();
        ReadyData readyData = new ReadyData(myPlayerID);

        data.Name = ReadyDataStr;
        data.Contents = JsonUtility.ToJson(readyData);

        _ = SendDataAsync(data);
    }

    public void SendBattleShipDataEvent(int _x, int _y, int _state=0, int[] _sunkX = null, int[] _sunkY = null)
    {
        PacketData data = new PacketData();
        BattleShipData battleshipData = new BattleShipData(_x, _y, _state, _sunkX, _sunkY);

        data.Name = BattleshipDataStr;
        data.Contents = JsonUtility.ToJson(battleshipData);

        _ = SendDataAsync(data);
    }

    private async Task SendDataAsync(PacketData _data)
    {
        if (writer == null) return;
        string json = JsonUtility.ToJson(_data);
        await sendLock.WaitAsync();
        try
        {
            await writer.WriteLineAsync(json);
        }
        finally
        {
            sendLock.Release();
        }

    }

    public void SendVoteDataEvent()
    {
        PacketData data = new PacketData();
        data.Name=VoteDataStr;
        data.Contents = "Vote";


        _ = SendDataAsync(data);
    }

    public void SendForceRestartEvent()
    {
        PacketData data = new PacketData();
        data.Name = ForceRestartDataStr;
        data.Contents = "Restart"; 
        _ = SendDataAsync(data);
    }

    public void SendTurnSyncData(int firstTurnId)
    {
        PacketData data = new PacketData();
        TurnSyncData turnData = new TurnSyncData(firstTurnId);

        data.Name = TurnSyncDataStr;
        data.Contents = JsonUtility.ToJson(turnData);

        _ = SendDataAsync(data);
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        if (writer != null) { writer.Close(); writer = null; }
        if (stream != null) { stream.Close(); stream = null; }
        if (client != null) { client.Close(); client = null; }
        if (listener != null) { listener.Stop(); listener = null; }

        isRunning = false;
        isServer = false;
    }

}
