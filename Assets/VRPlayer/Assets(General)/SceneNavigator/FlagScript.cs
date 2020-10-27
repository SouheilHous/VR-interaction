using UnityEngine;

namespace SceneNavigator
{

    public class FlagScript : MonoBehaviour
    {

        public Flag flagData;

        void OnDrawGizmos()
        {
            transform.position = flagData.tpos;
            Gizmos.DrawIcon(transform.position, "Flag.png");
        }

    }

}