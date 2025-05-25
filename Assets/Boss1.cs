using System.Collections;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Boss1 : Boss
{
    [SerializeField]
    private GameObject circleAttack;

    [SerializeField]
    private int normalAlpha;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        InitializeBoss();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBoss();
    }

    // Called when the object is destroyed\
    void OnDestroy()
    {
        OnDestroyBoss();

        StartCoroutine(ShowFadeThenEndScreen());
    }

    private IEnumerator ShowFadeThenEndScreen()
    {
        yield return new WaitForSeconds(10);
        GameManager.Instance.ShowFadeOutThenEndScreen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackOnTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AttackOnTrigger(collision);
    }

    public override void IdleState()
    {
        base.IdleState();

        percentageOfDamageApplied = 100;
    }

    public override void MoveState()
    {
        base.MoveState();
        percentageOfDamageApplied = 65;
    }

    public override void AttackState()
    {
        base.AttackState();

        int random = 0;
        int maxEnemies;
        BasicEnemy enemy = null;

        if(IsRaging())
            maxEnemies = 1;
        else
            maxEnemies = 3;

        for (int i = 0; i < Random.Range(2, maxEnemies); i++)
        {
            if (IsRaging())
            {
                random = Random.Range(0, GameManager.Instance.RageEnemyTypes.Length);
                enemy = GameManager.Instance.RageEnemyTypes[random];
            }
            else
            {
                random = Random.Range(0, GameManager.Instance.BasicEnemyTypes.Length);
                enemy = GameManager.Instance.BasicEnemyTypes[random];
            }

            var angle = i * Mathf.PI * 2 / maxEnemies;
            var x = Mathf.Cos(angle) * 2;
            var y = Mathf.Sin(angle) * 2;
            var enemyInstance = Instantiate(enemy, transform.position + new Vector3(x, y, 0), Quaternion.identity);
            var room = FindAnyObjectByType<Room>();
            room.totalEnemies++;
            room.enemiesLeft++;

            enemyInstance.transform.SetParent(room.transform.Find("Enemies"), true);

            enemyInstance.Initialize();
        }

        StartCoroutine(ScaleCircleUpThenFadeOut());
    }

    private IEnumerator ScaleCircleUpThenFadeOut()
    {
        circleAttack.SetActive(true);
        ResetAlpha();
        circleAttack.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        float time = Random.Range(1.5f, 3f);
        float scale = 20f;
        float scaleSpeed = scale / time;
        float currentScale = 0.1f;
        while (currentScale < scale)
        {
            currentScale += scaleSpeed * Time.deltaTime;
            circleAttack.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return null;
        }
        
        StartCoroutine(FadeCircleAlpha());
    }

    private IEnumerator FadeCircleAlpha()
    {
        float fadeTime = 0.5f;
        float fadeSpeed = 1f / fadeTime;
        Color color = circleAttack.GetComponent<SpriteRenderer>().color;
        while (color.a > 0)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            circleAttack.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }

        circleAttack.SetActive(false);
    }

    public override void RageState()
    {
        base.RageState();

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void ResetAlpha()
    {
        Color32 color = circleAttack.GetComponent<SpriteRenderer>().color;
        color.a = (byte)normalAlpha;
        circleAttack.GetComponent<SpriteRenderer>().color = color;
    }
}
