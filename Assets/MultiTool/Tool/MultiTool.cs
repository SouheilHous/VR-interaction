using UnityEngine;
using UnityEditor;
using System.Collections;

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

struct RulerTool
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

struct AngleTool
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

struct VolumeTool
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
        p2 += new Vector3( 1, 0, 1);
        p3 += new Vector3( 1, 0,-1);
        p4 += new Vector3(-1, 0,-1);
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

        // Front
        Handles.DrawLine(position + new Vector3(-half.x, -half.y, half.z), position + new Vector3(half.x, -half.y, half.z));
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
        Handles.DrawLine(position + new Vector3(half.x, half.y, -half.z), position + new Vector3(half.x, half.y, half.z));
    }
}

public class MultiTool : EditorWindow
{

    enum VOLUME_MODE
    {
        Box,
        Sphere,
        Plane
    }
    enum MEASUREMENT
    {
        Meter,
        Inch,
        Feet,
        Yards,
        Miles
    }
    /* - GUI - */
    Color color = Color.magenta;
    int decimals = 2;

    RulerTool ruler = new RulerTool(0.0f, false, 0.5f);
    AngleTool angle = new AngleTool(Vector3.zero);
    VolumeTool volume = new VolumeTool(new Bounds(Vector3.zero, Vector3.one));
    VOLUME_MODE VMODE = VOLUME_MODE.Box;
    MEASUREMENT MEASURE = MEASUREMENT.Meter;

    bool rulerToggle = true;
    bool angleToggle = false;
    bool volumeToggle = false;
    bool rotateToggle = false;

    Vector3 rotMover = Vector3.zero;



    [MenuItem("Tools/Multi-Tool")]
    public static void ShowWindow()
    {
        MultiTool mTool = (MultiTool)GetWindow(typeof(MultiTool), false, "Multi-Tool");
        mTool.Show();
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnLostFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    void OnSelectionChange()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }



    void OnGUI()
    {
        Transform t = Selection.activeTransform;


        EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
        rulerToggle = EditorGUILayout.Foldout(rulerToggle, "Ruler Tool");
        if (rulerToggle) { RToolGUI(); }

        angleToggle = EditorGUILayout.Foldout(angleToggle, "Angle Tool");
        if (angleToggle) { AToolGUI(); }

        volumeToggle = EditorGUILayout.Foldout(volumeToggle, "Volume Tool");
        if (volumeToggle) { VToolGUI(); }



        if (t != null)
        {
                GOToolGUI(t);
        }


        //General tool settings ---V
        decimals = EditorGUILayout.IntField("Decimals to Show:", decimals);
        decimals = Mathf.Clamp(decimals, 0, 5);

        color = EditorGUILayout.ColorField("Tool Color:", color);
        //General tool settings ---/\

        Repaint();
    }

    void RToolGUI()
    {
        ruler.from = EditorGUILayout.Vector3Field("From:", ruler.from);
        if (GUILayout.Button("Capture GameObject's Position"))
        {
            if (Selection.activeTransform != null)
            {
                ruler.from = Selection.activeTransform.position;
            }
            else
            {
                Debug.LogWarning("Please select an object before trying to capture its position.");
            }
        }
        EditorGUILayout.Space();


        ruler.to = EditorGUILayout.Vector3Field("To:", ruler.to);
        if (GUILayout.Button("Capture GameObject's Position"))
        {
            if (Selection.activeTransform != null)
            {
                ruler.to = Selection.activeTransform.position;
            }
            else
            {
                Debug.LogWarning("Please select an object before trying to capture its position.");
            }
        }
        EditorGUILayout.Space();

        MEASURE = (MEASUREMENT)EditorGUILayout.EnumPopup("Measurement in:", MEASURE);

        ruler.snap = EditorGUILayout.Toggle("Snap position:", ruler.snap);
        if (ruler.snap)
        {
            ruler.snapValue = EditorGUILayout.FloatField("Snap to value:", ruler.snapValue);
        }
        EditorGUILayout.Space();
    }

    void AToolGUI()
    {
        angle.from = EditorGUILayout.Vector3Field("Base:", angle.from);
        if (GUILayout.Button("Capture GameObject's Position"))
        {
            angle.from = Selection.activeTransform.position;
        }
        EditorGUILayout.Space();

        angle.p1 = EditorGUILayout.Vector3Field("Point 1:", angle.p1);
        if (GUILayout.Button("Capture GameObject's Position"))
        {
            angle.p1 = Selection.activeTransform.position;
        }
        EditorGUILayout.Space();

        angle.p2 = EditorGUILayout.Vector3Field("Point 2:", angle.p2);
        if (GUILayout.Button("Capture GameObject's Position"))
        {
            angle.p2 = Selection.activeTransform.position;
        }
        EditorGUILayout.Space();
    }

    void VToolGUI()
    {
        MEASURE = (MEASUREMENT)EditorGUILayout.EnumPopup("Measurement in:", MEASURE);

        VMODE = (VOLUME_MODE)EditorGUILayout.EnumPopup("Volume Mode:", VMODE);

        switch(VMODE)
        {
            case VOLUME_MODE.Box:
                volume.bounds = EditorGUILayout.BoundsField(volume.bounds);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Snap to value: (Hold CTRL)", GUILayout.MaxWidth(175.0f));
                volume.snap = EditorGUILayout.FloatField(volume.snap);
                EditorGUILayout.EndHorizontal();
                break;

            case VOLUME_MODE.Sphere:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sphere Radius", GUILayout.MaxWidth(100.0f));
                volume.radius = EditorGUILayout.FloatField(volume.radius);
                EditorGUILayout.EndHorizontal();
                break;

            case VOLUME_MODE.Plane:
                EditorGUILayout.BeginHorizontal();
                volume.p1 = EditorGUILayout.Vector3Field("Points (1)", volume.p1);
                EditorGUILayout.EndHorizontal();
                break;

            default:
                break;
        }



        EditorGUILayout.Space();

    }

    void GOToolGUI(Transform t)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Object", GUILayout.MaxWidth(100.0f));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        string n = EditorGUILayout.TextField(t.name);

        if (GUILayout.Button("Deselect")) 
        { 
         Selection.activeGameObject = null; 
        }
        EditorGUILayout.EndHorizontal();

        if (n != t.name) 
        { 
        t.name = n; 
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Position", GUILayout.MaxWidth(50.0f));
        t.localPosition = EditorGUILayout.Vector3Field("", t.localPosition);
        EditorGUILayout.EndHorizontal();
        

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotation", GUILayout.MaxWidth(50.0f));
        t.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("", t.localRotation.eulerAngles));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(50.0f));
        t.localScale = EditorGUILayout.Vector3Field("", t.localScale);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Position")) { t.localPosition = Vector3.zero; }
        if (GUILayout.Button("Reset Rotation"))
        {
            t.localRotation = Quaternion.identity;
            rotMover = t.position + (t.forward * 1.5f);
        }
        if (GUILayout.Button("Reset Scale")) { t.localScale = Vector3.one; }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.Space();

        Component[] c = t.GetComponents<Component>();

        for (int i = 0; i < c.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button(c[i].GetType().Name);
            if (GUILayout.Button("↑", GUILayout.Width(20.0f))) { UnityEditorInternal.ComponentUtility.MoveComponentUp(c[i]); }
            if (GUILayout.Button("↓", GUILayout.Width(20.0f))) { UnityEditorInternal.ComponentUtility.MoveComponentDown(c[i]); }
            if (GUILayout.Button("X", GUILayout.Width(20.0f))) { /*if (c[i].GetType() != typeof(Transform))*/ { Undo.DestroyObjectImmediate(c[i]); } }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rotate towards point", GUILayout.MaxWidth(125.0f));
        EditorGUI.BeginChangeCheck();
        rotateToggle = EditorGUILayout.Toggle(rotateToggle);
        if (EditorGUI.EndChangeCheck()) 
        { 
         rotMover = t.position + (t.forward * 1.5f); 
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Rename Children"))
        {
            ExamplePopup.InitPopup(t);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space();
    }

    void OnInspectorUpdate()
    {

    }
    
    void Update()
    {
        if (SceneView.onSceneGUIDelegate == null)
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (rulerToggle)  { RTool(MEASURE); }
        if (angleToggle)  { ATool(); }
        if (volumeToggle) { VTool(VMODE, MEASURE); }



        Transform t = Selection.activeTransform;
        if (t != null && rotateToggle)
        {
            GOTool(t);
        }




        sceneView.Repaint();
    }

    void RTool(MEASUREMENT Measure)
    {
        Handles.color = color;

        // Move Handles
        ruler.to   = Handles.PositionHandle(ruler.to,   Quaternion.identity);
        ruler.from = Handles.PositionHandle(ruler.from, Quaternion.identity);

        // Snapping
        if (ruler.snap)
        {
            ruler.to   = new Vector3(MTool.RoundTo(ruler.to.x,   Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.to.y,   Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.to.z,   Mathf.RoundToInt(1 / ruler.snapValue)));
            ruler.from = new Vector3(MTool.RoundTo(ruler.from.x, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.from.y, Mathf.RoundToInt(1 / ruler.snapValue)), MTool.RoundTo(ruler.from.z, Mathf.RoundToInt(1 / ruler.snapValue)));
        }

        // Draw
        Handles.DrawDottedLine(ruler.from, ruler.to, 10.0f);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 20;

        // Distance
        float d = Vector3.Distance(ruler.from, ruler.to);

        string m="";

        switch(Measure)
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
        Handles.Label((ruler.from + ruler.to) / 2, d.ToString()+m, style);
    }

    void ATool()
    {
        Handles.color = color;

        // Move Handles
        angle.from = Handles.PositionHandle(angle.from, Quaternion.identity);
        angle.p1 = Handles.PositionHandle(angle.p1, Quaternion.identity);
        angle.p2 = Handles.PositionHandle(angle.p2, Quaternion.identity);

        // Draw
        Handles.DrawLine(angle.from, angle.p1);
        Handles.DrawLine(angle.from, angle.p2);


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

        Handles.DrawWireArc(angle.from, n, (angle.p2 - angle.from).normalized, a, length / 2);
        Handles.Label(angle.from, a.ToString()+m, style);
    }

    void VTool(VOLUME_MODE MODE, MEASUREMENT Measure)
    {
        Handles.color = color;

        switch(MODE)
        {
            case VOLUME_MODE.Box:
                VolumeTool.DrawCube(volume.bounds.center, volume.bounds.size);


                Vector3 size = Handles.ScaleHandle(volume.bounds.size, volume.bounds.center, Quaternion.identity, HandleUtility.GetHandleSize(volume.bounds.center) * 1.25f);
                Vector3 center = Handles.PositionHandle(volume.bounds.center, Quaternion.identity);
                Vector3 textPos = center + new Vector3(0, -0.5f, 0);

                volume.bounds.size = size;
                volume.bounds.center = center;

                Vector3 minCorner = Handles.FreeMoveHandle(volume.bounds.min, Quaternion.identity, 0.075f, new Vector3(volume.snap, volume.snap, volume.snap), Handles.RectangleHandleCap);
                Vector3 maxCorner = Handles.FreeMoveHandle(volume.bounds.max, Quaternion.identity, 0.075f, new Vector3(volume.snap, volume.snap, volume.snap), Handles.RectangleHandleCap);

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

                Handles.Label(textPos, rectSize.ToString() + m + '\n' + size.ToString(), style);
                break;

            case VOLUME_MODE.Sphere:
                volume.position = Handles.PositionHandle(volume.position, Quaternion.identity);
                volume.sizer = Handles.FreeMoveHandle(volume.sizer, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                Vector3 tPos = volume.position + new Vector3(0, -0.25f, 0);

                volume.radius = Vector3.Distance(volume.position, volume.sizer);
                Handles.DrawLine(volume.sizer, volume.position);

                Vector3 norm = (volume.position - volume.sizer).normalized;
                Quaternion rotFacing = Quaternion.identity;

                if (norm.magnitude > 0.001f)
                {
                    rotFacing = Quaternion.LookRotation(norm);
                }

                Quaternion rotOpposite = rotFacing * Quaternion.Euler(0, 90.0f, 0);

                Handles.CircleHandleCap(0, volume.position, rotFacing, volume.radius, EventType.Repaint);
                Handles.CircleHandleCap(0, volume.position, rotOpposite, volume.radius, EventType.Repaint);

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

                Handles.Label(tPos, r.ToString()+m2, style);

                break;

            case VOLUME_MODE.Plane:

                volume.p1 = Handles.FreeMoveHandle(volume.p1, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                volume.p2 = Handles.FreeMoveHandle(volume.p2, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                volume.p3 = Handles.FreeMoveHandle(volume.p3, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                volume.p4 = Handles.FreeMoveHandle(volume.p4, Quaternion.identity, 0.075f, Vector3.zero, Handles.RectangleHandleCap);

                Handles.DrawLine(volume.p1, volume.p2);
                Handles.DrawLine(volume.p2, volume.p3);
                Handles.DrawLine(volume.p3, volume.p4);
                Handles.DrawLine(volume.p4, volume.p1);

                Handles.DrawDottedLine(volume.p1, volume.p3, 10f);

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

                Handles.Label((t1 + t2) / 2, total.ToString() + m3, style);
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



public class ExamplePopup : EditorWindow
{
    static Transform parentObject;
    string childName = "New name";
    bool renameAll = false;

    static public void InitPopup(Transform t)
    {
        ExamplePopup popup = CreateInstance<ExamplePopup>();

        parentObject = t;
        popup.name = "Rename Children";
        popup.titleContent.text = "Rename Children";
        popup.position = new Rect(Screen.width / 2, Screen.height / 2, 200, 400);
        popup.ShowUtility();
    }



    private void OnLostFocus()
    {
        Close();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Rename to:");
        childName = EditorGUILayout.TextField(childName);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rename children's children?", GUILayout.MaxWidth(200.0f));
        renameAll = EditorGUILayout.Toggle(renameAll);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply"))     { RenameChildren(childName, renameAll); }
        if (GUILayout.Button("Nevermind")) { Close();                   }
        EditorGUILayout.EndHorizontal();
    }

    private void RenameChildren(string name, bool all)
    {
        if (!all)
        {
            int children = parentObject.childCount;
            for (int i = 0; i < children; i++)
            {
                parentObject.GetChild(i).name = name;
            }
        }
        else
        {
            foreach(Transform child in parentObject.GetComponentsInChildren<Transform>())
            {
                if (child != parentObject)
                {
                    child.name = name;
                }
            }
        }
    }

    private void RecursiveRename(string name, Transform t)
    {
        
    }
}