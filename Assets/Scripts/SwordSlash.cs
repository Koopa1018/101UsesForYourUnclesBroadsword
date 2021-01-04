using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clouds.Platformer.Forces;

public class SwordSlash : MonoBehaviour
{
    public AcceptsForces forceAcceptor;
    public float swordLength = 1.5f;
    public Transform player;
    public float feetOffset = 0.75f;
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 offsetOrigin = player.position + Vector3.up * feetOffset;
        
        // mouse location relative to player
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition -= offsetOrigin;
        Debug.DrawRay(offsetOrigin, mousePosition);

        
        // click detect
        bool attack = Input.GetButtonDown("Slash");
        // when click create Hitbox
        if (attack){
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
        }
    }
    
}
