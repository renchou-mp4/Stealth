using UnityEngine;
using System.Collections;

public class CCTVPlayerDetection : MonoBehaviour 
{
    private GameObject player;
    private LastPlayerSighting lastPlayerSighting;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }

    void OnTriggerStay(Collider other)
    {
        //当碰撞体进入范围后会判断是不是玩家
        if(other.gameObject == player)
        {
            //因为玩家和摄像头可能隔着墙，所以使用射线来检测是否碰撞到玩家
            //先取得玩家和摄像头之间的相对距离
            Vector3 relPlayerPos = player.transform.position - transform.position;
            //RaycastHit类用于存储发射射线后产生的碰撞信息。先申明了一个RaycastHit类型的hit变量，在Physics.Raycast（）方法后，hit这个变量就携带了射线碰撞到那个物体的一些信息
            RaycastHit hit;

            //使用Raycast函数返回hit
            if(Physics.Raycast(relPlayerPos,transform.position,out hit))
            {
                //看hit中携带的信息是否是玩家的
                if(hit.collider.gameObject==player)
                {
                    //若是玩家的信息，则修改lastPlayerSighting中玩家的位置
                    lastPlayerSighting.position = player.transform.position;
                }
            }
        }
    }

}
