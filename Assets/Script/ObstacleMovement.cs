using UnityEngine;
using UnityEngine.Networking;

public class ObstacleMovement: NetworkBehaviour
{
    [SerializeField]
    public float movespeed = 1.5f;
    public float stopAtX = 0.0f;

    private bool startMoving = false;
    private Vector3 startPos;
	private Rigidbody rBody;

    void Start()
    {
        startPos = transform.position;
		rBody = GetComponent<Rigidbody> ();
    }

    void Update()
    {
        // Only Host Control
        if (!isServer)
            return;

        if (Input.GetKeyDown("q")) {
            if (transform.position.x >= stopAtX)
            {
                resetPos();
            }
            if (!startMoving)
            {
                startMoving = true;
				rBody.velocity = new Vector3(movespeed, 0, 0);
            }
        }

        if (Input.GetKeyDown("r"))
        {
            resetPos();
        }

        if (transform.position.x >= stopAtX)
        {
            startMoving = false;
			rBody.velocity = Vector3.zero;
        }
    }

    void resetPos()
    {
        transform.position = startPos;
        startMoving = false;
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}