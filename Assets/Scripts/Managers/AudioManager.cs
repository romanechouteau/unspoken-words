using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer[] mixers;
    public AudioSource[] sources;

    public void ToUserCountSnapshot(int faceIndex, int userCount) {
        if(userCount <= 4) {
            TransitionToSnapshot(faceIndex, "State " + userCount);
        }
    }

    public void TransitionToSnapshot(int faceIndex, string snapshotName) {
        AudioMixerSnapshot snapshot = mixers[faceIndex].FindSnapshot(snapshotName);
        snapshot.TransitionTo(.5f);
    }

    public void PlayEvent(int faceIndex, int audioIndex) {
        sources[faceIndex*2 + audioIndex].Play();
    }

}
