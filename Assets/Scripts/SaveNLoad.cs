using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveNLoad : MonoBehaviour {

    [System.Serializable] //01011 이런 식으로 직렬화가 됨, 컴퓨터가 좋아함
    public class Data
    {
        //Vector3를 안쓰고 이런 식인 이유는 Vector3는 직렬화가 안되기 때문 = 다른 클래스같은 것들 사용 불가, 자료형은 사용 가능
        public float playerX;
        public float playerY;
        public float playerZ;

        public int playerLv;
        public int playerHP;
        public int playerMP;

        public int playerCurrentHp;
        public int playerCurrentMp;
        public int playerCurrentExp;

        public int playerHPR;
        public int playerMPR;

        public int playerATK;
        public int playerDEF;

        public int added_atk;
        public int added_def;
        public int added_hpr;
        public int added_mpr;

        public List<int> playerItemInventory; //Item이 아닌 이유는 위와 같이 우리가 만든 클래스이기 때문에
        public List<int> playerItemInventoryCount;
        public List<int> playerEquipItem;

        public string mapName;
        public string sceneName;

        public List<bool> swList;
        public List<string> swNameList;
        public List<string> varnameList;
        public List<float> varNumberList;


    }

    private PlayerManager thePlayer;
    private PlayerStat thePlayerStat;
    private DatabaseManager theDatabase;
    private Inventory theInven;
    private Equipment theEquip;
    private FadeManager theFade;

    public Data data;

    private Vector3 vector;

    public void CallSave()
    {
        theDatabase = FindObjectOfType<DatabaseManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theEquip = FindObjectOfType<Equipment>();
        theInven = FindObjectOfType<Inventory>();

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.playerLv = thePlayerStat.character_Lv;
        data.playerHP = thePlayerStat.hp;
        data.playerMP = thePlayerStat.mp;
        data.playerCurrentHp = thePlayerStat.currentHp;
        data.playerCurrentMp = thePlayerStat.currentMp;
        data.playerCurrentExp = thePlayerStat.currentExp;
        data.playerATK = thePlayerStat.atk;
        data.playerDEF = thePlayerStat.def;
        data.playerMPR = thePlayerStat.recover_mp;
        data.playerHPR = thePlayerStat.recover_hp;
        data.added_atk = theEquip.added_atk;
        data.added_def =  theEquip.added_def;
        data.added_hpr = theEquip.added_hpr;
        data.added_mpr= theEquip.added_mpr;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        Debug.Log("기초 데이터 성공");

        data.playerItemInventory.Clear();//안하면 인벤토리가 복사됨
        data.playerItemInventoryCount.Clear();
        data.playerEquipItem.Clear();

        for (int i = 0; i < theDatabase.var_name.Length; i++)
        {
            data.varnameList.Add(theDatabase.var_name[i]);
            data.varNumberList.Add(theDatabase.var[i]);
        }
        for (int i = 0; i < theDatabase.switch_name.Length; i++)
        {
            data.swNameList.Add(theDatabase.switch_name[i]);
            data.swList.Add(theDatabase.switches[i]);
        }

        List<Item> itemList = theInven.SaveItem();

        for (int i = 0; i < itemList.Count; i++)
        {
            Debug.Log("인벤토리 아이템 저장 완료 : " + itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID);
            data.playerItemInventoryCount.Add(itemList[i].itemCount);
        }

        for (int i = 0; i < theEquip.equipItemList.Length; i++)
        {
            data.playerEquipItem.Add(theEquip.equipItemList[i].itemID);
            Debug.Log("장착된 아이템 저장 완료"+ theEquip.equipItemList[i].itemID);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat");//이 프로젝트가 설치된 폴더 +

        bf.Serialize(file, data); //data를 쓰고 직렬화
        file.Close();

        Debug.Log(Application.dataPath + "의 위치에 저장했습니다.");
    }

    public void CallLoad()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat", FileMode.Open);//이 프로젝트가 설치된 폴더 +

        if (file != null && file.Length > 0)
        {
            data = (Data)bf.Deserialize(file);

            theDatabase = FindObjectOfType<DatabaseManager>();
            thePlayer = FindObjectOfType<PlayerManager>();
            thePlayerStat = FindObjectOfType<PlayerStat>();
            theEquip = FindObjectOfType<Equipment>();
            theInven = FindObjectOfType<Inventory>();
            theFade = FindObjectOfType<FadeManager>();

            theFade.FadeOut();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;

            vector.Set(data.playerX, data.playerY, data.playerZ);
            thePlayer.transform.position = vector;

            thePlayerStat.character_Lv = data.playerLv;
            thePlayerStat.hp = data.playerHP;
            thePlayerStat.mp = data.playerMP;
            thePlayerStat.currentHp = data.playerCurrentHp;
            thePlayerStat.currentMp = data.playerCurrentMp;
            thePlayerStat.currentExp = data.playerCurrentExp;
            thePlayerStat.atk = data.playerATK;
            thePlayerStat.def = data.playerDEF;
            thePlayerStat.recover_hp = data.playerHPR;
            thePlayerStat.recover_mp = data.playerMPR;

            theEquip.added_atk = data.added_atk;
            theEquip.added_def = data.added_def;
            theEquip.added_hpr = data.added_hpr;
            theEquip.added_mpr = data.added_mpr;

            theDatabase.var = data.varNumberList.ToArray();//리스트를 배열화
            theDatabase.switches = data.swList.ToArray();
            theDatabase.switch_name = data.swNameList.ToArray();

            for (int i = 0; i < theEquip.equipItemList.Length; i++) //최대장비수만큼 하나씩 돌려봄
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++) //데이터베이스에 맞는 아이템이 있나 봄
                {
                    if (data.playerEquipItem[i] == theDatabase.itemList[x].itemID)//가져온 로드데이터가 데이터베이스에 일치하다면
                    {
                        theEquip.equipItemList[i] = theDatabase.itemList[x]; //장착
                        Debug.Log("장착된 아이템을 로드했습니다 : " + theEquip.equipItemList[i].itemID);
                        break;//장착했으면 이 부분은 더 돌필요없음
                    }
                }
            }

            List<Item> itemList = new List<Item>();

            for (int i = 0; i < data.playerItemInventory.Count; i++) //가져온 데이터의 개수만큼 돌려봄
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++) //데이터베이스에 맞는 아이템이 있나 봄
                {
                    if (data.playerItemInventory[i] == theDatabase.itemList[x].itemID)//가져온 로드데이터가 데이터베이스에 일치하다면
                    {
                        itemList.Add(theDatabase.itemList[x]);
                        Debug.Log("인벤토리 아이템을 로드했습니다 : " + theDatabase.itemList[x].itemID);
                        break;//장착했으면 이 부분은 더 돌필요없음
                    }
                }
            }

            for (int i = 0; i < data.playerItemInventoryCount.Count; i++)
            {
                itemList[i].itemCount = data.playerItemInventoryCount[i];
            }
            theInven.LoadItem(itemList);
            theEquip.ShowTxT();//반영시키기

            /*
            여기에 카메라를 설정할 수 없다. bound를 만져야하는데 카메라 바운더로 설정한 BoxColider가 다른 씬에 있다면 불러올 수가 없다.
            현재 씬과 다른 씬에 있는 객체들은 참조 불가능
            씬 이동이 이루어지고 그 씬에 붙어있는 맵의 바운드를 참조해야 한다.
            */

            StartCoroutine(WaitCoroutine());
        }
        else
        {
            Debug.Log("저장된 세이브 파일이 없습니다.");
        }

        file.Close();
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(2f);//start로 불러올텐데 이 때 코루틴 대기 후 바로 시작하는부분이 있어서
        GameManager theGM = FindObjectOfType<GameManager>();
        theGM.LoadStart();

        SceneManager.LoadScene(data.sceneName); //씬 이동이 이루어진 후에는 명령어를 듣지 않는다.
    }
}
