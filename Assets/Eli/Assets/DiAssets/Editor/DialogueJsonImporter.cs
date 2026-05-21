using System;
using UnityEditor;
using UnityEngine;

public class DialogueJsonImporter : EditorWindow
{
    [Serializable]
    public class DialogueImportFile
    {
        public DialogueImportEntry[] dialogues;
    }

    [Serializable]
    public class DialogueImportEntry
    {
        public string assetName;
        public DialogueLineImport[] lines;
    }

    [Serializable]
    public class DialogueLineImport
    {
        [TextArea(2, 6)]
        public string text;

        public string emotion;
        public bool continueCheck;
    }

    private TextAsset jsonFile;
    private CharacterProfile defaultCharacter;
    private string outputFolder = "Assets/Dialogues/Generated";

    [MenuItem("Tools/Dialogues/Import Dialogue JSON")]
    public static void Open()
    {
        GetWindow<DialogueJsonImporter>("Dialogue JSON Importer");
    }

    private void OnGUI()
    {
        jsonFile = (TextAsset)EditorGUILayout.ObjectField(
            "JSON File",
            jsonFile,
            typeof(TextAsset),
            false
        );

        defaultCharacter = (CharacterProfile)EditorGUILayout.ObjectField(
            "Default Character Profile",
            defaultCharacter,
            typeof(CharacterProfile),
            false
        );

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Import Dialogue Assets"))
        {
            Import();
        }
    }

    private void Import()
    {
        if (jsonFile == null)
        {
            Debug.LogError("[DialogueJsonImporter] JSON file missing.");
            return;
        }

        if (defaultCharacter == null)
        {
            Debug.LogError("[DialogueJsonImporter] Default Character Profile missing.");
            return;
        }

        EnsureFolderExists(outputFolder);

        DialogueImportFile data = JsonUtility.FromJson<DialogueImportFile>(jsonFile.text);

        if (data == null || data.dialogues == null)
        {
            Debug.LogError("[DialogueJsonImporter] Invalid JSON format.");
            return;
        }

        foreach (DialogueImportEntry entry in data.dialogues)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.assetName))
                continue;

            DialogueBase dialogueAsset = ScriptableObject.CreateInstance<DialogueBase>();

            int lineCount = entry.lines != null ? entry.lines.Length : 0;
            dialogueAsset.dialogueInfo = new DialogueBase.Info[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                DialogueLineImport line = entry.lines[i];

                DialogueBase.Info info = new DialogueBase.Info();
                info.character = defaultCharacter;
                info.charText = line.text;
                info.continueCheck = line.continueCheck;

                if (!string.IsNullOrWhiteSpace(line.emotion))
                {
                    if (Enum.TryParse(line.emotion, true, out EmotionType parsedEmotion))
                        info.characterEmotion = parsedEmotion;
                    else
                        Debug.LogWarning("[DialogueJsonImporter] Unknown emotion: " + line.emotion + " in " + entry.assetName);
                }

                dialogueAsset.dialogueInfo[i] = info;
            }

            string safeName = MakeSafeAssetName(entry.assetName);
            string path = $"{outputFolder}/{safeName}.asset";

            AssetDatabase.CreateAsset(dialogueAsset, AssetDatabase.GenerateUniqueAssetPath(path));
            EditorUtility.SetDirty(dialogueAsset);

            Debug.Log("[DialogueJsonImporter] Created DialogueBase: " + safeName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[DialogueJsonImporter] Import complete.");
    }

    private void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] parts = folderPath.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }

    private string MakeSafeAssetName(string raw)
    {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            raw = raw.Replace(c.ToString(), "");

        return raw.Trim().Replace(" ", "_");
    }
}