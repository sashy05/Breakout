using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollowObject : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float flipYRotationTime = 0.5f;
    private PlayerBehaviours playerrefer;
    private Coroutine turnCoroutine;
    private bool isFacingRight;
    // Start is called before the first frame update
    void Awake()
    {
        playerrefer = Player.gameObject.GetComponent<PlayerBehaviours>();
        isFacingRight = playerrefer.isFacingRight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.position;
    }

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject,DetermineEndRotation(),flipYRotationTime).setEaseInOutSine();
        
    }
/*
turnCoroutine = StartCoroutine(FlipYLerp());
    IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation,endRotationAmount,(elapsedTime/flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f,yRotation,0f);
            yield return null;
        }
    }*/

    private float DetermineEndRotation()
    {
        isFacingRight = !isFacingRight;
        if(!isFacingRight)
        {
            return 180f;
        }
        else
            return 0f;
    }
}
