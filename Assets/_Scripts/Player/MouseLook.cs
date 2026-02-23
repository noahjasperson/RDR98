using UnityEngine;

public class MouseLook : MonoBehaviour
{

    private InputManager _input;

    [SerializeField] private Transform playerParent;
    [SerializeField] private float sensitivity = 5;

    private float _xRot;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = InputManager.instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleLook(Time.deltaTime);
    }

    private void HandleLook(float delta)
    {
        // create the values we want to apply as rotation
        float mouseX = _input.Look.x * sensitivity * delta;
        float mouseY = _input.Look.y * sensitivity * delta;
        
        // avoid inverting look
        _xRot -= mouseY;
        
        // clamp X rotation to avoid breaking neck
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);
        
        // apply the rotations we made
        transform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);
        playerParent.Rotate(Vector3.up, mouseX);
    }
}
