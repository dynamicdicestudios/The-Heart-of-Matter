using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
	public GameObject boss;
	public GameObject wall;
	public GameObject treasure;
	
	public Transform treasureSpawnPos;
	public bool entryType;
	
	bool bossDefeated;
	
	// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!boss && !bossDefeated) {
			GameObject t = Instantiate(treasure);
			t.transform.position = treasureSpawnPos.transform.position;
			bossDefeated = true;
			GetComponent<Animator>().Play("BossExit");
		}
    }
	
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Enemy"))
			Destroy(collision.gameObject);
		boss.gameObject.SetActive(true);
		wall.gameObject.SetActive(true);
		
		if (entryType)
			GetComponent<Animator>().Play("BossEntry");
		else
			GetComponent<Animator>().Play("BossEntry2");
	}
}
