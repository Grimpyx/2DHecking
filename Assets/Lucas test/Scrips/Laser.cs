using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public float bladeLength = 2;
    public float turnOnSpeed = 1f;

    private bool isAnimating = false;
    
    // Start is called before the first frame update
    void Start()
    {
        DisableLaser();
    }


    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame && !lineRenderer.enabled && !isAnimating)
        {
            EnableLaser();
        }

        else if(Mouse.current.leftButton.wasPressedThisFrame && lineRenderer.enabled && !isAnimating)
        {
            DisableLaser();
        }
    }

    void EnableLaser()
    {
        StartCoroutine(nameof(TurnOnAnim));
    }

    void DisableLaser()
    {
        StartCoroutine(nameof(TurnOffAnim));
    }

    /// <summary>
    /// Turns on the line renderer and then gradually lengthens the blade until it reaches "bladeLength".
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnOnAnim()
    {
        isAnimating = true;
        lineRenderer.enabled = true;

        Vector3 curPos = Vector3.zero;
        while (curPos.magnitude < bladeLength)
        {
            curPos = lineRenderer.GetPosition(1);

            curPos += Time.deltaTime * turnOnSpeed * Vector3.up;

            lineRenderer.SetPosition(1, curPos);

            yield return null;
        }
        curPos = bladeLength * Vector3.up;
        lineRenderer.SetPosition(1, curPos);

        isAnimating = false;

        StopCoroutine(nameof(TurnOnAnim));
    }

    /// <summary>
    /// Gradually shortens the blade and then turns off the line renderer.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnOffAnim()
    {
        isAnimating = true;

        Vector3 curPos = 30000f * Vector3.up;
        while (curPos.magnitude > 0 && Vector3.Angle(curPos, Vector3.up) < 90)
        {
            curPos = lineRenderer.GetPosition(1);

            curPos -= Time.deltaTime * turnOnSpeed * Vector3.up;

            lineRenderer.SetPosition(1, curPos);

            yield return null;
        }
        curPos = Vector3.zero;
        lineRenderer.SetPosition(1, curPos);

        lineRenderer.enabled = false;

        isAnimating = false;

        StopCoroutine(nameof(TurnOffAnim));
    }

}
