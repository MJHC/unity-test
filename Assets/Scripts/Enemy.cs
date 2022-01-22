using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(health);
        if (health <= 0)
            Destroy(gameObject);

        Vector3 playerPos = new Vector3(player.position.x, 0.5f, player.position.z);
        
        transform.position = Vector3.MoveTowards(transform.position, playerPos, 1 * Time.deltaTime);
        
    }
}
