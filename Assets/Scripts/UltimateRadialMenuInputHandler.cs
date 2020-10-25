using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateRadialMenuInputHandler : MonoBehaviour
{
    public static UltimateRadialMenuInputHandler instance;

    public Transform tHandRayOrigin;
    public Vector2 input;
    public Camera camera;

    public Transform tQuad;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Plane plane = new Plane(tQuad.forward, tQuad.position);

        float enter;
        if (plane.Raycast(new Ray(tHandRayOrigin.position, tHandRayOrigin.forward), out enter))
        {
            Vector3 point = tHandRayOrigin.position + tHandRayOrigin.forward * enter;

            if (Vector3.Distance(point, tQuad.position) < tQuad.localScale.x/2f)
            {
                input = new Vector2(
                    Vector3.Dot(tQuad.right, point - tQuad.position) * 2 / tQuad.localScale.x,
                    Vector3.Dot(tQuad.up, point - tQuad.position) * 2 / tQuad.localScale.y
                    );
            }
            else
            {
                input = Vector2.zero;
            }


            /*
        Vector3 point = tHandRayOrigin.position + tHandRayOrigin.forward * enter;
        if (Vector3.Distance(point, transform.position) < transform.localScale.x) {
            input = new Vector2(
                Vector3.Dot(transform.right, point - transform.position) * 10f,
                Vector3.Dot(transform.up, point - transform.position) * 10f
                );
        } else
        {
            input = Vector2.zero;
        }*/
        } else
        {
            input = Vector2.zero;
        }
    }
}
