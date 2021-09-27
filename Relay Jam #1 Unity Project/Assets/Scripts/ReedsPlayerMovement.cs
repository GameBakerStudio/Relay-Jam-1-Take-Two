using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The goal of this movement system is to add challenge to the game in the vein of a "rage game". 
 * Currently how it's set up, the player can launch themself only while near a valid surface.
 * The distance (up to a certain limit) that the player clicks away from their character affects the 
 * speed at which they are launched.
 */

public class ReedsPlayerMovement : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private SpriteRenderer _playerSpriteRend;
	[SerializeField] private Rigidbody2D _playerRB;
	[SerializeField] private CircleCollider2D _playerCollider;
	[SerializeField] private PlayerAnimation _playerAnimation;

	[Header("Movement")]
	[SerializeField] private float _maxPhysicsVector = 10f;
	[SerializeField] private float _forceMult = 10f;
	[SerializeField] private float _skinWidth = 0.2f;
	[SerializeField] private LayerMask _walkableLayers;

	[Header("Effects")]
	[SerializeField] private GameObject _launchFX;
	[SerializeField] private Transform _launchFXOrigin;

	[Header("Sounds")]
	public AudioClip jumpSound;

	private bool _grounded;
	private Camera _mainCamera;

	void Start()
	{
		_mainCamera = Camera.main;
		App.acceptingMoveInput = true;
	}

	void Update()
	{
		_grounded = IsGrounded();
		HandleClick();
		UpdateAnimationState();
	}

	private void UpdateAnimationState()
	{
		if (!_grounded)
		{
			_playerAnimation.SwapToAnim(EAnimState.launch, true);
		}
		else
		{
			HorizontalHit hit = GetHorizontalHit();
			if (hit == HorizontalHit.none)
			{
				if (Math.Abs(_playerRB.velocity.x) > .1f)
				{
					_playerAnimation.SwapToAnim(EAnimState.surf, true);
				}
				else
				{
					_playerAnimation.SwapToAnim(EAnimState.idle, true);
				}
			}
			else
			{
				_playerAnimation.SwapToAnim(EAnimState.wallSlide, hit == HorizontalHit.left);
			}
		}
	}

	//For debug purposes
	void ShowCanJump()
	{
		_playerSpriteRend.color = _grounded ? Color.white : Color.grey;
	}

	private void HandleClick()
	{
		if (!App.acceptingMoveInput) { return; }

		if (Input.GetButtonDown("Fire1"))
		{
			if (!_grounded) { return; }

			Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPosition.z = 0;

			Vector2 forceVec = mouseWorldPosition - transform.position;

			if (forceVec.magnitude > _maxPhysicsVector)
			{
				forceVec = forceVec.normalized * _maxPhysicsVector;
			}

			_playerRB.velocity = forceVec * _forceMult;
			SpawnLaunchFX(forceVec);
			PlayJumpSound();
			_grounded = false;
		}
	}

	private void PlayJumpSound()
	{
		App.PlayVariedAudio(jumpSound, transform.position, 0.5f, 0.6f, 0.8f, 1.2f);
	}


	private void SpawnLaunchFX(Vector2 launchDirection)
	{
		//TODO pool this
		var launchFX = Instantiate(_launchFX, _launchFXOrigin.position, Quaternion.LookRotation(launchDirection));
		Destroy(launchFX.gameObject, 3f);
	}

	private bool IsGrounded()
	{
		var coll = Physics2D.OverlapCircle(transform.position, _playerCollider.radius + _skinWidth, _walkableLayers);
		return coll != null;
	}

	private HorizontalHit GetHorizontalHit()
	{
		if (Physics2D.Raycast(transform.position, Vector2.right, _playerCollider.radius + _skinWidth, _walkableLayers))
		{
			return HorizontalHit.right;
		}
		if (Physics2D.Raycast(transform.position, -Vector2.right, _playerCollider.radius + _skinWidth, _walkableLayers))
		{
			return HorizontalHit.left;
		}
		return HorizontalHit.none;
	}

	private enum HorizontalHit { right, left, none };

	//Just thought the wall was way too slidy
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (GetHorizontalHit() != HorizontalHit.none)
			_playerRB.velocity = new Vector2(0, _playerRB.velocity.y);

	}
}
