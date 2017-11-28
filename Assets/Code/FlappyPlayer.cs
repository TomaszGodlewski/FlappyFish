using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class FlappyPlayer : MonoBehaviour 
{
    public float movingSpeed = 5;
    public float jumpForce = 10;
    public float additionalGravityForce = 5;
    public float rotationPercentage = 0.5f;
	public float assumeDeadBelowY = -20;
    float timer = 0f;
   
    static float bestTime = 0f;
	public float waitDurationBeforeRestart = 1f;

    Text timeCounterText;
    Text BestTimeCounterText;
	GameObject dieText;
	AudioSource scream;

    public Rigidbody body 
	{
        get;
        private set;
	}

	public bool IsDead() 
	{
		return state == State.DEAD;
	}

    bool inputAllowed;

    enum State
	{
        INTRO, MOVING, DEAD
    }

    State state;

    void Awake() 
	{
        textsInitialization();
		scream = GetComponent<AudioSource> ();
        body = GetComponent<Rigidbody>();
        state = State.INTRO;
        inputAllowed = true;
		body.useGravity = false;
    }

    void Update() 
	{
		// Input update has to execute during Update, while logic can run in FixedUpdate
		if (inputAllowed && state == State.MOVING)
        {
            UpdateInput();
            timeFlow();
		}
    }

    void FixedUpdate()
	{
        switch (state) 
		{
            case State.INTRO:
                UpdateIntro();
                break;
            case State.MOVING:
                UpdateMoving();
                ApplyAdditionalGravity();
                break;
            case State.DEAD:
                ApplyAdditionalGravity();
                break;
        }
    }

    bool Tapped() 
	{
        return Input.GetMouseButtonDown(0);
    }

    void UpdateIntro()
	{
		// Constant velocity until first tap
		body.velocity = Vector3.zero;
        if (Tapped()) 
		{
			body.useGravity = true;
            state = State.MOVING;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            GameObject.Find("IntroText").SetActive(false);
        }
    }

    void UpdateMoving() 
	{
		// Physics controls only Y axis of velocity
        body.velocity = new Vector3(movingSpeed, body.velocity.y, 0);
		// Additional effect inspired by Flappy Bird - rotation based on velocity
        transform.forward = Vector3.Lerp(Vector3.right, body.velocity, rotationPercentage);
		if (transform.position.y < assumeDeadBelowY) Die();		
    }

    void UpdateInput()
	{
        if (Tapped()) Jump();        
    }

    void Jump() 
	{
        body.velocity = new Vector3(movingSpeed, 0, 0);
        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
	{
        bestTimeResolver();
		//scream.Play ();
		dieText.SetActive (true);
		Die();
    }

	void Die() 
	{
		inputAllowed = false;
		state = State.DEAD;
		body.constraints = RigidbodyConstraints.None;
		StartCoroutine(RestartAfterSeconds(waitDurationBeforeRestart));
	}

    void ApplyAdditionalGravity() 
	{
        body.AddForce(Vector3.down * additionalGravityForce);
    }

	IEnumerator RestartAfterSeconds(float seconds) 
	{
		yield return new WaitForSeconds(seconds);
		SceneManager.LoadScene(0); // load first scene
	}

    void timeFlow()
    {
        timer += Time.deltaTime;
        timeCounterText.text = timer.ToString("0.00");
    }

    void textsInitialization()
    {
		dieText = GameObject.Find("Died");
		dieText.SetActive(false);
		timeCounterText = GameObject.Find("TimeCounterText").GetComponent<Text>();
		timeCounterText.text = "0";
		BestTimeCounterText = GameObject.Find("BestTimeCounterText").GetComponent<Text>();
		BestTimeCounterText.text = bestTime.ToString();
    }

    void bestTimeResolver()
    {
       bestTime = timer > bestTime ? timer : bestTime; 
    }
}