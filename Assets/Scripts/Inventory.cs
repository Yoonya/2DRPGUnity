using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    static public Inventory instance;
    private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private OkOrCancel theOOC;
    private Equipment theEquip;

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound;

    private InventorySlot[] slots; //인벤토리 슬롯들

    private List<Item> inventoryItemList; //플레이어가 소지한 아이템 리스트들
    private List<Item> inventoryTabList; //선택한 탭에 따라 다르게 보여질 아이템 리스트

    public Text Description_Text;//부연 설명
    public string[] tabDescription; //탭 부연 설명

    public Transform tf; //slot 부모객체, 이를 이용해서 slots를 채울 것

    public GameObject go; //인벤토리 활성화 비활성화
    public GameObject[] selectedTabImages;
    public GameObject go_OOC; //선택지 활성화 비활성화
    public GameObject prefab_floating_Text;


    private int selectedItem; //선택된 아이템
    private int selectedTab; //선택된 탭

    private int page; //페이지
    private int slotCount; //활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 12; //한 페이지 슬롯 12개

    private bool activated; //인벤토리 활성화시 true;
    private bool tabActivated; //탭 활성화시 true;
    private bool itemActivated; //아이템 활성화시 true;
    private bool stopKeyInput; //키 입력 제한(소비할 때 질의가 나오는데 그때 키 입력 방지)
    private bool preventExec; //중복실행 제한, z키의 경우 소모품 사용과 탭 선택에 중복됨

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // Use this for initialization
    void Start () {
        instance = this;
        theDatabase = FindObjectOfType<DatabaseManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theAudio = FindObjectOfType<AudioManager>();
        theOOC = FindObjectOfType<OkOrCancel>();
        theEquip = FindObjectOfType<Equipment>();
        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>();//자식이 전부 찾아 들어감
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList; //내 아이템리스트 보내주기
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;//내 아이템리스트 덮어씌우기
    }

    public void EquipToInvevtory(Item _item)
    {
        inventoryItemList.Add(_item);
    }
    public void GetAnItem(int _itemID, int _count = 1)
    {
        for (int i = 0; i < theDatabase.itemList.Count; i++)//데이터베이스에 아이디 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID)//아이템 발견
            {

                var clone = Instantiate(prefab_floating_Text, PlayerManager.instance.transform.position, Quaternion.Euler(Vector3.zero)); //플로팅텍스트 프리펩 생성
                clone.GetComponent<FloatingText>().text.text = theDatabase.itemList[i].itemName + " " + _count + "개 흭득 +";
                clone.transform.SetParent(this.transform); //부모 객체를 생성에서 안으로 넣을 수 있다.


                for (int j = 0; j < inventoryItemList.Count; j++)//소지품에 있다
                {
                    if (inventoryItemList[j].itemID == _itemID)
                    {
                        if (inventoryItemList[j].itemType == Item.ItemType.Use) //소모품일 경우에만 개수로 다룸
                        {
                            inventoryItemList[j].itemCount += _count;
                            return;
                        }
                        else
                        {
                            inventoryItemList.Add(theDatabase.itemList[i]);
                        }
                        return;
                       
                    }
                }

                inventoryItemList.Add(theDatabase.itemList[i]);//소지품에 없다
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                return;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다.");
    }

    public void RemoveSlot() //일단 슬롯 전부 비활성화
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }

    public void ShowTab()//탭 활성화
    {
        RemoveSlot();
        SelectedTab();
    }
    public void SelectedTab()//선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값 0으로 조정
    {
        StopAllCoroutines();
        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < selectedTabImages.Length; i++)
        {
            selectedTabImages[i].GetComponent<Image>().color = color;
        }
        Description_Text.text = tabDescription[selectedTab];
        //여기까지가 초기화

        StartCoroutine(SelectedTabEffectCoroutine());
    }
    IEnumerator SelectedTabEffectCoroutine()//선택된 탭 반짝임 효과
    {
        while (tabActivated)
        {
            Color color = selectedTabImages[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void ShowItem()//아이템 활성화
    {
        //기존에 있던 것들 초기화
        inventoryTabList.Clear();
        RemoveSlot(); 
        selectedItem = 0;
        page = 0;

        switch (selectedTab) //탭에 따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        {
            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        }

        ShowPage();
        SelectedItem();
    }

    public void ShowPage()
    {
        slotCount = -1;

        for (int i = page*MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++) //인벤토리 탭 리스트의 내용을 인벤토리 슬롯에 추가
        {
            slotCount = i - (page * MAX_SLOTS_COUNT);
            slots[slotCount].gameObject.SetActive(true);
            slots[slotCount].Additem(inventoryTabList[i]);

            if (slotCount == MAX_SLOTS_COUNT - 1)
                break;
        }
    }

    public void SelectedItem()//선택된 아이템을 제외하고, 다른 모든 탭의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();
        if (slotCount > -1)
        {
            Color color = slots[0].selected_item.GetComponent<Image>().color;
            color.a = 0f;
            for (int i = 0; i <= slotCount; i++)
                slots[i].selected_item.GetComponent<Image>().color = color;

            Description_Text.text = inventoryTabList[selectedItem].itemDescription;
            StartCoroutine(SelectedItemEffectCoroutine());
        }
        else
            Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
    }
    IEnumerator SelectedItemEffectCoroutine()//선택된 아이템 반짝임 효과
    {
        while (itemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    // Update is called once per frame
    void Update () {
        if (!stopKeyInput)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                activated = !activated; //인벤토리 창을 누르면 켜지거나 꺼지거나

                if (activated)
                {
                    theAudio.Play(open_sound);
                    theOrder.NotMove();
                    go.SetActive(true);
                    selectedTab = 0;
                    tabActivated = true;
                    itemActivated = false;
                    ShowTab();
                }
                else
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;
                    theOrder.Move();
                }
            }
            if (activated) //인벤토리 활성화
            {
                if (tabActivated)//탭 쪽이 활성화
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedTab < selectedTabImages.Length - 1)
                            selectedTab++;
                        else
                            selectedTab = 0;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTab > 0)
                            selectedTab--;
                        else
                            selectedTab = selectedTabImages.Length - 1;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.Z))
                    {
                        theAudio.Play(enter_sound);
                        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                        color.a = 0.25f;
                        selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                        itemActivated = true;
                        tabActivated = false;
                        preventExec = true;
                        ShowItem();
                    }
                } //탭 활성화시 키입력 처리
                else if (itemActivated)
                {
                    if (inventoryTabList.Count > 0)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedItem + 2 > slotCount)
                            {
                                if(page < (inventoryTabList.Count -1)/MAX_SLOTS_COUNT)
                                    page++;
                                else
                                    page = 0;

                                RemoveSlot();
                                ShowPage();
                                selectedItem = -2;
                            }
                            if (selectedItem < slotCount - 1)
                                selectedItem += 2;
                            else
                                selectedItem %= 2; //짝수일 경우 왼쪽 줄만, 홀수일 경우 오른쪽 줄만 이동할것
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedItem - 2 < 0)
                            {
                                if (page != 0)
                                    page--;
                                else
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;

                                RemoveSlot();
                                ShowPage();
                            }
                            if (selectedItem > 1)
                                selectedItem -= 2;
                            else
                                selectedItem = slotCount - selectedItem; //짝수일 경우 왼쪽 줄만, 홀수일 경우 오른쪽 줄만 이동할것
                            theAudio.Play(key_sound);
                            SelectedItem();
                        } 
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedItem + 1 > slotCount)
                            {
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                    page++;
                                else
                                    page = 0;

                                RemoveSlot();
                                ShowPage();
                                selectedItem -= 1;
                            }
                            if (selectedItem < slotCount)
                                selectedItem++;
                            else
                                selectedItem = 0;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedItem - 1 < 0)
                            {
                                if (page != 0)
                                    page--;
                                else
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;

                                RemoveSlot();
                                ShowPage();
                            }
                            if (selectedItem > 0)
                                selectedItem--; 
                            else
                                selectedItem = slotCount;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
                        {
                            if (selectedTab == 0)
                            {
                                StartCoroutine(OOCCoroutine("사용", "취소"));
                            }
                            else if (selectedTab == 1)
                            {                             
                                StartCoroutine(OOCCoroutine("장착", "취소"));
                            }
                            else //나머지 탭은 비프음 출력
                            {
                                theAudio.Play(beep_sound);
                            }
                        }
                    }
                    
                    if (Input.GetKeyDown(KeyCode.X))//아이템에서 다시 탭으로 활성화
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        itemActivated = false;
                        tabActivated = true;
                        ShowTab();
                    }
                }//아이템 활성화시 키입력 처리
                if (Input.GetKeyUp(KeyCode.Z))//중복실행 방지
                    preventExec = false;
            }
        }
	}

    IEnumerator OOCCoroutine(string _up, string _down)
    {
        theAudio.Play(enter_sound);
        stopKeyInput = true;

        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);
        yield return new WaitUntil(() => !theOOC.activated);
        if (theOOC.GetResult())
        {
            for (int i = 0; i < inventoryItemList.Count; i++)
            {
                if (inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                {
                    if (selectedTab == 0)
                    {
                        theDatabase.UseItem(inventoryItemList[i].itemID);
                        if (inventoryItemList[i].itemCount > 1)
                            inventoryItemList[i].itemCount--;
                        else
                            inventoryItemList.RemoveAt(i);

                        ShowItem();
                        break;
                    }
                    else if (selectedTab == 1)
                    {
                        theEquip.EquipItem(inventoryItemList[i]);
                        inventoryItemList.RemoveAt(i);
                        ShowItem();
                        break;
                    }
                    
                }
                
            }
        }
        stopKeyInput = false;
        go_OOC.SetActive(false);
    }
}
