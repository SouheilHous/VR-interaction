using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Prisom;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public string url = "https://astrobackyard.com/wp-content/uploads/2019/07/andromeda-galaxy-location.jpg";
    public Renderer thisRenderer;
    private string _cpyUrl;

    private void Awake()
    {
        _cpyUrl = url;
    }

    // automatically called when game started
    void Start()
    {
        //  var testUrl = "https://images-na.ssl-images-amazon.com/images/I/41gd%2BcvHF9L._SX314_BO1,204,203,200_.jpg";
        BookCoverData.books.Add(new BookCoverData.BookCover
            {coverUrl = url, bookRenderer = GetComponent<Renderer>()});

    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("CollideCube"))
        {
            Debug.Log("-----------");
            if (url == null || url == _cpyUrl) return;
            _cpyUrl = url;
            StartCoroutine(nameof(LoadFromLikeCoroutine));
        }
        
      
    }


    // private void FixedUpdate()
    // {
    //     if (url == null || url == _cpyUrl) return;
    //     _cpyUrl = url;
    //     StartCoroutine(nameof(LoadFromLikeCoroutine));
    // }


    private IEnumerator LoadFromLikeCoroutine()
    {
        using (var webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                var material = thisRenderer.material;
                material.color = Color.white; // set white
                material.mainTexture = DownloadHandlerTexture.GetContent(webRequest); // set loaded image
            }

        }
    }
}