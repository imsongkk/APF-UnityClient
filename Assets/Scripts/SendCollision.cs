using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCollision : MonoBehaviour
{
    HumanController myCharacter;
    // Start is called before the first frame update
    void Start()
    {
        myCharacter = GameObject.Find("Character").GetComponent<HumanController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("VirtualCollider"))
        {
            myCharacter.ReceiveCollision(collision); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("VirtualCollider"))
        {
            myCharacter.ReceiveTrigger(other);
        }
    }
}
