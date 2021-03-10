using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour {

    static public ChoiceManager instance;

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

    private AudioManager theAudio;
    private string question;
    private List<string> answerList;

    public GameObject go;//평소에 비활성화 시킬 목적으로 선언. setActive

    public Text question_Text;
    public Text[] answer_Text;
    public GameObject[] answer_Panel;

    public Animator anim;

    public string keySound;
    public string enterSound;

    public bool choiceIng; //대기 
    private bool keyInput; //키처리 활성화, 비 활성화

    private int count; //배열의 크기
    private int result; //선택한 선택창

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // Use this for initialization
    void Start () {
        theAudio = FindObjectOfType<AudioManager>();
        answerList = new List<string>();
        for (int i = 0; i <= 3; i++)
        {
            answer_Text[i].text = "";
            answer_Panel[i].SetActive(false);
        }
        question_Text.text = "";
	}

    public void ShowChoice(Choice _choice) //선택지를 보이게 한다.
    {
        choiceIng = true;
        go.SetActive(true);       
        result = 0;
        question = _choice.question;
        for (int i = 0; i < _choice.answers.Length; i++) //선택지의 갯수만큼
        {
            answerList.Add(_choice.answers[i]);
            answer_Panel[i].SetActive(true); //보이게 활성화
            count = i;
        }
        anim.SetBool("Appear", true); //그 다음 뿌려줌
        Selection();
        StartCoroutine(ChoiceCoroutine());
    }

    public int GetResult()
    {
        return result;
    }

    public void ExitChoice()
    {       
        question_Text.text = "";
        for (int i = 0; i <= count; i++)
        {
            answer_Text[i].text = "";
            answer_Panel[i].SetActive(false);
        }
        answerList.Clear();  
        anim.SetBool("Appear", false);     
        answerList.Clear();
        choiceIng = false;
        go.SetActive(false);
    }

    IEnumerator ChoiceCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(TypingQuestion());
        StartCoroutine(TypingAnswer_0());
        if (count >= 1)
            StartCoroutine(TypingAnswer_1());
        if (count >= 2)
            StartCoroutine(TypingAnswer_2());
        if (count >= 3)
            StartCoroutine(TypingAnswer_3());

        yield return new WaitForSeconds(0.5f);

        keyInput = true;
    }

    IEnumerator TypingQuestion()
    {
        for (int i = 0; i < question.Length; i++)
        {
            question_Text.text += question[i]; //한글자씩
            yield return waitTime;
        }
    }

    //동시에 질문과 대답 텍스트를 출력할 것이기 때문에 코루틴을 그만큼 추가하는 것. 
    IEnumerator TypingAnswer_0()
    {
        yield return new WaitForSeconds(0.4f); //연출
        for (int i = 0; i < answerList[0].Length; i++)
        {
            answer_Text[0].text += answerList[0][i]; //한글자씩
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_1()
    {
        yield return new WaitForSeconds(0.5f); //연출
        for (int i = 0; i < answerList[1].Length; i++)
        {
            answer_Text[1].text += answerList[1][i]; //한글자씩
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer_2()
    {
        yield return new WaitForSeconds(0.6f); //연출
        for (int i = 0; i < answerList[2].Length; i++)
        {
            answer_Text[2].text += answerList[2][i]; //한글자씩
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer_3()
    {
        yield return new WaitForSeconds(0.7f); //연출
        for (int i = 0; i < answerList[3].Length; i++)
        {
            answer_Text[3].text += answerList[3][i]; //한글자씩
            yield return waitTime;
        }
    }

    // Update is called once per frame
    void Update () {
        if (keyInput)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                theAudio.Play(keySound);
                if (result > 0)
                    result--;
                else
                    result = count;
                Selection();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                theAudio.Play(keySound);
                if (result < count)
                    result++;
                else
                    result = 0;
                Selection();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                theAudio.Play(keySound);
                keyInput = false;
                ExitChoice();
            }
        }
	}

    public void Selection()
    {
        Color color = answer_Panel[0].GetComponent<Image>().color;
        color.a = 0.75f;
        for (int i = 0; i <= count; i++)
        {
            answer_Panel[i].GetComponent<Image>().color = color;
        }
        color.a = 1f;
        answer_Panel[result].GetComponent<Image>().color = color;
    }

}
