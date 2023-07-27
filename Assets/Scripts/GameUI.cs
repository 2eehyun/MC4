using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    private Player player; // Player 스크립트 참조를 저장할 변수

    void Start()
    {
        // print("test0");
        StartCoroutine(WaitForPlayer());
        // print("test4");
    }

    IEnumerator WaitForPlayer()
    {
        // print("test1");
        yield return new WaitForSeconds(10f);
        player = FindObjectOfType<Player>();
        // print("test2");

        if (player != null)
            player.OnDeath += OnPlayerDeath;
    }

    void OnPlayerDeath()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
}
