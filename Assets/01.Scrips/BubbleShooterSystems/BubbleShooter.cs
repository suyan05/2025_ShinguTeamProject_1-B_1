using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class BubbleShooter : MonoBehaviour
{
    public Sprite[] rotateSprites; //캐릭터 회전 배열
    private SpriteRenderer characterSpriteRenderer;
    private Coroutine rotateAnimCoroutine;

    public Transform firePoint; // 버블 발사 위치

    public float bubbleSpeed = 10f; // 버블 발사 속도
    public float minAngle = -150f, maxAngle = -20f; // 최소/최대 회전 각도

    // 06.17 한재용 수정
    public Transform characterTransform; // 캐릭터

    private BubbleData currentBubbleData; // 현재 발사할 버블 정보
    private BubbleData nextBubbleData;    // 다음 발사할 버블 정보

    public SpriteRenderer currentBubbleVisual; // 발사 준비 중인 버블을 표시하는 SpriteRenderer (Launcher에 붙음)
    public Image nextBubbleVisual;             // 다음 발사 버블 UI (Canvas 안 Image)

    public BubbleData[] bubbleDataList; // ScriptableObject 배열
    public int currentUnlockLevel = 1; // 현재 플레이어 해금 단계

    public Transform shoootedBubbleParent; // 발사된 버블의 부모 객체

    private bool canShoot = true; // 발사 가능 여부

    private GameManager gameManager;


    void Start()
    {
        characterSpriteRenderer = characterTransform.GetComponent<SpriteRenderer>();

        gameManager = FindObjectOfType<GameManager>(); // GameManager 참조
        UpdateCurrentUnlockLevel(gameManager.GetMaxBubbleLevel()); // 시작할 때 최신 레벨 반영
        // 처음 시작할 때는 nextBubbleData만 먼저 뽑고, PrepareNextBubble에서 current로 옮긴다
        nextBubbleData = GetRandomBubbleData();
        PrepareNextBubble();
    }

    void Update()
    {
        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0) && canShoot)
            ShootBubble();
    }

    public void UpdateCurrentUnlockLevel(int newLevel)
    {
        currentUnlockLevel = Mathf.Clamp(newLevel, 1, 7);
    }

    private void RotateTowardsMouse()
    {
        // 마우스 방향을 바라보게 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    private void ShootBubble()
    {
        canShoot = false;

        // 캐릭터 반동 애니메이션
        if (characterTransform != null)
        {
            characterTransform.DOJump(characterTransform.position, 0.2f, 1, 0.3f).SetEase(Ease.OutQuad);

            // 회전 애니메이션 시작
            rotateAnimCoroutine = StartCoroutine(PlayRotateAnimation(0.3f)); // 0.3초 동안 애니메이션

            // 0.3초 후 코루틴 중지 및 기본 스프라이트 복원
            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (rotateAnimCoroutine != null)
                {
                    StopCoroutine(rotateAnimCoroutine);
                    rotateAnimCoroutine = null;

                    // 기본 스프라이트로 복원 (처음 스프라이트 또는 원하는 스프라이트)
                    characterSpriteRenderer.sprite = rotateSprites[0];
                }
            });
        }

        // 현재 발사할 버블 생성
        GameObject bubbleObj = Instantiate(
            currentBubbleData.prefab,
            firePoint.position,
            Quaternion.identity,
            shoootedBubbleParent
        );

        // 스케일 초기화 + 커지게 애니메이션
        bubbleObj.transform.localScale = Vector3.one * 0.035f;
        bubbleObj.transform.DOScale(0.065f, 0.3f).SetEase(Ease.OutBack);

        Bubble b = bubbleObj.GetComponent<Bubble>();

        // 발사 방향 계산
        float angle = transform.rotation.eulerAngles.z;
        Vector2 shootDir = new Vector2(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

        b.SetDirection(shootDir);
        b.SetSpeed(bubbleSpeed);
        b.SetShooter(this);

        // 다음 버블 준비
        PrepareNextBubble();
    }

    public void EnableShooting() => canShoot = true;

    // 다음 버블을 현재로 옮기고, 다시 룰렛 돌리기
    private void PrepareNextBubble()
    {
        currentBubbleData = nextBubbleData;

        Vector3 originalScale = currentBubbleVisual.transform.localScale;
        SpriteRenderer spriteRenderer = currentBubbleVisual;

        // currentBubbleVisual 페이드 아웃 + 스케일 축소
        spriteRenderer.DOFade(0f, 0.1f).OnComplete(() =>
        {
            currentBubbleVisual.transform
                .DOScale(originalScale * 0.8f, 0.1f)
                .OnComplete(() =>
                {
                    spriteRenderer.sprite = currentBubbleData.prefab.GetComponent<SpriteRenderer>().sprite;

                    // currentBubbleVisual 스케일 원래대로 + 페이드 인
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

    // 룰렛 방식으로 확률 기반 버블 선택
    private BubbleData GetRandomBubbleData()
    {
        float total = 0f;
        List<float> weights = new List<float>();

        for (int i = 0; i < bubbleDataList.Length; i++)
        {
            float chance = 0f;

            // 현재 해금된 단계까지만 확률 계산
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

        // fallback: 아무것도 못 뽑았을 경우 첫 번째 반환
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
}