using UnityEngine;
using System.Collections;

public class CameraDirection : MonoBehaviour
{
    [SerializeField]
    protected Facing facing = Facing.DownRight;

    public virtual Facing Facing { get { return facing; } }
    //public virtual Vector2 Angle { get { return transform.eulerAngles; } }
    

    public virtual void LateUpdate()
    {
        //float rY = transform.eulerAngles.y;
        float y = transform.eulerAngles.y;

        //Debug.Log(string.Format("facing:{0}", facing));
        //Debug.Log(string.Format("rX:{0} x: {1}", rY, y));

        if (y>45.1f && y<135)
        {
            facing = Facing.UpRight;
        }
        else if (y>135.1f && y<225)
        {
            facing = Facing.UpLeft;
        }
        else if (y>225.1f && y<315)
        {
            facing = Facing.DownLeft;
        }
        else
        {
            facing = Facing.DownRight;
        }
    }
}
