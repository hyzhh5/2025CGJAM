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

    [Header("卡牌数据数组")]
    public CardData[] cardDataArray = new CardData[5]; // 存储5种卡牌的数据

    [Header("UI槽位")]
    public Image[] uiSlot1 = new Image[3]; // 第一行UI槽位
    public Image[] uiSlot2 = new Image[3]; // 第二行UI槽位
    public Image[] uiSlot3 = new Image[3]; // 第三行UI槽位

    private CardType[] curCards = new CardType[3];
    private int effectMultiplier = 1;
    private List<CardType> cardPool;

    private bool isSpinning = false; // 是否正在转动
    private float moveInterval = 0.3f; // 移动间隔
    private float spinDuration = 3f; // 总的旋转持续时间
    private float delayBetweenSlots = 1f; // 槽位之间的延迟

    public static DrawCardsManager Instance { get; private set; }

    private int curNumDraws = 0;// 当前抽卡次数
    [Header("魔法卡Type")]
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

    // 初始化UI槽位
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

    // 抽卡主函数
    public (CardType, int) DrawCards()
    {
        if (isSpinning) return (CardType.attack, 0); // 如果正在转动则返回默认值

        effectMultiplier = 1;

        // 随机抽取3张卡
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, cardPool.Count);
            curCards[i] = cardPool[randomIndex];
        }

        // 统计每种卡片的数量
        var cardCounts = new Dictionary<CardType, int>();
        foreach (var card in curCards)
        {
            if (!cardCounts.ContainsKey(card))
                cardCounts[card] = 0;
            cardCounts[card]++;
        }

        // 找出出现最多的卡片类型
        var maxCount = cardCounts.Max(x => x.Value);
        var mostFrequentCard = cardCounts.FirstOrDefault(x => x.Value == maxCount).Key;

        // 计算效果倍率
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

        // 开始转动动画
        StartCoroutine(SpinSlots());

        return (resultCard, effectMultiplier);
    }

    // 转动动画
    private IEnumerator SpinSlots()
    {
        isSpinning = true;

        // 依次启动每个槽位的转动
        StartCoroutine(SpinSingleSlot(uiSlot1, 0));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot2, 1));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot3, 2));

        // 等待所有槽位停止转动
        yield return new WaitForSeconds(spinDuration + delayBetweenSlots);

        isSpinning = false;
    }

    // 单个槽位的转动
    private IEnumerator SpinSingleSlot(Image[] slot, int targetIndex)
    {
        float elapsedTime = 0f;
        int currentOffset = 0;

        int targetCardIndex = GetCardDataIndex(curCards[targetIndex]);
        while (elapsedTime < spinDuration)
        {
            for (int i = 0; i < 3; i++)
            {
                int cardIndex = (i - currentOffset + 500) % 5; // 加500是为了避免负数
                slot[i].sprite = cardDataArray[cardIndex].sprite;
            }

            currentOffset++;
            elapsedTime += moveInterval;
            yield return new WaitForSeconds(moveInterval);
        }

        // 设置最终位置
        int finalOffset = (targetCardIndex - 1 + 5) % 5;
        for (int i = 0; i < 3; i++)
        {
            int cardIndex = (i + finalOffset) % 5;
            slot[i].sprite = cardDataArray[cardIndex].sprite;
        }
    }

    // 获取卡牌类型在数据数组中的索引
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

    [ContextMenu("抽卡")]
    public void TestDraw()
    {
        if (RoundManager.Instance.round_Parameter.currentEState != Estate.playerRound)
        {
            Debug.Log("现在不是player回合，无法抽卡");
            return;
        }
        if (isSpinning)
        {
            Debug.Log("正在抽卡中，请等待...");
            return;
        }

        var (cardType, multiplier) = DrawCards();
        curNumDraws += 1;
        Debug.Log("第" + (curNumDraws) + "次抽卡");
        Debug.Log($"抽到的卡牌: {cardType}, 效果倍率: {multiplier}");
        Debug.Log($"抽到的卡: {curCards[0]}, {curCards[1]}, {curCards[2]}");
        UseCardEffect(cardType, (int)multiplier);

        if (curNumDraws >= RoundManager.Instance.maxNumDraws)
        {
            Debug.Log("已达到最大抽卡次数");
            RoundManager.Instance.SwitchToEnemyRound();
            curNumDraws = 0; // 重置抽卡次数
            return;
        }
    }

    #region 卡牌概率相关

    [ContextMenu("更新概率")]
    public float[] UpdateProbability()
    {
        float[] probabilities = new float[System.Enum.GetValues(typeof(CardType)).Length];

        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogWarning("卡池为空，无法计算概率");
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
        Debug.Log("=== 卡片抽中概率 ===");
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            Debug.Log($"{type}: {probabilities[index]:P2}");
            index++;
        }
        Debug.Log($"卡池总数: {cardPool.Count}");
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

        Debug.Log($"修改后 {cardType} 的数量为: {count}");
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
                Debug.LogWarning("未知卡牌类型: " + cardType);
                break;
        }
        GamePanel.Instance.UpdateUI(); // 通知UI更新
    }

    public void UseAttackCard(int multiplier = 1)
    {
        Debug.Log("使用攻击卡");
        Player.Instance.Attack();
    }

    public void UseReplyCard(int multiplier = 1)
    {
        Debug.Log("使用回血卡");
        Player.Instance.AddCoin(10 * multiplier);
    }

    public void UseMagicCard(int multiplier = 1)
    {
        Debug.Log("使用魔法卡");
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
        Debug.Log("使用攻击增强卡");
        Player.Instance.AttackUp(5 * multiplier);
    }

    public void UseRoundEndCard(int multiplier = 1)
    {
        Debug.Log("使用回合结束卡");
        RoundManager.Instance.SwitchToEnemyRound();
        curNumDraws = 0; // 重置抽卡次数
    }

}