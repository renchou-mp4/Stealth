using UnityEngine;
using System.Collections;

public class LiftTrigger : MonoBehaviour 
{
    public float timeToDoorClose = 2f;
    public float timeToLiftStart = 3f;
    public float timeEndLevel = 6f;
    public float liftSpeed = 3f;

    private GameObject player;
    private Animator playerAnim;
    private HashIDs hash;
    private CameraMovement camMovement;
    private SceneFadeInOut sceneFadeInOut;
    private LiftDoorsTracking liftDoorsTracking;
    private PlayerMovement playerMovement;
    private bool playerInLift;
    private float timer;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerAnim = player.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        camMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
        sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
        liftDoorsTracking = GetComponent<LiftDoorsTracking>();
        playerMovement = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInLift = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInLift = false;
            timer = 0f;
        }
    }

    void Update()
    {
        if (playerInLift)
            LiftActivation();
        if(timer <timeToDoorClose)
        {
            liftDoorsTracking.DoorFollowing();
        }
        else
        {
            liftDoorsTracking.CloseDoor();
        }
    }

    void LiftActivation()
    {
        timer += Time.deltaTime;

        if(timer >= timeToLiftStart)
        {
            playerAnim.SetFloat(hash.speedFloat, 0f);
            camMovement.enabled = false;
            player.transform.parent = transform;
            playerMovement.enabled = false;

            transform.Translate(Vector3.up * liftSpeed * Time.deltaTime);

            if (!audio.isPlaying)
                audio.Play();
            if (timer >= timeEndLevel)
                sceneFadeInOut.EndScene();
        }
    }
}
