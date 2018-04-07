using UnityEngine;
using UnityEditor;

public class AudioAssetPostProcessor : AssetPostprocessor
{
    private static readonly string AUDIOCLIP_FOLDER = "AudioClips/";
    private static readonly string BGM = "BGM";
    private static readonly string GAMESE = "GameSe";
    private static readonly string MENUSE = "MenuSe";
    private static readonly string JINGLE = "Jingle";
    private static readonly string VOICE = "Voice";
    private static readonly string ENVIRONMENT = "Environment";

    private static readonly string PLATFORM_Android = "Android";
    private static readonly string PLATFORM_Standalone = "Standalone";

    private void OnPostprocessAudio(AudioClip audioClip)
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;

        string path = audioImporter.assetPath;

        if (!path.Contains(AUDIOCLIP_FOLDER))
        {
            return;
        }

        //AudioClips/以降のパスを切り出し//
        path = path.Substring(path.IndexOf(AUDIOCLIP_FOLDER) + AUDIOCLIP_FOLDER.Length);

        //SE系は全部モノラル化//
        audioImporter.forceToMono = path.Contains(MENUSE) || path.Contains(GAMESE);

        //BGMは全部バックグラウンド読み込み//
        audioImporter.loadInBackground = path.Contains(BGM);


        //Defaultの設定//
        AudioImporterSampleSettings defaultSettings = audioImporter.defaultSampleSettings;

        if (path.Contains(BGM) || path.Contains(VOICE))
        {  
            //BGMまたはボイスならストリーミング再生//
            defaultSettings.loadType = AudioClipLoadType.Streaming;
        }
        else
        {
            //それ以外はメモリ内圧縮//
            defaultSettings.loadType = AudioClipLoadType.CompressedInMemory;

            //Androidの圧縮設定（再生レイテンシーのためにメモリに展開するという想定）//
            AudioImporterSampleSettings androidSettings = audioImporter.GetOverrideSampleSettings(PLATFORM_Android);
            androidSettings.loadType = AudioClipLoadType.DecompressOnLoad;
            audioImporter.SetOverrideSampleSettings(PLATFORM_Android, androidSettings);
        }

        //圧縮設定//
        defaultSettings.compressionFormat = AudioCompressionFormat.Vorbis;
        defaultSettings.quality = 0.2f;

        audioImporter.defaultSampleSettings = defaultSettings;

        //PC、Macのは圧縮率を低めに（ファイル容量に余裕があるという想定）//
        AudioImporterSampleSettings standaloneSettings = audioImporter.GetOverrideSampleSettings(PLATFORM_Standalone);
        standaloneSettings.quality = 0.8f;
        audioImporter.SetOverrideSampleSettings(PLATFORM_Standalone, standaloneSettings);

    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //ディレクトリ移動の場合もインポートを発生させてOnPostprocessAudioを呼ぶ//
        foreach (string path in movedAssets)
        {
            if (path.Contains(AUDIOCLIP_FOLDER))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
    }

}