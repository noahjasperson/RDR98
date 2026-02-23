using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    [SerializeField] private GunData data;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (data.spareAmmo > data.spareAmmoMax - 7)
            {
                data.spareAmmo = data.spareAmmoMax;
            }
            else
            {
                data.spareAmmo += 6;
            }
            Destroy(this.gameObject);
        }
    }
}
