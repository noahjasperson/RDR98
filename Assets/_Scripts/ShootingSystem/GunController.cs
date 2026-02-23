using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using TMPro;

public class GunController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private GunData data;

    [Header("Shot Origins")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform sight;
    
    [Header("HUD")]
    [SerializeField]
    private TextMeshProUGUI crosshair;
    [SerializeField] private float ammo = 0;
    [SerializeField]
    private TextMeshProUGUI ammoText;

    [Header("Operational State")]
    [SerializeField] private bool canFire;
    [SerializeField] private bool triggerDown;
    [SerializeField] private bool reloading;
    private static float _lookX;
    private static float _lookY;

    [Header("Ammo Info")]
    [SerializeField] private int ammoInMag;
    [SerializeField] private int reserveAmmo;
    [SerializeField] private float fireRate;

    [Header("Positional Data")]
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private Quaternion originalRotation;

    [Header("Aiming")]
    private Coroutine _aimRoutine;
    private bool _isAiming;

    [Header("FX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource audioSource;

    private InputManager _input;
    
    // ADS Variables
    private float ADSStartTime;
    private float ADSJourneyLength;
    private Vector3 ADSStartPosition;

    private LayerMask shootable;

    void Start()
    {
        // 1. Get the InputManager. (hint: InputManager.instance)
        // 2. Connect your input actions:
        //      - FireAction.performed  → OnTriggerPulled
        //      - FireAction.canceled   → OnTriggerReleased
        //      - AimAction.performed   → OnAimPressed
        //      - AimAction.canceled    → OnAimReleased
        //
        // 3. Get the AudioSource component from this GameObject.
        //
        // 4. Store the starting local position and rotation
        //    so the gun can return here after recoil or aiming.
        //
        // 5. Copy the aim position from the GunData (data.aimPosition).
        shootable = LayerMask.GetMask("Shootable");
        
        _input = InputManager.instance;

        _input.ShootAction.performed += OnTriggerPulled;
        _input.ShootAction.canceled += OnTriggerReleased;
        
        _input.AimAction.performed += OnAimPressed;
        _input.AimAction.canceled += OnAimReleased;
        
        _input.ReloadAction.performed += ReloadPressed;

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        aimPosition = data.aimPosition;

        data.spareAmmo = 6;
    }

    void Update()
    {
        // These two things happen constantly:
        //
        // 1. Reset the gun back toward normal or aiming position.
        //    (Depending on whether the player is aiming.)
        //
        // 2. Apply weapon sway (small rotation changes) based on
        //    look input from the mouse or stick.
        ResetRecoil();
        HandleSway();
        ammoText.SetText(ammo + " | " + data.spareAmmo);
        if (ammo > 0)
        {
            ammoText.color = Color.black;
        }
        else
        {
            ammoText.color = Color.red;
        }
    }

    // -------------------------------
    #region Shooting
    // -------------------------------

    private void ReloadPressed(InputAction.CallbackContext context)
    {
        if (data.spareAmmo < 6)
        {
            ammo += data.spareAmmo;
            data.spareAmmo = 0;
        }
        else
        {
            data.spareAmmo -= 6-ammo;
            ammo = data.magSize;
        }
    }
    private void OnTriggerPulled(InputAction.CallbackContext obj)
    {
        // Before shooting, check if the gun is allowed to fire
        // (ex: not reloading, has ammo, etc.)

        // If the player is aiming: use the ADS shooting method.
        // If not aiming: use the hip fire shooting method.
        if (ammo > 0)
        {
            if (_isAiming)
            {
                ADSShoot();
            }
            else
            {
                HipShoot();
            }
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext obj)
    {
        // You can simply log something at first,
        // or set triggerDown = false for later use.
    }

    private void ADSShoot()
    {
        // STEP 1 — Raycast
        // Shoot a ray forward from the sight.
        // If it hits something, print what it hit.

        // STEP 2 — Recoil
        // Slightly rotate the gun backward and to the side.
        // Also move the gun back along the Z axis a little.

        // STEP 3 — Visual + Audio Effects
        // - Play the muzzle flash particle system.
        // - Spawn a bullet trail at the muzzle.
        // - Play the gunshot sound (audioSource.Play()).

        ammo -= 1;
        
        if (Physics.Raycast(sight.position, sight.TransformDirection(Vector3.forward),out var hit , Mathf.Infinity, shootable))
        {
            Debug.Log("Did Hit "+ hit.transform.name);
            BasicEnemy enemy = hit.transform.GetComponent<BasicEnemy>();
            enemy.takeDamage(data.damage);
        }else
        {
            Debug.Log("Did not Hit");
        }
        
        transform.localPosition += Vector3.back * data.zRecoil;
        StopAllCoroutines();

		muzzleFlash.Play();
    }

    private void HipShoot()
    {
        ammo -= 1;
        // STEP 1 — Create a "bloom" direction
        // Bloom is inaccurate aim for hip fire.
        // Make a vector based on the camera forward direction,
        // then add small random amounts on the up/right axes.

        // STEP 2 — Raycast
        // Raycast using the bloomed direction.
        // Log what you hit.

        // STEP 3 — Apply recoil (same idea as ADS)

        // STEP 4 — Effects
        // - Play particle flash
        // - Spawn bullet trail (pointing at the bloom direction)
        // - Play
        
        Vector3 bloom = mainCam.transform.forward;
        bloom.y += Random.Range(-.1f, .1f);
        bloom.x += Random.Range(-.1f, .1f);
        
        if (Physics.Raycast(mainCam.transform.position, bloom,out var hit , Mathf.Infinity, shootable))
        {
            Debug.Log("Did Hit "+ hit.transform.name);
            BasicEnemy enemy = hit.transform.GetComponent<BasicEnemy>();
            enemy.takeDamage(data.damage);
        }
        else
        {
            Debug.Log("Did not Hit");
        }
        
        transform.localPosition += Vector3.back * data.zRecoil;
        StopAllCoroutines();
        
        muzzleFlash.Play();
    }

    private void ResetRecoil()
    {
        // This method smoothly moves the gun back toward a stable position.
        //
        // If NOT aiming:
        //   Lerp localPosition → originalPosition
        //   Lerp localRotation → originalRotation
        //
        // If aiming:
        //   Lerp localPosition → aimPosition
        //   Lerp localRotation → originalRotation
        //
        // Use a small speed (e.g., Time.deltaTime * something).
        if (!_isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, data.smoothing);
        }

        if (_isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, data.smoothing);
        }
    }

    #endregion

    // -------------------------------
    #region Aiming
    // -------------------------------

    private void OnAimPressed(InputAction.CallbackContext obj)
    {
        // When player holds aim:
        // 1. Set _isAiming to true.
        // 2. If a coroutine is running, stop it.
        // 3. Start AimDownSights(), which slowly moves the gun forward.
        _isAiming = true;
        if (_aimRoutine != null)
        {
            StopCoroutine(_aimRoutine);
        }
        _aimRoutine = StartCoroutine(AimDownSights());
        crosshair.SetText(" ");
    }

    private void OnAimReleased(InputAction.CallbackContext obj)
    {
        // When player stops aiming:
        // 1. Set _isAiming to false.
        // 2. Stop any running coroutine.
        // 3. Start ReturnSightPosition(), which moves gun back.
        _isAiming = false;
        StopCoroutine(_aimRoutine);
        _aimRoutine = StartCoroutine(ReturnSightPosition());
        crosshair.SetText("+");
    }

    private IEnumerator AimDownSights()
    {
        // Forever loop:
        //   Move the gun's localPosition toward aimPosition
        //   (use Vector3.Lerp)
        //   yield return null to continue next frame.
        ADSStartTime = Time.time;
        ADSJourneyLength = Vector3.Distance(transform.localPosition, aimPosition);
        while (true)
        {
            float distCovered = (Time.time - ADSStartTime) * data.adsSpeed;
            float fractionOfJourney = distCovered / ADSJourneyLength;
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, fractionOfJourney);
            yield return null;
        }
    }

    private IEnumerator ReturnSightPosition()
    {
        // Similar to AimDownSights:
        //   Move the gun's localPosition toward originalPosition
        //   yield return null.
        ADSStartTime = Time.time;
        ADSJourneyLength = Vector3.Distance(transform.localPosition, originalPosition);
        while (true)
        {
            float distCovered = (Time.time - ADSStartTime) * data.adsSpeed;
            float fractionOfJourney = distCovered / ADSJourneyLength;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, fractionOfJourney);
            yield return null;
        }
    }

    #endregion

    // -------------------------------
    #region Sway
    // -------------------------------

    private void HandleSway()
    {
        // Weapon sway makes the gun slightly rotate based on mouse movement.
        //
        // 1. Get look input (x and y).
        //
        // 2. Make three small rotations:
        //      - Horizontal rotation
        //      - Vertical rotation
        //      - Maybe a little roll
        //    (Hint: Quaternion.AngleAxis)
        //
        // 3. Combine those rotations with the starting rotation.
        //
        // 4. Lerp localRotation toward this combined rotation.

        _lookX = _input.Look.x;
        _lookY = _input.Look.y;
        
        Quaternion horizontalSway =  Quaternion.AngleAxis(_lookX, Vector3.down);
        Quaternion verticalSway =  Quaternion.AngleAxis(_lookY, Vector3.right);
        Quaternion rollSway =  Quaternion.AngleAxis(_lookY, Vector3.forward);
        
        Quaternion combinedRotation =  (horizontalSway * verticalSway * rollSway) * originalRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, combinedRotation, Time.deltaTime*data.swayIntensity);
    }
        #endregion
}