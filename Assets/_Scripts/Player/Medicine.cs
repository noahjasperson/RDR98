using UnityEngine;

public class Medicine : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.addHealth(25);
            Destroy(this.gameObject);
        }
    }
}
