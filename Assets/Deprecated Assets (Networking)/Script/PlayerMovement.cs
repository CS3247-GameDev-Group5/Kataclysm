using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    public float movespeed = 0.2f;
    public float jumpStrength = 300f;

    Rigidbody rBody;
    private bool hasLanded = true;
    private bool isFalling = false;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Controls
        if (!isLocalPlayer)
            return;

        // Process Jump
        if (Input.GetKeyDown("space"))
            jump();

        // Move and Rotate
        var x = Input.GetAxis("Horizontal") * movespeed;
        var z = Input.GetAxis("Vertical") * movespeed;

        transform.Rotate(new Vector3(0, x * 10f, 0));
        transform.position += (transform.forward * z * movespeed);

        // Check Landed
        if (!hasLanded)
        {
            if (!isFalling)
            {
                if (rBody.velocity.y < 0)
                {
                    print("isFalling");
                    isFalling = true;
                }
            }
            else if (rBody.velocity.y > -Mathf.Epsilon)
            {
                print("hasLanded");
                hasLanded = true;
                isFalling = false;
            }
        }
    }

    void jump()
    {
        if (hasLanded)
        {
            hasLanded = false;
			isFalling = false;
            rBody.AddForce(Vector3.up * jumpStrength);
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}