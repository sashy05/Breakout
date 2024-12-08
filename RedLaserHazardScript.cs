using System.Collections;
using UnityEngine;

public class LineAndParticlesController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private ParticleSystem[] particleSystems;
    private Collider2D colliderObj;
    private bool isGrowing=false;
    private Vector2 startPoint, endPoint;
    private bool isXConstant;
    [SerializeField] private float growDuration=0.1f;
    [SerializeField] private float interval=2f;
    [SerializeField] private float startOffset=0f;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        colliderObj = GetComponentInChildren<Collider2D>();
        startPoint = lineRenderer.GetPosition(0);
        endPoint = lineRenderer.GetPosition(1);
        isXConstant = Mathf.Approximately(startPoint.x, endPoint.x);
        StartCoroutine(DelayedStart());
    }

    void Update()
    {

    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startOffset);
        StartCoroutine(AnimateLoop());
    }
    private IEnumerator AnimateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            StartCoroutine(AnimateLineAndParticles());
        }
    }
    private IEnumerator AnimateLineAndParticles()
{
    isGrowing = !isGrowing;
    colliderObj.enabled = isGrowing;

    // Handle particle systems
    foreach (var particle in particleSystems)
    {
        if (isGrowing)
            particle.Play();
        else
            particle.Stop();
    }

    float elapsed = 0f;
    while (elapsed < growDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / growDuration;

        // Adjust the interpolation direction based on growing or shrinking
        Vector2 newEndPoint = isGrowing ? Vector2.Lerp(startPoint, endPoint, t) : Vector2.Lerp(endPoint, startPoint, t);

        // Update the LineRenderer's endpoint
        lineRenderer.SetPosition(1, new Vector3(newEndPoint.x, newEndPoint.y, 0));
        yield return null;
    }

    // Ensure the endpoint is set correctly at the end of the animation
    if (isGrowing)
        lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));
    else
        lineRenderer.SetPosition(1, new Vector3(startPoint.x, startPoint.y, 0));
}

}
