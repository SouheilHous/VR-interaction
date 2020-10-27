using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Valve.VR;

namespace _Scripts.Prisom
{
    public class BookCoverLoader : MonoBehaviour
    {
        private int bookTrack = 15;

        private UnityEvent _imageLoadFinish;

        private void Start()
        {
            _imageLoadFinish = new UnityEvent();

            _imageLoadFinish.AddListener(OnImageLoadFinish);
            Invoke(nameof(StartBookCoverLoadSequence), 2f);
        }

        private void StartBookCoverLoadSequence()
        {
            for (var i = 0; i < 15; i++)
            {
                StartCoroutine(LoadFromLikeCoroutine(BookCoverData.books[i].coverUrl,
                    BookCoverData.books[i].bookRenderer));
            }
        }

        private void OnImageLoadFinish()
        {
            StartCoroutine(LoadFromLikeCoroutine(BookCoverData.books[bookTrack].coverUrl,
                BookCoverData.books[bookTrack].bookRenderer));
            bookTrack++;

            if (bookTrack >= (BookCoverData.books.Count - 1))
            {
                _imageLoadFinish.RemoveListener(OnImageLoadFinish);
            }
        }


        private IEnumerator LoadFromLikeCoroutine(string url, Renderer thisRenderer)
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


            // var wwwLoader = new WWW(url); // create WWW object pointing to the url
            // yield return wwwLoader; // start loading whatever in that url ( delay happens here )
            //
            // var material = thisRenderer.material;
            // material.color = Color.white; // set white
            // material.mainTexture = wwwLoader.texture; // set loaded image

            Debug.Log(bookTrack);

            _imageLoadFinish?.Invoke();

            // if (bookTrack < BookCoverData.books.Count)
            // {
            //     StartCoroutine(LoadFromLikeCoroutine(BookCoverData.books[bookTrack].coverUrl,
            //         BookCoverData.books[bookTrack].bookRenderer));
            //     bookTrack++;
            // }
        }
    }
}