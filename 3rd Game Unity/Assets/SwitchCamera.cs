using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
   public  GameObject timelineCaeera;
   public GameObject fpsCAmera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fpsCAmera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchCameras()
    {
        timelineCaeera.SetActive(false);
        fpsCAmera.SetActive(true);
        //Debug.LogError("Cameras not assigned!");
        return;
        /* if (cameraIntro == null || firstPersonCamera3 == null)
         {
             Debug.LogError("Cameras not assigned!");
             return;
         }

         if (hasSwapped)
         {
             Debug.Log("Already switched to First Person Camera3");
             return;
        */
    }
}