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
    [System.Serializable]
    public class MagicCard
    {
        public CardType cardType;
        public int Effect=1;
    }

    [Header("������������")]
    public CardData[] cardDataArray = new CardData[5]; // �洢5�ֿ��Ƶ�����

    [Header("UI��λ")]
    public Image[] uiSlot1 = new Image[3]; // ��һ��UI��λ
    public Image[] uiSlot2 = new Image[3]; // �ڶ���UI��λ
    public Image[] uiSlot3 = new Image[3]; // ������UI��λ

    private CardType[] curCards = new CardType[3];
    private int effectMultiplier = 1;
    private List<CardType> cardPool;

    private bool isSpinning = false; // �Ƿ�����ת��
    private float moveInterval = 0.3f; // �ƶ����
    private float spinDuration = 3f; // �ܵ���ת����ʱ��
    private float delayBetweenSlots = 1f; // ��λ֮����ӳ�

    public static DrawCardsManager Instance { get; private set; }

    private int curNumDraws = 0;// ��ǰ�鿨����
    [Header("ħ����Type")]
    public List<MagicCard> magicCards = new List<MagicCard>();

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

    // ��ʼ��UI��λ
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

    // �鿨������
    public (CardType, int) DrawCards()
    {
        if (isSpinning) return (CardType.attack, 0); // �������ת���򷵻�Ĭ��ֵ

        effectMultiplier = 1;

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

        // �ҳ��������Ŀ�Ƭ����
        var maxCount = cardCounts.Max(x => x.Value);
        var mostFrequentCard = cardCounts.FirstOrDefault(x => x.Value == maxCount).Key;

        // ����Ч������
        CardType resultCard;
        if (maxCount == 3)
        {
            effectMultiplier = 7;
            resultCard = mostFrequentCard;
        }
        else if (maxCount == 2)
        {
            effectMultiplier = 3;
            resultCard = mostFrequentCard;
        }
        else
        {
            effectMultiplier = 1;
            resultCard = curCards[0];
        }

        // ��ʼת������
        StartCoroutine(SpinSlots());

        return (resultCard, effectMultiplier);
    }

    // ת������
    private IEnumerator SpinSlots()
    {
        isSpinning = true;

        // ��������ÿ����λ��ת��
        StartCoroutine(SpinSingleSlot(uiSlot1, 0));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot2, 1));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot3, 2));

        // �ȴ����в�λֹͣת��
        yield return new WaitForSeconds(spinDuration + delayBetweenSlots);

        isSpinning = false;
    }

    // ������λ��ת��
    private IEnumerator SpinSingleSlot(Image[] slot, int targetIndex)
    {
        float elapsedTime = 0f;
        int currentOffset = 0;

        int targetCardIndex = GetCardDataIndex(curCards[targetIndex]);
        while (elapsedTime < spinDuration)
        {
            for (int i = 0; i < 3; i++)
            {
                int cardIndex = (i - currentOffset + 500) % 5; // ��500��Ϊ�˱��⸺��
                slot[i].sprite = cardDataArray[cardIndex].sprite;
            }

            currentOffset++;
            elapsedTime += moveInterval;
            yield return new WaitForSeconds(moveInterval);
        }

        // ��������λ��
        int finalOffset = (targetCardIndex - 1 + 5) % 5;
        for (int i = 0; i < 3; i++)
        {
            int cardIndex = (i + finalOffset) % 5;
            slot[i].sprite = cardDataArray[cardIndex].sprite;
        }
    }

    // ��ȡ�������������������е�����
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
            Debug.Log("���ڲ���player�غϣ��޷��鿨");
            return;
        }
        if (isSpinning)
        {
            Debug.Log("���ڳ鿨�У���ȴ�...");
            return;
        }

        var (cardType, multiplier) = DrawCards();
        curNumDraws += 1;
        Debug.Log("��" + (curNumDraws) + "�γ鿨");
        Debug.Log($"�鵽�Ŀ���: {cardType}, Ч������: {multiplier}");
        Debug.Log($"�鵽�Ŀ�: {curCards[0]}, {curCards[1]}, {curCards[2]}");
        UseCardEffect(cardType, (int)multiplier);

        if (curNumDraws >= RoundManager.Instance.maxNumDraws)
        {
            Debug.Log("�Ѵﵽ���鿨����");
            RoundManager.Instance.SwitchToEnemyRound();
            curNumDraws = 0; // ���ó鿨����
            return;
        }
    }

    #region ���Ƹ������

    [ContextMenu("���¸���")]
    public float[] UpdateProbability()
    {
        float[] probabilities = new float[System.Enum.GetValues(typeof(CardType)).Length];

        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogWarning("����Ϊ�գ��޷��������");
            return probabilities;
        }

        Dictionary<CardType, int> cardCounts = new Dictionary<CardType, int>();

        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            cardCounts[type] = 0;
        }

        foreach (CardType card in cardPool)
        {
            cardCounts[card]++;
        }

        int totalCards = cardPool.Count;

        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            probabilities[index] = (float)cardCounts[type] / totalCards;
            index++;
        }

        LogProbabilities(probabilities);

        return probabilities;
    }

    private void LogProbabilities(float[] probabilities)
    {
        Debug.Log("=== ��Ƭ���и��� ===");
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            Debug.Log($"{type}: {probabilities[index]:P2}");
            index++;
        }
        Debug.Log($"��������: {cardPool.Count}");
    }

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
    public void ModifyCardCount(CardType cardType, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            cardPool.Add(cardType);
        }

        Debug.Log($"�޸ĺ� {cardType} ������Ϊ: {count}");
    }
    #endregion
    public void UseCardEffect(CardType cardType, int multiplier = 1)
    {
        switch (cardType)
        {
            case CardType.attack:
                UseAttackCard(multiplier);
                break;
            case CardType.reply:
                UseReplyCard(multiplier);
                break;
            case CardType.magic:
                UseMagicCard(multiplier);
                break;
            case CardType.AttackUp:
                UseAttackUpCard(multiplier);
                break;
            case CardType.roundEnd:
                UseRoundEndCard(multiplier);
                break;
            default:
                Debug.LogWarning("δ֪��������: " + cardType);
                break;
        }
        GamePanel.Instance.UpdateUI(); // ֪ͨUI����
    }

    public void UseAttackCard(int multiplier = 1)
    {
        Debug.Log("ʹ�ù�����");
        Player.Instance.Attack();
    }

    public void UseReplyCard(int multiplier = 1)
    {
        Debug.Log("ʹ�û�Ѫ��");
        Player.Instance.AddCoin(10 * multiplier);
    }

    public void UseMagicCard(int multiplier = 1)
    {
        Debug.Log("ʹ��ħ����");
        int cardCount = 1;
        switch (multiplier)
        {
            case 1:
                cardCount = 1;
                break;
            case 3:
                cardCount = 2;
                break;
            default:
                cardCount = 3;
                break;
        }
        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = Random.Range(0, 5);
            int effect= Random.Range(0, 2) == 0 ? 1 : -1;
            CardType randomCard = (CardType)randomIndex;
            MagicCard magicCard = new MagicCard();
            magicCard.cardType = randomCard;
            magicCard.Effect = effect;
            magicCards.Add(magicCard);
        }
    }

    public void UseAttackUpCard(int multiplier = 1)
    {
        Debug.Log("ʹ�ù�����ǿ��");
        Player.Instance.AttackUp(5 * multiplier);
    }

    public void UseRoundEndCard(int multiplier = 1)
    {
        Debug.Log("ʹ�ûغϽ�����");
        RoundManager.Instance.SwitchToEnemyRound();
        curNumDraws = 0; // ���ó鿨����
    }

}