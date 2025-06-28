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

    [Header("卡片数据")]
    public CardData[] cardDataArray = new CardData[5]; // 存储5种卡片的数据

    [Header("UI设置")]
    public Image[] uiSlot1 = new Image[3]; // 第一个UI数组
    public Image[] uiSlot2 = new Image[3]; // 第二个UI数组
    public Image[] uiSlot3 = new Image[3]; // 第三个UI数组

    private CardType[] curCards = new CardType[3];
    private float effectMultiplier = 1f;
    private List<CardType> cardPool;

    private bool isSpinning = false; // 是否正在转动
    private float moveInterval = 0.3f; // 移动间隔
    private float spinDuration = 3f; // 每个卡片转动时间
    private float delayBetweenSlots = 1f; // 卡片组之间的延迟
    [SerializeField] private CardType magicCard = CardType.attack;
    [SerializeField] private int ExtraNum;

    public static DrawCardsManager Instance { get; private set; }

    private int curNumDraws=0;// 当前抽卡次数

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

    // 初始化UI显示
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

    // 抽奖方法
    public (CardType, float) DrawCards()
    {
        if (isSpinning) return (CardType.attack, 0f); // 如果正在转动，返回默认值

        effectMultiplier = 1f;

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

        // 找出出现次数最多的卡片类型
        var maxCount = cardCounts.Max(x => x.Value);
        var mostFrequentCard = cardCounts.FirstOrDefault(x => x.Value == maxCount).Key;

        // 设置效果倍率
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

        // 开始转动动画
        StartCoroutine(SpinSlots());

        return (resultCard, effectMultiplier);
    }

    // 转动老虎机
    private IEnumerator SpinSlots()
    {
        isSpinning = true;

        // 启动三个槽位的转动
        StartCoroutine(SpinSingleSlot(uiSlot1, 0));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot2, 1));
        yield return new WaitForSeconds(delayBetweenSlots);

        StartCoroutine(SpinSingleSlot(uiSlot3, 2));

        // 等待所有转动完成
        yield return new WaitForSeconds(spinDuration + delayBetweenSlots);

        isSpinning = false;
    }

    // 单个槽位的转动
    private IEnumerator SpinSingleSlot(Image[] slot, int targetIndex)
    {
        float elapsedTime = 0f;
        int currentOffset = 0;

        // 获取目标卡片在cardDataArray中的索引
        int targetCardIndex = GetCardDataIndex(curCards[targetIndex]);
        while (elapsedTime < spinDuration)
        {
            // 更新显示
            for (int i = 0; i < 3; i++)
            {
                int cardIndex = (i - currentOffset + 500) % 5; // 加500确保正数
                slot[i].sprite = cardDataArray[cardIndex].sprite;
            }

            currentOffset++;
            elapsedTime += moveInterval;
            yield return new WaitForSeconds(moveInterval);
        }

        // 确保最终停在正确的位置（中间位置显示目标卡片）
        int finalOffset = (targetCardIndex - 1 + 5) % 5;
        for (int i = 0; i < 3; i++)
        {
            int cardIndex = (i + finalOffset) % 5;
            slot[i].sprite = cardDataArray[cardIndex].sprite;
        }
    }

    // 获取卡片类型在数组中的索引
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
            Debug.Log("当前不是player回合，无法抽卡");
            return;
        }
        if (isSpinning)
        {
            Debug.Log("正在抽卡中，请稍候...");
            return;
        }

        var (cardType, multiplier) = DrawCards();
        curNumDraws += 1;
        Debug.Log("第"+(curNumDraws)+"次抽卡");
        // 显示抽到的卡片和效果倍率
        Debug.Log($"结算卡片类型: {cardType}, 效果倍率: {multiplier}");
        Debug.Log($"抽到的卡片: {curCards[0]}, {curCards[1]}, {curCards[2]}");
        if (curNumDraws >= RoundManager.Instance.maxNumDraws)
        {
            Debug.Log("抽卡次数已达上限");
            RoundManager.Instance.SwitchToEnemyRound();
            curNumDraws = 0; // 重置抽卡次数
            return;
        }
    }
    #region 概率相关函数
    [ContextMenu("返回概率")]
    // 更新并返回各卡片类型的抽中概率
    public float[] UpdateProbability()
    {
        // 创建概率数组，大小为卡片类型的数量
        float[] probabilities = new float[System.Enum.GetValues(typeof(CardType)).Length];

        // 如果卡池为空，返回全0数组
        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogWarning("卡池为空，无法计算概率");
            return probabilities;
        }

        // 统计每种卡片类型的数量
        Dictionary<CardType, int> cardCounts = new Dictionary<CardType, int>();

        // 初始化计数器
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            cardCounts[type] = 0;
        }

        // 统计卡池中每种类型的数量
        foreach (CardType card in cardPool)
        {
            cardCounts[card]++;
        }

        // 计算总卡片数
        int totalCards = cardPool.Count;

        // 计算每种类型的概率
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            probabilities[index] = (float)cardCounts[type] / totalCards;
            index++;
        }

        // 调试输出
        LogProbabilities(probabilities);

        return probabilities;
    }

    //测试：输出概率信息
    private void LogProbabilities(float[] probabilities)
    {
        Debug.Log("=== 卡片抽中概率 ===");
        int index = 0;
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            Debug.Log($"{type}: {probabilities[index]:P2}"); // P2表示百分比格式，保留2位小数
            index++;
        }
        Debug.Log($"卡池总数: {cardPool.Count}");
    }

    //获取特定卡片类型的概率
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
        Debug.Log("使用攻击卡片");
        
    }
    [ContextMenu("Use Reply Card")]
    public void UseReplyCard()
    {
        Debug.Log("使用回复卡片");

    }
    //修改卡池中特定类型卡片的数量（Magic卡片效果）
    [ContextMenu("Use Magic Card")]
    public void UseMagicCard()
    {
       Debug.Log("使用魔法卡片");
    }
       
    [ContextMenu("Use AttackUp Card")]
    public void UseAttackUpCard()
    {
        Debug.Log("使用攻击增强卡片");
    }
    [ContextMenu("Use RoundEnd Card")]
    public void UseRoundEndCard()
    {
        Debug.Log("使用回合结束卡片");
    }
    public void ModifyCardCount(CardType cardType, int count = 1)
    {
        // 移除所有该类型的卡片
        //cardPool.RemoveAll(card => card == cardType);

        // 添加指定数量的卡片
        for (int i = 0; i < count; i++)
        {
            cardPool.Add(cardType);
        }

        Debug.Log($"修改后 {cardType} 的数量为: {count}");
    }
    #endregion
}