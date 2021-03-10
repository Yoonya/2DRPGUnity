using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour {

    public string transferMapName;//이동할 맵의 이름

    public Transform target;
    public BoxCollider2D targetBound;

    public Animator anim_1;
    public Animator anim_2;

    public int door_count;

    [Tooltip("UP, DOWN, LEFT, RIGHT")]
    public string direction;//캐릭터가 바라보고 있는 방향
    private Vector2 vector;//getfloat("dirX")사용

    [Tooltip("문이 있으면 : true, 없으면 false")]
    public bool door; //문이 있냐 없냐

    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private FadeManager theFade; //페이드 인아웃
    private OrderManager theOrder;

	// Use this for initialization
	void Start () {
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!door)
        {
            if (collision.gameObject.name == "Player")
            {
                StartCoroutine(TransferCoroutine());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (door)
        {
            if (collision.gameObject.name == "Player")
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));
                    switch (direction)
                    {
                        case "UP":
                            if (vector.y == 1f)
                                StartCoroutine(TransferCoroutine());
                            break;
                        case "DOWN":
                            if (vector.y == -1f)
                                StartCoroutine(TransferCoroutine());
                            break;
                        case "RIGHT":
                            if (vector.x == 1f)
                                StartCoroutine(TransferCoroutine());
                            break;
                        case "LEFT":
                            if (vector.x == -1f)
                                StartCoroutine(TransferCoroutine());
                            break;
                        default:
                            StartCoroutine(TransferCoroutine());
                            break;

                    }
                    StartCoroutine(TransferCoroutine());
                }
                
            }
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.PreLoadCharacter(); //player를 찾으려면 캐릭터 로드해야함
        theOrder.NotMove();//페이드 인아웃되는 동안 이동금지
        theFade.FadeOut();
        if (door)
        {
            anim_1.SetBool("Open", true);
            if (door_count == 2)
                anim_2.SetBool("Open", true);        
        }
        yield return new WaitForSeconds(0.5f); //페이드인아웃 대기시간 + 문 여닫는시간

        theOrder.SetTransparent("player"); //캐릭터가 들어가는 연출 = 투명하게
        if (door)
        {
            anim_1.SetBool("Open", false);
            if (door_count == 2)
                anim_2.SetBool("Open", false);
        }
        yield return new WaitForSeconds(0.5f); //페이드인아웃 대기시간 + 문 여닫는시간
        theOrder.SetUnTransparent("player");

        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.transform.position;
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);//맵이 밝아지기 전에 움직이더라
        theOrder.Move();
    }
}
