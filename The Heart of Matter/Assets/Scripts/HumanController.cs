using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    [SerializeField]
	Transform groundCheckL;
	
	[SerializeField]
	Transform groundCheckR;
	
	[SerializeField]
	private int health;
	
	[SerializeField]
	GameObject attackHitBox;
	
	[SerializeField]
	private float moveSpeed;
	
	[SerializeField]
	private float jumpHeight;
	
	[SerializeField]
	GameObject hearts;
	
	Rigidbody2D rb2d;
	BoxCollider2D boxCollider2d;
	
	bool isGrounded;
	bool isAttacking;
	bool isDead;
	bool underWater;
	bool justHit;
	bool isJumping;
	
	public const string RIGHT = "right";
    public const string LEFT = "left";

	
	string buttonPressed;
	
	// Start is called before the first frame update
    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
		boxCollider2d = GetComponent<BoxCollider2D>();
		attackHitBox.SetActive(false);
		isGrounded = true;
    }

    // Update is called once per frame
    void Update() {
		if (!isDead) {
			Movement();
		
			if (transform.hasChanged == false && !isJumping) {
				if (!isGrounded)
					rb2d.velocity = new Vector2(0, -10);
			}
			
			transform.hasChanged = false;
		}
    }
	
	void Movement() {
		if (((Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground"))) ||
			(Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))))
		{
			isGrounded = true;
			isJumping = false;
			if (Input.GetKey("space")) {
				isJumping = true;
				rb2d.velocity = new Vector2(rb2d.velocity.x, jumpHeight);
			}
		} else 
			isGrounded = false;
		
		if (Input.GetKey(KeyCode.RightArrow)) {
				rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
				if (isGrounded) {
					GetComponent<Animator>().Play("Move");
					transform.localScale = new Vector3(5, 5, 1);
				}
				
				buttonPressed = RIGHT;
				
				if (Input.GetKey("z")) {
					GetComponent<Animator>().Play("Attack");
					rb2d.velocity = new Vector2(0, rb2d.velocity.y);
					isAttacking = true;
					StartCoroutine(DoAttack());
				}
		} else if(Input.GetKey(KeyCode.LeftArrow)) {
			rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
			if (isGrounded) {
				GetComponent<Animator>().Play("Move");
				transform.localScale = new Vector3(-5, 5, 1);
			}
				
			buttonPressed = LEFT;
				
			if (Input.GetKey("z")) {
				GetComponent<Animator>().Play("Attack");
				rb2d.velocity = new Vector2(0, rb2d.velocity.y);
				isAttacking = true;
				StartCoroutine(DoAttack());
			}
		} else {	
			if(Input.GetKey("z")) {
				GetComponent<Animator>().Play("Attack");
				rb2d.velocity = new Vector2(0, rb2d.velocity.y);
				isAttacking = true;
				StartCoroutine(DoAttack());
			} else {
				GetComponent<Animator>().Play("Idle");
			}
			rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		}
	}
	
	IEnumerator DoAttack() {
		attackHitBox.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		attackHitBox.SetActive(false);
		isAttacking = false;
	}
	
	IEnumerator Die() {
		isDead = true;
		GetComponent<Animator>().Play("Die");
		rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		yield return new WaitForSeconds(1.5f);
		this.gameObject.SetActive(false);
		//FindObjectOfType<ObjectiveManager>().GameOver();
		
	}
	
	IEnumerator Hurt() {
		justHit = true;
		GetComponent<Animator>().Play("Flash");
		yield return new WaitForSeconds(2f);
		justHit = false;
	}
	
	void RemoveHeart() {
		try {
			hearts.transform.GetChild(health).transform.gameObject.SetActive(false);
		} catch (Exception e) {}		
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Enemy") && !justHit) {
			if (isAttacking && ((buttonPressed == RIGHT &&
			collision.transform.position.x > transform.position.x) ||
			(buttonPressed == LEFT &&
			collision.transform.position.x < transform.position.x))) {
				return;
			} else {
				health -= 1; 
				RemoveHeart();
				StartCoroutine(Hurt());
			}
		} else if (collision.CompareTag("EnemyProjectile") && !justHit) {
			Destroy(collision.gameObject);
			if (isAttacking && ((buttonPressed == RIGHT &&
			collision.transform.position.x > transform.position.x) ||
			(buttonPressed == LEFT &&
			collision.transform.position.x < transform.position.x))) {
				if (buttonPressed == LEFT)
					rb2d.velocity = new Vector2(5, rb2d.velocity.y); 
				else
					rb2d.velocity = new Vector2(-5, rb2d.velocity.y); 
				return;
			} else {
				health -= 1; 
				RemoveHeart();
				StartCoroutine(Hurt());
			}
		} else if (collision.CompareTag("Water") && !underWater) {
			underWater = true;
			rb2d.gravityScale = 0.1f;
		} else if (collision.CompareTag("Air")) {
			underWater = false;
			rb2d.gravityScale = 1;
		} else if (collision.CompareTag("Lava")) {
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
		} else if (collision.CompareTag("Spike")) {
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
			
			health -= 1; 
			RemoveHeart();
		}
		if (health <= 0) {
			StartCoroutine(Die());
		}
	} 
}
