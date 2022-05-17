using UnityEngine;
using System.Collections;

public class DoorAnimation : MonoBehaviour 
{
    //为了增加脚本的复用性，让脚本控制单开门和双开门。所以添加是否需要钥匙的标识
    public bool requireKey;
    public AudioClip doorSwitchClip;
    public AudioClip accessDeniedClip;

    private Animator anim;
    private HashIDs hash;
    private GameObject player;
    private PlayerInventory playerInventory;
    //门的检测范围中，可能会有玩家和敌人多个碰撞器进入，使用count计算范围内碰撞器的数量
    private int count;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerInventory>();
    }

    void Update()
    {
        anim.SetBool(hash.openBool, count > 0);

        if(anim.IsInTransition(0) && !audio.isPlaying)
        {
            audio.clip = doorSwitchClip;
            audio.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            if(requireKey)
            {
               if(playerInventory.hasKey)
                {
                    count++;
                }
                else
                {
                    audio.clip = accessDeniedClip;
                    audio.Play();
                }
            }
            else
            {
                count++;
            }
        }
        else if(other.gameObject.tag == Tags.enemy && other is CapsuleCollider)
        {
            count++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject==player||(other.gameObject.tag==Tags.enemy&&other is CapsuleCollider))
        {
            count = Mathf.Max(0, count - 1);
        }
    }
}
