using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogTreeFactory
{
    // Base directory for all dialog tree assets
    private const string DIALOG_TREE_BASE_PATH = "Assets/_Stage of Dreams_/World/Dialog Trees/Testing/";
    
    /// <summary>
    /// Ensures the directory exists for the given asset path
    /// </summary>
    private static void EnsureDirectoryExists(string assetPath)
    {
        string directory = Path.GetDirectoryName(assetPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log($"Created directory: {directory}");
        }
    }
    
    [MenuItem("Dialog System/Create Test Dialog Tree")]
    public static void CreateTestDialogTree()
    {
        // Create the tree asset
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Backstage Encounter";
        tree.description = "A simple test conversation with a stage crew member";

        // Create dialog nodes using the tree's built-in methods with proper Node IDs
        DialogNode startNode = tree.CreateStartingNode(
            speakerName: "Stage Manager",
            dialogText: "Ah, you must be our new performer! Ready for tonight's show?",
            isPlayerSpeaking: false,
            nodeName: "npc_stagemgr_greeting_01"  // Following convention: npc_speaker_action_sequence
        );

        // Add player choice branches with proper IDs
        DialogNode choice1Response = tree.AddChoiceNode(
            parentNode: startNode,
            choiceText: "I'm a bit nervous, honestly.",
            speakerName: "Stage Manager",
            dialogText: "That's perfectly normal! Every great performance starts with butterflies. You'll do wonderfully.",
            isPlayerSpeaking: false,
            customActionId: "choice_nervous_01",
            nodeName: "npc_stagemgr_nervous_01"  // Choice outcome: nervous path
        );

        DialogNode choice2Response = tree.AddChoiceNode(
            parentNode: startNode,
            choiceText: "Born ready! Let's do this!",
            speakerName: "Stage Manager",
            dialogText: "That's the spirit! I love the confidence. Break a leg out there!",
            isPlayerSpeaking: false,
            customActionId: "choice_confident_01",
            nodeName: "npc_stagemgr_confident_01"  // Choice outcome: confident path
        );

        // Continue building the tree with sequential nodes
        tree.AddSequentialNode(
            parentNode: choice1Response,
            speakerName: "Stage Manager",
            dialogText: "Just remember: the audience is rooting for you. They want you to succeed!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_stagemgr_encourage_01"
        );

        tree.AddSequentialNode(
            parentNode: choice2Response,
            speakerName: "Stage Manager",
            dialogText: "Your dressing room is the third door on the left. Curtain rises in thirty minutes!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_stagemgr_directions_01"
        );

        // Save the asset to disk
        string path = DIALOG_TREE_BASE_PATH + "Test Dialog Tree.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);

        // Refresh and select
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Dialog Tree at: {path}");
        Debug.Log("✓ All nodes use proper Node ID naming convention");
    }

    [MenuItem("Dialog System/Create Director Audition Template")]
    public static void CreateDirectorAuditionTemplate()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Director Audition";
        tree.description = "An audition conversation with the director offering different role types";

        // Opening with proper Node ID
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Director",
            dialogText: "Welcome! Thank you for coming in today. What brings you to our theater?",
            isPlayerSpeaking: false,
            nodeName: "npc_director_greeting_01"
        );

        // Player introduces themselves
        DialogNode introduction = tree.AddSequentialNode(
            parentNode: start,
            speakerName: "Player",
            dialogText: "I'm here for the audition. I've dreamed of performing on this stage!",
            isPlayerSpeaking: true,
            autoAdvanceDelay: 0f,
            nodeName: "npc_player_intro_01"
        );

        // Director's response with role options - BRANCH POINT
        DialogNode roleQuestion = tree.AddSequentialNode(
            parentNode: introduction,
            speakerName: "Director",
            dialogText: "Wonderful passion! We have several roles available. What type of character interests you most?",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_branch_role_01"  // Branch point for role selection
        );

        // Role branches - CHOICE OUTCOMES
        DialogNode heroPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A heroic protagonist",
            speakerName: "Director",
            dialogText: "Excellent! The hero role requires courage and charisma. Show me your best heroic pose!",
            isPlayerSpeaking: false,
            customActionId: "audition_hero",
            nodeName: "npc_choice_hero_01"
        );

        DialogNode villainPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A dramatic antagonist",
            speakerName: "Director",
            dialogText: "Intriguing! Playing a villain requires depth and complexity. Let's see your menacing side!",
            isPlayerSpeaking: false,
            customActionId: "audition_villain",
            nodeName: "npc_choice_villain_01"
        );

        DialogNode supportPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A versatile supporting role",
            speakerName: "Director",
            dialogText: "Smart choice! Supporting roles are the backbone of any production. Flexibility is key!",
            isPlayerSpeaking: false,
            customActionId: "audition_support",
            nodeName: "npc_choice_support_01"
        );

        // Convergent ending - all paths lead here (CONVERGENCE POINT)
        DialogNode congratulations = tree.AddSequentialNode(
            parentNode: heroPath,
            speakerName: "Director",
            dialogText: "Wonderful! You clearly have talent. Welcome to the company!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_convergence_accepted_01"  // Convergence point
        );

        // Add choices to point to the same ending (convergent path)
        // Using named node references for convergent paths
        tree.AddChoiceToNamedNode(villainPath, "Deliver monologue", "npc_convergence_accepted_01", "complete_villain_audition");
        tree.AddChoiceToNamedNode(supportPath, "Perform character study", "npc_convergence_accepted_01", "complete_support_audition");

        // Save the asset
        string path = DIALOG_TREE_BASE_PATH + "Director Audition Template.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Director Audition Template with convergent paths at: {path}");
        Debug.Log("✓ Uses naming convention: branch → choice → convergence pattern");
    }

    [MenuItem("Dialog System/Create Rehearsal Feedback Template")]
    public static void CreateRehearsalFeedbackTemplate()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Rehearsal Feedback";
        tree.description = "Post-rehearsal conversation with constructive criticism and encouragement";

        // Director's initial feedback
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Director",
            dialogText: "Great work today! Let's talk about your performance. How do you think it went?",
            isPlayerSpeaking: false,
            nodeName: "npc_director_feedback_01"
        );

        // Player's self-assessment - BRANCH POINT
        DialogNode positiveAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I think I nailed it!",
            speakerName: "Director",
            dialogText: "I appreciate the confidence! Though remember, there's always room to grow. Let me share some notes.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_confident_01"
        );

        DialogNode uncertainAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I'm not sure... I felt a bit off.",
            speakerName: "Director",
            dialogText: "That self-awareness is valuable! Don't be too hard on yourself. Let's work through it together.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_uncertain_01"
        );

        DialogNode neutralAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Some parts were good, some need work.",
            speakerName: "Director",
            dialogText: "Perfect! That's exactly the mindset we need. Let's identify what to polish.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_balanced_01"
        );

        // Director provides specific feedback (CONVERGENCE POINT)
        DialogNode specificFeedback = tree.AddSequentialNode(
            parentNode: positiveAssessment,
            speakerName: "Director",
            dialogText: "Your stage presence is strong, but remember to project your voice to the back row. And watch your timing on the third act entrance.",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_convergence_notes_01"  // All paths converge here
        );

        // Link other paths to the same feedback node using named references
        tree.AddChoiceToNamedNode(uncertainAssessment, "What should I focus on?", "npc_convergence_notes_01");
        tree.AddChoiceToNamedNode(neutralAssessment, "I'd love your notes.", "npc_convergence_notes_01");

        // Player reaction to feedback - BRANCH POINT
        DialogNode acceptancePath = tree.AddChoiceNode(
            parentNode: specificFeedback,
            choiceText: "Thank you! I'll work on that.",
            speakerName: "Director",
            dialogText: "That's what I like to hear! Practice those notes before tomorrow's run-through.",
            isPlayerSpeaking: false,
            customActionId: "unlock_practice_session",
            nodeName: "npc_choice_accept_01"
        );

        DialogNode defensivePath = tree.AddChoiceNode(
            parentNode: specificFeedback,
            choiceText: "But I thought my timing was perfect...",
            speakerName: "Director",
            dialogText: "I understand it feels that way from your perspective. Trust me on this - a small adjustment will make it shine.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_defensive_01"
        );

        // Defensive path continues
        tree.AddSequentialNode(
            parentNode: defensivePath,
            speakerName: "Director",
            dialogText: "Let's run through it together after rehearsal. Sometimes you just need to feel it from a different angle.",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_director_compromise_01"
        );

        // Save the asset
        string path = DIALOG_TREE_BASE_PATH + "Rehearsal Feedback Template.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Rehearsal Feedback Template at: {path}");
        Debug.Log("✓ Demonstrates convergent path pattern with named node references");
    }

    [MenuItem("Dialog System/Create Stage Crew Banter Template")]
    public static void CreateStageCrewBanterTemplate()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Stage Crew Banter";
        tree.description = "Casual backstage conversation with crew members";

        // Lighting tech greets player
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Lighting Tech",
            dialogText: "Hey! First time working with our crew?",
            isPlayerSpeaking: false,
            nodeName: "npc_lighttech_greeting_01"
        );

        // Player responds - BRANCH POINT
        DialogNode firstTimeResponse = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Yes! Everyone seems really talented.",
            speakerName: "Lighting Tech",
            dialogText: "We've been working together for years. You're gonna love it here!",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_newcomer_01"
        );

        DialogNode veteranResponse = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I've worked with stage crews before.",
            speakerName: "Lighting Tech",
            dialogText: "Nice! Then you know how crazy things can get during tech week.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_veteran_01"
        );

        // Lighting tech shares a story (CONVERGENCE POINT)
        DialogNode techWeekStory = tree.AddSequentialNode(
            parentNode: firstTimeResponse,
            speakerName: "Lighting Tech",
            dialogText: "Last show, a spotlight died right before the big monologue. We had to improvise with handheld lights from the wings!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_convergence_story_01"  // Named node for convergent path
        );

        // Link veteran path to same story using sequential continuation
        DialogNode veteranLead = tree.AddSequentialNode(
            parentNode: veteranResponse,
            speakerName: "Lighting Tech",
            dialogText: "Want to hear about our most memorable tech disaster?",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_lighttech_leadup_01"
        );
        
        // Connect to convergent story node
        tree.AddChoiceToNamedNode(veteranLead, "Sure, tell me!", "npc_convergence_story_01");

        // Player responds to story - BRANCH POINT
        DialogNode laughResponse = tree.AddChoiceNode(
            parentNode: techWeekStory,
            choiceText: "That sounds terrifying!",
            speakerName: "Lighting Tech",
            dialogText: "It was! But the show went on. That's theater magic for you.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_terrified_01"
        );

        DialogNode curiousResponse = tree.AddChoiceNode(
            parentNode: techWeekStory,
            choiceText: "Did the audience notice?",
            speakerName: "Lighting Tech",
            dialogText: "Not at all! The actor played it off perfectly. That's the beauty of live performance.",
            isPlayerSpeaking: false,
            customActionId: null,
            nodeName: "npc_choice_curious_01"
        );

        // Both paths end with crew solidarity (FINAL CONVERGENCE)
        DialogNode solidarity = tree.AddSequentialNode(
            parentNode: laughResponse,
            speakerName: "Lighting Tech",
            dialogText: "Anyway, if you ever need anything backstage, just holler. We've got your back out there.",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_convergence_solidarity_01"  // Final convergence point
        );

        // Connect curious path to final convergence
        DialogNode curiousFollow = tree.AddSequentialNode(
            parentNode: curiousResponse,
            speakerName: "Lighting Tech", 
            dialogText: "Remember: on stage, we're all in this together. Break a leg tonight!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_lighttech_teamwork_01"
        );
        
        // Link to final solidarity
        tree.AddChoiceToNamedNode(curiousFollow, "Thanks for the support!", "npc_convergence_solidarity_01");

        // Save the asset
        string path = DIALOG_TREE_BASE_PATH + "Stage Crew Banter Template.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Stage Crew Banter Template at: {path}");
        Debug.Log("✓ Demonstrates multiple convergence points in single conversation");
    }

    [MenuItem("Dialog System/Create Minigame Test Tree")]
    public static void CreateMinigameTestTree()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Minigame System Test";
        tree.description = "Test tree demonstrating minigame nodes (RememberTheScript) as choice branches";

        // Starting node - Director offers practice choices
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Director",
            dialogText: "Let's warm up before the big performance! What would you like to practice?",
            isPlayerSpeaking: false,
            nodeName: "npc_director_warmup_01"
        );

        // CHOICE 1: Short easy phrase (RememberTheScript minigame node)
        DialogNode easyPhraseNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Practice a short line",
            speakerName: "Director",
            dialogText: "Good choice! Let's start with something simple. Type this line exactly as I say it:",
            isPlayerSpeaking: false,
            customActionId: "minigame_easy",
            nodeName: "minigame_easy_phrase_01"
        );

        // Configure as RememberTheScript minigame
        easyPhraseNode.NodeType = DialogNodeType.RememberTheScript;
        easyPhraseNode.ConfigureRememberScript(
            targetPhrase: "To be or not to be",
            maxMistakes: 5,
            timeLimit: 15f,
            scoreOnSuccess: 10f,
            scorePerMistake: -2f,
            caseSensitive: false,
            failureNodeName: ""
        );

        // Success node for easy phrase
        DialogNode easySuccess = new DialogNode(
            "Director",
            "Perfect! You nailed it! That was a classic line, delivered with confidence.",
            false,
            "minigame_easy_success_01"
        );
        easyPhraseNode.SetChildNode(easySuccess);

        // Failure node for easy phrase
        DialogNode easyFailure = new DialogNode(
            "Director",
            "Don't worry, even the greats stumble on their lines. Let's try something else!",
            false,
            "minigame_easy_failure_01"
        );
        easyPhraseNode.FailureNode = easyFailure;
        tree.AddNode(easyFailure); // Manually add failure node to tree

        // CHOICE 2: Medium difficulty phrase (RememberTheScript minigame node)
        DialogNode mediumPhraseNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Practice a dramatic monologue",
            speakerName: "Director",
            dialogText: "Excellent! This one requires more focus. Type exactly what you hear:",
            isPlayerSpeaking: false,
            customActionId: "minigame_medium",
            nodeName: "minigame_medium_phrase_01"
        );

        // Configure as RememberTheScript minigame
        mediumPhraseNode.NodeType = DialogNodeType.RememberTheScript;
        mediumPhraseNode.ConfigureRememberScript(
            targetPhrase: "All the world's a stage, and all the men and women merely players",
            maxMistakes: 3,
            timeLimit: 25f,
            scoreOnSuccess: 25f,
            scorePerMistake: -5f,
            caseSensitive: false,
            failureNodeName: ""
        );

        // Success node for medium phrase
        DialogNode mediumSuccess = new DialogNode(
            "Director",
            "Magnificent! You've captured the essence of Shakespeare himself! The audience will love this!",
            false,
            "minigame_medium_success_01"
        );
        mediumPhraseNode.SetChildNode(mediumSuccess);

        // Failure node for medium phrase
        DialogNode mediumFailure = new DialogNode(
            "Director",
            "That's a tough one. Remember, precision is key in theater. Let's keep working on it!",
            false,
            "minigame_medium_failure_01"
        );
        mediumPhraseNode.FailureNode = mediumFailure;
        tree.AddNode(mediumFailure); // Manually add failure node to tree

        // CHOICE 3: Hard difficulty phrase (RememberTheScript minigame node)
        DialogNode hardPhraseNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Practice a complex speech",
            speakerName: "Director",
            dialogText: "Bold choice! This is advanced material. Focus and type carefully:",
            isPlayerSpeaking: false,
            customActionId: "minigame_hard",
            nodeName: "minigame_hard_phrase_01"
        );

        // Configure as RememberTheScript minigame
        hardPhraseNode.NodeType = DialogNodeType.RememberTheScript;
        hardPhraseNode.ConfigureRememberScript(
            targetPhrase: "Now is the winter of our discontent made glorious summer by this sun of York",
            maxMistakes: 2,
            timeLimit: 30f,
            scoreOnSuccess: 50f,
            scorePerMistake: -10f,
            caseSensitive: true, // Case sensitive for hard mode
            failureNodeName: ""
        );

        // Success node for hard phrase
        DialogNode hardSuccess = new DialogNode(
            "Director",
            "OUTSTANDING! You're a natural! That level of precision is rare. The stage awaits you!",
            false,
            "minigame_hard_success_01"
        );
        hardPhraseNode.SetChildNode(hardSuccess);

        // Failure node for hard phrase
        DialogNode hardFailure = new DialogNode(
            "Director",
            "That's master-level material. Don't be discouraged - few can master it. Let's try something else.",
            false,
            "minigame_hard_failure_01"
        );
        hardPhraseNode.FailureNode = hardFailure;
        tree.AddNode(hardFailure); // Manually add failure node to tree

        // CHOICE 4: Skip practice (standard dialog)
        DialogNode skipNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I'm ready, no practice needed",
            speakerName: "Director",
            dialogText: "Confidence! I like it. Just remember - the show must go on. Break a leg!",
            isPlayerSpeaking: false,
            customActionId: "skip_practice",
            nodeName: "npc_director_confident_01"
        );

        // Convergent ending - all paths lead here
        DialogNode finalNode = tree.AddSequentialNode(
            parentNode: easySuccess,
            speakerName: "Director",
            dialogText: "Alright, warm-up's over. Time for the real performance. Places everyone!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_convergence_finalcall_01"
        );

        // Link other success/failure/skip paths to final node
        tree.AddChoiceToNamedNode(mediumSuccess, "Ready for the show!", "npc_convergence_finalcall_01");
        tree.AddChoiceToNamedNode(hardSuccess, "Let's do this!", "npc_convergence_finalcall_01");
        tree.AddChoiceToNamedNode(easyFailure, "I'll do better in the performance", "npc_convergence_finalcall_01");
        tree.AddChoiceToNamedNode(mediumFailure, "Let's move on", "npc_convergence_finalcall_01");
        tree.AddChoiceToNamedNode(hardFailure, "I'll give it my all anyway", "npc_convergence_finalcall_01");
        tree.AddChoiceToNamedNode(skipNode, "I'm ready!", "npc_convergence_finalcall_01");

        // Save the asset
        string path = DIALOG_TREE_BASE_PATH + "Minigame Test Tree.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Minigame Test Tree at: {path}");
        Debug.Log("✓ Demonstrates RememberTheScript minigame nodes as choice branches");
        Debug.Log("✓ Shows easy, medium, and hard difficulty configurations");
        Debug.Log("✓ Includes success and failure outcome nodes for each minigame");
        Debug.Log("✓ All paths converge to final node");
    }

    [MenuItem("Dialog System/Create GameState Test Tree")]
    public static void CreateGameStateTestTree()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "GameState & Audience Test";
        tree.description = "Test tree demonstrating GameStateManager integration and audience interaction";

        // Starting node with initial audience setup
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Stage Manager",
            dialogText: "Welcome to the stage! The audience is watching. Let's see how you perform under pressure.",
            isPlayerSpeaking: false,
            nodeName: "npc_stagemgr_intro_01"
        );

        // Add start event to reset audience scores
        var resetEvent = new MethodCallEvent();
        resetEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ResetAudienceScores();
                Debug.Log("[Test] Audience scores reset");
            }
        }, "Reset audience scores");
        start.AddStartEvent(resetEvent);

        // CHOICE 1: Confident performance (positive audience reaction)
        DialogNode confidentNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Deliver a powerful monologue",
            speakerName: "Stage Manager",
            dialogText: "The audience is captivated by your commanding presence!",
            isPlayerSpeaking: false,
            customActionId: "confident_performance",
            nodeName: "npc_choice_confident_01"
        );

        // Add end event to boost applause
        var confidentEvent = new MethodCallEvent();
        confidentEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustApplause(20f);
                GameStateManager.Instance.AddSceneScore(50);
                Debug.Log("[Test] +20 Applause, +50 Score");
            }
        }, "Boost applause and score");
        confidentNode.AddEndEvent(confidentEvent);

        // CHOICE 2: Nervous performance (minor negative reaction)
        DialogNode nervousNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Nervously recite your lines",
            speakerName: "Stage Manager",
            dialogText: "You stumble a bit, but the audience is patient. Keep going!",
            isPlayerSpeaking: false,
            customActionId: "nervous_performance",
            nodeName: "npc_choice_nervous_01"
        );

        // Add end event for minor penalty
        var nervousEvent = new MethodCallEvent();
        nervousEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustApplause(-5f);
                GameStateManager.Instance.AdjustBoo(10f);
                Debug.Log("[Test] -5 Applause, +10 Boo");
            }
        }, "Minor audience disappointment");
        nervousNode.AddEndEvent(nervousEvent);

        // CHOICE 3: Forget lines (major negative reaction)
        DialogNode forgetNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Completely forget your lines",
            speakerName: "Stage Manager",
            dialogText: "Oh no! The audience is starting to boo! Quick, improvise something!",
            isPlayerSpeaking: false,
            customActionId: "forget_lines",
            nodeName: "npc_choice_forget_01"
        );

        // Add end event for major penalty
        var forgetEvent = new MethodCallEvent();
        forgetEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustApplause(-15f);
                GameStateManager.Instance.AdjustBoo(25f);
                GameStateManager.Instance.RecordFailure();
                Debug.Log("[Test] -15 Applause, +25 Boo, Failure recorded");
            }
        }, "Major audience disappointment");
        forgetNode.AddEndEvent(forgetEvent);

        // CHOICE 4: Interact with audience (special positive reaction)
        DialogNode interactNode = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Break the fourth wall and address the audience",
            speakerName: "Stage Manager",
            dialogText: "The audience loves the bold choice! They're laughing and applauding!",
            isPlayerSpeaking: false,
            customActionId: "audience_interaction",
            nodeName: "npc_choice_interact_01"
        );

        // Add end event for major boost
        var interactEvent = new MethodCallEvent();
        interactEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustApplause(30f);
                GameStateManager.Instance.AddSceneScore(75);
                GameStateManager.Instance.RecordSuccess();
                Debug.Log("[Test] +30 Applause, +75 Score, Success recorded");
            }
        }, "Major audience approval");
        interactNode.AddEndEvent(interactEvent);

        // Audience reaction check node
        DialogNode reactionCheckNode = tree.AddSequentialNode(
            parentNode: confidentNode,
            speakerName: "Stage Manager",
            dialogText: "Let me check the audience reaction...",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 1f,
            nodeName: "npc_stagemgr_check_01"
        );

        // Link other paths to reaction check
        tree.AddChoiceToNamedNode(nervousNode, "Continue", "npc_stagemgr_check_01");
        tree.AddChoiceToNamedNode(forgetNode, "Try to recover", "npc_stagemgr_check_01");
        tree.AddChoiceToNamedNode(interactNode, "Take a bow", "npc_stagemgr_check_01");

        // Final feedback node - dynamically responds to audience state
        DialogNode feedbackNode = tree.AddSequentialNode(
            parentNode: reactionCheckNode,
            speakerName: "Stage Manager",
            dialogText: "Performance complete! Check the console for your audience metrics and score.",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f,
            nodeName: "npc_stagemgr_feedback_01"
        );

        // Add end event to print game state
        var printStateEvent = new MethodCallEvent();
        printStateEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.PrintCurrentState();
            }
        }, "Print final game state");
        feedbackNode.AddEndEvent(printStateEvent);

        // Minigame integration test branch
        DialogNode minigameTestNode = tree.AddChoiceNode(
            parentNode: feedbackNode,
            choiceText: "Try a minigame with audience scoring",
            speakerName: "Stage Manager",
            dialogText: "Let's test a minigame that affects the audience! Type this line:",
            isPlayerSpeaking: false,
            customActionId: "test_minigame_with_scoring",
            nodeName: "minigame_audience_test_01"
        );

        // Configure as RememberTheScript minigame with audience scoring
        minigameTestNode.NodeType = DialogNodeType.RememberTheScript;
        minigameTestNode.ConfigureRememberScript(
            targetPhrase: "The show must go on",
            maxMistakes: 3,
            timeLimit: 20f,
            scoreOnSuccess: 40f,  // This will be sent to GameStateManager
            scorePerMistake: -8f, // Penalty per mistake
            caseSensitive: false,
            failureNodeName: ""
        );

        // Success node with audience celebration
        DialogNode minigameSuccess = new DialogNode(
            "Stage Manager",
            "Perfect! The audience is on their feet! Check your scores!",
            false,
            "minigame_audience_success_01"
        );
        minigameTestNode.SetChildNode(minigameSuccess);

        // Add success event
        var successEvent = new MethodCallEvent();
        successEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustApplause(40f);
                GameStateManager.Instance.AddSceneScore(40);
                GameStateManager.Instance.EndMinigame("remember_script_test", true);
                Debug.Log("[Test] Minigame Success! +40 Applause, +40 Score");
            }
        }, "Minigame success rewards");
        minigameSuccess.AddEndEvent(successEvent);

        // Failure node with audience disappointment
        DialogNode minigameFailure = new DialogNode(
            "Stage Manager",
            "The audience seems confused. That's okay, let's keep practicing!",
            false,
            "minigame_audience_failure_01"
        );
        minigameTestNode.FailureNode = minigameFailure;
        tree.AddNode(minigameFailure);

        // Add failure event
        var failureEvent = new MethodCallEvent();
        failureEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AdjustBoo(15f);
                GameStateManager.Instance.EndMinigame("remember_script_test", false);
                Debug.Log("[Test] Minigame Failed! +15 Boo");
            }
        }, "Minigame failure penalty");
        minigameFailure.AddEndEvent(failureEvent);

        // Final end node
        DialogNode endNode = tree.AddChoiceNode(
            parentNode: feedbackNode,
            choiceText: "End performance test",
            speakerName: "Stage Manager",
            dialogText: "Thank you for testing! Check the console for your final stats. The GameStateManager is tracking everything!",
            isPlayerSpeaking: false,
            customActionId: "end_test",
            nodeName: "npc_stagemgr_end_01"
        );

        // Link minigame outcomes to end node
        tree.AddChoiceToNamedNode(minigameSuccess, "Finish test", "npc_stagemgr_end_01");
        tree.AddChoiceToNamedNode(minigameFailure, "Finish test", "npc_stagemgr_end_01");

        // Add final state print
        var finalPrintEvent = new MethodCallEvent();
        finalPrintEvent.SetMethod(() => {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.PrintCurrentState();
                Debug.Log("=== TEST COMPLETE ===");
                Debug.Log("✓ Audience scores tested");
                Debug.Log("✓ Performance scores tested");
                Debug.Log("✓ Minigame integration tested");
                Debug.Log("✓ Success/failure tracking tested");
            }
        }, "Final state summary");
        endNode.AddEndEvent(finalPrintEvent);

        // Save the asset
        string path = DIALOG_TREE_BASE_PATH + "GameState Test Tree.asset";
        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created GameState Test Tree at: {path}");
        Debug.Log("✓ Demonstrates GameStateManager integration");
        Debug.Log("✓ Tests audience metrics (applause/boo/mood)");
        Debug.Log("✓ Tests performance scoring");
        Debug.Log("✓ Tests success/failure tracking");
        Debug.Log("✓ Includes minigame with audience interaction");
        Debug.Log("✓ Uses DialogEvents with method delegates");
    }
}
