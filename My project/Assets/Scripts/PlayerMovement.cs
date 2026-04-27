using UnityEngine;
using UnityOSC;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Start () 
    {
        OSCHandler.Instance.Init ();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", "ready");
    }

    void Update()
    {
        // OSC Receiever
        OSCHandler.Instance.UpdateLogs();
        Dictionary<string, ServerLog> servers = OSCHandler.Instance.Servers;

        foreach (KeyValuePair<string, ServerLog> item in servers)
        {
            if (item.Value.log.Count > 0)
            {
                int lastPacketIndex = item.Value.packets.Count - 1;

                string address = item.Value.packets[lastPacketIndex].Address.ToString();
                string data = item.Value.packets[lastPacketIndex].Data[0].ToString();
            }
        }

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 1);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
