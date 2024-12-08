using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private Animator anim;
    public PlayerBehaviours refer;
    private Vector3 blockStartPos;
    public float moveDistance = 1f;
    public float moveSpeed = 2f;
    public bool isBlockOnButton = false;

    void Start()
    {
        refer = FindObjectOfType<PlayerBehaviours>();
        anim = GetComponent<Animator>();
        anim.SetBool("state", false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            refer.zoneCheck++;
            anim.SetBool("state", true);
            blockStartPos = transform.position + new Vector3(0, 1f, 0);
            isBlockOnButton = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isBlockOnButton && other.gameObject.CompareTag("Block") && Physics2D.gravity.y < 0f)
        {
            // Adjust the block’s position without detaching or deactivating
            float newX = Mathf.Lerp(other.transform.position.x, transform.position.x, Time.deltaTime * moveSpeed);
            float newY = blockStartPos.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            other.transform.position = new Vector2(newX, newY);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            refer.zoneCheck--;
            anim.SetBool("state", false);
            isBlockOnButton = false;
        }
    }
}
