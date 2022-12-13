using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    GameObject pointDot;
    [SerializeField] float maxLaserDistance;
    [SerializeField] LayerMask laserMask;
    [SerializeField] Vector3 raycastDirection;
    [SerializeField] float sizeCoefficient;
    [SerializeField] Material laserMaterial;
    private void Start()
    {
        pointDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointDot.GetComponent<Collider>().enabled = false;
        pointDot.GetComponent<Renderer>().material = laserMaterial;
    }

    private void Update()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, transform.TransformDirection(raycastDirection), out hitInfo,maxLaserDistance ,laserMask);
        if (hitInfo.collider)
        {
            pointDot.SetActive(true);
            pointDot.transform.position = hitInfo.point;
            pointDot.transform.localScale = Vector3.one * 0.1f * sizeCoefficient * (hitInfo.distance / hitInfo.distance);
        }
        else
        {
            pointDot.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(raycastDirection));
    }

}
