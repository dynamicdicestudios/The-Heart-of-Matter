using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcScript : MonoBehaviour
{
	[SerializeField]
	Transform groundCheckL;
	
	[SerializeField]
	Transform groundCheckR;
	
	[SerializeField]
	float agroRange;
	
	[SerializeField]
	float attackRange;
	
	[SerializeField]
	float moveSpeed;
	
	[SerializeField]
	private int health;
	
	bool isFacingLeft = true;
	bool isShooting;
	bool justHit;
	bool isDead;
	bool isGrounded;
	
	public Transform parentPlayer;
	
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
					
		if (((Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground"))) ||
			(Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))))
			isGrounded = true;
		else
			isGrounded = false;
		
		if (distToPlayer < attackRange && !isDead)
			AttackPlayer();
		else if (distToPlayer < agroRange && !isDead)
			ChasePlayer();

		if (transform.hasChanged == false) {
			if (!isGrounded)
				rb2d.velocity = new Vector2(0, -10);
		}		
		transform.hasChanged = false;
    }
	
	void ResetShoot() {
		isShooting = false;
	}
	
	IEnumerator Hurt() {
		justHit = true;
		GetComponent<Animator>().Play("Flash");
		this.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.gameObject.SetActive(true);
		justHit = false;
	}
	
	private void AttackPlayer() {
		if (transform.position.x < player.position.x) {
			rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			GetComponent<Animator>().Play("Attack");
			transform.localScale = new Vector3(7, 7, 1);
			isFacingLeft = false;
		 } else if (transform.position.x > player.position.x) {
			rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			GetComponent<Animator>().Play("Attack");
			transform.localScale = new Vector3(-7, 7, 1);
			isFacingLeft = true;
		 } else
			 StopChasingPlayer();
		
	}
	
	private void ChasePlayer() { 
		 if (transform.position.x < player.position.x) {
			rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
			GetComponent<Animator>().Play("Move");
			transform.localScale = new Vector3(7, 7, 1);
			isFacingLeft = false;
		 } else if (transform.position.x > player.position.x) {
			rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
			GetComponent<Animator>().Play("Move");
			transform.localScale = new Vector3(-7, 7, 1);
			isFacingLeft = true;
		 } else
			 StopChasingPlayer();
	} 
	
	private void StopChasingPlayer() { 
		rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		if (isFacingLeft)
			GetComponent<Animator>().Play("Idle");
		else
			GetComponent<Animator>().Play("Idle");
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Weapon") && !justHit) {
			health -= 1;
			if (isFacingLeft)
				rb2d.velocity = new Vector2(5, rb2d.velocity.y); 
			else
				rb2d.velocity = new Vector2(-5, rb2d.velocity.y);
			StartCoroutine(Hurt());
			
		} else if (collision.CompareTag("Lava"))
			health -= health;
		
		if (health < 1)
			Destroy(gameObject);
			
	}
}
