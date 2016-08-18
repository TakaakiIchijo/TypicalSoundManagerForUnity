using UnityEngine;

namespace TSM
{
    [RequireComponent(typeof(SoundManager))]
    public class SoundManagerDebug : MonoBehaviour
    {
        private SoundManager sm;

        private void Awake()
        {
            sm = GetComponent<SoundManager>();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 600, 30), "SoundManager Monitor");

            for (int i = 0; i < sm.BgmAudioSourceList.Count; i++)
            {
                if (sm.BgmAudioSourceList[i] != null)
                {
                    string setBGMName = sm.BgmAudioSourceList[i].clip == null ? "" : sm.BgmAudioSourceList[i].clip.name;
                    GUI.Label(new Rect(0, 10 * (i + 1), 600, 30), "BGMSource " + i + " " +
                        setBGMName
                        + " :isPlaying " +
                        sm.BgmAudioSourceList[i].isPlaying
                        + " :volume " +
                        sm.BgmAudioSourceList[i].volume);
                }
            }

            if (sm.JingleAudioSource.clip != null)
            {
                GUI.Label(new Rect(0, 30, 600, 30), "Jingle " + sm.JingleAudioSource.clip.name + ":isPlaying " + sm.JingleAudioSource.isPlaying);
            }

            if (!string.IsNullOrEmpty(sm.LastPlayedSEName))
            {
                GUI.Label(new Rect(0, 40, 600, 30), "SE " + sm.LastPlayedSEName + ":isPlayed");
            }
        }
    }
}