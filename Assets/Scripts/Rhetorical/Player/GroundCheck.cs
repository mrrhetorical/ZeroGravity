using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rhetorical.Player {
	public class GroundCheck : MonoBehaviour {

		private Dictionary<Collider2D, short> collisionDictionary = new Dictionary<Collider2D, short>();

		public bool isGrounded => collisionDictionary.Count > 0;
		
		public void OnTriggerEnter2D(Collider2D other) {
			if (!other.CompareTag("Environment"))
				return;

			short amt = 1;
			collisionDictionary.TryGetValue(other, out amt);

			if (collisionDictionary.ContainsKey(other)) {
				collisionDictionary[other] = amt;
			} else {
				collisionDictionary.Add(other, amt);
			}
		}

		public void OnTriggerExit2D(Collider2D other) {
			if (!collisionDictionary.ContainsKey(other))
				return;
					
			short amt = collisionDictionary[other];

			if (--amt <= 0) {
				collisionDictionary.Remove(other);
			} else {
				
			}
			
		}
	}
}