using UnityEngine;

public class SkyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float skyspeed;
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation",Time.time * skyspeed);
    }
}
