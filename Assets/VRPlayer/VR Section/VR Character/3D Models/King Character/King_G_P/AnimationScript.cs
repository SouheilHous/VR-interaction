using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public GameObject Guitar;
    public GameObject Piano;
    public GameObject PianoPlay, GuitarPlay;
    public GameObject PianoDisable, GuitarDisable
        ;

    public void Start()
    {
        PianoPlay.SetActive(false);
        GuitarPlay.SetActive(true);
        PianoDisable.SetActive(false);

    }

    public void OnGuitorPlay()
    {
        Guitar.GetComponent<Animation>().Play("GuitarAnim");
    }
    public void OnPianoPlay()
    {
        Piano.GetComponent<Animation>().Play("Take 002");
    }
    public void OnGuitarDisable()
    {
        Guitar.SetActive(false);
        Piano.SetActive(true);
        PianoPlay.SetActive(true);
        GuitarPlay.SetActive(false);
        PianoDisable.SetActive(true);
        GuitarDisable.SetActive(false);
    }
    public void OnPianoDisable()
    {
        Guitar.SetActive(true);
        Piano.SetActive(false);
        PianoPlay.SetActive(false);
        GuitarPlay.SetActive(true);
        PianoDisable.SetActive(false);
        GuitarDisable.SetActive(true);
    }
}
