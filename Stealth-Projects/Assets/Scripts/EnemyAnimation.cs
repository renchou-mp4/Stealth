using UnityEngine;
using System.Collections;

//守卫由Animator控制，通过Root Motion使敌人移动。Animator的参数决定播放哪个动画状态，而使用NavMeshAgent组件的速度向量，设置这些参数
public class EnemyAnimation : MonoBehaviour 
{
	//deadZone是守卫巡逻时无视的一个区间，当敌人巡逻时的前进方向与期望方向的夹角小于一定的角度时，就让他停止转向。
	//这个一定角度就是deadZone，否则敌人会转向过度，并沿曲线运动
	public float deadZone = 5f;

	private Transform player;
	private EnemySight enemySight;//是否发现玩家
	private NavMeshAgent nav;//控制守卫移动
	private Animator anim;
	private HashIDs hash;
	private AnimatorSetup animSetup;

	void Awake()
    {
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		//确保守卫转向时是由Animator控制而不是NavMeshAgent，这样当敌人在转向的时候就不会出现脚滑动的效果
		nav.updateRotation = false;
		//创建辅助类的实例
		animSetup = new AnimatorSetup(anim, hash);

		//设置动画权重让Shooting和Gun的动画优先播放
		anim.SetLayerWeight(1, 1f);
		anim.SetLayerWeight(2, 1f);

		//将deadZone的角度转换为弧度。因为守卫的动画控制器也是使用弧度进行控制
		deadZone *= Mathf.Deg2Rad;//这个常量可以将角度转换为弧度，只需要乘起来即可
    }
	
	void Update()
    {
		NavAnimSetup();
    }

	//通过脚本让守卫使用Root Motion移动，通常可以在组件中勾选Apply Root Motion。但是这样可以手动控制
	void OnAnimatorMove()
    {
		//这是每一帧移动的距离
		nav.velocity = anim.deltaPosition / Time.deltaTime;
		transform.rotation = anim.rootRotation;//转向已经在NavMeshAgent中设置了
	}

	void NavAnimSetup()
    {
		float speed;//定义这两个变量用于传递给AnimatorSetup脚本
		float angle;

		//若玩家被敌人发现
		if(enemySight.playerInSight)
        {
			//将速度设置为0
			speed = 0f;

			//调用函数计算面朝的方向与玩家位置方向的角度
			angle = FindAngle(transform.forward, player.position - transform.position, transform.up);
        }
        else//若玩家没有被发现，那么速度将基于NavMeshAgent的期望速度，可以通过投影向量实现，计算期望速度向量在敌人正前方向量上的投影，Speed即为投影向量的长度
		//若不使用投影向量，守卫会因为速度过快而左右摇摆，最后调整至正确的方向
        {
			speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
			angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);
			//因为加了转向的缓冲时间，而转向的时候还在运动，因此还会造成左右摇摆
			if(Mathf.Abs(angle)<deadZone)//检测是否角度小于deadZone，若是则不改变敌人的朝向
            {
				//将Angle设置为0，并让敌人朝向期望速度方向
				transform.LookAt(transform.position + nav.desiredVelocity);
				angle = 0f;
			}	
		}

		//有了速度和转向后传递给animSetup添加缓冲时间，并传递给动画控制器
		animSetup.Setup(speed, angle);
    }

	//计算守卫应该面对的方向和守卫当前面对的方向之间的夹角，向左是负角度，向右是正角度
	//第一个参数为当前的方向，第二个为目标方向，第三个为“上”方向。
	float FindAngle(Vector3 fromVector,Vector3 toVector,Vector3 upVector)
    {
		//toVector参数即为NavMeshAgent中的期望速度向量,这个期望速度变量有时候会为0，会出错
		if (toVector == Vector3.zero)
			return 0f;

		//使用Vector3.Angle获取角度的绝对值
		float angle = Vector3.Angle(fromVector, toVector);
		//normal为法向量。通过Unity的左手法则，由当前向量和目标向量可以得到法向量
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		//计算法向量和上向量的点积，若方向一致则结果>0，通过Sign获取符号，与Angle相乘即可得到正角度或负角度
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
		//将角度转换为弧度
		angle *= Mathf.Deg2Rad;

		return angle;

	}
}
