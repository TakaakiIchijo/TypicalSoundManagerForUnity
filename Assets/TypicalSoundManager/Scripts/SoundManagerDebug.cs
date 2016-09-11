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

            for (int i = 0; i < sm.BgmAudioSourceExList.Count; i++)
            {
                if (sm.BgmAudioSourceExList[i] != null)
                {
                    string setBGMName = sm.BgmAudioSourceExList[i].audioSource.clip == null ? "" : sm.BgmAudioSourceExList[i].audioSource.clip.name;
                    GUI.Label(new Rect(0, 10 * (i + 1), 600, 30), "BGMSource " + i + " " +
                        setBGMName
                        + " :isPlaying " +
                        sm.BgmAudioSourceExList[i].audioSource.isPlaying
                        + " :volume " +
                        sm.BgmAudioSourceExList[i].audioSource.volume);
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