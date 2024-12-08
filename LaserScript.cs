using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserScript : MonoBehaviour
{
    private Animator anim;
    private bool isLaserActive = false;
    private bool inBeamZone = false;
    public PlayerBehaviours refer;
    private bool enableLaser = true;
    public Transform firePoint;
    public Camera cam;
    public LineRenderer laser;
    private Quaternion rotation;
    public LayerMask layer;
    public GameObject startVFX;
    public GameObject endVFX;
    public LayerMask enemyLayer;
    private List<ParticleSystem> particles = new List<ParticleSystem>();
    public GameObject areaEffector;

    // Start is called before the first frame update
    void Start()
    {
        refer = FindObjectOfType<PlayerBehaviours>();
        areaEffector.GetComponent<AreaEffector2D>().enabled = false;
        areaEffector.GetComponent<ParticleSystem>().Stop();
        FillList();
        DisableLaser();
    }


    // Update is called once per frame
    void Update()
    {
        if (enableLaser)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                EnableLaser();
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                UpdateLaser();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                DisableLaser();
            }
            RotateBot();            
        }
    }
    void EnableLaser()
    {
        laser.enabled = true;
        isLaserActive = true;
        CheckBeamZone();
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
    }
    void UpdateLaser()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;
        laser.SetPosition(0, (Vector2)firePoint.position);
        startVFX.transform.position = (Vector2)firePoint.position;
        float laserLength = 100f;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)firePoint.position, direction, laserLength, ~layer);
        if (hit)
        {
            laser.SetPosition(1, hit.point);
        }
        else
        {
            laser.SetPosition(1, (Vector2)firePoint.position + direction * laserLength);
        }
        endVFX.transform.position = laser.GetPosition(1);
        CheckBeamZone(hit);
    }
    void DisableLaser()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;
        float laserLength = 100f;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)firePoint.position, direction, laserLength, ~layer);
        laser.enabled = false;
        isLaserActive = false;
        if (inBeamZone)
        {
            anim = hit.collider.gameObject.GetComponent<Animator>();
            anim.SetBool("state",false);
            areaEffector.GetComponent<AreaEffector2D>().enabled = false;
            areaEffector.GetComponent<ParticleSystem>().Stop();
            inBeamZone = false;
        }
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
    }

    void CheckBeamZone(RaycastHit2D hit = default)
    {
        if (hit.collider != null && hit.collider.CompareTag("BeamZone"))
        {
            anim = hit.collider.gameObject.GetComponent<Animator>();
            if (!inBeamZone)
            {
                anim.SetBool("state",true);
                areaEffector.GetComponent<AreaEffector2D>().enabled = true;
                areaEffector.GetComponent<ParticleSystem>().Play();
                inBeamZone = true;
            }
        }
        else if (inBeamZone)
        {
            if(hit.collider.gameObject.GetComponent<Animator>()==null)
            {
                GameObject beamZoneObj = GameObject.FindWithTag("BeamZone");
                anim = beamZoneObj.GetComponent<Animator>();
                anim.SetBool("state",false);
                areaEffector.GetComponent<AreaEffector2D>().enabled = false;
                areaEffector.GetComponent<ParticleSystem>().Stop();
                inBeamZone = false;
            }          
        }
    }

    void RotateBot()
    {
        Vector2 direction = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (transform.parent != null && transform.parent.localScale.x < 0)
        {
            direction *= -1;
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotation.eulerAngles = new Vector3(0, 0, angle);
        transform.rotation = rotation;
        startVFX.transform.position = (Vector2)firePoint.position;
        endVFX.transform.position = laser.GetPosition(1);
    }

    void FillList()
    {
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particles.Add(ps);
            }
        }

        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particles.Add(ps);
            }
        }
    }
}
