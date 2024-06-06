using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private GroundTile groundtile;

    public float RunSpeed = 20f;
    private float HorizontalSpeed = 10f;
    public float speedIncrease = 0.25f;
    public float maxSpeed = 45f;

    public Rigidbody rb;

    private int currentLane = 0;
    private float targetX;

    [SerializeField] private float JumpForce = 350;

    bool isAlive = true;
    bool isJumping = false;

    private Vector2 startTouchPos;
    private Vector2 startTouchPosMouse;
    private Vector2 currentTouchPos;
    private Vector2 currentTouchPosMouse;
    private bool stopTouch = false;
    private bool isSwiping = false;

    private float swipeRange = 500;
    private float swipeRangeTouch = 50;

    int scoreMultiplier = 1;

    public bool magnetActive = false;

    public GameObject player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        //magnetActive = true;
        targetX = rb.position.x;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void FixedUpdate()
    {
        MoveForward();
        MoveToTargetLane();
    }


    private void MoveForward()
    {
        Vector3 forwardMovement = transform.forward * RunSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMovement);
    }


    private void MoveToTargetLane()
    {
        Vector3 targetPosition = new Vector3(targetX, rb.position.y, rb.position.z);
        rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, HorizontalSpeed * Time.deltaTime));
    }


    // Update is called once per frame
    void Update()
    {
        //ArrowKeyDetection();

#if UNITY_EDITOR || UNITY_WEBGL
        //SwipeDetection();
        ArrowKeyDetection();
#else
        TouchSwipeDetection();
#endif
    }


    void ArrowKeyDetection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
    }


    void SwipeDetection()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0)
        {
            if (!isSwiping)
            {
                startTouchPosMouse = Input.mousePosition;
                isSwiping = true;
            }
            else if (isSwiping)
            {
                MouseSwipeDetection();
            }
        }
    }


    void MouseSwipeDetection()
    {
        currentTouchPosMouse = Input.mousePosition;
        Vector2 distance = currentTouchPosMouse - startTouchPosMouse;

            if (distance.x < -swipeRange)
            {
                ChangeLane(-1);
                isSwiping = false;
            }
            else if (distance.x > swipeRange)
            {
                ChangeLane(1);
                isSwiping = false;
            }
            else if (distance.y > swipeRange && !isJumping)
            {
                Jump();
                isSwiping = false;
            }
            else if (distance.y < -swipeRange)
            {
                Slide();
                isSwiping = false;
            }
    }


    void TouchSwipeDetection()
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    stopTouch = false;
                    break;

                case TouchPhase.Moved:
                    if (!stopTouch)
                    {
                        currentTouchPos = touch.position;
                        //StartPosDebugLandscape.text = "StartPos: " + startTouchPos;
                        //StartPosDebugPortrait.text = "StartPos: " + startTouchPos;
                        //CurrentPosDebugLandscape.text = "CurrentPos: " + currentTouchPos;
                        //CurrentPosDebugPortrait.text = "CurrentPos: " + currentTouchPos;
                        Vector2 distance = currentTouchPos - startTouchPos;

                        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
                        {
                            if (distance.x > swipeRangeTouch)
                            {
                                ChangeLane(1);
                                stopTouch = true;
                            }
                            else if (distance.x < -swipeRangeTouch)
                            {
                                ChangeLane(-1);
                                stopTouch = true;
                            }
                        }
                        else
                        {
                            if (distance.y > swipeRangeTouch && !isJumping)
                            {
                                Jump();
                                stopTouch = true;
                            }
                            else if (distance.y < -swipeRangeTouch)
                            {
                                Slide();
                                stopTouch = true;
                            }
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    stopTouch = false;
                    break;
            }
        }
    }


    private void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        targetLane = Mathf.Clamp(targetLane, -1, 1);

        if (targetLane != currentLane)
        {
            targetX = targetLane * 3f;
            currentLane = targetLane;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && GetComponent<CapsuleCollider>().center.y == 3.1f && isAlive)
        {
            isJumping = false;
            GetComponent<Animation>().Play("Run");
        }

        if (collision.gameObject.name == "Graphic" && collision.collider == collision.gameObject.GetComponent<BoxCollider>())
        {
            SoundManager.PlaySound("Crash");
            isAlive = false;
            RunSpeed = 0f;
            collision.rigidbody.AddForce(new Vector3(1,1,1) * 5f, ForceMode.Impulse);
            collision.rigidbody.AddTorque(new Vector3(1, 0, 5), ForceMode.Impulse);
            GetComponent<Animation>().Play("Dizzy");
            StartCoroutine(GameManager.MyInstance.Dead());
        }

        if (collision.gameObject.name == "Star(Clone)")
        {
            SoundManager.PlaySound("PowerUp");
            Destroy(collision.gameObject);

            StartCoroutine(TwoxScore());
        }

        IEnumerator TwoxScore()
        {
            float startTime = Time.unscaledTime;
            float waitTime = 5f;

            while (Time.unscaledTime - startTime < waitTime)
            {
                scoreMultiplier *= 2;
                yield return null;
                scoreMultiplier = scoreMultiplier / 2;
            }
        }

        if(collision.gameObject.name == "Magnet(Clone)")
        {
            SoundManager.PlaySound("PowerUp");
            Destroy(collision.gameObject);

            StartCoroutine(Magnet());
        }

        IEnumerator Magnet()
        {
            magnetActive = true;

            float startTime = Time.unscaledTime;
            float waitTime = 5f;
            
            while (Time.unscaledTime - startTime < waitTime)
            {
                yield return null;
            }
            magnetActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Coin(Clone)")
        {
            SoundManager.PlaySound("Coin");
            Destroy(other.gameObject);
            GameManager.MyInstance.Score += scoreMultiplier * 1;
            if (RunSpeed < maxSpeed)
            {
                RunSpeed += speedIncrease;
            }
        }
    }

    void Jump()
    {
        SoundManager.PlaySound("Jump");
        isJumping = true;
        GetComponent<Animation>().Play("Runtojumpspring");
        rb.AddForce(Vector3.up * JumpForce);
    }

    void Slide()
    {
        //SoundManager.PlaySound("Slide");
        GetComponent<Animation>().Play("Runtoslide");
    }
}