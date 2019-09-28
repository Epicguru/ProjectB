
using JNetworking;
using ProjectB.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB
{
    /// <summary>
    /// Represents a connected player in a game. Contains all of their 'client info' such as name, money etc.
    /// </summary>
    public class Player : NetBehaviour
    {
        public static readonly List<Player> AllPlayers = new List<Player>();

        [SyncVar(FirstOnly = true)]
        public string Name;

        [SyncVar]
        public long Money;

        public Texture DollarSignTexture;

        private void Start()
        {
            AllPlayers.Add(this);

            if (HasLocalOwnership)
            {
                UI.AddDrawer(DrawUI);
            }
        }

        private void OnDestroy()
        {
            AllPlayers.Remove(this);
        }

        public string FormatMoneyString()
        {
            return $"{Money} $";
        }

        private void DrawUI()
        {
            // Should go to top left.

            UI.Label($"Connected as {Name}");
            UI.Label(new GUIContent($"Money: {FormatMoneyString()}", DollarSignTexture));
        }
    }
}
