using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Twist")]
    public class MDM_Twist : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Mesh Twist = Component for objects with Mesh Renderer
        //---Twist mesh to the specific direction
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public enum Direction_ { X,Y,Z}
        public Direction_ ppTwistDirection = Direction_.X;

        public float ppAmount = 0;
        private float AmountStorage;

        public bool ppCreateNewReference = true;

        private List<Vector3> originalVertices = new List<Vector3>();

        private MeshFilter meshF;

        void Awake()
        {
            if (ppCreateNewReference)
                MD_MeshProEditor.MeshEditor_STATIC_CreateNewReference(this.gameObject);

            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modifiers. Please, remove exists modifier to access to the selected modifier...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (!GetComponent<MeshFilter>())
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }
            if (!Application.isPlaying)
                return;
            meshF = GetComponent<MeshFilter>();
            meshF.mesh.MarkDynamic();
            originalVertices.AddRange(meshF.mesh.vertices);
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;
            if (meshF.sharedMesh == null)
                return;

            if (ppAmount == AmountStorage)
                return;
            Vector3[] vets = originalVertices.ToArray();
            for (int i = 0; i < vets.Length; i++)
            {
                if (ppTwistDirection == Direction_.X)
                {
                    vets[i] = TwistObject(originalVertices[i], originalVertices[i].x * ppAmount);
                }
                else if (ppTwistDirection == Direction_.Y)
                {
                    vets[i] = TwistObject(originalVertices[i], originalVertices[i].y * ppAmount);
                }
                else if (ppTwistDirection == Direction_.Z)
                {
                    vets[i] = TwistObject(originalVertices[i], originalVertices[i].z * ppAmount);
                }
            }
            meshF.sharedMesh.vertices = vets;
            meshF.sharedMesh.RecalculateNormals();
        }
        private void LateUpdate()
        {
            AmountStorage = ppAmount;
        }
        //---Do Twist obj
        private Vector3 TwistObject(Vector3 origin, float direction)
        {
            var sin = Mathf.Sin(direction);
            var cos = Mathf.Cos(direction);
            Vector3 Direction = Vector3.zero;

            if (ppTwistDirection == Direction_.X)
            {
                Direction.y = origin.y * cos - origin.z * sin;
                Direction.z = origin.y * sin + origin.z * cos;
                Direction.x = origin.x;
            }
            else if (ppTwistDirection == Direction_.Y)
            {
                Direction.x = origin.x * cos - origin.z * sin;
                Direction.z = origin.x * sin + origin.z * cos;
                Direction.y = origin.y;
            }
            else if (ppTwistDirection == Direction_.Z)
            {
                Direction.x = origin.x * cos - origin.y * sin;
                Direction.y = origin.x * sin + origin.y * cos;
                Direction.z = origin.z;
            }

            return Direction;
        }

        public void TWIST(UnityEngine.UI.Slider Entry)
        {
            ppAmount = Entry.value;
        }
        public void TWIST(float Entry)
        {
            ppAmount = Entry;
        }
    }
}
