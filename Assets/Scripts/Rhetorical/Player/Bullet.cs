using Mirror;
using UnityEngine;

namespace Rhetorical.Player {
	public class Bullet : NetworkBehaviour {

		public uint spawner;
		
		public void OnTriggerEnter2D(Collider2D other) {
			if (!isServer)
				return;
			
			if (!other.CompareTag("Player"))
				return;
			
			other.GetComponent<Player>().KillPlayer(spawner);
			
			Destroy(gameObject);
		}
	}
}