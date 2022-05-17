using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour 
{
    public float patrolSpeed = 2f;//巡逻速度
    public float chaseSpeed = 5f;//追踪速度
    public float chaseWaitTime = 5f;//追踪等待时间
    public float patrolWaitTime = 1f;//巡逻等待时间
    public Transform[] patrolWayPoints;//巡逻路径存放数组

    private EnemySight enemySight;
    private NavMeshAgent nav;
    private Transform player;
    private PlayerHealth playerHealth;
    private LastPlayerSighting lastPlayerSighting;
    private float chaseTimer;//追踪计时器
    private float patrolTimer;//巡逻计时器
    private int wayPointIndex;//巡逻路径点索引（用于数组，就是游标）

    void Awake()
    {
        enemySight = GetComponent<EnemySight>();
        nav = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }

    void Update()
    {
        //优先级：Shooting>Chasing>Patrolling
        //若守卫看到了角色且角色活着
        if (enemySight.playerInSight && playerHealth.health > 0f)
            Shooting();
        //若守卫的私人位置改变且玩家或者
        else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f)
            Chasing();
        //若都不符合则巡逻
        else
            Patrolling();
    }

    //守卫的状态：射击
    void Shooting()
    {
        nav.Stop();
    }
    //守卫的状态：追踪
    void Chasing()
    {
        //当守卫当前位置与最后发现角色的位置大于一定距离时，将角色最后的位置赋值给守卫的追踪位置
        //获取守卫当前位置与最后发现玩家位置的向量
        Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
        //上面的一定数值设为2，所以要检测向量长度的平方是否大于4
        if (sightingDeltaPos.sqrMagnitude > 4f)
            //若条件满足，则追踪位置等于最后发现玩家的位置
            nav.destination = enemySight.personalLastSighting;
        nav.speed = chaseSpeed;

        //守卫与追踪的剩余距离小于可停止距离的时候，守卫原地等待，追踪计时器开始计时
        if(nav.remainingDistance<nav.stoppingDistance)
        {
            chaseTimer += Time.deltaTime;

            //若计时器大于了等待时间，则将全局位置和私人位置都重置。重置计时器
            if(chaseTimer>chaseWaitTime)
            {
                lastPlayerSighting.position = lastPlayerSighting.resetPosition;
                enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
                chaseTimer = 0f;
            }
        }
        else//若敌人没有到达玩家最后出现的位置，计时器也重置。因为当守卫追踪时，玩家可能又在其他位置被发现
        {
            chaseTimer = 0f;
        }
    }
    //守卫的状态：巡逻
    void Patrolling()
    {
        nav.speed = patrolSpeed;
        //判断路径的索引是否需要移动，需要检测玩家敌人是否已经接近目标路径点，或者没有目的地（即玩家位置重置）
        if (nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            //若巡逻时间大于计时器时间，那么索引移至下一个，从而设置新的目标路径点
            if (patrolTimer >= patrolWaitTime)
            {
                //判断路径的索引是否为巡逻路径的最后一个位置
                if (wayPointIndex == patrolWayPoints.Length - 1)
                {
                    //若是，则重置索引为0
                    wayPointIndex = 0;
                }
                else
                {
                    //若不是则索引++
                    wayPointIndex++;
                }
                patrolTimer = 0f;//重置计时器
            }

        }
        else
            patrolTimer = 0f;

        //由于其他代码能改变守卫目的地，所以这里设置目的地为路径点位置
        nav.destination = patrolWayPoints[wayPointIndex].position;
    }
}
