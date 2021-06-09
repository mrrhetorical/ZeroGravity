using System;
using System.Collections;
using Mirror;
using Rhetorical.Server;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rhetorical.Player {
	public class Player : NetworkBehaviour {
		internal static Player LocalInstance {get; private set;}

		private Rigidbody2D rb;
		
		private Vector2 gravityDirection = Vector2.zero;
		private const float gravity = -2f;
		
		internal Vector2 moveInput = Vector2.zero;
		internal Vector2 lookDir = Vector2.right;
		private bool isHoldingJump = false;
		private bool isGrounded => groundCheck.isGrounded;

		[SerializeField]
		private GroundCheck groundCheck;

		[SerializeField]
        private float bulletVelocity = 10f;
        [SerializeField]
        private float grenadeVelocity = 10f;
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private GameObject grenadePrefab;

        [SyncVar]
        public bool isAlive;

        public void Start() {
	        if (!isLocalPlayer) {
		        GetComponent<PlayerInput>().enabled = false;
	        }
        }

        public void FixedUpdate() {
	        if (!isLocalPlayer)
		        return;
	        
	        
        }
        
		public override void OnStartLocalPlayer() {
			base.OnStartLocalPlayer();

			LocalInstance = this;

			rb = GetComponent<Rigidbody2D>();
			
			GameManager.Instance.networkEventManager.Cmd_InvokeOnPlayerConnect(netId);
		}

		[TargetRpc]
		public void Target_Fire(Vector2 force) {
			rb.AddForce(force, ForceMode2D.Impulse);
		}
		
		[Server]
		public void Fire() {
			Transform t = transform;
			Vector3 pos = (t.up * moveInput.y + t.right * moveInput.x);

			GameObject bullet = Instantiate(bulletPrefab, t.position + (pos * 0.1f), Quaternion.identity, null);

			NetworkServer.Spawn(bullet);

			bullet.GetComponent<Bullet>().spawner = netId;
			bullet.GetComponent<Rigidbody2D>().AddForce(pos, ForceMode2D.Impulse);
			
			Target_Fire(-pos);
		}

		[Command]
		public void Cmd_Fire() {
			Fire();
		}
		
		[Server]
		public void KillPlayer(uint killer) {
			isAlive = false;
		}
		
		[Command]
		public void Cmd_KillPlayer(uint killer) {
			KillPlayer(killer);
		}

		public void OnCollisionEnter2D(Collision2D collision) {
			if (!isLocalPlayer)
				return;
			
			if (!collision.transform.CompareTag("Environment"))
				return;

			if (collision.contactCount <= 0)
				return;

			Vector2 surfaceNormal = collision.GetContact(0).normal;

			gravityDirection = -surfaceNormal;
			StartCoroutine(ReevaluateRotation(surfaceNormal, 0.4f));
			rb.velocity = Vector2.zero;
			rb.angularVelocity = 0f;
		}

		private IEnumerator ReevaluateRotation(Vector2 surfaceNormal, float time) {
			float elapsed = 0f;
			while (elapsed < time) {

				transform.up = Vector3.Lerp(transform.up, surfaceNormal, elapsed / time);
				
				yield return null;
				elapsed += Time.deltaTime;
			}

		}

		///// UNITY INPUT EVENTS /////

		public void OnMove(InputAction.CallbackContext context) {
			if (!isLocalPlayer)
				return;
			
			moveInput = context.ReadValue<Vector2>();
			if (moveInput != Vector2.zero)
				lookDir = moveInput;
		}

		public void OnJump(InputAction.CallbackContext context) {
			if (!isLocalPlayer)
				return;
			
			if (context.started) {
				isHoldingJump = true;
			} else if (context.canceled) {
				isHoldingJump = false;
			}
		}

		public void OnFire(InputAction.CallbackContext context) {
			if (!isLocalPlayer)
				return;
			
			if (context.performed)
				Cmd_Fire();
		}

		
	}
}