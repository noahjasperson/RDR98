using UnityEngine;

public class BigAmmoCrate : MonoBehaviour
{
    [SerializeField] private GunData data;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            data.spareAmmo = data.spareAmmoMax;
            Destroy(gameObject);
        }
    }
}
