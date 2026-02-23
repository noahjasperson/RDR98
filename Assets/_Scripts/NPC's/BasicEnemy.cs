using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent agent;

    [SerializeField]
    private int health;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        //Look for player in scene
        player = GameObject.FindGameObjectWithTag("Player");

        if(!player){
            Debug.Log("Make sure your player is tagged!!");
        }
        
        //Define Self
        agent = GetComponent<NavMeshAgent>();
        
        //Initial Properties
        agent.speed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 25)
        {
            agent.destination = player.transform.position;
            checkPlayerHit();
        }
        else
        {
            agent.destination = transform.position;
        }
    }
    
    public void takeDamage(int damage)
    {
        if (health > 0)
        {
            health-=damage;
        }
        else
        {
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.increaseScore(1);
            Destroy(gameObject);
        }
    }

    public void checkPlayerHit()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 2)
        {
            Debug.Log("Player is hit");
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.takeHealth(5);
            
            //calculate opposite direction of player
            Vector3 dirPlayer = transform.position - player.transform.position;
            Vector3 runVector = transform.position + dirPlayer;
            agent.destination = runVector;
        }
        if (Vector3.Distance(player.transform.position, transform.position) > 4)
        {
            agent.destination = player.transform.position;
        }
    }
}
