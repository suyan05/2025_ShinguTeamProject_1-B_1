using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using UnityEngine.Analytics;

public class BubbleShooter : MonoBehaviour
{
    public GameObject maxLevelBubblePrefab;

    public Sprite[] rotateSprites; //ĳ���� ȸ�� �迭
    private SpriteRenderer characterSpriteRenderer;
    private Coroutine rotateAnimCoroutine;

    public Transform firePoint; // ���� �߻� ��ġ

    public float bubbleSpeed = 10f; // ���� �߻� �ӵ�
    public float minAngle = -150f, maxAngle = -20f; // �ּ�/�ִ� ȸ�� ����

    // 06.17 ����� ����
    public Transform characterTransform; // ĳ����

    private BubbleData currentBubbleData; // ���� �߻��� ���� ����
    private BubbleData nextBubbleData;    // ���� �߻��� ���� ����

    public SpriteRenderer currentBubbleVisual; // �߻� �غ� ���� ������ ǥ���ϴ� SpriteRenderer (Launcher�� ����)
    public Image nextBubbleVisual;             // ���� �߻� ���� UI (Canvas �� Image)

    public BubbleData[] bubbleDataList; // ScriptableObject �迭
    public int currentUnlockLevel = 1; // ���� �÷��̾� �ر� �ܰ�

    public Transform shoootedBubbleParent; // �߻�� ������ �θ� ��ü

    public bool canShoot = true; // �߻� ���� ����
    public bool isMerging = false;

    private GameManager gameManager;

    public RectTransform areaInGameRect;

    private bool isShootSun = false;


    void Start()
    {
        isShootSun = false;

        characterSpriteRenderer = characterTransform.GetComponent<SpriteRenderer>();

        gameManager = FindObjectOfType<GameManager>(); // GameManager ����
        UpdateCurrentUnlockLevel(gameManager.GetMaxBubbleLevel()); // ������ �� �ֽ� ���� �ݿ�
        // ó�� ������ ���� nextBubbleData�� ���� �̰�, PrepareNextBubble���� current�� �ű��
        nextBubbleData = GetRandomBubbleData();
        PrepareNextBubble();
    }

    void Update()
    {
        RotateTowardsMouse();

        if (canShoot && IsMouseInInGameArea())
        {
            if (!Input.GetKey(KeyCode.X) && Input.GetMouseButtonDown(0))
            {
                ShootBubble();
            }                
            else if (Input.GetKey(KeyCode.X) && Input.GetMouseButtonDown(0))
            {
                ShootSpecificBubble(maxLevelBubblePrefab);
            }
        }

        //ġƮ
        /*if (Input.GetKeyUp(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.RightShift))
        {
            if (!isShootSun)
            {
                isShootSun = true;
                Debug.Log("isShootSun True");
            }
        }

        // �߻� ó��
        if (isShootSun && canShoot && !isMerging)
        {
            if (Input.GetKey(KeyCode.X))
            {
                if (Input.GetMouseButtonDown(0))
                    ShootSpecificBubble(maxLevelBubblePrefab);
            }
            else if (Input.anyKeyDown) // � Ű�� ������ ��
            {
                // ��, X Ű�� ���� (�̹� ������ ó�������ϱ�)
                if (!Input.GetKeyDown(KeyCode.X) || !Input.GetMouseButtonDown(0))
                {
                    isShootSun = false;
                    Debug.Log("isShootSun False");
                }
            }
        }*/
    }

    public void UpdateCurrentUnlockLevel()
    {
        int highestLevel = FindObjectOfType<BubbleGrid>().GetHighestBubbleLevel();
        currentUnlockLevel = Mathf.Max(currentUnlockLevel, highestLevel); // ������ ����, ���� X
    }


    public void UpdateCurrentUnlockLevel(int newLevel)
    {
        currentUnlockLevel = Mathf.Clamp(newLevel, 1, 7);
    }

    private void RotateTowardsMouse()
    {
        // ���콺 ������ �ٶ󺸰� ȸ��
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    private void ShootBubble()
    {
        canShoot = false;

        SoundManager.Instance.PlayShoot();

        // ĳ���� �ݵ� �ִϸ��̼�
        if (characterTransform != null)
        {
            characterTransform.DOJump(characterTransform.position, 0.2f, 1, 0.3f).SetEase(Ease.OutQuad);

            // ȸ�� �ִϸ��̼� ����
            rotateAnimCoroutine = StartCoroutine(PlayRotateAnimation(0.3f)); // 0.3�� ���� �ִϸ��̼�

            // 0.3�� �� �ڷ�ƾ ���� �� �⺻ ��������Ʈ ����
            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (rotateAnimCoroutine != null)
                {
                    StopCoroutine(rotateAnimCoroutine);
                    rotateAnimCoroutine = null;

                    // �⺻ ��������Ʈ�� ���� (ó�� ��������Ʈ �Ǵ� ���ϴ� ��������Ʈ)
                    characterSpriteRenderer.sprite = rotateSprites[0];
                }
            });
        }

        // ���� �߻��� ���� ����
        GameObject bubbleObj = Instantiate(
            currentBubbleData.prefab,
            firePoint.position,
            Quaternion.identity,
            shoootedBubbleParent
        );

        // ������ �ʱ�ȭ + Ŀ���� �ִϸ��̼�
        bubbleObj.transform.localScale = Vector3.one * 0.035f;
        bubbleObj.transform.DOScale(0.065f, 0.3f).SetEase(Ease.OutBack);

        Bubble b = bubbleObj.GetComponent<Bubble>();

        // �߻� ���� ���
        float angle = transform.rotation.eulerAngles.z;
        Vector2 shootDir = new Vector2(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

        b.SetDirection(shootDir);
        b.SetSpeed(bubbleSpeed);
        b.SetShooter(this);

        // ���� ���� �غ�
        PrepareNextBubble();
    }

    public void EnableShooting()
    {
        if (gameManager.gameOver) return;
        canShoot = true;
    }
    // ���� ������ ����� �ű��, �ٽ� �귿 ������
    private void PrepareNextBubble()
    {
        currentBubbleData = nextBubbleData;

        Vector3 originalScale = currentBubbleVisual.transform.localScale;
        SpriteRenderer spriteRenderer = currentBubbleVisual;

        // currentBubbleVisual ���̵� �ƿ� + ������ ���
        spriteRenderer.DOFade(0f, 0.1f).OnComplete(() =>
        {
            currentBubbleVisual.transform
                .DOScale(originalScale * 0.8f, 0.1f)
                .OnComplete(() =>
                {
                    spriteRenderer.sprite = currentBubbleData.prefab.GetComponent<SpriteRenderer>().sprite;

                    // currentBubbleVisual ������ ������� + ���̵� ��
                    currentBubbleVisual.transform
                        .DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
                    spriteRenderer.DOFade(1f, 0.2f);
                });
        });

        Vector3 nextOriginalScale = nextBubbleVisual.transform.localScale;

        nextBubbleVisual.transform
            .DOScale(nextOriginalScale * 0.8f, 0.1f)
            .OnComplete(() =>
            {
                nextBubbleData = GetRandomBubbleData();
                nextBubbleVisual.sprite = nextBubbleData.prefab.GetComponent<SpriteRenderer>().sprite;
                nextBubbleVisual.transform
                    .DOScale(nextOriginalScale, 0.2f)
                    .SetEase(Ease.OutBack);
            });

    }

    // �귿 ������� Ȯ�� ��� ���� ����
    private BubbleData GetRandomBubbleData()
    {
        float total = 0f;
        List<float> weights = new List<float>();

        for (int i = 0; i < bubbleDataList.Length; i++)
        {
            float chance = 0f;

            // ���� �رݵ� �ܰ������ Ȯ�� ���
            if (bubbleDataList[i].level <= currentUnlockLevel &&
                currentUnlockLevel - 1 < bubbleDataList[i].unlockChances.Length)
            {
                chance = bubbleDataList[i].unlockChances[currentUnlockLevel - 1];
            }

            weights.Add(chance);
            total += chance;
        }

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative)
            {
                return bubbleDataList[i];
            }
        }

        // fallback: �ƹ��͵� �� �̾��� ��� ù ��° ��ȯ
        return bubbleDataList[0];
    }

    private IEnumerator PlayRotateAnimation(float duration)
    {
        if (rotateSprites == null || rotateSprites.Length == 0) yield break;

        float timePerFrame = duration / rotateSprites.Length;
        int index = 0;

        while (true)
        {
            characterSpriteRenderer.sprite = rotateSprites[index];
            index = (index + 1) % rotateSprites.Length;
            yield return new WaitForSeconds(timePerFrame);
        }
    }

    public void ShootSpecificBubble(GameObject prefab)
    {
        canShoot = false;

        if (characterTransform != null)
        {
            characterTransform.DOJump(characterTransform.position, 0.2f, 1, 0.3f).SetEase(Ease.OutQuad);
            rotateAnimCoroutine = StartCoroutine(PlayRotateAnimation(0.3f));

            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (rotateAnimCoroutine != null)
                {
                    StopCoroutine(rotateAnimCoroutine);
                    rotateAnimCoroutine = null;
                    characterSpriteRenderer.sprite = rotateSprites[0];
                }
            });
        }

        // ���� ����
        GameObject bubbleObj = Instantiate(
            prefab,
            firePoint.position,
            Quaternion.identity,
            shoootedBubbleParent
        );

        bubbleObj.transform.localScale = Vector3.one * 0.035f;
        bubbleObj.transform.DOScale(0.065f, 0.3f).SetEase(Ease.OutBack);

        Bubble b = bubbleObj.GetComponent<Bubble>();

        float angle = transform.rotation.eulerAngles.z;
        Vector2 shootDir = new Vector2(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

        b.SetDirection(shootDir);
        b.SetSpeed(bubbleSpeed);
        b.SetShooter(this);
    }

    private bool IsMouseInInGameArea()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 localPoint;

        // �̹��� RectTransform �������� ���콺 ��ġ ��ȯ
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            areaInGameRect,
            mouseScreenPos,
            Camera.main, // World Space������ �ַ� Camera.main
            out localPoint
        );

        if (!inside) return false;

        // ���� �̹����� Rect ���� ������ Ȯ��
        Rect rect = areaInGameRect.rect;
        return rect.Contains(localPoint);
    }
}