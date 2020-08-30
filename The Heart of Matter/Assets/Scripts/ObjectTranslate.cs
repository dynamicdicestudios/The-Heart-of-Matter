using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTranslate : MonoBehaviour
{
	float push = -0.0001f;
	
    [SerializeField]
	float start;

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Translate());
    }
	
	IEnumerator Translate() {
		Vector2 pos = transform.position;
		if (pos.x == start + 15 || pos.x == start - 15)
			push = -push;
		
		pos.x += push;
		transform.position = pos;
		yield return new WaitForSeconds(5f);
	}
}
