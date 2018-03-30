using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Assets.TerraDefense.Implementations.Controllers
{
    public class MusicController : MonoBehaviour
    {
        public AudioSource AudioSource;
        public List<AudioClip> MusicList;


        private int _currentMusicIndex;
        private void Start()
        {
            AudioSource.clip = MusicList[0];
            AudioSource.Play();
            _currentMusicIndex = 0;

            StartCoroutine("AutomaticPlayback");
        }

        public void PlayNextTrack()
        {
            StopCoroutine("AutomaticPlayback");
            if (AudioSource.isPlaying) AudioSource.Stop();

            if (_currentMusicIndex >= MusicList.Count) _currentMusicIndex = 0;
            AudioSource.clip = MusicList[_currentMusicIndex++];
            AudioSource.Play();
            StartCoroutine("AutomaticPlayback");
        }

        public void PlayPreviousTrack()
        {
            StopCoroutine("AutomaticPlayback");
            if (AudioSource.isPlaying) AudioSource.Stop();

            if (_currentMusicIndex == 0) _currentMusicIndex = MusicList.Count;
            AudioSource.clip = MusicList[--_currentMusicIndex];
            AudioSource.Play();
            StartCoroutine("AutomaticPlayback");
        }

        private IEnumerator AutomaticPlayback()
        {
            yield return new WaitForSeconds(AudioSource.clip.length);
            PlayNextTrack();
        }
    }
}
