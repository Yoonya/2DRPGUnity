using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager instance; //얘 하나만 쓰니까 싱클톤화, 매니저들

    //숨김기능 주석
    #region Singleton
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
    #endregion Singleton 

    public GameObject DM; //setActive 관리

    public Text text;
    public SpriteRenderer rendererSprite;
    public SpriteRenderer rendererDialogueWindow;

    private List<string> listSentences;
    private List<Sprite> listSprites;
    private List<Sprite> listDialogueWindows;

    private int count;//대화 진행 상황 카운트

    public Animator animSprite;
    public Animator animDialogueWindow;

    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;

    public bool talking = false;//말하는 중
    private bool keyActivated = false;//waitforseconds들이 있기 때문에 키 연속입력 방지 
    private bool onlyText = false;

    // Use this for initialization
    void Start () {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void ShowText(string[] _sentences) //텍스트만 출력
    {
        DM.SetActive(true);
        talking = true;
        onlyText = true;

        for (int i = 0; i < _sentences.Length; i++)
        {
            listSentences.Add(_sentences[i]);
        }

        StartCoroutine(StartTextCoroutine());
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        DM.SetActive(true);
        talking = true;
        onlyText = false;

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }

        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        StartCoroutine(StartDialogueCoroutine());
    }

    public void ExitDialogue()
    {
        text.text = "";
        count = 0;
        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();
        animSprite.SetBool("Appear", false);
        animDialogueWindow.SetBool("Appear", false);
        talking = false;
    }

    IEnumerator StartDialogueCoroutine()
    {
        if (count > 0)
        {
            //대사바가 교체될 경우
            if (listDialogueWindows[count] != listDialogueWindows[count - 1])//앞 뒤 이미지가 다르다면->교체작업
            {
                animSprite.SetBool("Change", true);
                animDialogueWindow.SetBool("Appear", false);
                yield return new WaitForSeconds(0.2f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else
            {
                //캐릭터가 교체될 경우
                if (listSprites[count] != listSprites[count - 1])
                {
                    animSprite.SetBool("Change", true);
                    yield return new WaitForSeconds(0.1f);
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
                    animSprite.SetBool("Change", false);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
        else
        {
            rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count];
            rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count];
        }

        keyActivated = true;
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; //1번째 문장의 첫글자부터 하나씩
            if(i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator StartTextCoroutine()
    {       
        keyActivated = true;
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            text.text += listSentences[count][i]; //1번째 문장의 첫글자부터 하나씩
            if (i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Update is called once per frame
    void Update () {
        if (talking && keyActivated)
        {
            if (Input.GetKeyDown(KeyCode.Z))//Z키가 눌릴 때, 다음 문장으로 넘어가기, down은 누르고 있어도 적용
            {
                keyActivated = false;
                count++;
                text.text = "";//문장이 중복되면 안되니까 한번 초기화
                theAudio.Play(enterSound);
                if (count == listSentences.Count)//끝이라면, Count+1 아닌 이유는 count++가 먼저 이루어져서
                {
                    StopAllCoroutines();
                    ExitDialogue();
                }
                else
                {
                    StopAllCoroutines();
                    if (onlyText)
                        StartCoroutine(StartTextCoroutine());
                    else
                        StartCoroutine(StartDialogueCoroutine());
                }
            }
        }
        
	}
}
