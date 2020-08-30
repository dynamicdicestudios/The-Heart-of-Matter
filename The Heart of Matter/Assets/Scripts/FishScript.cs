using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
    [SerializeField]
	float agroRange;
	
	[SerializeField]
	float moveSpeed;
	
	[SerializeField]
	private int health;
	
	[SerializeField]
	public int damage;
	
	bool isFacingLeft;
	bool isChasing;
	
	public Transform parentPlayer;
	
	Rigidbody2D rb2d;
	SpriteRenderer spriteRenderer;
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
		
		if (distToPlayer < agroRange) {
			if (!isChasing) {
				rb2d.velocity = new Vector2(rb2d.velocity.x, -7);
				isChasing = true;
			}
			ChasePlayer();
			StartCoroutine(Up());
		}else {
			StopChasingPlayer();
		}
    }
	
	private void ChasePlayer() {
		if (transform.position.x < player.position.x) {
			rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
			transform.localScale = new Vector3(6, 6, 1);
			isFacingLeft = false;
		} else {
			rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
			transform.localScale = new Vector3(-6, 6, 1);
			isFacingLeft = true;
		}
	} 
	
	private void StopChasingPlayer() { 
		rb2d.velocity = new Vector2(0, rb2d.velocity.y);
	}
	
	IEnumerator Die() {
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}
	
	IEnumerator Up() {
		yield return new WaitForSeconds(1f);
		if (isChasing) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, 4);
			isChasing = false;
		}
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Weapon")) {
			health -= 5;
			if (transform.position.x < collision.transform.position.x)
				rb2d.velocity = new Vector2(moveSpeed, 0);
			else
				rb2d.velocity = new Vector2(-moveSpeed, 0);
			
			if (health <= 0) {
				StartCoroutine(Die());
			}
			StartCoroutine(Die());
		} else if (collision.CompareTag("Air")) {
			health -= health;
			StartCoroutine(Die());
		}
		
	}
}
