using UnityEngine;

public class PawCollliderBubbler : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		try {
			this.GetComponent<Collider>().attachedRigidbody.SendMessage("OnPawTriggerEnter",other, SendMessageOptions.DontRequireReceiver);
		} catch (System.Exception e) {
			Debug.LogWarning (e.ToString());
		}
	}
}
