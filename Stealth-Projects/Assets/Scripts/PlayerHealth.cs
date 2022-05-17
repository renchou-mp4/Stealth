using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour 
{
    public float health = 100f;//血量
    public float resetAfterDeathTime = 5f;//因为有死亡动画，所以延迟重置关卡
    public AudioClip deathClip;//播放死亡音效

    private Animator anim;//引用获取死亡状态
    private PlayerMovement playerMovement;//引用这个，在死亡后禁用移动
    private HashIDs hash;
    private SceneFadeInOut sceneFadeInOut;//重置关卡的时候要黑屏
    private LastPlayerSighting lastPlayerSighting;//死亡后要恢复默认位置来关闭警报
    private float timer;//计时器，用来定时重置关卡
    private bool playerDead;//用这个变量来判断角色是否死亡

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
        lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();

    }

    void Update()
    {
        //判断是否死亡
        if(health<=0f)
        {
            //若已死亡但是死亡标识还为flase，说明没有运行dying，运行
            if(!playerDead)
            {
                PlayerDying();
            }
            else//若已死亡但是标识为true，说明已经运行dying了，运行剩下的
            {
                PlayerDead();
                LevelReset();
            }
        }
    }

    void PlayerDying()
    {
        //设置死亡变量
        playerDead = true;
        //设置Animator中的变量，让其播放死亡动画
        anim.SetBool(hash.dyingBool, true);
        //播放死亡音频
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }

    void PlayerDead()
    {
        //检查是否是死亡状态
        if(anim.GetCurrentAnimatorStateInfo(0).nameHash==hash.dyingState)
        {
            //若是，则将死亡变量设置为false，确保死亡动画只播放一次
            playerDead = false;
        }
        anim.SetFloat(hash.speedFloat, 0f);//速度设为0
        playerMovement.enabled = false;//禁用移动
        lastPlayerSighting.position = lastPlayerSighting.resetPosition;//重设位置让警报停止
        audio.Stop();//停止播放脚步声
    }

    void LevelReset()
    {
        timer += Time.deltaTime;
        if(timer>=resetAfterDeathTime)
        {
            sceneFadeInOut.EndScene();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }
}
