using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Collider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        print(boxCollider.name);
    }

	private void OnCollisionEnter(Collision collision)
	{
        print(collision.gameObject.name);
	}

	private void OnTriggerEnter(Collider other)
	{
        print(other.gameObject.name);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
