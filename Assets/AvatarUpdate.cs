using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarUpdate : MonoBehaviour
{
    [SerializeField] GameObject Head;

    [SerializeField] Vector3 saveposHead;

    [SerializeField] GameObject EditorHead;
   

    // Start is called before the first frame update
    void Start()
    {
        saveposHead = Head.transform.localPosition;
        EditorHead = GameObject.FindGameObjectWithTag("Player");
        Head.transform.parent = EditorHead.transform;
        Head.transform.localPosition = saveposHead;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
}
