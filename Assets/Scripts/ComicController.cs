using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    private Image comicImage;          // 漫画を表示するImage
    public Sprite[] comicFrames;       // 漫画の各コマ
    private int currentFrame = 0;      // 現在のフレーム
    public LoadingMainScene loadingScene; // ロード画面のスクリプト参照

    void Start()
    {
        // 子オブジェクトからImageコンポーネントを取得
        comicImage = GetComponentInChildren<Image>();

        // 最初のコマを表示
        if (comicImage != null && comicFrames.Length > 0)
        {
            comicImage.sprite = comicFrames[currentFrame];
        }
    }

    void Update()
    {
        // Enterキーで次のコマへ
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextFrame();
        }

        // ESCキーで直接ロード画面を表示してシーン遷移
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCキーが押されました。ロード画面を表示してメインシーンへ移行します。");
            StartLoadingScene();
        }
    }

    void ShowNextFrame()
    {
        if (comicImage != null && currentFrame < comicFrames.Length - 1)
        {
            // 次のコマを表示
            currentFrame++;
            comicImage.sprite = comicFrames[currentFrame];
        }
        else if (currentFrame == comicFrames.Length - 1)
        {
            // 最後のコマのときにロード画面を表示してシーン遷移
            Debug.Log("漫画終了！ ロード画面を表示してメインシーンへ移行します。");
            StartLoadingScene();
        }
    }

    void StartLoadingScene()
    {
        // ロード画面を表示してシーン遷移
        if (loadingScene != null)
        {
            loadingScene.LoadNextScene();
        }
        else
        {
            Debug.LogError("LoadingMainSceneが設定されていません。");
        }
    }
}
