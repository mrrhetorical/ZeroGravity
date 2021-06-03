using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Rhetorical.Server {
	
	[RequireComponent(typeof(NetworkEventManager))]
	public class GameManager : NetworkBehaviour {

		private static GameManager instance;
		public static GameManager Instance {
			get {
				if (instance != null)
					return instance;

				GameObject m = Instantiate(new GameObject("GameManager"));
				m.AddComponent<NetworkEventManager>();
				Instance = m.AddComponent<GameManager>();
				return instance;
			}
			private set => instance = value;
		}
		
		internal NetworkEventManager networkEventManager;
		
		private readonly List<uint> players = new List<uint>();

		public int playerCount => players.Count;

		public void Awake() {
			networkEventManager = GetComponent<NetworkEventManager>();
		}
		
		public void Start() {
			networkEventManager.OnPlayerConnect += OnPlayerConnected;
			networkEventManager.OnPlayerDisconnect += OnPlayerDisconnected;
		}

		public void OnDisable() {
			networkEventManager.OnPlayerConnect -= OnPlayerConnected;
			networkEventManager.OnPlayerDisconnect -= OnPlayerDisconnected;
		}

		private void OnPlayerConnected(uint player) {
			players.Add(player);
		}

		private void OnPlayerDisconnected(uint player) {
			players.Remove(player);
		}
	}
}