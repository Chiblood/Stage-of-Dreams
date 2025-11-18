using UnityEngine;
using UnityEditor;

public class CreateLinearDialog
{
    [MenuItem("Dialog System/Create Linear Conversation")]
    public static void CreateLinearConversation()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Assistant Director Conversation";
        tree.description = "A conversation with the Assistant Director about rehearsal preparation";

        // Define conversation as arrays
        string[] speakers = new string[]
        {
            "Assistant Director",
            "Player",
            "Assistant Director",
            "Player",
            "Assistant Director"
        };

        string[] dialogTexts = new string[]
        {
            "Excuse me! You're not supposed to be backstage during rehearsal.",
            "I'm the lead actor for tonight's performance.",
            "Oh! My apologies. The director said you wouldn't arrive until later.",
            "I wanted to get a feel for the stage. This is my first time performing here.",
            "I understand completely. Well then, let me show you to your dressing room. Break a leg tonight!"
        };

        bool[] isPlayerSpeaking = new bool[]
        {
            false, // Assistant Director
            true,  // Player
            false, // Assistant Director
            true,  // Player
            false  // Assistant Director
        };

        // Create the entire conversation at once
        tree.CreateLinearConversation(speakers, dialogTexts, isPlayerSpeaking);

        // Save to disk
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Assistant Director Conversation.asset";
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"Created linear conversation with {speakers.Length} nodes: {tree.treeName}");
    }
}