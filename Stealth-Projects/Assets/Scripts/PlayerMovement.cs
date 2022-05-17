using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
    public AudioClip shoutingClip;//shout音频
    public float turnSmoothing = 15f;//转向速度
    public float speedDampTime = 0.1f;//速度缓冲时间

    private Animator anim;//引用PlayerAnimator
    private HashIDs hash;//引用HashIDs

    void Awake()
    {
        //取得引用
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        anim.SetLayerWeight(1, 1f);
    }

    void FixedUpdate()
    {
        //将玩家的输入赋值给变量，这样就不用再次调用Input函数了
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool sneaking = Input.GetButton("Sneak");
        MovementManagement(h, v, sneaking);
    }

    void Update()
    {
        bool shout = Input.GetButtonDown("Attract");
        anim.SetBool(hash.shoutingBool, shout);
        AudioManager(shout);
    }

    void MovementManagement(float horizontal,float vertical,bool sneaking)
    {
        anim.SetBool(hash.sneakingBool, sneaking);
        if(horizontal!=0f||vertical!=0f)
        {
            Rotating(horizontal, vertical);
            anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
        }
        else
        {
            anim.SetFloat(hash.speedFloat, 0f);
        }
    }

    void Rotating(float horizontal,float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
        rigidbody.MoveRotation(newRotation);
    }

    void AudioManager(bool shout)
    {
        if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.locomotionState)
        {
            if(!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            audio.Stop();
        }

        if(shout)
        {
            AudioSource.PlayClipAtPoint(shoutingClip, transform.position);
        }
    }
}
