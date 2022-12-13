using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeRenderTextureCreator : MonoBehaviour
{

    [SerializeField] RenderTexture newRenTex;
    public int size;
    public Camera cam;
    Renderer img;
    // Start is called before the first frame update
    void OnEnable()
    {
        img = GetComponent<Renderer>();
        newRenTex = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32);
        newRenTex.Create();
        cam.targetTexture = newRenTex;
        img.material.SetTexture("_MainTex", newRenTex);
        Debug.Log("created render texture for camera " + cam.name);
        newRenTex.wrapMode = TextureWrapMode.Clamp;
    }
    private void Update()
    {
        
    }

    private void OnDisable()
    {
        newRenTex.Release();
        Destroy(newRenTex);
    }
}
