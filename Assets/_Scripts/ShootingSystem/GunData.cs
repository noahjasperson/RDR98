using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
     public string gunName;

     [Header("Combat Stats")] 
     public int damage = 10;
     
     public float magSize = 50f;
     public float spareAmmo = 6;
     public float spareAmmoMax = 60;

     public float fireRate = 850f;
     public float adsSpeed;

     public Vector3 hipPosition;
     public Vector3 aimPosition;

     public int burstSize = -1;
     [Header("Recoil Stats")]
     public float hipBloom;

     public float yRecoil;
     public float xRecoil;
     public float zRecoil;

     [Header("Feel")] 
     public float swayIntensity;
     public float smoothing;
}
