using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Clouds.Platformer.Forces;

public class SwordSlash : MonoBehaviour
{
	[Header("Inputs")]
	[SerializeField] InputActionAsset inputMap;
	[SerializeField] string actionMapName = "Gameplay";
	[SerializeField] InputActionReference mouseAction;
	[SerializeField] InputActionReference attackAction;

	[Header("Fields")]
    public AcceptsForces forceAcceptor;
    public float swordLength = 1.5f;
    public Transform player;
    public float feetOffset = 0.75f;
    public LayerMask layerMask;

	[Header("Animator")]
	public Animator swordAnimator;
    
    bool attack = false;

	void Start () {
		//Necessary new-input-system init[ialization].
		inputMap.FindActionMap(actionMapName, true).Enable();

		
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 offsetOrigin = player.position + Vector3.up * feetOffset;
        
        // mouse location relative to player
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseAction.action.ReadValue<Vector2>());
        mousePosition -= offsetOrigin;
        Debug.DrawRay(offsetOrigin, mousePosition);

        // when click create Hitbox
        if (attackAction.action.ReadValue<float>() > 0) {
			//animate sword to attack
			swordAnimator.SetTrigger("Swing");

            RaycastHit2D swordHit = Physics2D.Raycast(
                offsetOrigin, // origin of the raycast
                mousePosition.normalized, // direction of the raycast
                swordLength, // length of the raycast
                layerMask
            );
            Debug.DrawRay(offsetOrigin, Vector3.Normalize(mousePosition) * swordLength, Color.red, 0.5f);
            
            // hitbox collision check for object component
            // check object normal
            Vector2 collideAngle = swordHit.normal;
            
            // amount of force from normal
            forceAcceptor.AcceptForce(collideAngle);
            // object responses

            attack = false;
        }
    }
    
}
