using Mirror;

namespace Rhetorical.Server {
	public class NetworkEventManager : NetworkBehaviour {

		#region Event declarations
		
		public delegate void OnPlayerConnectDelegate(uint player);
		public event OnPlayerConnectDelegate OnPlayerConnect;

		public delegate void OnPlayerDisconnectDelegate(uint player);
		public event OnPlayerDisconnectDelegate OnPlayerDisconnect;

		#endregion
		
		#region Event callers

		[Command]
		public void Cmd_InvokeOnPlayerConnect(uint player) {
			OnPlayerConnect?.Invoke(player);
			Rpc_InvokeOnPlayerConnect(player);
		}

		[ClientRpc]
		public void Rpc_InvokeOnPlayerConnect(uint player) {
			OnPlayerConnect?.Invoke(player);
		}


		[Command]
		public void Cmd_InvokeOnPlayerDisconnect(uint player) {
			OnPlayerDisconnect?.Invoke(player);
			Rpc_InvokeOnPlayerDisconnect(player);
		}

		[ClientRpc]
		public void Rpc_InvokeOnPlayerDisconnect(uint player) {
			OnPlayerDisconnect?.Invoke(player);
		}
		
		#endregion
		
		
	}
}