using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MovingObject {

    public float attackDelay;//공격 유예

    public float inter_MoveWaitTime; //대기시간
    private float current_interMWT;

    public string atkSound;

    private Vector2 PlayerPos; //플레이어 좌표값

    private int random_int;
    private string direction;

    public GameObject healthBar;

	// Use this for initialization
	void Start () {
        queue = new Queue<string>();
        current_interMWT = inter_MoveWaitTime;
	}
	
	// Update is called once per frame
	void Update () {
        current_interMWT -= Time.deltaTime;
      
        if (current_interMWT <= 0)
        {
            current_interMWT = inter_MoveWaitTime;

            if (NearPlayer())
            {
                Flip();
                return;
            }

            RandomDirection();

            if (base.CheckCollsion())
                return;

            base.Move(direction);
        }

    }

    private void Flip()
    {
        Vector3 flip = transform.localScale;
        if (PlayerPos.x > this.transform.position.x)
            flip.x = -1f;
        else
            flip.x = 1f;
        this.transform.localScale = flip;

        healthBar.transform.localScale = flip;

        animator.SetTrigger("Attack");
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(attackDelay); //애니메이션으로 공격모션 대기시간
        AudioManager.instance.Play(atkSound);
        if (NearPlayer())
            PlayerStat.instance.Hit(GetComponent<EnemyStat>().atk);
    }

    private bool NearPlayer()
    {
        PlayerPos = PlayerManager.instance.transform.position;
        if (Mathf.Abs(Mathf.Abs(PlayerPos.x) - Mathf.Abs(this.transform.position.x)) <= speed * walkCount * 1.01f) //약간의 오차는 허용 약간의 여유
        {
            if (Mathf.Abs(Mathf.Abs(PlayerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount * 0.5f) //약간의 오차는 허용 약간의 여유
            {
                return true;
            }
        }

        if (Mathf.Abs(Mathf.Abs(PlayerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount * 1.01f) //약간의 오차는 허용 약간의 여유
        {
            if (Mathf.Abs(Mathf.Abs(PlayerPos.x) - Mathf.Abs(this.transform.position.x)) <= speed * walkCount * 0.5f) //약간의 오차는 허용 약간의 여유
            {
                return true;
            }
        }

        return false;
    }

    private void RandomDirection()
    {
        vector.Set(0, 0, vector.z);
        random_int = Random.Range(0, 4);
        switch (random_int)
        {
            case 0:
                vector.y = 1f;
                direction = "UP";
                break;
            case 1:
                vector.y = -1f;
                direction = "DOWN";
                break;
            case 2:
                vector.x = 1f;
                direction = "RIGHT";
                break;
            case 3:
                vector.x = -1f;
                direction = "LEFT";
                break;
        }
    }
}
