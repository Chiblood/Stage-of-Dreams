using UnityEngine;
using UnityEditor;

public class DialogTreeFactory
{
    [MenuItem("Dialog System/Create Test Dialog Tree")]
    public static void CreateTestDialogTree()
    {
        // Create the tree asset
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Backstage Encounter";
        tree.description = "A simple test conversation with a stage crew member";

        // Create dialog nodes using the tree's built-in methods
        DialogNode startNode = tree.CreateStartingNode(
            speakerName: "Stage Manager",
            dialogText: "Ah, you must be our new performer! Ready for tonight's show?",
            isPlayerSpeaking: false,
            nodeName: "greeting"
        );

        // Add player choice branches
        DialogNode choice1Response = tree.AddChoiceNode(
            parentNode: startNode,
            choiceText: "I'm a bit nervous, honestly.",
            speakerName: "Stage Manager",
            dialogText: "That's perfectly normal! Every great performance starts with butterflies. You'll do wonderfully.",
            isPlayerSpeaking: false,
            nodeName: "nervous_path"
        );

        DialogNode choice2Response = tree.AddChoiceNode(
            parentNode: startNode,
            choiceText: "Born ready! Let's do this!",
            speakerName: "Stage Manager",
            dialogText: "That's the spirit! I love the confidence. Break a leg out there!",
            isPlayerSpeaking: false,
            nodeName: "confident_path"
        );

        // Continue building the tree
        tree.AddSequentialNode(
            parentNode: choice1Response,
            speakerName: "Stage Manager",
            dialogText: "Just remember: the audience is rooting for you. They want you to succeed!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f
        );

        tree.AddSequentialNode(
            parentNode: choice2Response,
            speakerName: "Stage Manager",
            dialogText: "Your dressing room is the third door on the left. Curtain rises in thirty minutes!",
            isPlayerSpeaking: false,
            autoAdvanceDelay: 0f
        );

        // Save the asset to disk
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Test Dialog Tree.asset";
        AssetDatabase.CreateAsset(tree, path);

        // Refresh and select
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Dialog Tree at: {path}");
    }

    [MenuItem("Dialog System/Create Director Audition Template")]
    public static void CreateDirectorAuditionTemplate()
    {
        DialogTree tree = ScriptableObject.CreateInstance<DialogTree>();
        tree.treeName = "Director Audition";
        tree.description = "An audition conversation with the director offering different role types";

        // Opening
        DialogNode start = tree.CreateStartingNode(
            speakerName: "Director",
            dialogText: "Welcome! Thank you for coming in today. What brings you to our theater?",
            isPlayerSpeaking: false,
            nodeName: "audition_start"
        );

        // Player introduces themselves
        DialogNode introduction = tree.AddSequentialNode(
            parentNode: start,
            speakerName: "Player",
            dialogText: "I'm here for the audition. I've dreamed of performing on this stage!",
            isPlayerSpeaking: true
        );

        // Director's response with role options
        DialogNode roleQuestion = tree.AddSequentialNode(
            parentNode: introduction,
            speakerName: "Director",
            dialogText: "Wonderful passion! We have several roles available. What type of character interests you most?",
            isPlayerSpeaking: false,
            nodeName: "role_selection"
        );

        // Role branches
        DialogNode heroPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A heroic protagonist",
            speakerName: "Director",
            dialogText: "Excellent! The hero role requires courage and charisma. Show me your best heroic pose!",
            isPlayerSpeaking: false,
            customActionId: "audition_hero",
            nodeName: "hero_audition"
        );

        DialogNode villainPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A dramatic antagonist",
            speakerName: "Director",
            dialogText: "Intriguing! Playing a villain requires depth and complexity. Let's see your menacing side!",
            isPlayerSpeaking: false,
            customActionId: "audition_villain",
            nodeName: "villain_audition"
        );

        DialogNode supportPath = tree.AddChoiceNode(
            parentNode: roleQuestion,
            choiceText: "A versatile supporting role",
            speakerName: "Director",
            dialogText: "Smart choice! Supporting roles are the backbone of any production. Flexibility is key!",
            isPlayerSpeaking: false,
            customActionId: "audition_support",
            nodeName: "support_audition"
        );

        // Convergent ending - all paths lead here
        DialogNode congratulations = tree.AddSequentialNode(
            parentNode: heroPath,
            speakerName: "Director",
            dialogText: "Wonderful! You clearly have talent. Welcome to the company!",
            isPlayerSpeaking: false,
            nodeName: "accepted"
        );

        // Add choices to point to the same ending (convergent path)
        tree.AddChoiceToNamedNode(villainPath, "Deliver monologue", "accepted", "complete_villain_audition");
        tree.AddChoiceToNamedNode(supportPath, "Perform character study", "accepted", "complete_support_audition");

        // Save the asset
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Director Audition Template.asset";
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Director Audition Template with convergent paths at: {path}");
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
            nodeName: "feedback_start"
        );

        // Player's self-assessment
        DialogNode positiveAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I think I nailed it!",
            speakerName: "Director",
            dialogText: "I appreciate the confidence! Though remember, there's always room to grow. Let me share some notes.",
            isPlayerSpeaking: false,
            nodeName: "confident_response"
        );

        DialogNode uncertainAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I'm not sure... I felt a bit off.",
            speakerName: "Director",
            dialogText: "That self-awareness is valuable! Don't be too hard on yourself. Let's work through it together.",
            isPlayerSpeaking: false,
            nodeName: "uncertain_response"
        );

        DialogNode neutralAssessment = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Some parts were good, some need work.",
            speakerName: "Director",
            dialogText: "Perfect! That's exactly the mindset we need. Let's identify what to polish.",
            isPlayerSpeaking: false,
            nodeName: "balanced_response"
        );

        // Director provides specific feedback (convergent point)
        DialogNode specificFeedback = tree.AddSequentialNode(
            parentNode: positiveAssessment,
            speakerName: "Director",
            dialogText: "Your stage presence is strong, but remember to project your voice to the back row. And watch your timing on the third act entrance.",
            isPlayerSpeaking: false,
            nodeName: "director_notes"
        );

        // Link other paths to the same feedback node
        tree.AddChoiceToNamedNode(uncertainAssessment, "What should I focus on?", "director_notes");
        tree.AddChoiceToNamedNode(neutralAssessment, "I'd love your notes.", "director_notes");

        // Player reaction to feedback
        DialogNode acceptancePath = tree.AddChoiceNode(
            parentNode: specificFeedback,
            choiceText: "Thank you! I'll work on that.",
            speakerName: "Director",
            dialogText: "That's what I like to hear! Practice those notes before tomorrow's run-through.",
            isPlayerSpeaking: false,
            customActionId: "unlock_practice_session",
            nodeName: "accepting_feedback"
        );

        DialogNode defensivePath = tree.AddChoiceNode(
            parentNode: specificFeedback,
            choiceText: "But I thought my timing was perfect...",
            speakerName: "Director",
            dialogText: "I understand it feels that way from your perspective. Trust me on this - a small adjustment will make it shine.",
            isPlayerSpeaking: false,
            nodeName: "defensive_reaction"
        );

        // Defensive path continues
        tree.AddSequentialNode(
            parentNode: defensivePath,
            speakerName: "Director",
            dialogText: "Let's run through it together after rehearsal. Sometimes you just need to feel it from a different angle.",
            isPlayerSpeaking: false
        );

        // Save the asset
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Rehearsal Feedback Template.asset";
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Rehearsal Feedback Template at: {path}");
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
            nodeName: "crew_greeting"
        );

        DialogNode firstTimeResponse = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "Yes! Everyone seems really talented.",
            speakerName: "Lighting Tech",
            dialogText: "We've been working together for years. You're gonna love it here!",
            isPlayerSpeaking: false,
            nodeName: "first_time_path"
        );

        DialogNode veteranResponse = tree.AddChoiceNode(
            parentNode: start,
            choiceText: "I've worked with stage crews before.",
            speakerName: "Lighting Tech",
            dialogText: "Nice! Then you know how crazy things can get during tech week.",
            isPlayerSpeaking: false,
            nodeName: "veteran_path"
        );

        // Lighting tech shares a story (convergent point)
        DialogNode techWeekStory = tree.AddSequentialNode(
            parentNode: firstTimeResponse,
            speakerName: "Lighting Tech",
            dialogText: "Last show, a spotlight died right before the big monologue. We had to improvise with handheld lights from the wings!",
            isPlayerSpeaking: false,
            nodeName: "tech_week_story"
        );

        // Link veteran path to same story
        tree.AddSequentialNode(
            parentNode: veteranResponse,
            speakerName: "Lighting Tech",
            dialogText: "Want to hear about our most memorable tech disaster?",
            isPlayerSpeaking: false
        ).ChildNode = techWeekStory;

        // Player responds to story
        DialogNode laughResponse = tree.AddChoiceNode(
            parentNode: techWeekStory,
            choiceText: "That sounds terrifying!",
            speakerName: "Lighting Tech",
            dialogText: "It was! But the show went on. That's theater magic for you.",
            isPlayerSpeaking: false
        );

        DialogNode curiousResponse = tree.AddChoiceNode(
            parentNode: techWeekStory,
            choiceText: "Did the audience notice?",
            speakerName: "Lighting Tech",
            dialogText: "Not at all! The actor played it off perfectly. That's the beauty of live performance.",
            isPlayerSpeaking: false
        );

        // Both paths end with crew solidarity
        DialogNode solidarity = tree.AddSequentialNode(
            parentNode: laughResponse,
            speakerName: "Lighting Tech",
            dialogText: "Anyway, if you ever need anything backstage, just holler. We've got your back out there.",
            isPlayerSpeaking: false,
            nodeName: "crew_solidarity"
        );

        tree.AddSequentialNode(
            parentNode: curiousResponse,
            speakerName: "Lighting Tech", 
            dialogText: "Remember: on stage, we're all in this together. Break a leg tonight!",
            isPlayerSpeaking: false
        ).ChildNode = solidarity;

        // Save the asset
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Stage Crew Banter Template.asset";
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tree;

        Debug.Log($"Created Stage Crew Banter Template at: {path}");
    }
}