using JNetworking;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class NetManager : NetBehaviour
{
    private void Start()
    {
        JNet.Init("Project B");
        Spawnables.NetRegisterAll();
    }

    private void StartClient()
    {
        JNet.StartClient();

        JNet.GetClient().UponConnect = () =>
        {
            Debug.Log($"Client connected.");

        };
        JNet.GetClient().UponDisconnect = (reason) =>
        {
            Debug.Log($"Client disconnected. ({reason})");

        };
        JNet.GetClient().UponCustomData = (id, msg) =>
        {
            switch (id)
            {
                case CustomMsg.PROJECTILE_SPAWN:
                    Projectile.ProcessMessage(msg);
                    break;

                default:
                    Debug.LogError($"Unhandled custom data id: {id}");
                    break;
            }
        };

        if (JNet.IsServer)
            JNet.ConnectClientToHost(null);
        else
            JNet.ConnectClientToRemote("127.0.0.1", 7777);
    }

    private void Update()
    {
        JNet.Update();
    }

    private void StartServer()
    {
        JNet.StartServer("My Server Name", 7777, 4);
    }

    private void OnGUI()
    {
        if (JNet.IsClient || JNet.IsServer)
        {
            return;
        }

        if (GUILayout.Button("Start Client"))
        {
            StartClient();
        }

        if (GUILayout.Button("Start Server"))
        {
            StartServer();
        }

        if (GUILayout.Button("Start Host"))
        {
            StartServer();
            StartClient();
        }
    }
}
