using JNetworking;
using ProjectB.Ballistics;
using ProjectB.Commands;
using ProjectB.Interface;
using System;
using System.IO;
using UnityEngine;

namespace ProjectB
{
    [DefaultExecutionOrder(-500)]
    public class NetManager : NetBehaviour
    {
        private bool record = false;

        private void Start()
        {
            JNet.Init("Project B");
            Spawnables.NetRegisterAll();
            UI.AddDrawer(DrawUI);
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
                    case CustomMsg.AUTO_DESTROY_SPAWN:
                        AutoDestroy.ProcessMessage(msg);
                        break;

                    default:
                        Debug.LogError($"Unhandled custom data id: {id}");
                        break;
                }
            };

            if (record)
            {
                StartRecording();
            }

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

        private void DrawUI()
        {
            if (JNet.IsClient || JNet.IsServer)
            {
                return;
            }

            record = GUILayout.Toggle(record, "Record client");
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

        [Command]
        private static string StartPlayback()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PBRecording.txt");

            if (File.Exists(path))
            {
                JNet.Playback.StartPlayback(path);
                return "Started playback!";
            }
            else
            {
                return "Recording file not found!";
            }
        }

        [Command]
        private static string StopPlayback()
        {
            JNet.Playback.StopPlayback();
            return "Stopped playback.";
        }

        [Command]
        private static string StartRecording()
        {
            JNet.Playback.StartRecording(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PBRecording.txt"), true);
            return "Started recording.";
        }

        [Command]
        private static string StopRecording()
        {
            JNet.Playback.StopRecording();
            return "Stopped recording.";
        }
    }
}
