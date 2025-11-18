using System;
using System.Collections;
using UnityEngine;

public class ScreamerPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [HideInInspector] public bool IsScreamerTime;
    private GameObject enemy;
    private Animator enemyAnimator;
    [SerializeField] private Vector3 endPosition = new Vector3(-13.86f, -0.79f, -13.92f);
    [SerializeField] private Transform player;
    [SerializeField] private GameObject shootPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsScreamerTime) return;
        if (enemy != null) return;

        if (other.transform.CompareTag("Player"))
        {
            StartScreamer();
        }
    }

    private void StartScreamer()
    {
        enemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0, 90, 0));
        enemy.GetComponent<EnemyController>().player = player;
        enemyAnimator = enemy.GetComponent<Animator>();
        enemyAnimator.SetTrigger("Peeking");
        StartCoroutine(MoveEnemy());
        GameManager.instance.LockPlayer(enemy.transform.position);
        shootPoint.SetActive(true);
    }

    private IEnumerator MoveEnemy()
    {
        Vector3 startPos = enemy.transform.position;
        float elapsedTime = 0f;

        // Плавно перемещаем врага
        while (elapsedTime < 0.5f)
        {
            enemy.transform.position = Vector3.Lerp(startPos, endPosition, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Завершаем перемещение
        enemy.transform.position = endPosition;
        GameManager.instance.ActivatePlayer();
    }
}
