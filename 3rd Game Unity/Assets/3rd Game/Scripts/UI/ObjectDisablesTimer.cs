using UnityEngine;

public class ObjectDisablesTimer : MonoBehaviour
{
    public int setTimer;
    public float timer;

    private void Start()
    {
        timer = setTimer;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
            timer = setTimer;
        }
    }
}
