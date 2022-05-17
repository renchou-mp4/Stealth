using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour 
{
    public float maxDamage = 120f;//最大伤害
    public float minDamage = 45f;//最小伤害
    public AudioClip shotClip;//射击音频
    public float flashIntansity = 3f;//灯光强度（模拟开枪效果）
    public float fadeSpeed = 10f;//灯光衰减速度

    private Animator anim;
    private HashIDs hash;
    private LineRenderer laserShotLine;
    private Light laserShotLight;
    private SphereCollider col;//计算射击范围
    private Transform player;
    private PlayerHealth playerHealth;
    private bool shooting;//判断是否射击
    private float scaledDamage;//伤害范围

    void Awake()
    {
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        laserShotLine = GetComponentInChildren<LineRenderer>();
        laserShotLight = laserShotLine.gameObject.light;
        col = GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = player.GetComponent<PlayerHealth>();

        laserShotLine.enabled = false;
        laserShotLight.intensity = 0f;

        scaledDamage = maxDamage - minDamage;
    }

    void Update()
    {
        //shot有一个射击曲线，值从0到1再到0。获取这个值
        float shot = anim.GetFloat(hash.shotFloat);

        //若大于0.5且没有射击，那么就调用射击函数
        if(shot>0.5f&&!shooting)
        {
            Shot();
        }
        
        if(shot<0.5)
        {
            //若小于0.5，那么射击标识设为false，且禁用lineRenderer
            shooting = false;
            laserShotLine.enabled = false;
        }

        laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);

    }

    //让守卫指向玩家的位置
    void OnAnimatorIK(int layerIndex)
    {
        float aimWeight = anim.GetFloat(hash.aimWeightFloat);
        //通过SetIKPosition函数设置IK位置，让其指向玩家的位置
        anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up * 1.5f);
        //设置IK的权重
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
    }

    void Shot()
    {
        //设置标识为true
        shooting = true;
        //计算守卫与玩家之间的距离占碰撞器半径的比值。比值为1，伤害为0，比值为0，伤害为1
        float fractionalDistance = (col.radius - Vector3.Distance(transform.position, player.position) / col.radius);
        float damage = scaledDamage * fractionalDistance + minDamage;
        playerHealth.TakeDamage(damage);//计算并传递伤害
        ShotEffects();
    }

    void ShotEffects()
    {
        //设置光线的起点
        laserShotLine.SetPosition(0, laserShotLine.transform.position);
        //设置光线的终点
        laserShotLine.SetPosition(1, player.position + Vector3.up * 1.5f);//因为位置是在角色脚下，所以向上抬起
        laserShotLine.enabled = true;
        //设置点光源强度
        laserShotLight.intensity = flashIntansity;
        AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);
    }
}
