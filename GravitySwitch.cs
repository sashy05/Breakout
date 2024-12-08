using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    [SerializeField] private Vector2 downGravity;
    [SerializeField] private Vector2 upGravity;
    public bool canSwitch = true;
    private float timer = 0;
    void Start()
    {
        Physics2D.gravity = downGravity;
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Space)&&canSwitch)
        {
            SwitchGravity();
        }
        if(!canSwitch)
        {
            timer+=Time.deltaTime;
            if(timer>2f)
            {
                canSwitch = true;
                timer = 0;
            }

        }
    }
    void SwitchGravity()
    {
        canSwitch = false;
        if (Physics2D.gravity == downGravity)
        {
            Physics2D.gravity = upGravity;
        }
        else
        {
            Physics2D.gravity = downGravity;
        }
    }
}
