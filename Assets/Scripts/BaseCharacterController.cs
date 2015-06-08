using UnityEngine;
using System.Collections;

public class BaseCharacterController : MonoBehaviour {
	//外部パラメータ（Inspector表示）
	public Vector2 velocityMin=new Vector2(-100.0f,-100.0f);
	public Vector2 velocityMax=new Vector2(+100.0f,+50.0f);


	//===外部パラメータ===================
	[Syestem.NonSerialized] public float hpMax=10.0f;
	[Syestem.NonSerialized] public float hp=10.0f;
	[Syestem.NonSerialized] public float dir=1.0f;
	[Syestem.NonSerialized] public float speed=6.0f;
	[Syestem.NonSerialized] public float basScaleX=1.0f;
	[Syestem.NonSerialized] public bool activeSts=false;
	[Syestem.NonSerialized] public bool jumped=false;
	[Syestem.NonSerialized] public bool grounded=false;
	[Syestem.NonSerialized] public bool groundedPrev=false;

	//===キャッシュ======================
	[System.NonSerialized] public Animator animator;
	protected Transform groundCheck_L;
	protected Transform groundCheck_C;
	protected Transform groundCheck_R;

	//===内部パラメータ===================
	protected float speedVx=0.0f;
	protected float speedVxAddPower=10.0f;
	protected float gravityScale=10.0f;
	protected float jumpStartTime=0.0f;

	protected GameObject groundCheck_OnRoadObject;
	protected GameObject groundCheck_OnMoveObject;
	protected GameObject groundCheck_OmEmemyObject;


	//コード（Monobehaviour基本機能の実装)=======
	protected virtual void Awake()
	{
		animator = GetComponent<Animator> ();
		groundCheck_L=transform.Find ("GroundCheck_L");
		groundCheck_C=transform.Find ("GroundCheck_C");
		groundCheck_R=transform.Find ("GroundCheck_R");

	dir = (transform.localScale.x > 0.0f) ? 1 : -1;
		basScaleX = transform.localScale.x * dir;





		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
