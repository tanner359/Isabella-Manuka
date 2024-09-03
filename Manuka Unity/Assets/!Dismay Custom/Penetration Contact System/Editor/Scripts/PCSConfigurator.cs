using System.IO;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEngine.Animations;
using System.Linq;
using VRC.SDK3.Dynamics.Contact.Components;
using System;
using System.Collections.Generic;

namespace DMCustom
{
    public class PCSConfigurator : EditorWindow
    {
        private GUIStyle paramStyle, infoStyle;
        private static readonly float winWidth = 512, winHeight = 640;
        private Vector2 scrollPosition = new(0, 300);
        private Texture2D logo;
        private Animator animator;
        private int TotalCost;
        private string trigger_oldName = null;
        private bool flag1 = true, flag2 = true, flag3 = true, flag4 = true, flag5 = true, flag6 = true, flag7 = true;

        //###
        private readonly string nameOfGimmick = "Penetration Contact System";
        private readonly string nameOfPrefab = "Penetration Contact System";
        private readonly string nameOfBanner = "DMC_PCS_banner";
        private readonly string nameOfInfo = "DMC_PCS_info";
        private readonly string gimmickVersion = "1.7.0";
        private readonly string infoText = "Please join my discord and let me know if you encounter any errors or having questions about this product. Thank you for purchasing.";
        private readonly int maxWidth_field = 485;
        private readonly int maxWidth_label = 145;

        private VRCAvatarDescriptor targetAvatar;
        private readonly string[] soundPosition_name = new string[] { "Disable", "1", "2", "3", "4", "5", "6", "7", "8" };
        private readonly int[] soundPosition_sizes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private readonly GameObject[] ref_soundPosition = new GameObject[8];
        private GameObject ref_mouth = null, ref_boobs = null, ref_pussy = null, ref_ass = null;
        private readonly string[] ref_soundPositionName = new string[] { "Custom #1", "Custom #2", "Custom #3", "Custom #4", "Custom #5", "Custom #6", "Custom #7","Custom #8"};
        private bool useMouth = true, useBoobs = true, usePussy = true, useAss = true, lustFeature = false;
        private bool localOnlyToggle = false, noSpatialAudio = false, manualInstallMenu = true;
        private float smashSensitivity = 0.025f, lustMultiplierValue = 0.4f;
        private int selected_soundPosition;
        private bool hidePlacement = true;

        private enum SetupTool { ModularAvatar, VRCFury }
        private SetupTool setupTool = SetupTool.ModularAvatar;
        private enum Preset
        {
            GENERIC,
            REFERENCE,

            Anon,
            Aria,
            Eyo,
            Imeris,
            Karin,
            Kikyo,
            Leefa,
            Lime,
            Manuka,
            Maya,
            Moe,
            Mophira,
            Rindo,
            Runa_Robotic,
            Sio,
            Selestia,
            Shinra,
            UltimateKissMa,
            Uzuki,
            Velle,
            Wolferia,

            Body_LexzBase,
            Body_Panda,
            Body_ToriBase,
            Body_TVF,
            Body_ZinFit,
            Body_ZinRP,

        }
        private Preset preset = Preset.GENERIC;
        private enum VoicePack
        {
            Disable, Anime, Mature
        }
        private VoicePack voicePack = VoicePack.Disable;
        private class SaveObject
        {
            public SetupTool installer;
            public bool localToggle;
            public Preset preset;
            public bool useMouth, useBoobs, usePussy, useAss;
            public string refMouth, refBoobs, refPussy, refAss;
            public float sensitivity;
            public int customPosAmount;
            public string refCosName1, refCosName2, refCosName3, refCosName4, refCosName5, refCosName6, refCosName7, refCosName8;
            public string soundPosName1, soundPosName2, soundPosName3, soundPosName4, soundPosName5, soundPosName6, soundPosName7, soundPosName8;
            public bool useLust;
            public VoicePack voicePack;
            public float lustMultiplier;
            public bool noSpatialAudio;
            public bool manualInstallMenu;
        }
        
        [MenuItem("Tools/Dismay Custom/Penetration Contact System")]
        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSConfigurator));

            window.titleContent = new GUIContent("Penetration Contact System");
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = window.position;
            float centerWidth = (main.width - pos.width) * 0.5f;
            float centerHeight = (main.height - pos.height) * 0.3f;
            pos.x = main.x + centerWidth + 360;
            pos.y = main.y + centerHeight;
            window.position = pos;
            window.minSize = new Vector2(winWidth, winHeight);
            window.maxSize = new Vector2(winWidth, 1920);
            window.Show();
        }
        private void MakeLineCenter(string mode, bool useBorder)
        {
            if (!useBorder)
            {
                if (mode == "/")
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }
                else if (mode == "//")
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                if (mode == "/")
                {
                    EditorGUILayout.BeginHorizontal("ProgressBarBack");
                    GUILayout.FlexibleSpace();
                }
                else if (mode == "//")
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        private void OnGUI()
        {
            if(targetAvatar == null)
            {
                trigger_oldName = "";
            }

            paramStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) },
            };

            infoStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
                alignment = TextAnchor.LowerLeft
            };

            if (Application.isPlaying)
            {
                flag5 = false;
            }
            else
            {
                flag5 = true;
            }

            MakeLineCenter("/", false);
            logo = Resources.Load<Texture2D>("Components/" + nameOfBanner);
            GUILayout.Label(logo, new GUIStyle { fixedWidth = winWidth, fixedHeight = 90 });
            MakeLineCenter("//", false);

            MakeLineCenter("/", false);
            targetAvatar = EditorGUILayout.ObjectField(targetAvatar, typeof(VRCAvatarDescriptor), true, GUILayout.Height(30), GUILayout.Width(winWidth - 10)) as VRCAvatarDescriptor;
            MakeLineCenter("//", false);

            if (targetAvatar)
            {
                animator = targetAvatar.GetComponent<Animator>();
                var Gimmick = targetAvatar.transform.Find(nameOfPrefab);

                if (trigger_oldName != targetAvatar.name) //Trigger Initial
                {
                    //expressionsMenu = targetAvatar.expressionsMenu;
                    trigger_oldName = targetAvatar.name;
                    
                    LoadSettings();
                    SaveSettings();
                }

                EditorStyles.label.fontStyle = FontStyle.Bold;
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                EditorGUILayout.Space(1);
                
                //start of menu list

                MakeLineCenter("/", false);
                setupTool = (SetupTool)EditorGUILayout.EnumPopup(new GUIContent("Installer:", "Please select your installer. Choosing the wrong one will show you a \"Missing Script\" warning and PCS will not be able to install."), setupTool, GUILayout.Width(maxWidth_field));
                MakeLineCenter("//", false);

                //Manual Install Menu
                flag1 = true;
                manualInstallMenu = true;

                if (preset != Preset.REFERENCE)
                {
                    MakeLineCenter("/", false);
                    preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent("Alignment Preset:", "Choose the Alignment Preset that matches your avatar. If there is no preset available for your avatar, please select Generic or Reference instead. Generic and avatar presets will estimate and position all sounds target for you, while Reference will position them based on your chosen location."), preset, GUILayout.Width(maxWidth_field));
                    MakeLineCenter("//", false);
                }

                //Find SPS Button
                else
                { 
                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("Alignment Preset:", GUILayout.MaxWidth(149));
                    preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent("", "Choose the Alignment Preset that matches your avatar. If there is no preset available for your avatar, please select Generic or Reference instead. Generic and avatar presets will estimate and position all sounds target for you, while Reference will position them based on your chosen location."), preset, GUILayout.Width(199));

                    if (GUILayout.Button("Find SPS sockets", GUILayout.Width(130)))
                    {
                        if (EditorUtility.DisplayDialog(nameOfGimmick, "This option will attempt to find any existing SPS sockets that were created by Wholesome's SPS Configurator. If you have changed the name or messed up with the hierarchy, this option will not be able to find them.", "OK"))
                        {
                            GameObject find_mouth, find_boobs, find_pussy, find_ass;

                            find_mouth = GameObject.Find("SPS/Blowjob");
                            find_boobs = GameObject.Find("SPS/Special/Titjob");
                            find_pussy = GameObject.Find("SPS/Pussy");
                            find_ass = GameObject.Find("SPS/Anal");

                            if (find_mouth != null && find_mouth.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_mouth = find_mouth;
                            }
                            if (find_boobs != null && find_boobs.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_boobs = find_boobs;
                            }
                            if (find_pussy != null && find_pussy.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_pussy = find_pussy;
                            }
                            if (find_ass != null && find_ass.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_ass = find_ass;
                            }
                        }
                    }

                    MakeLineCenter("//", false);
                }

                if (preset != Preset.REFERENCE)
                {
                    flag3 = true;
                    GUILayout.BeginVertical("ProgressBarBack");
                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("Main Location", GUILayout.MaxWidth(maxWidth_label + 2));
                    EditorGUILayout.LabelField("Mouth:", GUILayout.MaxWidth(maxWidth_label - 80));
                    useMouth = EditorGUILayout.Toggle(useMouth, GUILayout.MaxWidth(maxWidth_label - 48));
                    EditorGUILayout.LabelField("Boobs:", GUILayout.MaxWidth(maxWidth_label - 80));
                    useBoobs = EditorGUILayout.Toggle(useBoobs, GUILayout.MaxWidth(maxWidth_label - 48));
                    MakeLineCenter("//", false);

                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(maxWidth_label + 2));
                    EditorGUILayout.LabelField("Pussy:", GUILayout.MaxWidth(maxWidth_label - 80));
                    usePussy = EditorGUILayout.Toggle(usePussy, GUILayout.MaxWidth(maxWidth_label - 48));
                    EditorGUILayout.LabelField("Ass:", GUILayout.MaxWidth(maxWidth_label - 80));
                    useAss = EditorGUILayout.Toggle(useAss, GUILayout.MaxWidth(maxWidth_label - 48));
                    MakeLineCenter("//", false);

                    if (!useMouth && !useBoobs && !usePussy && !useAss)
                    {
                        flag2 = false;
                        EditorGUILayout.HelpBox("You must select at least one location . Otherwise, this system will be useless!!    (ó﹏ò｡)", MessageType.Error);
                    }
                    else
                    {
                        flag2 = true;
                    }

                    GUILayout.EndVertical();
                }
                else
                {
                    flag2 = true;
                    GUILayout.BeginVertical("ProgressBarBack");
                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("Mouth:", GUILayout.MaxWidth(maxWidth_label - 100));
                    ref_mouth = EditorGUILayout.ObjectField(ref_mouth, typeof(GameObject), true, GUILayout.MaxWidth(maxWidth_field / 2.5f)) as GameObject;
                    EditorGUILayout.LabelField("Boobs:", GUILayout.MaxWidth(maxWidth_label - 100));
                    ref_boobs = EditorGUILayout.ObjectField(ref_boobs, typeof(GameObject), true, GUILayout.MaxWidth(maxWidth_field / 2.5f)) as GameObject;
                    MakeLineCenter("//", false);

                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("Pussy:", GUILayout.MaxWidth(maxWidth_label - 100));
                    ref_pussy = EditorGUILayout.ObjectField(ref_pussy, typeof(GameObject), true, GUILayout.MaxWidth(maxWidth_field / 2.5f)) as GameObject;
                    EditorGUILayout.LabelField("Ass:", GUILayout.MaxWidth(maxWidth_label - 100));
                    ref_ass = EditorGUILayout.ObjectField(ref_ass, typeof(GameObject), true, GUILayout.MaxWidth(maxWidth_field / 2.5f)) as GameObject;
                    MakeLineCenter("//", false);

                    bool[] check_pass = new bool[5];
                    if (ref_mouth == null && ref_boobs == null && ref_pussy == null && ref_ass == null) //If all slots are empty
                    {
                        EditorGUILayout.HelpBox("Please specify at least one reference location. You can drag and drop any gameObject into the slot where you want PCS to attach. " +
                           "Leave it empty if you want to disable.", MessageType.Warning);
                        check_pass[4] = false;                     
                    }
                    else if (ref_mouth != null && ref_boobs != null && ref_pussy != null && ref_ass != null)
                    {
                        check_pass[4] = true;
                    }
                    else if (ref_mouth != null || ref_boobs != null || ref_pussy != null || ref_ass != null) //If some slots are filled
                    {
                        check_pass[4] = true;
                    }

                    if (ref_mouth != null)
                    {
                        if (!ref_mouth.transform.IsChildOf(targetAvatar.transform))
                        {
                            check_pass[0] = false;
                            EditorGUILayout.HelpBox("Mouth reference target is not a child gameObject of your avatar. Please choose what is under your avatar prefab.", MessageType.Warning);
                        }
                        else
                        {
                            check_pass[0] = true;
                        }
                    }
                    else
                    {
                        check_pass[0] = true;
                    }
                    if (ref_boobs != null)
                    {
                        if (!ref_boobs.transform.IsChildOf(targetAvatar.transform))
                        {
                            check_pass[1] = false;
                            EditorGUILayout.HelpBox("Boobs reference target is not a child gameObject of your avatar. Please choose what is under your avatar prefab.", MessageType.Warning);
                        }
                        else
                        {
                            check_pass[1] = true;
                        }
                    }
                    else
                    {
                        check_pass[1] = true;
                    }
                    if (ref_pussy != null)
                    {
                        if (!ref_pussy.transform.IsChildOf(targetAvatar.transform))
                        {
                            check_pass[2] = false;
                            EditorGUILayout.HelpBox("Pussy reference target is not a child gameObject of your avatar. Please choose what is under your avatar prefab.", MessageType.Warning);
                        }
                        else
                        {
                            check_pass[2] = true;
                        }
                    }
                    else
                    {
                        check_pass[2] = true;
                    }
                    if (ref_ass != null)
                    {
                        if (!ref_ass.transform.IsChildOf(targetAvatar.transform))
                        {
                            check_pass[3] = false;
                            EditorGUILayout.HelpBox("Ass reference target is not a child gameObject of your avatar. Please choose what is under your avatar prefab.", MessageType.Warning);
                        }
                        else
                        {
                            check_pass[3] = true;
                        }
                    }
                    else
                    {
                        check_pass[3] = true;
                    }

                    if (check_pass[0] && check_pass[1] && check_pass[2] && check_pass[3] && check_pass[4])
                    {
                        flag3 = true;
                    }
                    else
                    {
                        flag3 = false;
                    }

                    GUILayout.EndVertical();
                }

                MakeLineCenter("/", false);
                smashSensitivity = EditorGUILayout.Slider(new GUIContent("Smash Sensitivity:", "This option allows you to adjust the sensitivity for impact detection. Lower this value if you require more force to trigger the sound.)"), smashSensitivity, 0.001f, 0.025f, GUILayout.MaxWidth(maxWidth_field));
                MakeLineCenter("//", false);

                MakeLineCenter("/", false);
                selected_soundPosition = EditorGUILayout.IntPopup("Custom Location:", selected_soundPosition, soundPosition_name, soundPosition_sizes, GUILayout.MaxWidth(maxWidth_field));
                MakeLineCenter("//", false);

                bool justHideThis = true;
                if (justHideThis)
                {
                    if (selected_soundPosition == 0)
                    {
                        flag4 = true;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("This is where PCS will attach to while using Custom Mode. Please make sure you assign all custom target and their menu name. You can set it to anywhere you want like your feet.", MessageType.Info);

                        GUILayout.BeginVertical("ProgressBarBack");
                        if (selected_soundPosition == 1)
                        {
                            if (ref_soundPosition[0] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                        }
                        else if (selected_soundPosition == 2)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                        }
                        else if (selected_soundPosition == 3)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                        }
                        else if (selected_soundPosition == 4)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                            ShowSourceSetup(4);
                        }
                        else if (selected_soundPosition == 5)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                            ShowSourceSetup(4);
                            ShowSourceSetup(5);
                        }
                        else if (selected_soundPosition == 6)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                            ShowSourceSetup(4);
                            ShowSourceSetup(5);
                            ShowSourceSetup(6);
                        }
                        else if (selected_soundPosition == 7)
                        {
                            if (ref_soundPosition[0] == null && ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null || ref_soundPosition[6] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                            ShowSourceSetup(4);
                            ShowSourceSetup(5);
                            ShowSourceSetup(6);
                            ShowSourceSetup(7);
                        }
                        else if (selected_soundPosition == 8)
                        {
                            if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null || ref_soundPosition[6] == null || ref_soundPosition[7] == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                flag4 = true;
                            }
                            ShowSourceSetup(1);
                            ShowSourceSetup(2);
                            ShowSourceSetup(3);
                            ShowSourceSetup(4);
                            ShowSourceSetup(5);
                            ShowSourceSetup(6);
                            ShowSourceSetup(7);
                            ShowSourceSetup(8);
                        }
                        GUILayout.EndVertical();
                    }          
                }

                MakeLineCenter("/", false);
                lustFeature = EditorGUILayout.Toggle(new GUIContent("Lust Feature & Moan:", "This option allows you to gain and accumulate Lust Value while being penetrated. You can use this value for other purposes, such as making your avatar breathe faster when this value is nearly full! This option also include moan sounds and auto-orgasm."), lustFeature, GUILayout.MaxWidth(maxWidth_field - 245));
                localOnlyToggle = EditorGUILayout.Toggle("Local-Only Toggle:", localOnlyToggle, GUILayout.MaxWidth(maxWidth_field - 245));
                MakeLineCenter("//", false);

                if (localOnlyToggle)
                {
                    EditorGUILayout.HelpBox("Warning! Local-Only Toggle option will set all PCS audio switching to local-only, which use no extra parameters but the menu will be unusable. This option is meant to be used only if you want to switch the audio using VRC Parameter Driver or linking them with SPS socket.", MessageType.Warning);
                }

                if (lustFeature)
                {
                    GUILayout.BeginVertical("ProgressBarBack");
                    MakeLineCenter("/", false);
                    voicePack = (VoicePack)EditorGUILayout.EnumPopup("Voice Pack:", voicePack, GUILayout.MaxWidth(maxWidth_field));
                    MakeLineCenter("//", false);

                    MakeLineCenter("/", false);
                    lustMultiplierValue = EditorGUILayout.Slider(new GUIContent("Lust Multiplier:", "Adjust this value to determine how quickly you can gain 1 point from a penetrating stroke. Increasing this value means you'll reach climax more quickly."), lustMultiplierValue, 0.2f, 1, GUILayout.MaxWidth(maxWidth_field));
                    MakeLineCenter("//", false);
                    GUILayout.EndVertical();
                }

                //end of menu list
                GUILayout.EndScrollView();

                if (Gimmick != null)
                {
                    ShowFinalButton(Gimmick.gameObject);
                }
                else
                {
                    ShowFinalButton(null);
                }

                if (targetAvatar.expressionParameters == null)
                {
                    EditorGUILayout.HelpBox("PCS can't find the Parameter Asset on your avatar. Please go to your \"VRC Avatar Descriptor\" and make sure you have it assigned.)", MessageType.Error);
                    flag6 = false;
                }
                else
                {
                    flag6 = true;
                }

                //ADD QUICK ACCESS!

                if (Gimmick != null)
                {
                    string saveString = File.ReadAllText(Application.dataPath + "/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name + "/settings.txt");
                    SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                    EditorGUILayout.BeginVertical("ProgressBarBack");

                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField("Quick Access", GUILayout.MaxWidth(486));
                    MakeLineCenter("//", false);

                    //Line1
                    MakeLineCenter("/", false);
                    GUI.enabled = true;
                    if (GUILayout.Button("Hide/Show Placement Icon", GUILayout.MaxWidth(243))) //488.5f
                    {
                        if (!hidePlacement)
                        {
                            hidePlacement = true;
                        }
                        else
                        {
                            hidePlacement = false;
                        }

                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name == "PCS Preview Icon (will be auto removed)")
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    if (!hidePlacement)
                                    {
                                        SceneVisibilityManager.instance.Hide(gameObj, true);
                                    }
                                    else
                                    {
                                        SceneVisibilityManager.instance.Show(gameObj, true);
                                    }
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Spawn a Test Penetrator", GUILayout.MaxWidth(243))) //488.5f
                    {
                        GameObject x = Instantiate(Resources.Load<GameObject>("PCS Test Penetrator")) as GameObject;
                        x.name = "PCS Test Penetrator";
                    }
                    MakeLineCenter("//", false);

                    //Line2
                    MakeLineCenter("/", false);

                    bool[] check_ref = new bool[4];
                    if (saveObject.preset == Preset.REFERENCE)
                    {
                        if (saveObject.refMouth == "")
                        {
                            check_ref[0] = false;
                        }
                        else
                        {
                            check_ref[0] = true;
                        }
                        if (saveObject.refBoobs == "")
                        {
                            check_ref[1] = false;
                        }
                        else
                        {
                            check_ref[1] = true;
                        }
                        if (saveObject.refPussy == "")
                        {
                            check_ref[2] = false;
                        }
                        else
                        {
                            check_ref[2] = true;
                        }
                        if (saveObject.refAss == "")
                        {
                            check_ref[3] = false;
                        }
                        else
                        {
                            check_ref[3] = true;
                        }
                    }
                    else
                    {
                        if (saveObject.useMouth)
                        {
                            check_ref[0] = true;
                        }
                        else
                        {
                            check_ref[0] = false;
                        }

                        if (saveObject.useBoobs)
                        {
                            check_ref[1] = true;
                        }
                        else
                        {
                            check_ref[1] = false;
                        }

                        if (saveObject.usePussy)
                        {
                            check_ref[2] = true;
                        }
                        else
                        {
                            check_ref[2] = false;
                        }

                        if (saveObject.useAss)
                        {
                            check_ref[3] = true;
                        }
                        else
                        {
                            check_ref[3] = false;
                        }

                    }
                    GUI.enabled = check_ref[0];
                    if (GUILayout.Button("Locate Mouth", GUILayout.MaxWidth(120)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name == "<PCS Target> Mouth")
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    SceneView.FrameLastActiveSceneView();
                                }
                            }
                        }
                    }
                    GUI.enabled = check_ref[1];
                    if (GUILayout.Button("Locate Boobs", GUILayout.MaxWidth(120)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name == "<PCS Target> Boobs")
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    SceneView.FrameLastActiveSceneView();
                                }
                            }
                        }
                    }
                    GUI.enabled = check_ref[2];
                    if (GUILayout.Button("Locate Pussy", GUILayout.MaxWidth(120)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name == "<PCS Target> Pussy")
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    SceneView.FrameLastActiveSceneView();
                                }
                            }
                        }
                    }
                    GUI.enabled = check_ref[3];
                    if (GUILayout.Button("Locate Ass", GUILayout.MaxWidth(120)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name == "<PCS Target> Ass")
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    SceneView.FrameLastActiveSceneView();
                                }
                            }
                        }
                    }
                    MakeLineCenter("//", false);

                    //Line 3
                    if (saveObject.customPosAmount != 0)
                    {
                        bool[] check = new bool[8];
                        bool justHide = true;

                        if (justHide)
                        {
                            if (saveObject.customPosAmount == 1)
                            {
                                check[0] = true;
                                check[1] = false;
                                check[2] = false;
                                check[3] = false;
                                check[4] = false;
                                check[5] = false;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 2)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = false;
                                check[3] = false;
                                check[4] = false;
                                check[5] = false;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 3)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = false;
                                check[4] = false;
                                check[5] = false;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 4)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = true;
                                check[4] = false;
                                check[5] = false;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 5)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = true;
                                check[4] = true;
                                check[5] = false;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 6)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = true;
                                check[4] = true;
                                check[5] = true;
                                check[6] = false;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 7)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = true;
                                check[4] = true;
                                check[5] = true;
                                check[6] = true;
                                check[7] = false;
                            }
                            else if (saveObject.customPosAmount == 8)
                            {
                                check[0] = true;
                                check[1] = true;
                                check[2] = true;
                                check[3] = true;
                                check[4] = true;
                                check[5] = true;
                                check[6] = true;
                                check[7] = true;
                            }
                        }
                        MakeLineCenter("/", false);
                        GUI.enabled = check[0];
                        if (GUILayout.Button("Locate Custom #1", GUILayout.MaxWidth(120)))
                        {
                            foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                            {
                                if (gameObj.name.Contains("<PCS Custom Target #1>"))
                                {
                                    if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                    {
                                        EditorGUIUtility.PingObject(gameObj);
                                        Selection.activeObject = gameObj;
                                        SceneView.FrameLastActiveSceneView();
                                    }
                                }
                            }
                        }
                        GUI.enabled = check[1];
                        if (GUILayout.Button("Locate Custom #2", GUILayout.MaxWidth(120)))
                        {
                            foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                            {
                                if (gameObj.name.Contains("<PCS Custom Target #2>"))
                                {
                                    if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                    {
                                        EditorGUIUtility.PingObject(gameObj);
                                        Selection.activeObject = gameObj;
                                        SceneView.FrameLastActiveSceneView();
                                    }
                                }
                            }
                        }
                        GUI.enabled = check[2];
                        if (GUILayout.Button("Locate Custom #3", GUILayout.MaxWidth(120)))
                        {
                            foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                            {
                                if (gameObj.name.Contains("<PCS Custom Target #3>"))
                                {
                                    if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                    {
                                        EditorGUIUtility.PingObject(gameObj);
                                        Selection.activeObject = gameObj;
                                        SceneView.FrameLastActiveSceneView();
                                    }
                                }
                            }
                        }
                        GUI.enabled = check[3];
                        if (GUILayout.Button("Locate Custom #4", GUILayout.MaxWidth(120)))
                        {
                            foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                            {
                                if (gameObj.name.Contains("<PCS Custom Target #4>"))
                                {
                                    if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                    {
                                        EditorGUIUtility.PingObject(gameObj);
                                        Selection.activeObject = gameObj;
                                        SceneView.FrameLastActiveSceneView();
                                    }
                                }
                            }
                        }
                        MakeLineCenter("//", false);

                        if(saveObject.customPosAmount >= 5)
                        {
                            //Line 4
                            MakeLineCenter("/", false);
                            GUI.enabled = check[4];
                            if (GUILayout.Button("Locate Custom #5", GUILayout.MaxWidth(120)))
                            {
                                foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                                {
                                    if (gameObj.name.Contains("<PCS Custom Target #5>"))
                                    {
                                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                        {
                                            EditorGUIUtility.PingObject(gameObj);
                                            Selection.activeObject = gameObj;
                                            SceneView.FrameLastActiveSceneView();
                                        }
                                    }
                                }
                            }
                            GUI.enabled = check[5];
                            if (GUILayout.Button("Locate Custom #6", GUILayout.MaxWidth(120)))
                            {
                                foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                                {
                                    if (gameObj.name.Contains("<PCS Custom Target #6>"))
                                    {
                                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                        {
                                            EditorGUIUtility.PingObject(gameObj);
                                            Selection.activeObject = gameObj;
                                            SceneView.FrameLastActiveSceneView();
                                        }
                                    }
                                }
                            }
                            GUI.enabled = check[6];
                            if (GUILayout.Button("Locate Custom #7", GUILayout.MaxWidth(120)))
                            {
                                foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                                {
                                    if (gameObj.name.Contains("<PCS Custom Target #7>"))
                                    {
                                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                        {
                                            EditorGUIUtility.PingObject(gameObj);
                                            Selection.activeObject = gameObj;
                                            SceneView.FrameLastActiveSceneView();
                                        }
                                    }
                                }
                            }
                            GUI.enabled = check[7];
                            if (GUILayout.Button("Locate Custom #8", GUILayout.MaxWidth(120)))
                            {
                                foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                                {
                                    if (gameObj.name.Contains("<PCS Custom Target #8>"))
                                    {
                                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                        {
                                            EditorGUIUtility.PingObject(gameObj);
                                            Selection.activeObject = gameObj;
                                            SceneView.FrameLastActiveSceneView();
                                        }
                                    }
                                }
                            }
                            MakeLineCenter("//", false);
                        }
                    }

                    //Line 4 Radius
                    GUI.enabled = true;
                    var PCSContact = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/Proximity/ON-OFF").gameObject;
                    var depthDetectionA = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/Proximity/ON-OFF/Depth Detection (Allow Self)");
                    var depthDetectionB = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/Proximity/ON-OFF/Depth Detection (Disallow Self)");
                    VRCContactReceiver proximityA = depthDetectionA.GetComponent<VRCContactReceiver>();
                    VRCContactReceiver proximityB = depthDetectionB.GetComponent<VRCContactReceiver>();

                    MakeLineCenter("/", false);
                    EditorGUILayout.LabelField(new GUIContent("Adjust Contact Radius:", "Increase the radius of PCS proximity when you feel like the penetrator can't go any deeper and stop making noise."), GUILayout.Width(maxWidth_label));

                    var radius_offset = 0.01f;
                    if (GUILayout.Button("<", GUILayout.Width(167)) && proximityA.radius > 0.1f)
                    {
                        Selection.activeGameObject = PCSContact;
                        proximityA.radius -= radius_offset;
                        proximityA.position.z += radius_offset;
                        depthDetectionA.gameObject.SetActive(false);
                        depthDetectionA.gameObject.SetActive(true);

                        proximityB.radius = proximityA.radius;
                        proximityB.position.z = proximityA.position.z;
                    }
                    if (GUILayout.Button(">", GUILayout.Width(167)) && proximityA.radius < 2)
                    {
                        Selection.activeGameObject = PCSContact;
                        proximityA.radius += radius_offset;
                        proximityA.position.z -= radius_offset;
                        depthDetectionA.gameObject.SetActive(false);
                        depthDetectionA.gameObject.SetActive(true);

                        proximityB.radius = proximityA.radius;
                        proximityB.position.z = proximityA.position.z;
                    }
                    MakeLineCenter("//", false);

                    //Line 5
                    GUI.enabled = true;
                    MakeLineCenter("/", false);
                    if (GUILayout.Button("Go to Asset Folder", GUILayout.MaxWidth(243)))
                    {
                        string path = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name + "/!PCS Install Menu_" + targetAvatar.name + ".asset";
                        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(obj);
                        Selection.activeObject = obj;
                    }
                    if (GUILayout.Button("Go to Backup Folder", GUILayout.MaxWidth(243)))
                    {
                        string path = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name + "/#BACKUP";
                        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(obj);
                        Selection.activeObject = obj;
                    }
                    MakeLineCenter("//", false);

                    GUILayout.EndVertical();

                }

                //End of apply prefab
                ShowParameter();
            }
            else
            {


            }
            ShowFooter();
        }   
        private void ShowFinalButton(GameObject Gimmick)
        {
            if (TotalCost <= 256)
            {

                if (Gimmick == null)
                {
                    if (flag1 && flag2 && flag3 && flag4 && flag5 && flag6 && flag7)
                    {
                        GUI.enabled = true;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Apply", GUILayout.Width(maxWidth_field)))
                        {
                            ApplyPrefab(true);
                        }
                        MakeLineCenter("//", false);
                        GUI.enabled = false;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)))
                        {
                            RemovePrefab(false);
                        }
                        MakeLineCenter("//", false);
                    }
                    else
                    {
                        GUI.enabled = false;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Apply", GUILayout.Width(maxWidth_field))) { }
                        MakeLineCenter("//", false);

                        GUI.enabled = false;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)) && flag5)
                        {
                            RemovePrefab(false);
                        }
                        MakeLineCenter("//", false);
                    }
                }
                else
                {
                    if (flag1 && flag2 && flag3 && flag4 && flag5 && flag6 && flag7)
                    {
                        GUI.enabled = true;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Replace", GUILayout.Width(maxWidth_field)))
                        {
                            ReplacePrefab();
                        }
                        MakeLineCenter("//", false);

                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)))
                        {
                            RemovePrefab(true);
                        }
                        MakeLineCenter("//", false);
                    }
                    else
                    {
                        GUI.enabled = false;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Replace", GUILayout.Width(maxWidth_field))) { }
                        MakeLineCenter("//", false);

                        GUI.enabled = true;
                        MakeLineCenter("/", false);
                        if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)) && flag5)
                        {
                            RemovePrefab(true);
                        }
                        MakeLineCenter("//", false);
                    }
                }

                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.Space();
                if (Gimmick == null)
                {
                    GUI.enabled = false;
                    MakeLineCenter("/", false);
                    if (GUILayout.Button("Apply", GUILayout.Width(maxWidth_field)))
                    {

                    }
                    MakeLineCenter("//", false);
                    MakeLineCenter("/", false);
                    if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)))
                    {

                    }
                    MakeLineCenter("//", false);
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                    MakeLineCenter("/", false);
                    if (GUILayout.Button("Replace", GUILayout.Width(maxWidth_field)))
                    {

                    }
                    MakeLineCenter("//", false);
                    GUI.enabled = true;
                    MakeLineCenter("/", false);
                    if (GUILayout.Button("Remove", GUILayout.Width(maxWidth_field)) && flag5)
                    {
                        RemovePrefab(true);
                    }
                    MakeLineCenter("//", false);
                }
            }
        }
        private void ShowSourceSetup(int slot)
        {
            MakeLineCenter("/", false);
            EditorGUILayout.LabelField("Target #" + (slot) + ":", GUILayout.MaxWidth(maxWidth_label - 80));
            ref_soundPosition[slot - 1] = EditorGUILayout.ObjectField(ref_soundPosition[slot - 1], typeof(GameObject), true, GUILayout.MaxWidth(maxWidth_field / 2.5f)) as GameObject;
            EditorGUILayout.LabelField("Menu Name:", GUILayout.MaxWidth(maxWidth_label - 70));
            ref_soundPositionName[slot - 1] = EditorGUILayout.TextField(ref_soundPositionName[slot - 1], GUILayout.MaxWidth(maxWidth_field /3.5f));
            MakeLineCenter("//", false);
        }
        private void RemovePrefab(bool showDialog)
        {
            if (showDialog)
            {
                if (EditorUtility.DisplayDialog("Penetration Contact System", "Are you sure you want to remove PCS?\n\n• This option will clear all objects including your custom locations and generated assets.", "Yes", "No"))
                {
                    RemoveFunction();
                }
            }
            else
            {
                RemoveFunction();
            }
        }
        private void RemoveFunction()
        {
            trigger_oldName = "";
            var removeTarget = targetAvatar.transform.Find(nameOfPrefab).gameObject;
            //Remove functions       

            //Remove Params
            VRCExpressionParameters.Parameter[] parameterArray = targetAvatar.expressionParameters.parameters;
            parameterArray = parameterArray.Where(x => !x.name.StartsWith("pcs/")).ToArray();
            targetAvatar.expressionParameters.parameters = parameterArray;
            EditorUtility.SetDirty(targetAvatar.expressionParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //Remove Menu
            VRCExpressionsMenu.Control[] arrayY = targetAvatar.expressionsMenu.controls.ToArray();
            if (targetAvatar.expressionsMenu != null)
            {
                foreach (VRCExpressionsMenu.Control c in targetAvatar.expressionsMenu.controls)
                {
                    arrayY = arrayY.Where(x => !x.name.StartsWith("<b>PCS")).ToArray();
                    targetAvatar.expressionsMenu.controls = arrayY.ToList();
                }
            }
            EditorUtility.SetDirty(targetAvatar.expressionsMenu);

            var children = FindObjectsOfType(typeof(GameObject), true) as GameObject[];
            GameObject[] mainTarget = new GameObject[children.Length];
            GameObject[] customTarget = new GameObject[children.Length];

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name.Contains("<PCS Custom Target #") && (children[i].transform.IsChildOf(targetAvatar.transform)))
                {
                    customTarget[i] = children[i];
                }
                if (children[i].name.Contains("<PCS Target>") && (children[i].transform.IsChildOf(targetAvatar.transform)))
                {
                    mainTarget[i] = children[i];
                }
            }

            for(int i =0; i < children.Length; i++)
            {
                DestroyImmediate((customTarget[i]));
                DestroyImmediate((mainTarget[i]));
            }

            DestroyImmediate(removeTarget);
            AssetDatabase.DeleteAsset("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name);

            //end of fucntions
        }
        private void ApplyPrefab(bool showDialog)
        {
            //Function lists
            SaveSettings();
            Backup();
            MergeMenu();
            MergeParameters();
            hidePlacement = true;

            //Copy prefab
            if (setupTool == SetupTool.ModularAvatar)
            {
                GameObject x = Instantiate(Resources.Load<GameObject>("Main Prefab/PCS MA Prefab")) as GameObject;
                x.name = "Penetration Contact System";
                x.transform.localScale = new(1, 1, 1);
                if (lustFeature)
                {
                    if (voicePack == VoicePack.Anime)
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#MA Anime Voice Installer"), x.transform) as GameObject;
                        y.name = "#MA Anime Voice Installer";
                    }
                    else if (voicePack == VoicePack.Mature)
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#MA Mature Voice Installer"), x.transform) as GameObject;
                        y.name = "#MA Mature Voice Installer";
                    }
                    else
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#MA No Voice Installer"), x.transform) as GameObject;
                        y.name = "#MA No Voice Installer";
                    }
                }
                x.transform.parent = targetAvatar.transform;
            }
            else
            {
                GameObject x = Instantiate(Resources.Load<GameObject>("Main Prefab/PCS VRCF Prefab")) as GameObject;
                x.name = "Penetration Contact System";
                x.transform.localScale = new(1, 1, 1);
                if (lustFeature)
                {
                    if (voicePack == VoicePack.Anime)
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#VRCF Anime Voice Installer"), x.transform) as GameObject;
                        y.name = "#VRCF Anime Voice Installer";
                    }
                    else if (voicePack == VoicePack.Mature)
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#VRCF Mature Voice Installer"), x.transform) as GameObject;
                        y.name = "#VRCF Mature Voice Installer";
                    }
                    else
                    {
                        GameObject y = Instantiate(Resources.Load<GameObject>("Main Prefab/#VRCF No Voice Installer"), x.transform) as GameObject;
                        y.name = "#VRCF No Voice Installer";
                    }
                }
                x.transform.parent = targetAvatar.transform;
            }

            ApplyPreset();

            var PCSContact = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts").gameObject;
            ParentConstraint parentConstraint = PCSContact.GetComponent<ParentConstraint>();
            
            //Place all custom sound targets
            GameObject[] customSourceObj = new GameObject[8];
            for (int i = 0; i < selected_soundPosition; i++)
            {
                customSourceObj[i] = Instantiate(Resources.Load<GameObject>("Components/PCS Target #Custom"), ref_soundPosition[i].transform) as GameObject;
                customSourceObj[i].transform.localPosition = new Vector3(0, 0, 0);
                customSourceObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                customSourceObj[i].transform.localScale = new Vector3(1, 1, 1);
            }

            //Set all custom position to parent constraint
            for (int i = 0; i < selected_soundPosition; i++)
            {
                customSourceObj[i].name = "<PCS Custom Target #" + (i + 1) + "> " + ref_soundPositionName[i];
                ConstraintSource[] customSource = new ConstraintSource[8];
                customSource[i].sourceTransform = customSourceObj[i].transform;
                parentConstraint.SetSource(4 + i, customSource[i]);
            }

            //clear unused parent constraint slot
            for (int i = 0; i < 8 - selected_soundPosition; i++)
            {
                parentConstraint.RemoveSource(11 - i);
            }

            //Set smash sensitivity
            var sensitivityObj = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/Receiver/Receiver Local Position/Smash Hit");
            sensitivityObj.transform.localPosition = new Vector3(0, 0, -0.025f + smashSensitivity);

            //Set particles
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var heart_particles = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/PCS Particles - Heart").gameObject;
            var squirt_particles = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/PCS Particles - Squirt").gameObject;
            heart_particles.transform.position = head.position;

            if(preset != Preset.REFERENCE)
            {
                if (usePussy)
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name.Contains("<PCS Target> Pussy"))
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                squirt_particles.transform.position = gameObj.transform.position;
                            }
                        }
                    }
                }
                else
                {
                    DestroyImmediate(squirt_particles);
                }
            }
            else
            {
                if (ref_pussy != null)
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name.Contains("<PCS Target> Pussy"))
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                squirt_particles.transform.position = gameObj.transform.position;
                            }
                        }
                    }
                }
                else
                {
                    DestroyImmediate(squirt_particles);
                }
            }

            //Spatial Audio
            if (noSpatialAudio)
            {
                var audio1 = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/SFX/Insert Exit");
                var audio2 = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/SFX/Smash Hit");
                var audio3 = targetAvatar.transform.Find("Penetration Contact System/Voice Pack/Audio Source");
                var audio4 = targetAvatar.transform.Find("Penetration Contact System/PCS Contacts/SFX/Squirt");
                VRCSpatialAudioSource spatial1 = audio1.GetComponent<VRCSpatialAudioSource>();
                VRCSpatialAudioSource spatial2 = audio2.GetComponent<VRCSpatialAudioSource>();
                VRCSpatialAudioSource spatia13 = audio3.GetComponent<VRCSpatialAudioSource>();
                VRCSpatialAudioSource spatia14 = audio4.GetComponent<VRCSpatialAudioSource>();

                DestroyImmediate(spatial1);
                DestroyImmediate(spatial2);
                DestroyImmediate(spatia13);
                DestroyImmediate(spatia14);
            }

            //Setup Voice Pack
            if(voicePack != VoicePack.Disable) //Set constraint to head
            {
                var voicepack = targetAvatar.transform.Find("Penetration Contact System/Voice Pack");
                PositionConstraint positionConst = voicepack.GetComponent<PositionConstraint>();
                ConstraintSource customSource = new();
                customSource.sourceTransform = head.transform;
                customSource.weight = 1;
                positionConst.SetSource(0, customSource);
            }
            else
            {
                var voicepack = targetAvatar.transform.Find("Penetration Contact System/Voice Pack").gameObject;
                DestroyImmediate(voicepack);
            }

            if (manualInstallMenu)
            {
                string path = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name + "/!PCS Install Menu_" + targetAvatar.name + ".asset";
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;

                if (setupTool == SetupTool.ModularAvatar)
                {
                    GameObject menuInstaller = targetAvatar.transform.Find("Penetration Contact System/#MA Menu Installer").gameObject;
                    EditorGUIUtility.PingObject(menuInstaller);
                    Selection.activeObject = menuInstaller;
                }
                else
                {
                    GameObject PCS = targetAvatar.transform.Find("Penetration Contact System").gameObject;
                    EditorGUIUtility.PingObject(PCS);
                    Selection.activeObject = PCS;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (showDialog)
            {
                EditorUtility.DisplayDialog(nameOfGimmick, "Setup Complete!\n\n• You can now select each sound position and add a Test Penetrator from the Quick Access menu.", "OK");
            }
        }
        private void ReplacePrefab()
        {
            RemoveFunction();
            ApplyPrefab(false);
        }
        private void ApplyPreset()
        {
            var mouth = targetAvatar.transform.Find("Penetration Contact System/<PCS Target> Mouth");
            var boobs = targetAvatar.transform.Find("Penetration Contact System/<PCS Target> Boobs");
            var pussy = targetAvatar.transform.Find("Penetration Contact System/<PCS Target> Pussy" );
            var ass = targetAvatar.transform.Find("Penetration Contact System/<PCS Target> Ass");

            mouth.localScale = new(1, 1, 1);
            boobs.localScale = new(1, 1, 1);
            pussy.localScale = new(1, 1, 1);
            ass.localScale = new(1, 1, 1);

            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            switch (preset)
            {
                case Preset.REFERENCE:
                    if (ref_mouth != null)
                    {
                        mouth.transform.position = ref_mouth.transform.position;
                        mouth.transform.eulerAngles = ref_mouth.transform.eulerAngles;
                    }
                    if (ref_boobs != null)
                    {
                        boobs.transform.position = ref_boobs.transform.position;
                        boobs.transform.eulerAngles = ref_boobs.transform.eulerAngles;
                    }
                    if (ref_pussy != null)
                    {
                        pussy.transform.position = ref_pussy.transform.position;
                        pussy.transform.eulerAngles = ref_pussy.transform.eulerAngles;
                    }
                    if (ref_ass != null)
                    {
                        ass.transform.position = ref_ass.transform.position;
                        ass.transform.eulerAngles = ref_ass.transform.eulerAngles;
                    }
                    break;

                case Preset.GENERIC:
                    mouth.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, head.transform.position.z + 0.05f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(chest.transform.position.x, chest.transform.position.y + 0.08f, chest.transform.position.z + 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - 0.08f, hips.transform.position.z + 0.03f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - 0.08f, hips.transform.position.z - 0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Uzuki:
                    mouth.transform.position = new Vector3(0, 1.13f, 0.055f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.975f, 0.1035f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.702f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.705f, -0.006f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Aria:
                    mouth.transform.position = new Vector3(0, 1.105f, 0.093f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.945f, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.65f, 0.045f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, -0.009f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Kikyo:
                    mouth.transform.position = new Vector3(0, 1.187f, 0.0745f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.08f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.6825f, 0.0185f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.69f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Maya:
                    mouth.transform.position = new Vector3(0, 1.119f, 0.11f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.653f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, 0.002f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Rindo:
                    mouth.transform.position = new Vector3(0, 1.1265f, 0.076f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.97f, 0.075f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.6685f, 0.036f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.678f, -0.015f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Selestia:
                    mouth.transform.position = new Vector3(0, 1.124f, 0.078f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.96f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.67f, 0.03f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.UltimateKissMa:
                    mouth.transform.position = new Vector3(0, 1.115f, 0.065f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.098f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.644f, 0.017f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.645f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Moe:
                    mouth.transform.position = new Vector3(0, 1.219f, 0.088f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.038f, 0.125f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.717f, 0.03f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.723f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Karin:
                    mouth.transform.position = new Vector3(0, 1.061f, 0.052f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.905f, 0.063f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.607f, 0.0115f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.612f, -0.027f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Lime:
                    mouth.transform.position = new Vector3(0, 1.1205f, 0.039f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.955f, 0.0555f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.652f, -0.0045f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, -0.0475f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Anon:
                    mouth.transform.position = new Vector3(0, 1.13f, 0.078f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.965f, 0.093f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.663f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.665f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Leefa:
                    mouth.transform.position = new Vector3(0, 1.104f, 0.0755f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.083f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.654f, 0.0215f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.656f, -0.024f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Imeris:
                    mouth.transform.position = new Vector3(0, 1.22f, 0.0655f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.13f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.699f, 0.0105f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.705f, -0.034f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Manuka:
                    mouth.transform.position = new Vector3(0, 1.092f, 0.072f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.945f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.672f, 0.0225f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.014f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_Panda:
                    mouth.transform.position = new Vector3(0, 1.15f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.98f, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.652f, 0.038f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.6525f, -0.0105f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_ZinRP:
                    mouth.transform.position = new Vector3(0, 1.15f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.07f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.65f, 0.005f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_ZinFit:
                    mouth.transform.position = new Vector3(0, 1.15f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.068f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.636f, 0.005f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.637f, -0.045f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_LexzBase:
                    mouth.transform.position = new Vector3(0, 1.15f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.01f, 0.145f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.659f, 0.048f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.663f, 0.0145f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_ToriBase:
                    mouth.transform.position = new Vector3(0, 1.3f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.075f, 0.13f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.733f, 0.0395f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.74f, 0);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Body_TVF:
                    mouth.transform.position = new Vector3(0, 1.15f, 0.15f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1, 0.126f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.662f, 0.032f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.672f, -0.021f);
                    ass.transform.eulerAngles = new Vector3(110, 0, 0);
                    break;

                case Preset.Velle:
                    mouth.transform.position = new Vector3(0, 1.194f, 0.0835f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.11f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.698f, 0.025f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.712f, -0.018f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Shinra:
                    mouth.transform.position = new Vector3(0, 1.295f, 0.07f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.09f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.7495f, 0);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.756f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Eyo:
                    mouth.transform.position = new Vector3(0, 1.17f, 0.07f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.99f, 0.11f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.673f, 0.013f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.04f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Mophira:
                    mouth.transform.position = new Vector3(0, 1.218f, 0.11f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.03f, 0.14f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.717f, 0.055f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.745f, 0.005f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Runa_Robotic:
                    mouth.transform.position = new Vector3(0, 1.188f, 0.05f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.035f, 0.075f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.685f, 0.005f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.685f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Sio:
                    mouth.transform.position = new Vector3(0, 1.2f, 0.045f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.03f, 0.085f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.726f, -0.01f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.74f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Wolferia:
                    mouth.transform.position = new Vector3(0, 1.196f, 0.085f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.688f, 0.0125f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.69f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;
            }

            if(preset != Preset.REFERENCE)
            {
                mouth.transform.parent = head.transform;
                boobs.transform.parent = chest.transform;
                pussy.transform.parent = hips.transform;
                ass.transform.parent = hips.transform;
            }

            //Remove unused parts
            if (preset == Preset.REFERENCE)
            {
                if (ref_mouth != null)
                {
                    mouth.transform.parent = ref_mouth.transform;
                    if (!mouth.transform.IsChildOf(head.transform))
                    {
                        var components_target = mouth.GetComponents(typeof(Component)).Where(o => !(o is Transform));
                        foreach (var comp in components_target)
                        {
                            DestroyImmediate(comp);
                        }
                    }
                }
                else
                {
                    DestroyImmediate(mouth.gameObject);
                }

                if (ref_boobs != null)
                {
                    boobs.transform.parent = ref_boobs.transform;
                }
                else
                {
                    DestroyImmediate(boobs.gameObject);

                }

                if (ref_pussy != null)
                {
                    pussy.transform.parent = ref_pussy.transform;
                }
                else
                {
                    DestroyImmediate(pussy.gameObject);

                }

                if (ref_ass != null)
                {
                    ass.transform.parent = ref_ass.transform;
                }
                else
                {
                    DestroyImmediate(ass.gameObject);
                }
            }
            else
            {
                if (!useMouth)
                { 
                    DestroyImmediate(mouth.gameObject);
                }
                if (!useBoobs)
                {
                    DestroyImmediate(boobs.gameObject);
                }
                if (!usePussy)
                {
                    DestroyImmediate(pussy.gameObject);
                }
                if (!useAss)
                {
                    DestroyImmediate(ass.gameObject);
                }
            }
        }
        private void Backup()
        {
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name + "/#BACKUP";
            var param = targetAvatar.expressionParameters;
            var originalParam = AssetDatabase.GetAssetPath(param);
            AssetDatabase.CopyAsset(originalParam, folderPath + "/parameter_" + targetAvatar.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void MergeMenu()
        {
            string iconPath = "Assets/!Dismay Custom/Penetration Contact System/Assets/Icons/";
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name;
            Texture2D icon_custom = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
            Texture2D icon_mouth = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "mouth.png", typeof(Texture2D));
            Texture2D icon_boobs = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "boobs.png", typeof(Texture2D));
            Texture2D icon_pussy = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "pussy.png", typeof(Texture2D));
            Texture2D icon_ass = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "ass.png", typeof(Texture2D));
            Texture2D icon_heart = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "heart.png", typeof(Texture2D));
            Texture2D icon_pcs = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "PCS Icon.png", typeof(Texture2D));

            if (AssetDatabase.IsValidFolder(folderPath) == true)
            {
                //Generate main selection submenu
                string mouth_label, boobs_label, pussy_label, ass_label;
                var selectionMenu_ref = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(selectionMenu_ref), folderPath + "/PCS Selection_" + targetAvatar.name + ".asset");
                var selectionMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/PCS Selection_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));

                mouth_label = "Mouth";
                boobs_label = "Boobs";
                pussy_label = "Pussy";
                ass_label = "Ass";

                //Main Poosition
                if (!localOnlyToggle)
                {
                    if (preset != Preset.REFERENCE)
                    {
                        if (useMouth)
                        {

                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/mouth",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = mouth_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_mouth
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (useBoobs)
                        {

                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/boobs",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = boobs_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_boobs
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (usePussy)
                        {

                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/pussy",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = pussy_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_pussy
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (useAss)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/ass",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = ass_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_ass
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                    }
                    else
                    {
                        if (ref_mouth != null)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/mouth",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = mouth_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_mouth
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (ref_boobs != null)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/boobs",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = boobs_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_boobs
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (ref_pussy != null)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/pussy",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = pussy_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_pussy
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                        if (ref_ass != null)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/select/ass",
                            };

                            VRCExpressionsMenu.Control selection_menu_control = new()
                            {
                                name = ass_label,
                                parameter = selection_parameter,
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = icon_ass
                            };
                            selectionMenu.controls.Add(selection_menu_control);
                        }
                    }
                }

                //Generate custom selection submenu
                if (selected_soundPosition != 0)
                {
                    int menuCount, mainCount;
                    int mouth = Convert.ToInt32(this.useMouth);
                    int boobs = Convert.ToInt32(this.useBoobs);
                    int pussy = Convert.ToInt32(this.usePussy);
                    int ass = Convert.ToInt32(this.useAss);

                    mainCount = (mouth + boobs + pussy + ass) * (1 - Convert.ToInt32(localOnlyToggle));
                    menuCount = selected_soundPosition + (mouth + boobs + pussy + ass) * (1 - Convert.ToInt32(localOnlyToggle));
                    if (!localOnlyToggle)
                    {
                        if (menuCount < 9)
                        {
                            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
                            for (int i = 0; i < selected_soundPosition; i++)
                            {
                                VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter = new VRCExpressionsMenu.Control.Parameter[8];
                                selection_menu_parameter[i] = new VRCExpressionsMenu.Control.Parameter
                                {
                                    name = "pcs/select/custom" + (i + 1),
                                };

                                VRCExpressionsMenu.Control[] selection_menu_control = new VRCExpressionsMenu.Control[8];
                                selection_menu_control[i] = new VRCExpressionsMenu.Control
                                {
                                    name = ref_soundPositionName[i],
                                    parameter = selection_menu_parameter[i],
                                    value = 1,
                                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                    icon = icon
                                };
                                selectionMenu.controls.Add(selection_menu_control[i]);
                            }
                        }
                        else
                        {
                            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
                            for (int i = 0; i < 7 - mainCount; i++)
                            {
                                VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter = new VRCExpressionsMenu.Control.Parameter[8];
                                selection_menu_parameter[i] = new VRCExpressionsMenu.Control.Parameter
                                {
                                    name = "pcs/select/custom" + (i + 1),
                                };

                                VRCExpressionsMenu.Control[] selection_menu_control = new VRCExpressionsMenu.Control[8];
                                selection_menu_control[i] = new VRCExpressionsMenu.Control
                                {
                                    name = ref_soundPositionName[i],
                                    parameter = selection_menu_parameter[i],
                                    value = 1,
                                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                    icon = icon
                                };
                                selectionMenu.controls.Add(selection_menu_control[i]);
                            }
                            //Next page
                            var nextPage_blank = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(nextPage_blank), folderPath + "/PCS next selection_" + targetAvatar.name + ".asset");
                            var nextPage = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/PCS next selection_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));
                            Texture2D icon_nextPage = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "next.png", typeof(Texture2D));

                            VRCExpressionsMenu.Control selection_menu_next;
                            selection_menu_next = new VRCExpressionsMenu.Control
                            {
                                name = "Next >",
                                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                                icon = icon_nextPage,
                                subMenu = nextPage,
                            };
                            selectionMenu.controls.Add(selection_menu_next);

                            for (int i = (7 - mainCount); i < selected_soundPosition; i++)
                            {
                                VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter = new VRCExpressionsMenu.Control.Parameter[8];
                                selection_menu_parameter[i] = new VRCExpressionsMenu.Control.Parameter
                                {
                                    name = "pcs/select/custom" + (i + 1),
                                };

                                VRCExpressionsMenu.Control[] selection_menu_control = new VRCExpressionsMenu.Control[8];
                                selection_menu_control[i] = new VRCExpressionsMenu.Control
                                {
                                    name = ref_soundPositionName[i],
                                    parameter = selection_menu_parameter[i],
                                    value = 1,
                                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                    icon = icon
                                };
                                nextPage.controls.Add(selection_menu_control[i]);
                            }
                            EditorUtility.SetDirty(nextPage);
                        }
                    }
                    else
                    {
                        Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
                        VRCExpressionsMenu.Control.Parameter selection_menu_parameter = new ();
                        selection_menu_parameter = new VRCExpressionsMenu.Control.Parameter
                        {
                            name = "",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new();
                        selection_menu_control = new VRCExpressionsMenu.Control
                        {
                            name = "These toggles are controlled by SPS or something else.",
                            parameter = selection_menu_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Button,
                            icon = icon
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                }

                //Generate main menu
                var menu_ref = Resources.Load("Expression Menu/PCS Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(menu_ref), folderPath + "/PCS Menu_" + targetAvatar.name + ".asset");
                var main_menu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/PCS Menu_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));

                //Add selection menu to main menu
                VRCExpressionsMenu.Control control_selection = new()
                {
                    name = "Sound & Location", //Selection menu name
                    icon = icon_custom,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = selectionMenu,
                };
                main_menu.controls.Add(control_selection);

                //Satisfaction menu
                var satis_ref = Resources.Load("Expression Menu/PCS Satisfaction Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(satis_ref), folderPath + "/PCS Satis_" + targetAvatar.name + ".asset");
                var satisMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/PCS Satis_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));
                VRCExpressionsMenu.Control control_satisfaction = new()
                {
                    name = "Satisfaction",
                    icon = icon_heart,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = satisMenu,
                };
                main_menu.controls.Add(control_satisfaction);
                
                //Remove unused menu if lust is not in use
                if (!lustFeature)
                {                  
                    VRCExpressionsMenu.Control[] array = satisMenu.controls.ToArray();

                    array = array.Where(x => !x.name.StartsWith("Edging (Pause Event)")).ToArray();
                    satisMenu.controls = array.ToList();

                    array = array.Where(x => !x.name.StartsWith("Voice")).ToArray();
                    satisMenu.controls = array.ToList();
                }
                else
                {
                    if (voicePack == VoicePack.Disable)
                    {
                        Debug.Log("TEST");
                        VRCExpressionsMenu.Control[] array = satisMenu.controls.ToArray();
                        array = array.Where(x => !x.name.StartsWith("Voice")).ToArray();
                        satisMenu.controls = array.ToList();
                    }
                }

                //Placeholder Top Menu
                var topMenu_blank = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(topMenu_blank), folderPath + "/!PCS Install Menu_" + targetAvatar.name + ".asset");
                var topMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/!PCS Install Menu_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));

                VRCExpressionsMenu.Control control_mainMenu = new()
                {
                    name = "<b>PCS " + gimmickVersion + "</b>",
                    icon = icon_pcs,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = main_menu,
                };
                topMenu.controls.Add(control_mainMenu);

                EditorUtility.SetDirty(main_menu);
                EditorUtility.SetDirty(selectionMenu);
                EditorUtility.SetDirty(topMenu);
                EditorUtility.SetDirty(satisMenu);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("PCS: The tool can't find the following folder path. Folder path: " + folderPath);
            }
        }
        private void MergeParameters()
        {
            VRCExpressionParameters.Parameter[] parameterArray = targetAvatar.expressionParameters.parameters;
            parameterArray = parameterArray.Where(x => !x.name.StartsWith("pcs/")).ToArray();
            targetAvatar.expressionParameters.parameters = parameterArray;
            EditorUtility.SetDirty(targetAvatar.expressionParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            VRCExpressionParameters parametersOriginal = (VRCExpressionParameters)targetAvatar.expressionParameters; //Get Parameters
            List<VRCExpressionParameters.Parameter> addparams = new(); //Make new empty list of parameters
            
            //Add main parameters
            var parametersToAdd = Resources.Load("Expression Menu/PCS Parameter", typeof(VRCExpressionParameters)) as VRCExpressionParameters;
            for (int i = 0; i < parametersToAdd.parameters.Length; i++)
            {
                VRCExpressionParameters.Parameter p = parametersToAdd.parameters[i];
                VRCExpressionParameters.Parameter temp = new()
                {
                    name = p.name,
                    valueType = p.valueType,
                    defaultValue = p.defaultValue,
                    networkSynced = p.networkSynced,
                    saved = p.saved
                };
                addparams.Add(temp);
            }

            //Add selection parameters
            if (!localOnlyToggle)
            {
                if (preset != Preset.REFERENCE)
                {
                    if (useMouth)
                    {
                        VRCExpressionParameters.Parameter isEnable = new ()
                        {
                            name = "pcs/select/mouth",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (useBoobs)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/boobs",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (usePussy)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/pussy",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (useAss)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/ass",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                }
                else
                {
                    if (ref_mouth != null)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/mouth",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (ref_boobs != null)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/boobs",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (ref_pussy != null)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/pussy",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                    if (ref_ass != null)
                    {
                        VRCExpressionParameters.Parameter isEnable = new()
                        {
                            name = "pcs/select/ass",
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        addparams.Add(isEnable);
                    }
                }
            }

            //Add Custom positions
            if (!localOnlyToggle)
            {
                for (int i = 0; i < selected_soundPosition; i++)
                {
                    VRCExpressionParameters.Parameter[] paramList = new VRCExpressionParameters.Parameter[8];
                    paramList[i] = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/select/custom" + soundPosition_name[i + 1],
                        defaultValue = 0,
                        networkSynced = true,
                        saved = true,
                        valueType = VRCExpressionParameters.ValueType.Bool
                    };
                    addparams.Add(paramList[i]);
                }
            }

            if (lustFeature) //If use lust, then copy another set of parameters for lust
            {
                var parametersToAddB = Resources.Load("Expression Menu/PCS Satisfaction Param", typeof(VRCExpressionParameters)) as VRCExpressionParameters;
                for (int i = 0; i < parametersToAddB.parameters.Length; i++)
                {
                    VRCExpressionParameters.Parameter p = parametersToAddB.parameters[i];
                    VRCExpressionParameters.Parameter temp = new()
                    {
                        name = p.name,
                        valueType = p.valueType,
                        defaultValue = p.defaultValue,
                        networkSynced = p.networkSynced,
                        saved = p.saved
                    };
                    addparams.Add(temp);
                }

                //Set lust multiplier
                VRCExpressionParameters.Parameter temp2 = new()
                {
                    name = "pcs/local/lustMultiplier",
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = lustMultiplierValue,
                    networkSynced = false,
                    saved = false
                };
                addparams.Add(temp2);

                //Add moan param if use
                if (voicePack != VoicePack.Disable)
                {
                    VRCExpressionParameters.Parameter temp4 = new()
                    {
                        name = "pcs/sound/moan",
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = 1,
                        networkSynced = true,
                        saved = true
                    };
                    addparams.Add(temp4);
                }
            }

            EditorUtility.SetDirty(targetAvatar.expressionParameters);
            targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams.ToArray()).ToArray();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void SaveSettings()
        {
            GenerateFolder();

            var temp_refMouth = "";
            var temp_refBoobs = "";
            var temp_refPussy = "";
            var temp_refAss = "";

            if (ref_mouth != null)
            {
                temp_refMouth = ref_mouth.name;
            }
            if(ref_boobs != null)
            {
                temp_refBoobs = ref_boobs.name;
            }
            if (ref_pussy != null)
            {
                temp_refPussy = ref_pussy.name;
            }
            if (ref_ass != null)
            {
                temp_refAss = ref_ass.name;
            }

            string[] temp_customPos = new string[] { "","","","","","","",""};

            for(int i = 0; i < 8; i++)
            {
                if (ref_soundPosition[i] != null)
                {
                    temp_customPos[i] = ref_soundPosition[i].name;
                }
            }

            SaveObject saveObject = new()
            {
                installer = setupTool,
                localToggle = localOnlyToggle,
                preset = preset,
                useMouth = useMouth,
                useBoobs = useBoobs,
                usePussy = usePussy,
                useAss = useAss,
                sensitivity = smashSensitivity,
                refMouth = temp_refMouth,
                refBoobs = temp_refBoobs,
                refPussy = temp_refPussy,
                refAss = temp_refAss,
                customPosAmount = selected_soundPosition,
                refCosName1 = temp_customPos[0],
                refCosName2 = temp_customPos[1],
                refCosName3 = temp_customPos[2],
                refCosName4 = temp_customPos[3],
                refCosName5 = temp_customPos[4],
                refCosName6 = temp_customPos[5],
                refCosName7 = temp_customPos[6],
                refCosName8 = temp_customPos[7],
                soundPosName1 = ref_soundPositionName[0],
                soundPosName2 = ref_soundPositionName[1],
                soundPosName3 = ref_soundPositionName[2],
                soundPosName4 = ref_soundPositionName[3],
                soundPosName5 = ref_soundPositionName[4],
                soundPosName6 = ref_soundPositionName[5],
                soundPosName7 = ref_soundPositionName[6],
                soundPosName8 = ref_soundPositionName[7],
                useLust = lustFeature,
                voicePack = voicePack,
                lustMultiplier = lustMultiplierValue,
                noSpatialAudio = noSpatialAudio,
                manualInstallMenu = manualInstallMenu,
            };

            string json1 = JsonUtility.ToJson(saveObject);
            File.WriteAllText(Application.dataPath + "/!Dismay Custom/"+ nameOfGimmick + "/#GENERATED/" + targetAvatar.name + "/settings.txt", json1);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void LoadSettings()
        {
            GenerateFolder();

            if (File.Exists(Application.dataPath + "/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name + "/settings.txt"))
            {
                string saveString = File.ReadAllText(Application.dataPath + "/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name + "/settings.txt");
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                setupTool = saveObject.installer;
                localOnlyToggle = saveObject.localToggle;
                preset = saveObject.preset;
                useMouth = saveObject.useMouth;
                useBoobs = saveObject.useBoobs;
                usePussy = saveObject.usePussy;
                useAss = saveObject.useAss;
                smashSensitivity = saveObject.sensitivity;
                selected_soundPosition = saveObject.customPosAmount;
                ref_soundPositionName[0] = saveObject.soundPosName1;
                ref_soundPositionName[1] = saveObject.soundPosName2;
                ref_soundPositionName[2] = saveObject.soundPosName3;
                ref_soundPositionName[3] = saveObject.soundPosName4;
                ref_soundPositionName[4] = saveObject.soundPosName5;
                ref_soundPositionName[5] = saveObject.soundPosName6;
                ref_soundPositionName[6] = saveObject.soundPosName7;
                ref_soundPositionName[7] = saveObject.soundPosName8;
                lustFeature = saveObject.useLust;
                voicePack = saveObject.voicePack;
                lustMultiplierValue = saveObject.lustMultiplier;
                noSpatialAudio = saveObject.noSpatialAudio;
                manualInstallMenu = saveObject.manualInstallMenu;

                foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                {
                    //Find all reference that UID might change
                    if (preset == Preset.REFERENCE)
                    {
                        if (gameObj.name == saveObject.refMouth)
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_mouth = gameObj;
                            }
                        }
                        if (gameObj.name == saveObject.refBoobs)
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_boobs = gameObj;
                            }
                        }
                        if (gameObj.name == saveObject.refPussy)
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_pussy = gameObj;
                            }
                        }
                        if (gameObj.name == saveObject.refAss)
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                ref_ass = gameObj;
                            }
                        }
                    }
                    if (gameObj.name == saveObject.refCosName1)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[0] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName2)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[1] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName3)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[2] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName4)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[3] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName5)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[4] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName6)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[5] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName7)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[6] = gameObj;
                        }
                    }
                    if (gameObj.name == saveObject.refCosName8)
                    {
                        if (gameObj.transform.IsChildOf(targetAvatar.transform))
                        {
                            ref_soundPosition[7] = gameObj;
                        }
                    }
                }              
            }
        }       
        private void GenerateFolder()
        {
            if (AssetDatabase.IsValidFolder("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED") == false)
            {
                AssetDatabase.CreateFolder("Assets/!Dismay Custom/" + nameOfGimmick, "#GENERATED");
            }
            if (AssetDatabase.IsValidFolder("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name) == false)
            {
                AssetDatabase.CreateFolder("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED", targetAvatar.name);
            }

            if (AssetDatabase.IsValidFolder("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name + "/#BACKUP") == false)
            {
                AssetDatabase.CreateFolder("Assets/!Dismay Custom/" + nameOfGimmick + "/#GENERATED/" + targetAvatar.name, "#BACKUP");
            }
        }
        private int CalculateParamUse()
        {
            int defaultUsage = 8;
            int useLust;
            bool useVoice;
            int result;

            if (lustFeature)
            {
                useLust = 9;

                if (voicePack != VoicePack.Disable)
                {
                    useVoice = true;
                }
                else
                {
                    useVoice = false;
                }
            }
            else
            {
                useLust = 0;
                useVoice = false;
            }

            if(preset == Preset.REFERENCE)
            {

                if (localOnlyToggle)
                {
                    result = defaultUsage + useLust + Convert.ToInt32(useVoice);
                    return result;
                }
                else
                {
                    result = defaultUsage + useLust + Convert.ToInt32(useVoice) + (Convert.ToInt32(ref_mouth) + Convert.ToInt32(ref_boobs) + Convert.ToInt32(ref_pussy) + Convert.ToInt32(ref_ass) + selected_soundPosition);
                    return result;
                }               
            }
            else
            {
                if (localOnlyToggle)
                {
                    result = defaultUsage + useLust + Convert.ToInt32(useVoice);
                    return result;
                }
                else
                {
                    result = defaultUsage + useLust + Convert.ToInt32(useVoice) + (Convert.ToInt32(useMouth) + Convert.ToInt32(useBoobs) + Convert.ToInt32(usePussy) + Convert.ToInt32(useAss) + selected_soundPosition);
                    return result;
                }
            }                
        }
        private void ShowParameter()
        {
            EditorGUILayout.Space();
            
            int paramCost;
            paramCost = CalculateParamUse();
            MakeLineCenter("/", false);
            GUILayout.Label("Memory Usage: " + paramCost, paramStyle, GUILayout.Width(495));
            MakeLineCenter("//", false);

            if (targetAvatar.expressionParameters != null)
            {
                TotalCost = targetAvatar.expressionParameters.CalcTotalCost() + paramCost;
            }
            else
            {
                TotalCost = paramCost;
            }

            MakeLineCenter("/", false);
            if (TotalCost <= 256)
            {
                GUILayout.Label("Total Memory: <color=lime>" + TotalCost.ToString() + "</color>/256", paramStyle, GUILayout.Width(495));
            }
            else
            {
                GUILayout.Label("Total Memory: <color=red>" + TotalCost.ToString() + "</color>/256", paramStyle, GUILayout.Width(495));
            }
            MakeLineCenter("//", false);
        }
        private void ShowFooter()
        {
            EditorGUILayout.Space();

            if (!targetAvatar)
            {
                EditorGUILayout.HelpBox("Please drag and drop your avatar into the box.", MessageType.Warning);
                EditorGUILayout.HelpBox("Tip: Toggle off Self-Touch in PCS menu so you won't touch yourself while someone else is doing it. This prevent an issue like when it stuck because more than one sender is detected.", MessageType.Info);
                EditorGUILayout.HelpBox(infoText, MessageType.Info);
            }
            else
            {
                if (TotalCost > 256)
                {
                    EditorGUILayout.HelpBox("It seems that you do not have enough parameters to install this system." +
                        " Please delete some unnecessary parameters from your avatar and try again.", MessageType.Warning);
                }
                else
                {
                    if (targetAvatar.name.Contains("<") || targetAvatar.name.Contains(">") || targetAvatar.name.Contains(":") || targetAvatar.name.Contains("\"") || targetAvatar.name.Contains("/") || targetAvatar.name.Contains("|") || targetAvatar.name.Contains("?") || targetAvatar.name.Contains("*"))
                    {
                        flag7 = false;
                        EditorGUILayout.HelpBox("Your avatar name contains these prohibited characters which are not suitable for naming a folder. (<, >, :, \", /, |, ?, * ). https://help.interfaceware.com/v6/windows-reserved-file-names", MessageType.Error);
                    }
                    else
                    {
                        flag7 = true;
                        EditorGUILayout.HelpBox("This version of PCS supports LMS version 1.3 or newer version only. If you are using an older version of LMS, please update it, don't use them together or it might malfunction.", MessageType.Info);
                    }
                }
            }

            GUILayout.FlexibleSpace();

            
            var info = Resources.Load<TextAsset>("Components/" + nameOfInfo).ToString();

            MakeLineCenter("/", false);
            GUILayout.Label(info.Replace("$", "v" + gimmickVersion), infoStyle, GUILayout.Width(295));
            if (GUILayout.Button("Tutorial", GUILayout.Width(100)))
            {
                Application.OpenURL("https://youtube.com/playlist?list=PLEvAOTfSR8u2fdM_HnFtkXuaqvAEp2WtS&si=fIm6oMbREBQ8hNf_");
            }
            if (GUILayout.Button("Discord", GUILayout.Width(100)))
            {
                Application.OpenURL("https://discord.gg/TkfRyQDNQC");
            }
            MakeLineCenter("//", false);
            EditorGUILayout.Space(5);
        }
    }
}