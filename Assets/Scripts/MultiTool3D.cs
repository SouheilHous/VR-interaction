using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace MultiTool3D
{
    static class MTool
    {
        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, digits);
            return Mathf.Round(value * mult) / mult;
        }

        public static float RoundTo(float value, int digits)
        {
            return Mathf.Floor(value * digits) / digits;
        }
    }

    [System.Serializable]
    public struct RulerTool
    {
        // Cctor
        public RulerTool(float d, bool s, float sv)
        {
            from = Vector3.zero;
            to = new Vector3(0.0f, 2.0f, 0.0f);
            distance = Vector3.Distance(from, to);
            snap = s;
            snapValue = sv;
        }

        // Variables
        public Vector3 from;
        public Vector3 to;
        public float distance;
        public bool snap;
        public float snapValue;


    }

    public struct AngleTool
    {
        public AngleTool(Vector3 v)
        {
            from = Vector3.zero;
            p1 = new Vector3(-1.0f, 2.0f, 0.0f);
            p2 = new Vector3(1.0f, 2.0f, 0.0f);
        }

        public Vector3 from;
        public Vector3 p1;
        public Vector3 p2;

        public static float Smallest(float a, float b)
        {
            float smallest = a;
            if (b < a)
            {
                smallest = b;
            }

            return smallest;
        }
    }

    public struct VolumeTool
    {
        public VolumeTool(Bounds b)
        {
            bounds = b;
            snap = 0.0f;
            position = Vector3.zero;
            sizer = Vector3.one;
            radius = 1.0f;

            pY = 0.0f;
            p1 = p2 = p3 = p4 = Vector3.zero;

            p1 += new Vector3(-1, 0, 1);
            p2 += new Vector3(1, 0, 1);
            p3 += new Vector3(1, 0, -1);
            p4 += new Vector3(-1, 0, -1);
        }


        //Box
        public Bounds bounds;
        public float snap;

        //Sphere
        public Vector3 position;
        public Vector3 sizer;
        public float radius;

        //Plane
        public float pY;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector3 p4;

        public static MultiTool3D multiTool3D;

        public static float FindArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float s1, s2, s3;

            s1 = Vector3.Distance(p1, p2);
            s2 = Vector3.Distance(p2, p3);
            s3 = Vector3.Distance(p3, p1);

            float t = (s1 + s2 + s3) / 2;
            float final = Mathf.Sqrt(t * (t - s1) * (t - s2) * (t - s3));

            return final;
        }

        public static void DrawCube(Vector3 position, Vector3 size)
        {
            Vector3 half = size / 2;

            if (multiTool3D != null)
            {
                multiTool3D = GameObject.Find("MultiTool3D").GetComponent<MultiTool3D>();
            }

            /*multiTool3D.tVolumeCube.position = position;
            multiTool3D.tVolumeCube.localScale = size;*/

            // Front
            /*Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, half.z), position + new Vector3(-half.x, half.y, half.z));
            // Back
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, -half.y, -half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, -half.z));
            // Corners
            Handles.DrawLine(position + new Vector3(-half.x, -half.y, -half.z), position + new Vector3(-half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, -half.y, -half.z), position + new Vector3(half.x, -half.y, half.z));
            Handles.DrawLine(position + new Vector3(-half.x, half.y, -half.z), position + new Vector3(-half.x, half.y, half.z));
            Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, half.y, half.z));*/
        }
    }

    //[ExecuteInEditMode]
    public class MultiTool3D : MonoBehaviour
    {
        public enum VOLUME_MODE
        {
            Box,
            Sphere,
            Plane
        }
        public enum MEASUREMENT
        {
            Meter,
            Inch,
            Feet,
            Yards,
            Miles
        }


        
        public Transform tRulerFrom;
        public Transform tRulerTo;
        public LineRenderer lrRuler;
        public TextMeshPro tmpRulerDistance;

        public Transform tAngleFrom;
        public Transform tAnglePoint1;
        public Transform tAnglePoint2;
        public LineRenderer lrAngle;
        public LineRenderer lrAngle1;
        public LineRenderer lrAngle2;
        public TextMeshPro tmpAngle;

        public Transform tVolumeCube;
        public TextMeshPro tmpVolume;
        public Transform tVolumeCubeCorner1;
        public Transform tVolumeCubeCorner2;
        public Transform tVolumeSphere;
        public Transform tVolumeSphereRadius;
        public Transform tVolumeSpherePosition;
        public LineRenderer lrVolumePlaneLine;
        public LineRenderer lrVolumePlaneDiagonal;
        public Transform tVolumePlanePoint1;
        public Transform tVolumePlanePoint2;
        public Transform tVolumePlanePoint3;
        public Transform tVolumePlanePoint4;




        /* - GUI - */
        Color color = Color.magenta;
        int decimals = 2;

        public RulerTool ruler = new RulerTool(0.0f, false, 0.5f);
        public AngleTool angle = new AngleTool(Vector3.zero);
        public VolumeTool volume = new VolumeTool(new Bounds(Vector3.zero, Vector3.one));
        public VOLUME_MODE VMODE = VOLUME_MODE.Box;
        public MEASUREMENT MEASURE = MEASUREMENT.Meter;

        public bool rulerToggle = true;
        public bool angleToggle = false;
        public bool volumeToggle = false;
        bool rotateToggle = false;

        Vector3 rotMover = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {

            UpdateVisual();
        }

        // Update is called once per frame
        void UpdateVisual()
        {
            if (rulerToggle) { RTool(MEASURE); }

            tRulerFrom.gameObject.SetActive(rulerToggle);
            tRulerTo.gameObject.SetActive(rulerToggle);
            lrRuler.gameObject.SetActive(rulerToggle);
            tmpRulerDistance.gameObject.SetActive(rulerToggle);

            if (angleToggle) { ATool(); }

            tAngleFrom.gameObject.SetActive(angleToggle);
            tAnglePoint1.gameObject.SetActive(angleToggle);
            tAnglePoint2.gameObject.SetActive(angleToggle);
            lrAngle.gameObject.SetActive(angleToggle);
            lrAngle1.gameObject.SetActive(angleToggle);
            lrAngle2.gameObject.SetActive(angleToggle);
            tmpAngle.gameObject.SetActive(angleToggle);

            if (volumeToggle) { 
                VTool(VMODE, MEASURE); 
            } else
            {
                tVolumeCube.gameObject.SetActive(volumeToggle);
                tmpVolume.gameObject.SetActive(volumeToggle);
                tVolumeCubeCorner1.gameObject.SetActive(volumeToggle);
                tVolumeCubeCorner2.gameObject.SetActive(volumeToggle);
                tVolumeSphere.gameObject.SetActive(volumeToggle);
                tVolumeSphereRadius.gameObject.SetActive(volumeToggle);
                tVolumeSpherePosition.gameObject.SetActive(volumeToggle);
                lrVolumePlaneLine.gameObject.SetActive(volumeToggle);
                lrVolumePlaneDiagonal.gameObject.SetActive(volumeToggle);
                tVolumePlanePoint1.gameObject.SetActive(volumeToggle);
                tVolumePlanePoint2.gameObject.SetActive(volumeToggle);
                tVolumePlanePoint3.gameObject.SetActive(volumeToggle);
                tVolumePlanePoint4.gameObject.SetActive(volumeToggle);
            }



            Transform t = Selection.activeTransform;
            if (t != null && rotateToggle)
            {
                GOTool(t);
            }

            //sceneView.Repaint();
        }

        void RTool(MEASUREMENT Measure)
        {
            //Handles.color = color;

            // Move Handles
            //ruler.to = Handles.PositionHandle(ruler.to, Quaternion.identity);
            //ruler.from = Handles.PositionHandle(ruler.from, Quaternion.identity);
            ruler.from = tRulerFrom.position;
            ruler.to = tRulerTo.position;

            // Snapping
            if (ruler.snap)
            {
                ruler.to = new Vector3(MTool.RoundTo(ruler.to.x, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.to.y, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.to.z, Mathf.RoundToInt(1 / ruler.snapValue)));
                ruler.from = new Vector3(MTool.RoundTo(ruler.from.x, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.from.y, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.from.z, Mathf.RoundToInt(1 / ruler.snapValue)));
            }

            // Draw
            //Handles.DrawDottedLine(ruler.from, ruler.to, 10.0f);
            lrRuler.SetPositions(new Vector3[] { ruler.from, ruler.to });

            /*GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 20;*/

            // Distance
            float d = Vector3.Distance(ruler.from, ruler.to);

            string m = "";

            switch (Measure)
            {
                case MEASUREMENT.Meter:
                    m = "m";
                    break;
                case MEASUREMENT.Feet:
                    d = (d * 3.28084f);
                    m = "ft";
                    break;
                case MEASUREMENT.Inch:
                    d = (d * 39.3701f);
                    m = "Inc";
                    break;
                case MEASUREMENT.Miles:
                    d = (d * 0.000621371f);
                    m = "Mi";
                    break;
                case MEASUREMENT.Yards:
                    d = (d * 1.09361f);
                    m = "Yd";
                    break;
            }

            d = MTool.Round(d, decimals);
            //Handles.Label((ruler.from + ruler.to) / 2, d.ToString() + m, style);
            tmpRulerDistance.transform.position = (ruler.from + ruler.to) / 2;
            tmpRulerDistance.transform.LookAt(tmpRulerDistance.transform.position+Camera.main.transform.forward, Vector3.up);
            tmpRulerDistance.text = d.ToString() + m;
        }

        void ATool()
        {
            Handles.color = color;

            // Move Handles
            /*angle.from = Handles.PositionHandle(angle.from, Quaternion.identity);
            angle.p1 = Handles.PositionHandle(angle.p1, Quaternion.identity);
            angle.p2 = Handles.PositionHandle(angle.p2, Quaternion.identity);*/
            angle.from = tAngleFrom.position;
            angle.p1 = tAnglePoint1.position;
            angle.p2 = tAnglePoint2.position;

            // Draw
            /*Handles.DrawLine(angle.from, angle.p1);
            Handles.DrawLine(angle.from, angle.p2);*/
            lrAngle1.SetPositions(new Vector3[] { angle.from, angle.p1 });
            lrAngle2.SetPositions(new Vector3[] { angle.from, angle.p2 });


            Vector3 n, s1, s2;
            s1 = (angle.p2 - angle.from).normalized;
            s2 = (angle.p1 - angle.from).normalized;
            n = Vector3.Cross(s1, s2);

            float s1Length = Vector3.Distance(angle.from, angle.p1);
            float s2Length = Vector3.Distance(angle.from, angle.p2);
            float length = AngleTool.Smallest(s1Length, s2Length);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = 20;

            float a = Vector3.Angle(s1, s2);
            a = MTool.Round(a, decimals);
            char m = '°';

            //Handles.DrawWireArc(angle.from, n, (angle.p2 - angle.from).normalized, a, length / 2);
            lrAngle.positionCount = 21;
            Vector3 angleStart = (angle.p2 - angle.from).normalized * length / 2;
            Vector3 angleEnd = (angle.p1 - angle.from).normalized * length / 2;
            for (int i=0; i<=20; i++)
            {
                lrAngle.SetPosition(i, Vector3.RotateTowards(angleStart, angleEnd, (i + 0f) / 20 * Mathf.Deg2Rad * a, 0f)+angle.from);
            }


            //Handles.Label(angle.from, a.ToString() + m, style);
            tmpAngle.transform.position = Vector3.Lerp(angleStart, angleEnd, 0.5f)+angle.from;
            tmpAngle.transform.LookAt(tmpAngle.transform.position + Camera.main.transform.forward, Vector3.up);
            tmpAngle.text = a.ToString() + m;
        }

        void VTool(VOLUME_MODE MODE, MEASUREMENT Measure)
        {
            Handles.color = color;
            
            tVolumeCube.gameObject.SetActive(MODE == VOLUME_MODE.Box);
            tmpVolume.gameObject.SetActive(true);
            tVolumeCubeCorner1.gameObject.SetActive(MODE == VOLUME_MODE.Box);
            tVolumeCubeCorner2.gameObject.SetActive(MODE == VOLUME_MODE.Box);
            tVolumeSphere.gameObject.SetActive(MODE == VOLUME_MODE.Sphere);
            tVolumeSphereRadius.gameObject.SetActive(MODE == VOLUME_MODE.Sphere);
            tVolumeSpherePosition.gameObject.SetActive(MODE == VOLUME_MODE.Sphere);
            lrVolumePlaneLine.gameObject.SetActive(MODE == VOLUME_MODE.Plane);
            lrVolumePlaneDiagonal.gameObject.SetActive(MODE == VOLUME_MODE.Plane);
            tVolumePlanePoint1.gameObject.SetActive(MODE == VOLUME_MODE.Plane);
            tVolumePlanePoint2.gameObject.SetActive(MODE == VOLUME_MODE.Plane);
            tVolumePlanePoint3.gameObject.SetActive(MODE == VOLUME_MODE.Plane);
            tVolumePlanePoint4.gameObject.SetActive(MODE == VOLUME_MODE.Plane);


            switch (MODE)
            {
                case VOLUME_MODE.Box:
                    //VolumeTool.DrawCube(volume.bounds.center, volume.bounds.size);
                    tVolumeCube.position = volume.bounds.center;
                    tVolumeCube.localScale = volume.bounds.size;


                    //Vector3 size = Handles.ScaleHandle(volume.bounds.size, volume.bounds.center, Quaternion.identity, HandleUtility.GetHandleSize(volume.bounds.center) * 1.25f);
                    //Vector3 center = Handles.PositionHandle(volume.bounds.center, Quaternion.identity);
                    Vector3 size = tVolumeCube.localScale;
                    Vector3 center = tVolumeCube.position;
                    Vector3 textPos = center + new Vector3(0, -0.5f, 0);

                    volume.bounds.size = size;
                    volume.bounds.center = center;

                    //Vector3 minCorner = Handles.FreeMoveHandle(volume.bounds.min, Quaternion.identity, 0.075f, new Vector3(volume.snap, volume.snap, volume.snap), Handles.RectangleHandleCap);
                    //Vector3 maxCorner = Handles.FreeMoveHandle(volume.bounds.max, Quaternion.identity, 0.075f, new Vector3(volume.snap, volume.snap, volume.snap), Handles.RectangleHandleCap);
                    Vector3 minCorner = tVolumeCubeCorner1.position;
                    Vector3 maxCorner = tVolumeCubeCorner2.position;

                    volume.bounds.min = minCorner;
                    volume.bounds.max = maxCorner;


                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = color;
                    style.fontSize = 20;
                    style.alignment = TextAnchor.MiddleCenter;

                    float rectSize = size.x * size.y * size.z;


                    string m = "";

                    switch (Measure)
                    {
                        case MEASUREMENT.Meter:
                            m = "m³";
                            break;
                        case MEASUREMENT.Feet:
                            rectSize = (rectSize * 3.28084f);
                            m = "ft³";
                            break;
                        case MEASUREMENT.Inch:
                            rectSize = (rectSize * 39.3701f);
                            m = "Inc³";
                            break;
                        case MEASUREMENT.Miles:
                            rectSize = (rectSize * 0.000621371f);
                            m = "Mi³";
                            break;
                        case MEASUREMENT.Yards:
                            rectSize = (rectSize * 1.09361f);
                            m = "Yd³";
                            break;
                    }

                    rectSize = MTool.Round(rectSize, decimals);
                    rectSize = Mathf.Abs(rectSize);

                    //Handles.Label(textPos, rectSize.ToString() + m + '\n' + size.ToString(), style);
                    tmpVolume.transform.position = textPos;
                    tmpVolume.transform.LookAt(tmpVolume.transform.position + Camera.main.transform.forward, Vector3.up);
                    tmpVolume.text = rectSize.ToString() + m + '\n' + size.ToString();
                    break;

                case VOLUME_MODE.Sphere:
                    //volume.position = Handles.PositionHandle(volume.position, Quaternion.identity);
                    volume.position = tVolumeSpherePosition.position;
                    //volume.sizer = Handles.FreeMoveHandle(volume.sizer, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                    Vector3 tPos = volume.position + new Vector3(0, -0.25f, 0);

                    //volume.radius = Vector3.Distance(volume.position, volume.sizer);
                    volume.radius = Vector3.Distance(tVolumeSphereRadius.position, tVolumeSpherePosition.position)*2f;
                    //Handles.DrawLine(volume.sizer, volume.position);
                    tVolumeSphere.position = volume.position;
                    tVolumeSphere.localScale = volume.radius * Vector3.one;

                    Vector3 norm = (volume.position - volume.sizer).normalized;
                    Quaternion rotFacing = Quaternion.identity;

                    if (norm.magnitude > 0.001f)
                    {
                        rotFacing = Quaternion.LookRotation(norm);
                    }

                    Quaternion rotOpposite = rotFacing * Quaternion.Euler(0, 90.0f, 0);

                    //Handles.CircleHandleCap(0, volume.position, rotFacing, volume.radius, EventType.Repaint);
                    //Handles.CircleHandleCap(0, volume.position, rotOpposite, volume.radius, EventType.Repaint);

                    float r = volume.radius * 4 * Mathf.PI;

                    string m2 = "";

                    switch (Measure)
                    {
                        case MEASUREMENT.Meter:
                            m2 = "m²";
                            break;
                        case MEASUREMENT.Feet:
                            r = (r * 3.28084f);
                            m2 = "ft²";
                            break;
                        case MEASUREMENT.Inch:
                            r = (r * 39.3701f);
                            m2 = "Inc²";
                            break;
                        case MEASUREMENT.Miles:
                            r = (r * 0.000621371f);
                            m2 = "Mi²";
                            break;
                        case MEASUREMENT.Yards:
                            r = (r * 1.09361f);
                            m2 = "Yd²";
                            break;
                    }
                    r = MTool.Round(r, decimals);

                    style = new GUIStyle();
                    style.normal.textColor = color;
                    style.fontSize = 20;
                    style.alignment = TextAnchor.MiddleCenter;

                    //Handles.Label(tPos, r.ToString() + m2, style);
                    tmpVolume.transform.position = tPos;
                    tmpVolume.transform.LookAt(tmpVolume.transform.position + Camera.main.transform.forward, Vector3.up);
                    tmpVolume.text = r.ToString() + m2;

                    break;

                case VOLUME_MODE.Plane:

                    //volume.p1 = Handles.FreeMoveHandle(volume.p1, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                    //volume.p2 = Handles.FreeMoveHandle(volume.p2, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                    //volume.p3 = Handles.FreeMoveHandle(volume.p3, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                    //volume.p4 = Handles.FreeMoveHandle(volume.p4, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                    volume.p1 = tVolumePlanePoint1.position;
                    volume.p2 = tVolumePlanePoint2.position;
                    volume.p3 = tVolumePlanePoint3.position;
                    volume.p4 = tVolumePlanePoint4.position;

                    //Handles.DrawLine(volume.p1, volume.p2);
                    //Handles.DrawLine(volume.p2, volume.p3);
                    //Handles.DrawLine(volume.p3, volume.p4);
                    //Handles.DrawLine(volume.p4, volume.p1);
                    lrVolumePlaneLine.SetPosition(0, volume.p1);
                    lrVolumePlaneLine.SetPosition(1, volume.p2);
                    lrVolumePlaneLine.SetPosition(2, volume.p3);
                    lrVolumePlaneLine.SetPosition(3, volume.p4);

                    //Handles.DrawDottedLine(volume.p1, volume.p3, 10f);
                    lrVolumePlaneDiagonal.SetPositions(new Vector3[] { volume.p1, volume.p3 });

                    float area1 = VolumeTool.FindArea(volume.p1, volume.p3, volume.p4);
                    float area2 = VolumeTool.FindArea(volume.p1, volume.p2, volume.p3);

                    float total = area1 + area2;

                    string m3 = "";

                    switch (Measure)
                    {
                        case MEASUREMENT.Meter:
                            m3 = "m²";
                            break;
                        case MEASUREMENT.Feet:
                            total = (total * 3.28084f);
                            m3 = "ft²";
                            break;
                        case MEASUREMENT.Inch:
                            total = (total * 39.3701f);
                            m3 = "Inc²";
                            break;
                        case MEASUREMENT.Miles:
                            total = (total * 0.000621371f);
                            m3 = "Mi²";
                            break;
                        case MEASUREMENT.Yards:
                            total = (total * 1.09361f);
                            m3 = "Yd²";
                            break;
                    }

                    total = MTool.Round(total, decimals);


                    style = new GUIStyle();
                    style.normal.textColor = color;
                    style.fontSize = 20;
                    style.alignment = TextAnchor.MiddleCenter;

                    Vector3 t1 = (volume.p1 + volume.p3 + volume.p4) / 3;
                    Vector3 t2 = (volume.p1 + volume.p2 + volume.p3) / 3;

                    //Handles.Label((t1 + t2) / 2, total.ToString() + m3, style);
                    tmpVolume.transform.position = (t1 + t2) / 2;
                    tmpVolume.transform.LookAt(tmpVolume.transform.position + Camera.main.transform.forward, Vector3.up);
                    tmpVolume.text = total.ToString() + m3;
                    break;

                default:
                    break;
            }
        }

        void GOTool(Transform t)
        {
            Handles.color = color;

            Handles.DrawLine(t.position, rotMover);

            EditorGUI.BeginChangeCheck();
            rotMover = Handles.FreeMoveHandle(rotMover, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                // Rotate
                t.LookAt(rotMover);
            }
        }
    }
}