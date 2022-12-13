using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomShaderScreenPosition : MonoBehaviour
{
    [SerializeField] Renderer rend;
    public Camera refCam;
    // Update is called once per frame

    [SerializeField] bool useMainCam;

    [ExecuteAlways]
    void Update()
    {
        if (rend)
        {
            if (useMainCam)
            {
                Vector2 screenpix = Camera.main.WorldToScreenPoint(transform.position);
                screenpix = new Vector2(screenpix.x / Screen.width, screenpix.y / Screen.height);
                rend.material.SetVector("_objscreenpos", screenpix);
            }
            else if(refCam)
            {
                Vector2 screenpix = refCam.WorldToScreenPoint(transform.position);
                screenpix = new Vector2(screenpix.x / Screen.width, screenpix.y / Screen.height);
                rend.material.SetVector("_objscreenpos", screenpix);
            }
        }
    }
}
