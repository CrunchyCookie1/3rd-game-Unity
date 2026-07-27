using UnityEngine;

public class LimitCamera : MonoBehaviour
{
    public GameObject Player;

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
    }
}
