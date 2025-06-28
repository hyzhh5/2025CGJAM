using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DrawCardsManager : MonoBehaviour
{
    public enum CardType
    {
        attack,
        reply,
        magic,
        AttackUp,
        roundEnd
    }

    [System.Serializable]
    public class CardData
    {
        public Sprite sprite;
        public CardType cardType;
    }

    [Header("��Ƭ����")]
    public CardData[] cardDataArray = new CardData[5]; // �洢5�ֿ�Ƭ������

    [Header("UI����")]
    public Image[] uiSlot1 = new Image[3]; // ��һ��UI����
    public Image[] uiSlot2 = new Image[3]; // �ڶ���UI����
    public Image[] uiSlot3 = new Image[3]; // ������UI����

    private CardType[] curCards = new CardType[3];
    private float effectMultiplier = 1f;
    private List<CardType> cardPool;

    private bool isSpinning = false; // �Ƿ�����ת��
    private float moveInterval = 0.3f; // �ƶ����
    private float spinDuration = 3f; // ÿ����Ƭת��ʱ��
    private float delayBetweenSlots = 1f; // ��Ƭ��֮����ӳ�
    [SerializeField] private CardType magicCard = CardType.attack;
    [SerializeField] private int ExtraNum;

    public static DrawCardsManager Instance { get; private set; }

    private int curNumDraws=0;// ��ǰ�鿨����

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeCardPool();
        InitializeUI();
    }

    private void InitializeCardPool()
    {
        cardPool = new List<CardType>();
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            cardPool.Add(type);
        }
    }

    // ��ʼ��UI��ʾ
    private void InitializeUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < cardDataArray.Length)
            {
                uiSlot1[i].sprite = cardDataArray[i].sprite;
                uiSlot2[i].sprite = cardDataArray[i].sprite;
                uiSlot3[i].sprite = cardDataArray[i].sprite;
            }
        }
    }

    // �齱����
    public (CardType, float) DrawCards()
    {
        if (isSpinning) return (CardType.attack, 0f); // �������ת��������Ĭ��ֵ

        effectMultiplier = 1f;

        // �����ȡ3�ſ�
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, cardPool.Count);
            curCards[i] = cardPool[randomIndex];
        }

        // ͳ��ÿ�ֿ�Ƭ������
        var cardCounts = new Dictionary<CardType, int>();
        foreach (var card in curCards)
        {
            if (!cardCounts.ContainsKey(card))
                cardCounts[card] = 0;
            cardCounts[card]++;
        }

        // �ҳ����ִ������Ŀ�Ƭ����
        var maxCount = cardCounts.Max(x => x.Value);
        var mostFrequentCard = cardCounts.FirstOrDefault(x => x.Value == maxCount).Key;

        // ����Ч������
        CardType resultCard;
        if (maxCount == 3)
        {
            effectMultiplier = 7f;
            resultCard = mostFrequentCard;
        }
        else if (maxCount == 2)
        {
            effectMultiplier = 3f;
            resultCard = mostFrequentCard;
        }
        else
        {
            effectMultiplier = 1f;
            resultCard = curCards[0];
        }

        // ��ʼת������
        StartCoroutine(SpinSlots());

        return (resultCard, effectMultiplier);
    }

    // ת���ϻ���
    private IEnumerator SpinSlots()
    {
        isSpinning = true;

        // ����������λ��ת��
        StartCoroutine(SpinSingleSlot(uiSlot1, 0));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot2, 1));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot3, 2));

        // �ȴ�����ת�����
        yield return new WaitForSeconds(spinDuration + delayBetweenSlots);

        isSpinning = false;
    }

    // ������λ��ת��
    private IEnumerator SpinSingleSlot(Image[] slot, int targetIndex)
    {
        float elapsedTime = 0f;
        int currentOffset = 0;

        // ��ȡĿ�꿨Ƭ��cardDataArray�е�����
        int targetCardIndex = GetCardDataIndex(curCards[targetIndex]);
        while (elapsedTime < spinDuration)
        {
            // ������ʾ
            for (int i = 0; i < 3; i++)
            {
                int cardIndex = (i - currentOffset + 500) % 5; // ��500ȷ������
                slot[i].sprite = cardDataArray[cardIndex].sprite;
            }

            currentOffset++;
            elapsedTime += moveInterval;
            yield return new WaitForSeconds(moveInterval);
        }

        // ȷ������ͣ����ȷ��λ�ã��м�λ����ʾĿ�꿨Ƭ��
        int finalOffset = (targetCardIndex - 1 + 5) % 5;
        for (int i = 0; i < 3; i++)
        {
            int cardIndex = (i + finalOffset) % 5;
            slot[i].sprite = cardDataArray[cardIndex].sprite;
        }
    }

    // ��ȡ��Ƭ�����������е�����
    private int GetCardDataIndex(CardType cardType)
    {
        for (int i = 0; i < cardDataArray.Length; i++)
        {
            if (cardDataArray[i].cardType == cardType)
                return i;
        }
        return 0;
    }

    public CardType[] GetCurrentCards()
    {
        return curCards;
    }

    [ContextMenu("�鿨")]
    public void TestDraw()
    {
        if (RoundManager.Instance.round_Parameter.currentEState != Estate.playerRound)
        {
            Debug.Log("��ǰ����player�غϣ��޷��鿨");
            return;
        }
        if (isSpinning)
        {
            Debug.Log("���ڳ鿨�У����Ժ�...");
            return;
        }

        var (cardType, multiplier) = DrawCards();
        curNumDraws += 1;
        Debug.Log("��"+(curNumDraws)+"�γ鿨");
        // ��ʾ�鵽�Ŀ�Ƭ��Ч������
        Debug.Log($"���㿨Ƭ����: {cardType}, Ч������: {multiplier}");
        Debug.Log($"�鵽�Ŀ�Ƭ: {curCards[0]}, {curCards[1]}, {curCards[2]}");
        if (curNumDraws >= RoundManager.Instance.maxNumDraws)
        {
            Debug.Log("�鿨�����Ѵ�����");
            RoundManager.Instance.SwitchToEnemyRound();
            curNumDraws = 0; // ���ó鿨����
            return;
        }
    }
    #region ������غ���
    [ContextMenu("���ظ���")]
    // ���²����ظ���Ƭ���͵ĳ��и���
    public float[] UpdateProbability()
    {
        // �����������飬��СΪ��Ƭ���͵�����
        float[] probabilities = new float[System.Enum.GetValues(typeof(CardType)).Length];

        // �������Ϊ�գ�����ȫ0����
        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogWarning("����Ϊ�գ��޷��������");
            return probabilities;
        }

        // ͳ��ÿ�ֿ�Ƭ���͵�����
        Dictionary<CardType, int> cardCounts = new Dictionary<CardType, int>();

        // ��ʼ��������
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            cardCounts[type] = 0;
        }

        // ͳ�ƿ�����ÿ�����͵�����
        foreach (CardType card in cardPool)
        {
            cardCounts[card]++;
        }

        // �����ܿ�Ƭ��
        int totalCards = cardPool.Count;

        // ����ÿ�����͵ĸ���
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            probabilities[index] = (float)cardCounts[type] / totalCards;
            index++;
        }

        // �������
        LogProbabilities(probabilities);

        return probabilities;
    }

    //���ԣ����������Ϣ
    private void LogProbabilities(float[] probabilities)
    {
        Debug.Log("=== ��Ƭ���и��� ===");
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            Debug.Log($"{type}: {probabilities[index]:P2}"); // P2��ʾ�ٷֱȸ�ʽ������2λС��
            index++;
        }
        Debug.Log($"��������: {cardPool.Count}");
    }

    //��ȡ�ض���Ƭ���͵ĸ���
    public float GetCardTypeProbability(CardType cardType)
    {
        float[] probabilities = UpdateProbability();
        int index = (int)cardType;

        if (index >= 0 && index < probabilities.Length)
        {
            return probabilities[index];
        }

        return 0f;
    }
    [ContextMenu("Use Attack Card")]
    public void UseAttackCard()
    {
        Debug.Log("ʹ�ù�����Ƭ");
        
    }
    [ContextMenu("Use Reply Card")]
    public void UseReplyCard()
    {
        Debug.Log("ʹ�ûظ���Ƭ");

    }
    //�޸Ŀ������ض����Ϳ�Ƭ��������Magic��ƬЧ����
    [ContextMenu("Use Magic Card")]
    public void UseMagicCard()
    {
       Debug.Log("ʹ��ħ����Ƭ");
    }
       
    [ContextMenu("Use AttackUp Card")]
    public void UseAttackUpCard()
    {
        Debug.Log("ʹ�ù�����ǿ��Ƭ");
    }
    [ContextMenu("Use RoundEnd Card")]
    public void UseRoundEndCard()
    {
        Debug.Log("ʹ�ûغϽ�����Ƭ");
    }
    public void ModifyCardCount(CardType cardType, int count = 1)
    {
        // �Ƴ����и����͵Ŀ�Ƭ
        //cardPool.RemoveAll(card => card == cardType);

        // ���ָ�������Ŀ�Ƭ
        for (int i = 0; i < count; i++)
        {
            cardPool.Add(cardType);
        }

        Debug.Log($"�޸ĺ� {cardType} ������Ϊ: {count}");
    }
    #endregion
}