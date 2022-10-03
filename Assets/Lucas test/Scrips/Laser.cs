using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform swordPoint;
    public Camera cam;

    private Quaternion rotation;
    
    // Start is called before the first frame update
    void Start()
    {
        DisableLaser();
    }


    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame && !lineRenderer.enabled)
        {
            EnableLaser();
        }

        

        else if(Mouse.current.leftButton.wasPressedThisFrame && lineRenderer.enabled)
        {
            DisableLaser();
        }

        //RotateToMouse();

        //if (lineRenderer.enabled)
        //{
            //UpdateLaser();
        //}
    }

    void EnableLaser()
    {
        lineRenderer.enabled = true;
        
    }

    void DisableLaser() => lineRenderer.enabled = false;

    //void UpdateLaser()
    //{
        //var mousePos = (Vector2)cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        //lineRenderer.SetPosition(0, swordPoint.position);

        //lineRenderer.SetPosition(1, mousePos);
    //}

    //void RotateToMouse()
    //{
        //Vector2 direction = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rotation.eulerAngles = new Vector3(0, 0, angle);
        //transform.rotation = rotation;
    //}

}
