using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace GGJ2026
{
    public class SoundController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => GameApp.Interface;
        //public AudioClip clip;
        public AudioSource musicSource;  // 音乐AudioSource
        public AudioSource sfxSource;

        [Header("回合对应的音乐")]
        public AudioClip musicForSession1;
        public AudioClip musicForSession2;
        public AudioClip musicForSession3;
        public AudioClip musicForSession4;

        [Header("音效")]
        public AudioClip lightSwitchClip;

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

        void Start()
        {
            var audioSources = GetComponents<AudioSource>();
            if (audioSources.Length >= 2)
            {
                musicSource = audioSources[0];
                sfxSource = audioSources[1];
            }
        }

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            var gameState = this.GetModel<GameStateModel>();
            gameState.Round.RegisterWithInitValue(OnRoundChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnRoundChanged(int round)
        {
            AudioClip newClip = null;

/*            switch (round)
            {
                case 1:
                    //newClip = musicForRound1;
                    break;
                case 2:
                    //newClip = musicForRound2;
                    break;
                case 3:
                    //newClip = musicForRound3;
                    break;
                default:
                    newClip = musicFordefault;
                    break;
            }*/
            if(round <= 2)
            {
                newClip = musicForSession1;
            }
            else if(round <= 3)
            {
                newClip = musicForSession2;
            }
            else if(round <= 5)
            {
                newClip = musicForSession3;
            }
            else
            {
                newClip = musicForSession1;
            }

            if (newClip != null && musicSource != null)
            {
                ChangeAndPlayMusic(newClip);
            }
        }

        private void OnLightOn()
        {   
             var model = GameApp.Interface.GetModel<UIStage_2_Model>();
            /*var selectedIndex = model.Selected_idx;
             GameApp.Interface.GetModel<ICaseLibraryModel>().TryGet(selectedIndex.Value.ToString(), out var casePackSO);
                if(casePackSO != null)
                {
                    ChangeAndPlaySFX(casePackSO.audioClip2);
                }
            else
            {
                ChangeAndPlaySFX(lightSwitchClip);
            }*/
        }
        private void OnDialogueGiven()
        {
            //根据DialogueGraphId播放对应音效
            //if(GameApp.Interface.GetModel<UIStage_2_Model>().
            var model = GameApp.Interface.GetModel<UIStage_2_Model>();
             GameApp.Interface.GetModel<ICaseLibraryModel>().TryGet(model.SelectedIndex.Value.ToString(), out var casePackSO);
                if(casePackSO != null){
                    ChangeAndPlaySFX(casePackSO.audioClip2);
                }
        }
    }
}
