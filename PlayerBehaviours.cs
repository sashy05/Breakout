using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Cinemachine;


public class PlayerBehaviours : MonoBehaviour
{
    public Vector2 safePosition;
    [SerializeField] private List<int> zones = new List<int>();
    [SerializeField] private List<GameObject> doors = new List<GameObject>();
    [SerializeField] private List<Light2D> lights = new List<Light2D>();
    [SerializeField] private Transform[] checkpoints;
    private Transform currentCheckpoint;
    private List<Transform> activatedCheckpoints = new List<Transform>();
    private CinemachineImpulseSource cinemachineImpulseSource;
    public GravitySwitch gravitySwitch;
    public int totalHealth = 50, currentHealth;
    public int zoneCheck = 0;
    public RoomCameras refer;
    public Rigidbody2D rb;
    private float X;
    public bool flag = false, isFacingRight = true;
    [SerializeField] private float speed;
    public FixedJoint2D joint;
    public LayerMask draggableLayer;
    public float rad = 1f, tpCooldown = 1f, flickerDuration = 1.0f, flickerSpeed = 0.1f, speedReduction;
    public TextMeshProUGUI completion;
    public Image img;
    private Rigidbody2D attachedObject = null;
    private SpriteRenderer sprite;
    //[SerializeField] private GameObject cameraFollowGO;
    //private CameraFollowObject cameraFollowObject;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform originObject;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        gravitySwitch = FindObjectOfType<GravitySwitch>();
        currentHealth = totalHealth;
        refer = FindObjectOfType<RoomCameras>();
        img.gameObject.SetActive(false);
        completion.gameObject.SetActive(false);
        joint = GetComponent<FixedJoint2D>();
        joint.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        //cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
        safePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (img.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        safePosition = transform.position;

        Dragger();

        if (Physics2D.gravity.y > 0f && !flag)
        {
            Vector3 localScale = transform.localScale;
            localScale.y *= -1f;
            transform.localScale = localScale;
            flag = !flag;
        }
        else if (Physics2D.gravity.y < 0f && flag)
        {
            Vector3 localScale = transform.localScale;
            localScale.y *= -1f;
            transform.localScale = localScale;
            flag = !flag;
        }
        if (Input.GetKeyDown(KeyCode.Space) && gravitySwitch.canSwitch)
        {
            cinemachineImpulseSource.GenerateImpulse();
        }

        DoorChecker();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void DoorChecker()
    {
        if (zoneCheck >= zones[refer.currentRoomIndex])
        {
            doors[refer.currentRoomIndex].gameObject.GetComponent<Collider2D>().enabled = false;
            lights[refer.currentRoomIndex].gameObject.GetComponent<Light2D>().color = Color.green;

        }
        else
        {
            doors[refer.currentRoomIndex].gameObject.GetComponent<Collider2D>().enabled = true;
            lights[refer.currentRoomIndex].gameObject.GetComponent<Light2D>().color = Color.red;
        }
    }
    void FixedUpdate()
    {
        X = Input.GetAxisRaw("Horizontal");
        FlipMovement();
        float moveSpeed = isGrounded() ? speed : speed * speedReduction;
        Vector2 currentVelocity = rb.velocity;
        float horizontalVelocity = Mathf.Lerp(currentVelocity.x, X * moveSpeed, 0.8f);
        rb.velocity = new Vector2(horizontalVelocity, currentVelocity.y);
    }


    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void FlipMovement()
    {
        if (X < 0f && isFacingRight)
        {
            FlipCharacter(180f);
        }
        else if (X > 0f && !isFacingRight)
        {
            FlipCharacter(0f);
        }
    }

    void FlipCharacter(float rotationY)
    {
        isFacingRight = !isFacingRight;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void Dragger()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, rad, draggableLayer);

        if (attachedObject != null)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                DetachObject();
                return;
            }

            Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(originObject.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Physics2D.gravity.y < 0f ? Mathf.Clamp(angle, 0f, 180f) : Mathf.Clamp(angle, -180f, 0f);

            Vector3 orbitPosition = originObject.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 2f;
            attachedObject.transform.position = Vector3.Lerp(attachedObject.transform.position, orbitPosition, Time.deltaTime * 5f);
        }

        if (nearbyObjects.Length > 0 && Input.GetKeyDown(KeyCode.Mouse1))
        {
            Rigidbody2D thatObject = nearbyObjects[0].GetComponent<Rigidbody2D>();
            ButtonScript buttonScript = thatObject.GetComponentInParent<ButtonScript>();
            if (thatObject != null && (buttonScript == null || !buttonScript.isBlockOnButton))
            {
                attachedObject = thatObject;
                //attachedObject.transform.SetParent(gameObject.transform, true);
                AttachToObject(attachedObject);
            }
        }
    }


    void AttachToObject(Rigidbody2D objectToAttach)
    {
        joint.enabled = true;
        joint.connectedBody = objectToAttach;
    }

    public void DetachObject()
    {
        if (joint.enabled)
        {
            joint.enabled = false;
            joint.connectedBody = null;
            attachedObject.transform.SetParent(null);
            attachedObject = null;

        }
    }


    public void Damage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            transform.position = currentCheckpoint.position;
            currentHealth = totalHealth;
        }
    }

    public void MakeFlicker()
    {
        StartCoroutine(Flicker());
    }
    private IEnumerator Flicker()
    {
        float endTime = Time.time + flickerDuration;
        while (Time.time < endTime)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(flickerSpeed);
        }
        sprite.enabled = true;
    }
    private Vector2 GetSafePosition(Vector2 hazardPosition)
    {
        Vector2 directionAway = (Vector2)transform.position - hazardPosition;
        directionAway.Normalize();
        return (Vector2)transform.position + directionAway * 1f;
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Finish"))
        {
            img.gameObject.SetActive(true);
            completion.gameObject.SetActive(true);
        }

        if (other.gameObject.CompareTag("Collectible"))
            Destroy(other.gameObject);

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            Transform checkpoint = other.transform;
            foreach (Transform validCheckpoint in checkpoints)
            {
                if (checkpoint == validCheckpoint)
                {
                    if (!activatedCheckpoints.Contains(checkpoint))
                    {
                        activatedCheckpoints.Add(checkpoint);
                        currentCheckpoint = checkpoint;
                    }
                    break;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            MakeFlicker();
            Vector2 hazardPosition = other.contacts[0].point;
            safePosition = GetSafePosition(hazardPosition);
            transform.position = safePosition;
            Damage(20);
        }
    }
}