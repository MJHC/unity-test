using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public Camera camera;
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 90f;
    public float maxSpeed = 5;
    public float maxSprintSpeed = 12;
    public float jumpSpeed = 2;
    public float gravity = Physics.gravity.y;
    
    private float _rotationY = 0f;
    private float _rotationX = 0f;

    private float ySpeed;
    
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _fireAction;
    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["move"];
        _lookAction = _playerInput.actions["look"];
        _jumpAction = _playerInput.actions["jump"];
        _sprintAction = _playerInput.actions["sprint"];
        _fireAction = _playerInput.actions["fire"];

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = _moveAction.ReadValue<Vector2>();
        
        float sprint = _sprintAction.ReadValue<float>();
        float speed = maxSpeed + sprint * (maxSprintSpeed - maxSpeed);

        ySpeed += gravity * Time.deltaTime;

        if (controller.isGrounded)
        {
            ySpeed = -0.5f;
            if (_jumpAction.triggered)
                ySpeed = jumpSpeed;
        }
        else
        {
            controller.stepOffset = 0;
        }

        Vector3 velocity = new Vector3(move.x, ySpeed, move.y) * speed;
        
        velocity = transform.TransformDirection(velocity);
    
        controller.Move( velocity * Time.deltaTime) ;
        
        MouseLook();
        
        checkFire();
    }

    void MouseLook()
    {
        Vector2 look = _lookAction.ReadValue<Vector2>();
    
        _rotationY += look.x * mouseSensitivity * Time.deltaTime;
        _rotationX += -look.y * mouseSensitivity * Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, -clampAngle, clampAngle);
    
        transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
        camera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }

    private void checkFire()
    {
        if (_fireAction.triggered)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward),
                out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(camera.transform.position,
                    camera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 10);
                Debug.Log("Did Hit");
                if(hit.rigidbody != null)
                    hit.rigidbody.velocity = transform.TransformDirection(Vector3.forward * 10);
                if (hit.collider.gameObject.GetComponent<Enemy>() != null)
                    hit.collider.gameObject.GetComponent<Enemy>().health -= 10f;
            }
            else
            {
                Debug.DrawRay(camera.transform.position, camera.transform.TransformDirection(Vector3.forward) * 1000,
                    Color.white, 10);
                Debug.Log("Did not Hit");
            }
        }
    }
}
