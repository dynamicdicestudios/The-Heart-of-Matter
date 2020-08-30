using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
	float xOffset;
	
	public Transform parentPlayer;
	
	Transform player;
	
	// Start is called before the first frame update
    void Start()
    {
        player = parentPlayer.transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = transform.position;
		
		temp.x = player.position.x;
		temp.y = player.position.y;
		
		temp.x += xOffset;
		
		transform.position = Vector3.Lerp(transform.position, temp, 5 * Time.deltaTime);
    }
}
