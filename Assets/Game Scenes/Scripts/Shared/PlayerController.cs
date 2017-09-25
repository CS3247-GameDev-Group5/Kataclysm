using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles the movement of the player. Should be able to work with both Controller/Keyboard
// There is no method to detect controller mid-game. So what you want to play with should be set before playing the scene
// involving player. 
//
// Also, as of Now, local z is forward 
// Also also, in the input setting, vertical axis need to be inverted somehow... 

public class PlayerController : MonoBehaviour
{
  public float jumpForce = 10.0f;
  public float movementSpeed = 5.0f;
  public float rotateSpeed = 20.0f;
  public float maxVelocity = 3.0f;
  public int playerNumber = 0; // For Game Manager to identify

  Rigidbody playerRBody;
  float originalMass;
  bool isGrounded;
  bool isJumping;
  Vector3 movingForce;

  // Player Components
  Animator animator;
  PlayerCling clingComponent;
  AudioSource meowSound;

  InputDevice attachedDevice;

  // player Current facing angle
  //float currentAngle;

	Rigidbody playerAbove;
	Rigidbody playerBelow;
  bool isCharging = false;
  Vector3 chargeDir;
  float chargePushForce = 5.5f;
  List<Rigidbody> chargedList = new List<Rigidbody>();
  AudioSource pushMeow;
  void Awake()
  {
    // Setup default controls
    attachedDevice = VirtualController.inputDevices[0]; //get first 
  }

  void Start()
  {
    playerRBody = GetComponent<Rigidbody>();
    playerRBody.maxAngularVelocity = 3.0f;

    originalMass = playerRBody.mass;
    animator = GetComponentInChildren<Animator>();
    //print (animator.name);
    clingComponent = GetComponentInChildren<PlayerCling>();
    //print (clingComponent.name);
    var audioSources = GetComponents<AudioSource>();
    meowSound = GetComponent<AudioSource>();
    pushMeow = audioSources[1];
    isGrounded = false;
    isJumping = false;

    //currentAngle = getAngle(transform.forward.z, transform.forward.x);
  }

  public void storeDevice(InputDevice device)
  {
    attachedDevice = device;
  }

  void OnDisable()
  {
		seperateFromPlayerBelow();
    if (clingComponent.clinging)
    {
      clingComponent.Cling();
    }
    seperateFromPlayerAbove();
  }

  // Update is called once per frame
  void Update()
  {
    if (attachedDevice.GetButtonDown(MappedButton.Start))
    {
      // Pause? (Handled by LevelManager			
    }
    if (isCharging)
    {
      return;
    }
    // update player angle
    //currentAngle = getAngle(transform.forward.z, transform.forward.x);
    //print ("current angle " + currentAngle);

    // Get Button is frame dependent - JUMP
    if (isGrounded && attachedDevice.GetButtonDown(MappedButton.Jump))
    {
			seperateFromPlayerBelow();
      playerRBody.AddForce(new Vector3(0.0f, jumpForce, 0.0f), ForceMode.Impulse);
      isGrounded = false;
      isJumping = true;
      animator.SetBool("isJumping", isJumping);
      meowSound.Play();
    }

    if (attachedDevice.GetButtonDown(MappedButton.Cling))
    {
      // Primary
      print("cling");
      clingComponent.Cling();
    }
    // Charge!
    if (isGrounded && !isCharging && attachedDevice.GetButtonDown(MappedButton.Button4))
    {
			Debug.Log("charging");
      chargeDir = transform.forward * 5.0f + transform.up;
      chargeDir = Vector3.Normalize(chargeDir);
			seperateFromPlayerBelow();
      playerRBody.AddForce(chargeDir * 3.0f, ForceMode.Impulse);
      isCharging = true;
      pushMeow.Play();
      chargedList.Clear();
    }

    if (attachedDevice.GetButtonDown(MappedButton.LeftBumper))
    {
      // Left Strafe
      if (isGrounded)
      {
        Vector3 leftForce = transform.right * -movementSpeed * 0.1f;
				seperateFromPlayerBelow();
        playerRBody.AddForce(leftForce + new Vector3(0.0f, jumpForce * 0.5f, 0.0f), ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
        animator.SetBool("isJumping", isJumping);
        meowSound.Play();
      }
    }

    if (attachedDevice.GetButtonDown(MappedButton.RightBumper))
    {
      // Right Strafe
      if (isGrounded)
      {
        Vector3 rightForce = transform.right * movementSpeed * 0.1f;
				seperateFromPlayerBelow();
        playerRBody.AddForce(rightForce + new Vector3(0.0f, jumpForce * 0.5f, 0.0f), ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
        animator.SetBool("isJumping", isJumping);
        meowSound.Play();
      }
    }
  }

  void FixedUpdate()
  {
    float verticalAxis = attachedDevice.GetAxis(MappedAxis.MoveVertical);
    float horizontalLeftAxis = attachedDevice.GetAxis(MappedAxis.MoveHorizontal);
    float horizontalAxis = horizontalLeftAxis;

    //float horizontalAxis = attachedDevice.GetAxis (MappedAxis.LookHorizontal);
    //float verticalRightAxis = attachedDevice.GetAxis (MappedAxis.LookVertical);

    if (playerRBody == null)
    {
      return;
    }
    if (playerRBody.velocity.sqrMagnitude <= float.Epsilon)
    {
      isCharging = false;
    }
    if (playerRBody.velocity.y < -1.0f && playerBelow == null)
    {//bounce issue
      isGrounded = false;
			seperateFromPlayerBelow();
    }
    if (isCharging)
    {
      return;
    }
    // MOVEMENT
    if (isGrounded)
    {
			Quaternion a = Quaternion.LookRotation(transform.forward, transform.up);
			if(horizontalAxis != 0.0f || verticalAxis != 0.0f) {
				Vector3 targetV = (new Vector3(horizontalLeftAxis, 0.0f, verticalAxis)).normalized;
				Quaternion b = Quaternion.LookRotation(targetV, transform.up);
				Quaternion rot = Quaternion.Lerp(a, b, Time.fixedDeltaTime * rotateSpeed);
    		playerRBody.MoveRotation(rot);
				if(playerAbove != null) { 
					playerAbove.SendMessage("OnPlayerBelowRotate", rot * Quaternion.Inverse(a), SendMessageOptions.DontRequireReceiver);
				}
				if (Vector3.Dot(targetV, transform.forward) > 0.95f && playerRBody.velocity.magnitude < maxVelocity)
        {
				  seperateFromPlayerBelow();
          Vector3 movingForce = movementSpeed * transform.forward * Time.fixedDeltaTime * 100.0f;
          playerRBody.AddForce(movingForce, ForceMode.Force);
        }
			}
			/*
      float pressedAngle = getAngle(verticalAxis, horizontalLeftAxis);

      if (pressedAngle >= 1.0f || pressedAngle <= 0.0f)
      {
        float pressedOpposite = pressedAngle + 180;
        float currentOpposite = currentAngle + 180;
        if (pressedOpposite >= 360)
        {
          pressedOpposite -= 360;
        }
        if (currentOpposite >= 360)
        {
          currentOpposite -= 360;
        }
        if (Mathf.Abs(currentAngle - pressedAngle) > 5.0f)
        {

          bool isPressedOpposite = false;
          bool isCurrentOpposite = false;

          // check if pressedAngle is between 0 - 180
          if (pressedAngle >= 180 && pressedAngle < 360)
          {
            isPressedOpposite = true;
          }
          // check if current is between 0 - 180
          if (currentAngle >= 180 && currentAngle < 360)
          {
            isCurrentOpposite = true;
          }

          if (isPressedOpposite && isCurrentOpposite)
          {
            if (currentOpposite >= pressedOpposite)
            {
              // turn cw
              turnClockWise();
            }
            else
            {
              // turn acw
              turnAntiClockWise();
            }
          }
          else if (isPressedOpposite && !isCurrentOpposite)
          {
            if (currentAngle >= pressedOpposite)
            {
              // turn acw
              turnAntiClockWise();
            }
            else
            {
              // turn cw
              turnClockWise();
            }
          }
          else if (!isPressedOpposite && isCurrentOpposite)
          {
            if (currentOpposite >= pressedAngle)
            {
              // turn acw
              turnAntiClockWise();
            }
            else
            {
              // turn cw
              turnClockWise();
            }
          }
          else if (!isPressedOpposite && !isCurrentOpposite)
          {
            if (currentAngle >= pressedAngle)
            {
              // turn cw
              turnClockWise();

            }
            else
            {
              // turn acw
              turnAntiClockWise();
            }
          }
        }
        else
        {
          // stop rotation
          //stopRotation();
        }
        if (playerRBody.velocity.magnitude < maxVelocity)
        {
          Vector3 movingForce = movementSpeed * transform.forward * Time.fixedDeltaTime * 100;
          playerRBody.AddForce(movingForce, ForceMode.Force);
        }
      }
      else
      {
        // stop rotation
        //stopRotation();
      }*/

      /*if (pressedAngle > currentAngle && pressedAngle < currentOpposite) {
				if (Mathf.Abs (currentAngle - pressedAngle) > 5.0f) {
					// acw
					Quaternion newRotation = Quaternion.Euler (transform.up * -3.0f);
					playerRBody.MoveRotation (playerRBody.rotation * newRotation);
				}
			} else {
				if (Mathf.Abs (currentAngle - pressedAngle) > 5.0f) {
					//cw
					Quaternion newRotation = Quaternion.Euler (transform.up * 3.0f);
					playerRBody.MoveRotation (playerRBody.rotation * newRotation);
				}
			}*/

      /*if (verticalAxis != 0.0f && playerRBody.velocity.magnitude < maxVelocity) {
				Vector3 movingForce = verticalAxis * movementSpeed * transform.forward * Time.fixedDeltaTime;
				playerRBody.AddForce (movingForce, ForceMode.Impulse);
			}*/
    }
		float mag = playerRBody.velocity.magnitude;
    // Velocity Check
    if (mag < 0.01f || playerBelow == null)
    {
      animator.SetFloat("velocity", mag);
    }

    // ROTATION
    //Quaternion newRotation = Quaternion.Euler( transform.up * horizontalAxis * rotateSpeed);
    //playerRBody.MoveRotation (playerRBody.rotation * newRotation);

    //		print ("drag " + playerRBody.drag);
    //		print ("isBouncing " + isBouncing);
    //		print ("isJumping " + isJumping);
    //		print ("isGrounded " + isGrounded);
  }
  void OnCollisionEnter(Collision collisionInfo)
  {
    // if contacted below
    if (collisionInfo.contacts[0].point.y < transform.position.y
      && Vector3.Dot(collisionInfo.contacts[0].normal, new Vector3(0f, 1f, 0f)) > 0.9)
    {
      isGrounded = true;
      isJumping = false;
      animator.SetBool("isJumping", false);
    }
    if (isCharging)
    {
      chargePush(collisionInfo);
    }
  }

  void chargePush(Collision collisionInfo)
  {
    if (collisionInfo.gameObject.CompareTag("Player") && !chargedList.Contains(collisionInfo.collider.attachedRigidbody))
    {
      chargedList.Add(collisionInfo.collider.attachedRigidbody);
      Vector3 force = collisionInfo.contacts[0].point - transform.position;
      force.y = 0.0f;
      force = Vector3.Normalize(force);
      float dot = Vector3.Dot(force, transform.forward);
      Debug.Log(dot);
      if (dot > 0.1)
      {
        playerRBody.velocity = new Vector3(0, 0, 0);
        collisionInfo.gameObject.GetComponent<Rigidbody>().mass = originalMass; //cats have same mass
        collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(force * chargePushForce * dot, ForceMode.Impulse);
      }
    }
  }
  void OnNewRigidBody(Rigidbody rb)
  {
    playerRBody = rb;
  }

  void OnGUI()
  {
    /*GUIStyle myGUIStyle = new GUIStyle();
    myGUIStyle.font = GUI.skin.font;
    myGUIStyle.font.material.color = Color.black;
    GUILayout.BeginArea(new Rect(Screen.width - 130, 10, 200, 100));
    string str = "";
    str += "velocity: ";
    str += (playerRBody != null) ? playerRBody.velocity.ToString() + "\n" : "(null)\n";
    str += "isGrounded: " + isGrounded.ToString() + "\n";
    str += "isCharging: ";
    str += isCharging.ToString() + "\n";
    GUILayout.Label(str, myGUIStyle);
    GUILayout.EndArea();*/
  }

  public InputDevice getAttachedDevice()
  {
    return attachedDevice;
  }

	void seperateFromPlayerBelow() {		
		if (playerBelow != null)
		{
			playerRBody.mass = originalMass;
			playerBelow.SendMessage("OnSeperateFromAbove", SendMessageOptions.DontRequireReceiver);
			playerBelow = null;
		}
	}
	void seperateFromPlayerAbove() {		
		if (playerAbove != null)
		{
			playerAbove.SendMessage("OnSeperateFromBelow", SendMessageOptions.DontRequireReceiver);
			playerAbove = null;
		}
	}

  void OnPlayerBelowTriggerStay(Rigidbody otherPlayerRB)
  {
    if (isGrounded)
    {
			playerRBody.mass = 0.01f;
      if(playerBelow != otherPlayerRB) {
        seperateFromPlayerBelow();
      }
			playerBelow = otherPlayerRB;
      otherPlayerRB.SendMessage("OnPlayerAboveTriggerStay", playerRBody);  }
  }
	public void OnPlayerBelowRotate(Quaternion rot) 
  {		
		playerRBody.MoveRotation(Quaternion.LookRotation(playerRBody.transform.forward, playerRBody.transform.up) * rot);
		// if(playerAbove != null && playerAbove != playerRBody) { 
    //   playerAbove.gameObject.GetComponent<PlayerController>().OnPlayerBelowRotate(rot);
		// }
	}
	void OnPlayerAboveTriggerStay(Rigidbody otherPlayerRB) 
	{		
			playerAbove = otherPlayerRB;
	}
	void OnSeperateFromBelow() 
  {
		playerRBody.mass = originalMass;
		playerBelow = null;
	}
	void OnSeperateFromAbove() 
  {
		playerAbove = null;
	}

  public float getAngle(float opposite, float adjacent)
  {
    float degreeAngle = Mathf.Rad2Deg * Mathf.Atan(opposite / adjacent);

    if (adjacent > 0 && opposite > 0)
    {
    }
    else if (adjacent < 0 && opposite > 0)
    {
      degreeAngle = 180 + degreeAngle;
    }
    else if (adjacent < 0 && opposite <= 0)
    {
      degreeAngle = 180 + degreeAngle;
    }
    else if (adjacent >= 0 && opposite < 0)
    {
      degreeAngle = 360 + degreeAngle;
    }

    return degreeAngle;
  }

  public void turnClockWise()
  {
    Quaternion newRotation = Quaternion.Euler(transform.up * rotateSpeed);
    playerRBody.MoveRotation(playerRBody.rotation * newRotation);
  }

  public void turnAntiClockWise()
  {
    Quaternion newRotation = Quaternion.Euler(transform.up * -rotateSpeed);
    playerRBody.MoveRotation(playerRBody.rotation * newRotation);
  }

  public void stopRotation()
  {
    Quaternion newRotation = Quaternion.Euler(transform.up * 0.0f);
    playerRBody.MoveRotation(playerRBody.rotation * newRotation);
  }
}
