using UnityEngine;
using System.Collections;

public class PlayerController : BaseCharacterController {
	
	// === 外部パラメータ（インスペクタ表示） =====================
	public float 	initHpMax = 20.0f;
	[Range(0.1f,100.0f)] public float 	initSpeed = 12.0f;

	//セーブデータパラメータ
	public static	float 		nowHpMax 				= 0;
	public static	float 		nowHp 	  				= 0;
	public static 	int			score 					= 0;
	
	// アニメーションのハッシュ名
	public readonly static int ANISTS_Idle 	 		= Animator.StringToHash("Base Layer.Player_Idle");
	public readonly static int ANISTS_Walk 	 		= Animator.StringToHash("Base Layer.Player_Walk");
	public readonly static int ANISTS_Run 	 	 	= Animator.StringToHash("Base Layer.Player_Run");
	public readonly static int ANISTS_Jump 	 		= Animator.StringToHash("Base Layer.Player_Jump");
	public readonly static int ANISTS_ATTACK_A 		= Animator.StringToHash("Base Layer.Player_ATK_A");
	public readonly static int ANISTS_DEAD  		= Animator.StringToHash("Base Layer.Player_Dead");

	//============キャッシュ===================================
	LineRenderer hudHpBar;
	TextMesh hudScore;

	// === 内部パラメータ ======================================
	int 			jumpCount			= 0;
	bool			breakEnabled		= true;
	float 			groundFriction		= 0.0f;
	//int			    score 					= 0;

	float comboTimer=0.0f;

	// === コード（サポート関数） ===============================
	public static GameObject GetGameObject() {
		return GameObject.FindGameObjectWithTag ("Player");
	}
	public static Transform GetTranform() {
		return GameObject.FindGameObjectWithTag ("Player").transform;
	}
	public static PlayerController GetController() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
	}
	public static Animator GetAnimator() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<Animator>();
	}

	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();

		//キャッシュ
		hudHpBar = GameObject.Find ("HUD_HPBar").GetComponent<LineRenderer> ();
		hudScore = GameObject.Find ("HUD_Score").GetComponent<TextMesh> ();
		

		// パラメータ初期化
		speed = initSpeed;
		SetHP(initHpMax,initHpMax);
	}
	protected override void Update (){
		base.Update ();

		hudHpBar.SetPosition (1, new Vector3 (5.0f * (hp / hpMax), 0.0f, 0.0f));
		hudScore.text = string.Format ("Score{0}", score);

	
	}
	
	protected override void FixedUpdateCharacter () {
		// 着地チェック
		if (jumped) {
			if ((grounded && !groundedPrev) || 
			    (grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				animator.SetTrigger ("Idle");
				jumped = false;
				jumpCount = 0;
			}
		} 
		if (!jumped) {
			jumpCount = 0;
		}
		
		// キャラの方向
		transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
		
		// ジャンプ中の横移動減速
		if (jumped && !grounded) {
			if (breakEnabled) {
				breakEnabled = false;
				speedVx *= 0.9f;
			}
		}
		
		// 移動停止（減速）処理
		if (breakEnabled) {
			speedVx *= groundFriction;
		}
		
		// カメラ
		Camera.main.transform.position = transform.position - Vector3.forward;
	}
	
	// === コード（基本アクション） =============================
	public override void ActionMove(float n) {
		if (!activeSts) {
			return;
		}
		
		// 初期化
		float dirOld = dir;
		breakEnabled = false;
		
		// アニメーション指定
		float moveSpeed = Mathf.Clamp(Mathf.Abs (n),-1.0f,+1.0f);
		animator.SetFloat("MovSpeed",moveSpeed);
		//animator.speed = 1.0f + moveSpeed;
		
		// 移動チェック
		if (n != 0.0f) {
			// 移動
			dir 	  = Mathf.Sign(n);
			moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
			speedVx   = initSpeed * moveSpeed * dir;
		} else {
			// 移動停止
			breakEnabled = true;
		}
		
		// その場振り向きチェック
		if (dirOld != dir) {
			breakEnabled = true;
		}
	}
	
	public void ActionJump() {
		switch(jumpCount) {
		case 0 :
			if (grounded) {
				animator.SetTrigger ("Jump");
				GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;
				jumpStartTime = Time.fixedTime;
				jumped = true;
				jumpCount ++;
			}
			break;
		case 1 :
			if (!grounded) {
				animator.Play("Player_Jump",0,0.0f);
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,20.0f);
				jumped = true;
				jumpCount ++;
			}
			break;
		}
	}
	public void ActionAttack(){
		animator.SetTrigger ("Attack_A");
	}

	
	public void ActionDamage(float damage) {
		if (!activeSts) {
			return;
		}
		
		animator.SetTrigger ("DMG_A");
		speedVx = 0;
		GetComponent<Rigidbody2D>().gravityScale = gravityScale;
		
		if (jumped) {
			damage *= 1.5f;
		}
		
		if (SetHP(hp - damage,hpMax)) {
			Dead(true); // 死亡
		}
	}

	// === コード（その他） ====================================
	public override void Dead(bool gameOver) {
		// 死亡処理をしてもいいか？
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!activeSts || stateInfo.nameHash == ANISTS_DEAD) {
			return;
		}
		
		base.Dead (gameOver);
		
		SetHP(0,hpMax);
		Invoke ("GameOver", 3.0f);
	}
	
	public void GameOver() {
		PlayerController.score = 0;
		Application.LoadLevel(Application.loadedLevelName);
	}
	
	public override bool SetHP(float _hp,float _hpMax) {
		if (_hp > _hpMax) {
			_hp = _hpMax;
		}
		
		nowHp 		= _hp;
		nowHpMax 	= _hpMax;
		return base.SetHP (_hp, _hpMax);
	}

}


