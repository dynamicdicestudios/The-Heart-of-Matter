using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElfScript : MonoBehaviour
{
    [SerializeField]
	Transform groundCheckL;
	
	[SerializeField]
	Transform groundCheckR;
	
	[SerializeField]
	GameObject projectile;
	
	[SerializeField]
	Transform projectileSpawnPos;
	
	[SerializeField]
	float agroRange;
	
	[SerializeField]
	float firingRange;
	
	[SerializeField]
	float moveSpeed;
	
	[SerializeField]
	float jumpSpeed;
	
	[SerializeField]
	private int health;
	
	public Transform parentPlayer;
	
	bool isFacingLeft;
	bool justHit;
	bool shouldJump;
	bool isShooting;
	
	Rigidbody2D rb2d;
	Transform player;
	
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
		player = parentPlayer.transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {		
		float distToPlayer = Vector2.Distance(transform.position, player.position);
		
		if (distToPlayer < firingRange) {
			if (transform.position.x > player.position.x) {
				GetComponent<Animator>().Play("Attack");
				transform.localScale = new Vector3(-5, 5, 1);
			} else {
				GetComponent<Animator>().Play("Attack");
				transform.localScale = new Vector3(5, 5, 1);
			}
		} else if (distToPlayer < agroRange)
			ChasePlayer();
		else
			StopChasingPlayer();
		
		if (transform.hasChanged == false) {
			if (isGrounded())
				shouldJump = true;
			else
				rb2d.velocity = new Vector2(0, -10);
		} else
			shouldJump = false;
			
		
		transform.hasChanged = false;
    }
	
	void ResetShoot() {
		isShooting = false;
	}
	
	bool isGrounded() {
		if (((Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground"))) ||
			(Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))))
			return true;     
		else
			return false;
	}
	
	private void ShootPlayer() {
		if (isShooting) return;
		
		GameObject a = Instantiate(projectile);
		if (isFacingLeft)
			a.GetComponent<ArrowScript>().StartShoot("left");
		else
			a.GetComponent<ArrowScript>().StartShoot("right");
		a.transform.position = projectileSpawnPos.transform.position;
		
		isShooting = true;
		Invoke("ResetShoot", 1);
		
	}
	
	private void ChasePlayer() { 
		 if (transform.position.x < player.position.x) {
			if (shouldJump) {
				rb2d.velocity = new Vector2(moveSpeed, jumpSpeed);
				shouldJump = false;
			} else	
				rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
			GetComponent<Animator>().Play("Move");
			transform.localScale = new Vector3(5, 5, 1);
			isFacingLeft = false;
		 } else {
			if (shouldJump) {
				rb2d.velocity = new Vector2(-moveSpeed, jumpSpeed);
				shouldJump = false;
			} else	
				rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
			GetComponent<Animator>().Play("Move");
			transform.localScale = new Vector3(-5, 5, 1);
			isFacingLeft = true;
		 }
	} 
	
	private void StopChasingPlayer() { 
		rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		if (isFacingLeft)
			GetComponent<Animator>().Play("Idle");
		else
			GetComponent<Animator>().Play("Idle");
	}
	
	IEnumerator Hurt() {
		justHit = true;
		GetComponent<Animator>().Play("Flash");
		yield return new WaitForSeconds(1f);
		justHit = false;
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Weapon")) {
			health -= 1;
			StartCoroutine(Hurt());
			if (isFacingLeft)
				rb2d.velocity = new Vector2(5, rb2d.velocity.y); 
			else
				rb2d.velocity = new Vector2(-5, rb2d.velocity.y);
		} else if (collision.CompareTag("Lava"))
			health -= health;
		
		if (health < 1)
			Destroy(gameObject);
			
	}
}
