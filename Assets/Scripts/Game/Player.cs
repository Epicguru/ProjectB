
using JNetworking;
using System.Collections.Generic;

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

        private void Awake()
        {
            AllPlayers.Add(this);
        }

        private void OnDestroy()
        {
            AllPlayers.Remove(this);
        }
    }
}
