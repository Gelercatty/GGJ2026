using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GGJ2026
{
    public class SoundController : MonoBehaviour, IController
    {
        // Start is called before the first frame update
        public IArchitecture GetArchitecture() => GameApp.Interface;
        public AudioClip clip;
        public AudioSource musicSource;  // 音乐AudioSource
        public AudioSource sfxSource;
        public void ChangeAndPlayMusic(AudioClip newClip)
        {
            if (musicSource == null) return;

            // 停止当前播放
            musicSource.Stop();

            // 更换Clip
            musicSource.clip = newClip;

            // 播放新的Clip
            musicSource.Play();
        }
        public void ChangeAndPlaySFX(AudioClip newClip)
        {
            if (sfxSource == null) return;

            // 停止当前播放
            sfxSource.Stop();

            // 更换Clip
            sfxSource.clip = newClip;

            // 播放新的Clip
            sfxSource.Play();
        }
        public void ChangeMusicWithFade(AudioClip newClip, float fadeDuration = 1f)
        {
            StartCoroutine(FadeAndChangeMusic(newClip, fadeDuration));
        }

        private IEnumerator FadeAndChangeMusic(AudioClip newClip, float fadeDuration)
        {
            // 淡出当前音乐
            float startVolume = musicSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.Play();

            // 淡入新音乐
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
                yield return null;
            }
        }

        void start()
        {
            musicSource = GetComponents<AudioSource>()[0];
            sfxSource = GetComponents<AudioSource>()[1];


            // 或者通过公共变量在Inspector中手动拖拽指定
        }
        private void Awake()
        {
            
        }
        private void OnEnable()
        {
            
        }

    }
}
