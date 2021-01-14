using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

using Clouds.Platformer.Forces;

public class SwordSlash : MonoBehaviour
{
	[Header("Inputs")]
	[SerializeField] InputActionAsset inputMap;
	[SerializeField] string actionMapName = "Gameplay";
	[SerializeField] InputActionReference mouseAction;
	[SerializeField] InputActionReference attackAction;
	[SerializeField] InputActionReference abortAction;

	[Header("Fields")]
    public AcceptsForces forceAcceptor;
    public float swordLength = 1.5f;
    public Transform player;
    public float feetOffset = 0.75f;
    public LayerMask layerMask;

	[Header("Out Events")] 
	public Clouds.PlayerInput.InputGetter playerInput;

	[Header("Animator")]
	public Transform swordTransform;
	public Animator swordAnimator;

	bool isReadyToThrow = false;

	void Start () {
		//Necessary new-input-system init[ialization].
		inputMap.FindActionMap(actionMapName, true).Enable();

		//Register functions to run on input events.
		attackAction.action.started += Slash;
		attackAction.action.performed += HoldToThrow;
		attackAction.action.canceled += ReleaseThrow;
		
		abortAction.action.performed += EndThrow;
	}

	Vector3 mousePosition, offsetOrigin = Vector3.zero;

	// FixedUpdate will be used to store mouse location
	void FixedUpdate () {
		// spot above the player's position, specifically feetOffset units above
		offsetOrigin = player.position + Vector3.up * feetOffset;
        
        // mouse location relative to player
        mousePosition = Camera.main.ScreenToWorldPoint(mouseAction.action.ReadValue<Vector2>());
        mousePosition -= offsetOrigin;
        Debug.DrawRay(offsetOrigin, mousePosition);
	}
	

    // Update is called once per frame
    void Slash(InputAction.CallbackContext context)
    {
		Quaternion swordAngle = getSwordRotation();

		// when click create Hitbox
		RaycastHit2D swordHit = Physics2D.Raycast(
			offsetOrigin, // origin of the raycast
			swordAngle * Vector3.right, // direction of the raycast
			swordLength, // length of the raycast
			layerMask
		);
		Debug.DrawRay(offsetOrigin, swordAngle * Vector3.right, Color.red, 0.025f);
		
		if(swordHit)
		{
			// hitbox collision check for object component
			// check object normal
			Vector2 collideAngle = swordHit.normal;
			
			// amount of force from normal
			forceAcceptor.AcceptForce(collideAngle);
			// object responses
			RespondToSwordHit responder = swordHit.collider.GetComponent<RespondToSwordHit>();
			responder?.Respond();

			// animate sword to attack
			swordAnimator.SetTrigger("Swing");
			// point displayer at slash target
			swordTransform.rotation = swordAngle;
		}
	}

	Quaternion getSwordRotation () {
		// Get angle of swing;
		Vector3 mousePosNormalized = mousePosition.normalized;
		Vector2 pos2D = new Vector2(mousePosNormalized.x, mousePosNormalized.y);
		float swordAngle = Vector2.SignedAngle(Vector2.right, pos2D);

		return Quaternion.AngleAxis(swordAngle, Vector3.forward);
	}

	void HoldToThrow (InputAction.CallbackContext context) {
		//Lock player in place.
		playerInput.enabled = false;
		//Activate let-go-of-button action.
		isReadyToThrow = true;

		// visualize ready-to-throw state
		swordAnimator.SetBool("Ready To Throw", true);

		//Debug.Log("end hold interaction, begin throw.");
	}

	void EndThrow (InputAction.CallbackContext context) {
		// We are ready to throw; unsetting that flag should keep ReleaseThrow away from beginning a throw.
		isReadyToThrow = false;
		playerInput.enabled = true;

		// visualize throw's end.
		swordAnimator.SetBool("Ready To Throw", false);		
	}

	void ReleaseThrow (InputAction.CallbackContext context) {
		if (isReadyToThrow) {
			// do throw

			// throw done, let everything back to normal
			EndThrow(context);
		}

		// CTJ: If I don't do this, it's going to swing twice per click, which isn't at all correct!
		swordAnimator.ResetTrigger("Swing");
	}

    
}
