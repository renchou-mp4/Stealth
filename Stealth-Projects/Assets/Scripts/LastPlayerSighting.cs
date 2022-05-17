using UnityEngine;
using System.Collections;

public class LastPlayerSighting : MonoBehaviour 
{
    public Vector3 position = new Vector3(1000f, 1000f, 1000f);
    public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);
    public float lightHightIntensity = 0.25f;
    public float lightLowIntensity = 0f;
    public float fadeSpeed = 7f;
    public float musicFadeSpeed = 1f;//背景音乐切换速率

    private AlarmLight alarm;
    private Light mainlight;
    private AudioSource panicAudio;
    private AudioSource[] sriens;

    void Awake()
    {
        alarm = GameObject.FindGameObjectWithTag(Tags.alarm).GetComponent<AlarmLight>();
        mainlight = GameObject.FindGameObjectWithTag(Tags.mainLight).GetComponent<Light>();
        panicAudio = transform.Find("secondaryMusic").audio;
        GameObject[] sriensObject = GameObject.FindGameObjectsWithTag(Tags.siren);
        sriens = new AudioSource[sriensObject.Length];
        for(int i=0;i<sriens.Length;i++)
        {
            sriens[i] = sriensObject[i].audio;
        }
    }

    void Update()
    {
        SwitchAlarms();
        MusicFading();
    }

    void SwitchAlarms()
    {
        alarm.alarmOn = (position != resetPosition);

        float targetIntenity;
        if(position!=resetPosition)
        {
            targetIntenity = lightLowIntensity;
        }
        else
        {
            targetIntenity = lightHightIntensity;
        }
        mainlight.intensity = Mathf.Lerp(mainlight.intensity, targetIntenity, fadeSpeed * Time.deltaTime);

        for(int i=0;i<sriens.Length;i++)
        {
            if(position != resetPosition && !sriens[i].isPlaying)
            {
                sriens[i].Play();
            }
            else if(position==resetPosition)
            {
                sriens[i].Stop();
            }
        }
    }


    void MusicFading()
    {
        if(position != resetPosition)
        {
            audio.volume = Mathf.Lerp(audio.volume, 0f, musicFadeSpeed * Time.deltaTime);
            panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
        }
        else
        {
            audio.volume = Mathf.Lerp(audio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
            panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0f, musicFadeSpeed * Time.deltaTime);
        }
    }
}
