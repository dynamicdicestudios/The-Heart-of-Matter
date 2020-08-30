using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogScript : MonoBehaviour
{
	[SerializeField]
	int health;
	
	[SerializeField]
	float shootDelay;
	
	[SerializeField]
	Transform groundCheckL;
	
	[SerializeField]
	Transform groundCheckR;
	
	[SerializeField]
	GameObject projectile;
	
	[SerializeField]
	Transform projectileSpawnPos;
	
	[SerializeField]
	GameObject leftPush;
	
	[SerializeField]
	GameObject rightPush;
		
	[SerializeField]
	GameObject hearts;
	
	Rigidbody2D rb2d;	
	
	bool isFacingLeft = true;
	bool isShooting;
	bool underWater;
	bool isGrounded;
	bool hasLanded;
	bool justHit;
	bool isDead;
	
	// Start is called before the first frame update
    void Start()
    {
		rb2d = GetComponent<Rigidbody2D>();
		
		EntryMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (((Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground"))) ||
			(Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))))
			isGrounded = true;
		else
			isGrounded = false;
		
		if (transform.hasChanged == false) {
			if (!isGrounded)
				rb2d.velocity = new Vector2(0, -10);
		}
		
		if (!transform.hasChanged && hasLanded && !isDead) {
			rb2d.velocity = new Vector2(0, 0);
			GetComponent<Animator>().Play("Spit");
			if (isFacingLeft)
				transform.localScale = new Vector3(20, 20, 1);
			else
				transform.localScale = new Vector3(-20, 20, 1);
			AttackPlayer();
		}
		
		transform.hasChanged = false;
		
		if (health < 0)
			StartCoroutine(Die());
    }
	
	void ResetShoot() {
		isShooting = false;
	}
	
	void EntryMovement() {
		rb2d.velocity = new Vector2(5, 20);
		hasLanded = true;
	}
	
	void RemoveHeart() {
		try {
			hearts.transform.GetChild(health).transform.gameObject.SetActive(false);
		} catch (Exception e) {}		
	}
	
	void AttackPlayer() {
		if (isShooting) return;
		
		GameObject w = Instantiate(projectile);
		if (isFacingLeft)
			w.GetComponent<WaterBallScript>().StartShoot("left");
		else
			w.GetComponent<WaterBallScript>().StartShoot("right");
		w.transform.position = projectileSpawnPos.transform.position;
		
		isShooting = true;
		Invoke("ResetShoot", shootDelay);
		
	}
	
	IEnumerator JumpAway() {
		if (isFacingLeft)
			rb2d.velocity = new Vector2(-15, 10);
		else
			rb2d.velocity = new Vector2(15, 10);
		yield return new WaitForSeconds(5f);
		if (isFacingLeft)
			isFacingLeft = false;
		else
			isFacingLeft = true;
	}
	
	IEnumerator Hurt() {
		justHit = true;
		GetComponent<Animator>().Play("Flash");
		yield return new WaitForSeconds(3f);
		justHit = false;
	}
	
	IEnumerator Die() {
		isDead = true;
		GetComponent<Animator>().Play("Die");
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Water") && !underWater) {
			underWater = true;
			rb2d.gravityScale = 0.1f;
		} else if (collision.CompareTag("Air")) {
			underWater = false;
			rb2d.gravityScale = 1;
		} else if (collision.CompareTag("Weapon") && transform.hasChanged) {
			health -= 1;
			RemoveHeart();
			StartCoroutine(Hurt());
			
			if (isFacingLeft) {
				GetComponent<Animator>().Play("LeftPush");
				transform.localScale = new Vector3(20, 20, 1);
			} else {
				GetComponent<Animator>().Play("RightPush");
				transform.localScale = new Vector3(-20, 20, 1);
			}
		} else if (collision.CompareTag("Weapon")) {
			health -= 1;
			RemoveHeart();
			StartCoroutine(Hurt());
			
			StartCoroutine(JumpAway());
		}
	}
}
