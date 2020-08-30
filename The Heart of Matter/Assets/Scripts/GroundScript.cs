using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("EnemyProjectile"))
			Destroy(collision.gameObject);
	}
}
