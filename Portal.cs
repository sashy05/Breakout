using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform[] portalsA;
    public Transform[] portalsB;
    private PlayerBehaviours refer;
    private bool isCooldown = false;
    private float cooldownTime = 1f;
    void Start()
    {
        refer = FindAnyObjectByType<PlayerBehaviours>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Portal") && !isCooldown)
        {
            int index = System.Array.IndexOf(portalsA, other.transform);

            if (index != -1 && index < portalsB.Length)
            {
                TeleportPlayer(portalsB[index]);
            }
            else
            {
                index = System.Array.IndexOf(portalsB, other.transform);

                if (index != -1 && index < portalsA.Length) 
                {
                    TeleportPlayer(portalsA[index]);
                }
            }
        }
    }

    private void TeleportPlayer(Transform destinationPortal)
    {
        if (destinationPortal != null)
        {
            refer.DetachObject();
            transform.position = destinationPortal.position;
            StartCoroutine(StartCooldown());
        }
    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
