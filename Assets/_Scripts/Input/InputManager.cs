using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private Controls _controls;
    
    // make properties for our controls
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    
    public InputAction ShootAction { get; private set; }
    public InputAction AimAction { get; private set; }
    public InputAction ReloadAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    
    public bool ShootDown  { get; private set; }
    public bool AimDown  { get; private set; }
    
    public bool SprintDown { get; private set; }
    
    public bool ReloadPressed  { get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        
        _controls = new Controls();
        _controls.Enable();
        
        // initialize InputActions
        ShootAction = _controls.Locomotion.Shoot;
        AimAction = _controls.Locomotion.Aim;
        ReloadAction = _controls.Locomotion.Reload;
        JumpAction = _controls.Locomotion.Jump;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controls.Locomotion.Shoot.performed += context => ShootDown = true;
        _controls.Locomotion.Shoot.canceled += context => ShootDown = false;
        
        _controls.Locomotion.Reload.performed += context =>  ReloadPressed = true;
        _controls.Locomotion.Reload.canceled += context => ReloadPressed = false;
        
        _controls.Locomotion.Aim.performed += context => AimDown = true;
        _controls.Locomotion.Aim.canceled += context => AimDown = false;
        
        _controls.Locomotion.Sprint.performed += context => SprintDown = true;
        _controls.Locomotion.Sprint.canceled += context => SprintDown = false;
        
        _controls.Locomotion.Move.performed += context => Move = context.ReadValue<Vector2>();
        _controls.Locomotion.Look.performed += context => Look = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
