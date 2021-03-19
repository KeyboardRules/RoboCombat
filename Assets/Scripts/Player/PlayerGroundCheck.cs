using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController pc;
    // Start is called before the first frame update
    private void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(true);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(false);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == pc.gameObject)
            return;
        pc.SetGrounded(true);
    }
}
