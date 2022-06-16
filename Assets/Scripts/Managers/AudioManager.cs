using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer[] mixers;
    public AudioSource[] sources;
    private float[] pitches = new float[6];
    public void Start() {
        for(int i=0; i<sources.Length; i++) {
            pitches[i] = sources[i].pitch;
        }
    }
    public void ToUserCountSnapshot(int faceIndex, int userCount) {
        if(userCount <= 5) {
            TransitionToSnapshot(faceIndex, "State " + userCount);
        }
    }

    public void TransitionToSnapshot(int faceIndex, string snapshotName) {
        AudioMixerSnapshot snapshot = mixers[faceIndex].FindSnapshot(snapshotName);
        snapshot.TransitionTo(.5f);
    }

    public void PlayEvent(int faceIndex, int audioIndex) {
        int sourceIndex = faceIndex*2 + audioIndex;
        sources[sourceIndex].pitch = pitches[sourceIndex] + Random.Range(-2f, 2f);
        sources[sourceIndex].Play();
    }

}
