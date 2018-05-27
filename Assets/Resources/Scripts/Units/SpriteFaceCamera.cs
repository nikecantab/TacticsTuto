using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour {
    public CameraDirection cameraDirection;
    private Animator animator;
    private Vector2 prevTransform;

    //todo: add a way to change the editor's spriterenderer based on this?
    public Facing facing = Facing.UpRight;

    //private Transform t;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        prevTransform = new Vector2(transform.position.x, transform.position.z);
    }
	
	// Update is called once per frame
	void Update ()
    {
        int offset = ((int)facing + (int)cameraDirection.Facing) % 4;


        //Debug.Log(string.Format("offset:{0} facing:{1} camera:{2}", offset, facing, cameraDirection.Facing));
        Facing direction = (Facing)offset;

        //Flipping the sprite doesn't currently work with the billboard renderer, using hand-flipped spritessheet instead
        animator.SetInteger("facing", (int)direction);

        prevTransform = new Vector2(transform.position.x, transform.position.z);
    }

    public void FaceDirection(Facing direction)
    {
        facing = direction;
    }
}
