using UnityEngine;
using UnityEngine.Networking;

public class AutoRespawn : NetworkBehaviour
{
    [SerializeField]
    public static float killZ = -10.0f;

    NetworkStartPosition startPosObj;
    Rigidbody rBody;

    void Start()
    {
        startPosObj = FindObjectOfType<NetworkStartPosition>();
        rBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(transform.position.y < killZ)
        {
            reset();
        }
    }

    void reset()
    {
        rBody.ResetInertiaTensor();
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = startPosObj.transform.position;
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}