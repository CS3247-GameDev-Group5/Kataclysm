using UnityEngine;

public class PlayerCling : MonoBehaviour {

	BoxCollider pawCollider;
	public bool clinging = false;
	bool isLookingToCling = false;
	float clingDelay = 1f;
	float time = 0f;
	// Use this for initialization
	void Start () {
		print(transform.GetChild(1).GetComponentsInChildren<BoxCollider>().Length);
		foreach(var box in transform.GetChild(1).GetComponentsInChildren<BoxCollider>()) {
			if(box.tag == "Paw") {
				pawCollider = box;
			}
		}
		print(pawCollider.name);
		print(pawCollider.transform.root.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		if(time < 0) {
			isLookingToCling = false;
		}
	}

	public void Cling() {
		if(clinging) {
			if( time < 0 ) {
				letGoFromClingNode();
			}
		} else {
			time = clingDelay;
			isLookingToCling = true;
		}
	}

	void OnPawTriggerEnter(Collider other) {
		if(other.attachedRigidbody == null) {
			return;
		}
		if(other.tag == "ClingNode") {
			var dot = Vector3.Dot(other.transform.forward, transform.forward);
			if(dot < -0.9) {//facing almost opposite direction
				print("can cling");
				if(isLookingToCling) {
					clingToClingNode(other.gameObject);
				}
			}
		}
	}

	void clingToClingNode(GameObject clingNode) {
		//set paw as parent
		var player = pawCollider.transform.root.gameObject;
		print("clinged");
		var rb = player.GetComponent<Rigidbody>();
		var parentRB = clingNode.transform.root.GetComponent<Rigidbody>();
		var cg = rb.centerOfMass;

		GameObject.Destroy(rb);
		pawCollider.transform.SetParent(null);
		player.transform.SetParent(pawCollider.transform);
		pawCollider.transform.position = clingNode.transform.position;
		pawCollider.transform.SetParent(clingNode.transform);
		pawCollider.transform.Rotate(new Vector3(1f,0f, 0f), -30);
		pawCollider.enabled = false;
		clinging = true;
		parentRB.centerOfMass = cg;
	}

	void letGoFromClingNode() {
		print("released");
		var parentRB = transform.root.GetComponent<Rigidbody>();
		pawCollider.transform.Rotate(new Vector3(1f,0f, 0f), 30);
		pawCollider.transform.localPosition = new Vector3(pawCollider.transform.localPosition.x,
			pawCollider.transform.localPosition.y, pawCollider.transform.localPosition.z+0.25f);
		var player = pawCollider.transform.GetChild(0);
		player.SetParent(null);
		pawCollider.transform.SetParent(player.GetChild(0).transform);
		var rb = player.gameObject.AddComponent<Rigidbody>();;
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		clinging = false;
		pawCollider.enabled = true;
		parentRB.ResetCenterOfMass();
		rb.SendMessage("OnNewRigidBody",rb);
	}
}
