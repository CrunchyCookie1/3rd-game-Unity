using UnityEngine;

public class Mount : MonoBehaviour
{
    [SerializeField] private Transform mountPoint;
    [SerializeField] private Transform mount;

    [SerializeField] private bool onStartMount = true;

    private void Start()
    {
        if (onStartMount)
        {
            mount.gameObject.transform.SetParent(mountPoint.transform);
            mount.position = mountPoint.position;
            mount.rotation = mountPoint.rotation;
        }
    }

    public void MountObject()
    {
        mount.gameObject.transform.SetParent(mountPoint.transform);
        mount.position = mountPoint.position;
        mount.rotation = mountPoint.rotation;
    }

    public void DestroyMount()
    {
        Destroy(mount.gameObject);
    }
}
