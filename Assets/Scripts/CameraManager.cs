using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public CameraManager instance;

    public GameObject target;//카메라가 따라갈 대상
    public float moveSpeed; //카메라 속도
    private Vector3 targetPosition; //대상 현재 위치 값

    public BoxCollider2D bound;

    //박스 컬라이더 영역의 최소 최대 xyz값을 가짐
    private Vector3 minBound;
    private Vector3 maxBound;

    //카메라 영역이 박스 바깥으로 나가지 않아야하는데 축이 중심에 있다보니 반너비만큼 연산을 해줘야한다.
    private float halfWidth;
    private float halfHeight;

    //카메라의 반높이값을 구할 속성을 이용하기 위한 변수
    private Camera theCamera;

    //Start보다 먼저 실행됨
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
        theCamera = GetComponent<Camera>();
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
        halfHeight = theCamera.orthographicSize;//size값
        halfWidth = halfHeight * Screen.width / Screen.height; // 뒤에 스크린은 해상도 값을 의미

       
    }

    // Update is called once per frame
    void Update()
    {
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);//z값은 자신 고유의 값을 갖는다.

            //그냥 포지션 값을 하는 경우가 아니라 Lerp를 쓰는 이유는 딱딱히 카메라가 움직이는게 아니라 캐릭터가 움직이면 천천히 따라오는 효과를 주기 위해 선택한 것 같다.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime); //1부터 2까지 3의 속도로 움직이게 한다

            //(값, 최소, 최대) -> 최소 최대값 사이에 값이 있다면 값 반환, 최소보다 작다면 최소반환, 최대보다 크다면 최대반환
            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);       
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y+ halfHeight, maxBound.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound)
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;

    }
}
