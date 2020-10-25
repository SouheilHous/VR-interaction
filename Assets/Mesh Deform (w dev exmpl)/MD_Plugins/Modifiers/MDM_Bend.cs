using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Bend")]
    public class MDM_Bend : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Mesh Bend = Component for objects with Mesh Renderer
        //---Bend mesh to the specific direction
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public enum Direction_ { X,Y,Z}
        public Direction_ ppBendDirection = Direction_.X;

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
                if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modifiers. Please, remove exists modifier to access to the selected modifier...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (!GetComponent<MeshFilter>())
            {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }
            if (!Application.isPlaying)
                return;
            meshF = GetComponent<MeshFilter>();
            meshF.mesh.MarkDynamic();
            originalVertices.Clear();
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
                if (ppBendDirection == Direction_.X)
                {
                    vets[i] = BendObject(originalVertices[i], ppAmount);
                }
                else if (ppBendDirection == Direction_.Y)
                {
                    vets[i] = BendObject(originalVertices[i], ppAmount);
                }
                else if (ppBendDirection == Direction_.Z)
                {
                    vets[i] = BendObject(originalVertices[i], ppAmount);
                }
            }
            meshF.sharedMesh.vertices = vets;
            meshF.sharedMesh.RecalculateNormals();
        }
        private void LateUpdate()
        {
            AmountStorage = ppAmount;
        }

        private Vector3 BendObject(Vector3 origin, float direction)
        {
            Vector3 Direction = origin;

            if (ppBendDirection == Direction_.X)
            {
                float rotExpl = (Mathf.PI / 2) + (direction * Direction.z);

                float rotS = Mathf.Sin(rotExpl) * ((1/ direction) + Direction.x);
                float rotC = Mathf.Cos(rotExpl) * ((1 / direction) + Direction.x);

                Direction.z = -rotC;
                Direction.x = rotS - (1 / direction);
            }
            else if (ppBendDirection == Direction_.Y)
            {
                float rotExpl = (Mathf.PI / 2) + (direction * Direction.y);

                float rotS = Mathf.Sin(rotExpl) * ((1 / direction) + Direction.x);
                float rotC = Mathf.Cos(rotExpl) * ((1 / direction) + Direction.x);

                Direction.y = -rotC;
                Direction.x = rotS - (1 / direction);
            }
            else if (ppBendDirection == Direction_.Z)
            {
                float rotExpl = (Mathf.PI / 2) + (direction * Direction.x);

                float rotS = Mathf.Sin(rotExpl) * ((1 / direction) + Direction.z);
                float rotC = Mathf.Cos(rotExpl) * ((1 / direction) + Direction.z);

                Direction.x = -rotC;
                Direction.z = rotS - (1 / direction);
            }

            return Direction;
        }

        public void BEND(UnityEngine.UI.Slider Entry)
        {
            ppAmount = Entry.value;
        }
        public void BEND(float Entry)
        {
            ppAmount = Entry;
        }
    }
}
