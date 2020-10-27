namespace echo17.EndlessBook.Demo02
{
    using UnityEngine;

    /// <summary>
    /// Makes an object act like a child of another object.
    /// This is useful because we can have objects follow the book's
    /// transforms and not disappear when the animated book is
    /// set inactive, switching to a static standin
    /// </summary>
    public class MimicChild : MonoBehaviour
    {
        private Vector3 parentInitialPosition;
        private Quaternion parentInitialRotation;
        private Vector3 childInitialPosition;
        private Quaternion childInitialRotation;
        private Matrix4x4 parentMatrix;

        public Transform parentTransform;

        void Start()
        {
            parentInitialPosition = parentTransform.position;
            parentInitialRotation = parentTransform.rotation;

            childInitialPosition = transform.position;
            childInitialRotation = transform.rotation;

            childInitialPosition = DivideVectors(Quaternion.Inverse(parentTransform.rotation) * (childInitialPosition - parentInitialPosition), parentTransform.lossyScale);
        }

        void LateUpdate()
        {
            parentMatrix = Matrix4x4.TRS(parentTransform.position, parentTransform.rotation, parentTransform.lossyScale);
            transform.position = parentMatrix.MultiplyPoint3x4(childInitialPosition);
            transform.rotation = (parentTransform.rotation * Quaternion.Inverse(parentInitialRotation)) * childInitialRotation;
        }

        Vector3 DivideVectors(Vector3 num, Vector3 den)
        {
            return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
        }
    }
}