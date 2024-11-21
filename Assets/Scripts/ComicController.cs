using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    private Image comicImage;                  // 漫画を表示するImage
    public Sprite[] comicFrames;               // 漫画の各コマ
    private int currentAudioIndex = 0;         // 現在のAudioClipインデックス
    public AudioClip[] audioClips;             // 各コマに対応する音声
    public AudioSource audioSource;            // 音声を再生するAudioSource
    public LoadingMainScene loadingScene;      // ロード画面のスクリプト参照

    // AudioClipとComicFrameの対応リスト
    private readonly Dictionary<int, int> audioToFrameMap = new Dictionary<int, int>
    {
        { 0, 0 },  // AudioClip[0] に対して ComicFrame[0] を表示
        { 2, 1 },
        { 8, 2 },
        { 12, 3 },
        { 14, 4 },
        { 17, 5 }
    };

    private bool isPlaying = false;            // 再生中かどうかのフラグ

    void Start()
    {
        // 子オブジェクトからImageコンポーネントを取得
        comicImage = GetComponentInChildren<Image>();

        // 最初のコマと音声を設定
        if (comicImage != null && comicFrames.Length > 0)
        {
            comicImage.sprite = comicFrames[0]; // 最初のコマを表示
            PlayAudio(); // 最初の音声を再生
        }
    }

    void Update()
    {
        // Enterキーで次の音声を再生
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextFrameAndAudio();
        }

        // ESCキーで直接ロード画面を表示してシーン遷移
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCキーが押されました。ロード画面を表示してメインシーンへ移行します。");
            StartLoadingScene();
        }

        // 音声の再生終了をチェックして次のフレームと音声を再生
        if (!audioSource.isPlaying && isPlaying)
        {
            ShowNextFrameAndAudio();
        }
    }

    void ShowNextFrameAndAudio()
    {
        if (currentAudioIndex < audioClips.Length - 1)
        {
            currentAudioIndex++; // 次の音声に進む
            PlayAudio();

            // AudioClipインデックスが対応リストに含まれる場合のみComicFrameを更新
            if (audioToFrameMap.ContainsKey(currentAudioIndex))
            {
                int frameIndex = audioToFrameMap[currentAudioIndex];
                if (frameIndex < comicFrames.Length)
                {
                    comicImage.sprite = comicFrames[frameIndex];
                }
                else
                {
                    Debug.LogError($"対応するComicFrameのインデックス {frameIndex} が範囲外です。");
                }
            }
        }
        else
        {
            Debug.Log("最後の音声が再生されました。");
            StartLoadingScene(); // 最後の音声再生後はシーン遷移
        }
    }

    void PlayAudio()
    {
        // 音声を再生
        if (audioClips.Length > currentAudioIndex && audioSource != null)
        {
            audioSource.clip = audioClips[currentAudioIndex];
            audioSource.Play();
            isPlaying = true; // 再生中フラグをオンにする
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
