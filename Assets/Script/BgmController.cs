using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Music
{
    public enum EnumMusic
    {
        Start,
        Lobby,
        PlayingRoom
    }

    public class BgmController : MonoBehaviour
    {
        private Dictionary<EnumMusic, AudioClip> musicDict = new Dictionary<EnumMusic, AudioClip>();

        [SerializeField] private AudioClip startMusic = null;
        [SerializeField] private AudioClip lobbyMusic = null;
        [SerializeField] private AudioClip playingRoomMusic = null;

        void Start() 
        {
            AddMusic(EnumMusic.Start,startMusic);
            AddMusic(EnumMusic.Lobby,lobbyMusic);
            AddMusic(EnumMusic.PlayingRoom,playingRoomMusic);
        }

        public void AddMusic(EnumMusic enumMusic, AudioClip music)
        {
            Debug.Log($"add music: {enumMusic}");
            if (!musicDict.ContainsKey(enumMusic))
                musicDict.Add(enumMusic, music);
        }

        public void PlayMusic(EnumMusic enumMusic)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.Stop();
            switch (enumMusic)
            {
                
                case EnumMusic.Start:
                    try
                    {
                        audioSource.clip = musicDict[EnumMusic.Start];
                        audioSource.Play();
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumMusic.Lobby:
                    try
                    {
                        audioSource.clip = musicDict[EnumMusic.Lobby];
                        audioSource.Play();
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumMusic.PlayingRoom:
                    try
                    {
                        audioSource.clip = musicDict[EnumMusic.PlayingRoom];
                        audioSource.Play();
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
            }
        }

        public void setVolume(float volume)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

}