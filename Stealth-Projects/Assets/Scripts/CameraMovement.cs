using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    public float smooth = 1.5f;

    private Transform player;//角色的Transform
    //为了让相机与角色一起移动
    private Vector3 relCameraPos;//相机与角色的相对位置
    //为了让相机与角色保持固定距离
    private float relCameraPosMag;//相机与角色相对位置的向量长度
    private Vector3 newPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        relCameraPos = transform.position - player.position;
        //相对位置的向量长度比实际值小一些。因为角色位置的坐标是在模型的脚下，当你使用光线投射到角色上时，可能会碰撞到地面。
        relCameraPosMag = relCameraPos.magnitude - 0.5f;
    }

    //在FixedUpdate中给相机赋值，虽然相机不是物理对象，但是他跟随的角色是物理对象
    void FixedUpdate()
    {
        //相机的标准位置
        Vector3 standardPos = player.position + relCameraPos;
        //相机的俯视位置
        Vector3 abovePos = player.position + Vector3.up * relCameraPosMag;
        //从标准位置到俯视位置，寻找三个中间位置，看是否可以看到角色。若可以看到则设置为那个位置
        //将5个位置都放到数组中，通过循环来寻找合适的位置
        Vector3[] checkPos = new Vector3[5];
        //Lerp函数的前两个参数确定了一个范围，第三个参数为0，返回第一个参数，第三个为1，返回第二个参数。若为0到1之间的数则返回确定范围的百分比
        //例如
        checkPos[0] = standardPos;
        checkPos[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);//返回standardPos与abovePos范围中25%位置的值
        checkPos[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);//返回standardPos与abovePos范围中50%位置的值
        checkPos[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);//返回standardPos与abovePos范围中75%位置的值
        checkPos[4] = abovePos;

        for(int i=0;i<checkPos.Length;i++)
        {
            if(ViewingPosCheck(checkPos[i]))
            {
                break;//若不跳出循环，那么即使中间有位置可以看到玩家，最后还是会变成俯视位置
            }
        }
        //使用Lerp函数完成位置赋值
        transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
        //摄像机一到移动到新位置后，还要确保他的视角方向是正确的。可以使用transform.LookAt函数。
        //但是每次执行FixedUpdate时，摄像机都会固定朝向角色的位置，由于摄像机的位置过于精确，所以角色移动时会让摄像机抖动。
        //所以需要让摄像机平滑的捕捉角色的位置
        SmoothLookAt();
    }

    bool ViewingPosCheck(Vector3 checkPos)
    {
        RaycastHit hit;
        //若射线碰到物体了
        if(Physics.Raycast(checkPos, player.position-checkPos, out hit,relCameraPosMag))//参数为射线起点，射线方向，Raycast，射线长度
        {
            //若碰撞的物体不是玩家，则返回false，说明这个位置不可用
            if(hit.transform !=player)
            {
                return false;
            }
        }
        //若来到了这里说明碰到的是玩家，则将位置赋值给newPos
        newPos = checkPos;
        return true;
    }

    void SmoothLookAt()
    {
        Vector3 relPlayerPosition = player.position - transform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }
}
