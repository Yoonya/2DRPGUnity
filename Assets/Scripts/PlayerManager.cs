using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject {

    static public PlayerManager instance;

    public string currentMapName; //transfreMap 스크립트에 있는 transferMapName 변수 값을 저장
    public string currentSceneName; 

    public string walkSound_1;
    public string walkSound_2;
    public string walkSound_3;
    public string walkSound_4;

    private AudioManager theAudio;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false; //shift입력시 2칸 단위로 움직이는 것을 속도는 빠르게하고 1칸으로 움직이도록 

    private bool canMove = true; //코루틴이 계속 입력되는 것을 방지하기 위하여

    public bool notMove = false;

    private bool attacking = false;
    public float attackDelay;
    private float currentAttackDelay;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);//다른 신으로 로드할 때마다 이 객체를 파괴하지 말아라
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<string>();
        boxColider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        theAudio = FindObjectOfType<AudioManager>();
    }

    IEnumerator MoveCoroutine() //1초 대기를 위한 코루틴, 픽셀마다 움직이는데 그게 1프레임마다 움직이는 것을 막기 위해
    {
        //여기서 반복문을 한번 더 입력해서 걷는 입력을 한 프레임당 한번한 것을 애니메이션에 반영되는 것을 막기 위함
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && !notMove && !attacking)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            if (vector.x != 0)//상하좌우 동시입력 방지
                vector.y = 0;

            animator.SetFloat("DirX", vector.x); //Dirx에 vector.x값 설정
            animator.SetFloat("DirY", vector.y);

            bool checkCollsionFalg = base.CheckCollsion(); //base는 부모
            if (checkCollsionFalg)//앞에 무언가가 있다.
                break;//움직임을 멈춘다.

            animator.SetBool("Walking", true);

            int temp = UnityEngine.Random.Range(1, 4);
            switch (temp)
            {
                case 1:
                    theAudio.Play(walkSound_1);
                    break;
                case 2:
                    theAudio.Play(walkSound_2);
                    break;
                case 3:
                    theAudio.Play(walkSound_3);
                    break;
                case 4:
                    theAudio.Play(walkSound_4);
                    break;
            }

            boxColider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);//콜라이더 위치

            //픽셀단위의 움직임을 반복문으로
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * (speed + applyRunSpeed), 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * (speed + applyRunSpeed), 0);
                }
                if (applyRunFlag) //shift입력시 한번 더 입력
                {
                    currentWalkCount++;
                }
                currentWalkCount++;
                if (currentWalkCount == 12)
                    boxColider.offset = Vector2.zero;
                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;
        }

        animator.SetBool("Walking", false);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !notMove && !attacking)
        {
            //기본적인 움직임
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //Horizontal은 좌우로 눌렸을 때 1, -1 반환, Vertical은 반대
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }

        if (!notMove && !attacking)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentAttackDelay = attackDelay;
                attacking = true;
                animator.SetBool("Attacking", true);
            }
        }

        if (attacking)
        {
            currentAttackDelay -= Time.deltaTime;
            if (currentAttackDelay <= 0)
            {
                animator.SetBool("Attacking", false);
                attacking = false;
            }
        }
    }
}
