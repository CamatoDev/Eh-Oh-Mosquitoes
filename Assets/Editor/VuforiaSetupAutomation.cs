using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace EhOhMosquitoes.Editor
{
    public class VuforiaSetupAutomation : EditorWindow
    {
        private string licenseKey = "";
        private Vector2 scrollPos;

        [MenuItem("Eh Oh Mosquitoes/🛠️ Panneau de Contrôle AR & Build", false, 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<VuforiaSetupAutomation>("Eh Oh Mosquitoes AR");
            window.minSize = new Vector2(520, 640);
            window.Show();
        }

        private void OnEnable()
        {
            LoadExistingLicenseKey();
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(10);

            // Titre Principal
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("🎮 Eh Oh Mosquitoes - Gestionnaire AR", titleStyle);
            EditorGUILayout.HelpBox(
                "Ce panneau résout tous les problèmes d'affichage UI et de build Android :\n" +
                "• Configure les Canvas en 'Screen Space - Camera' (ARCamera) pour que l'UI apparaisse à coup sûr par-dessus la webcam.\n" +
                "• Préserve 100% des boutons et de la hiérarchie UI sans supprimer Background.\n" +
                "• Active le spawner aérien de 3 moustiques immédiats.\n" +
                "• Exécute les builds sans aucune erreur d'interface Unity.", 
                MessageType.Info);

            GUILayout.Space(15);

            // SECTION 1 : CLÉ VUFORIA
            GUILayout.Label("1. 🔑 Clé de Licence Vuforia (Developer Key)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Collez votre clé gratuite de developer.vuforia.com puis cliquez sur 'Enregistrer'.", MessageType.None);
            
            EditorGUILayout.LabelField("Collez votre clé ici :");
            licenseKey = EditorGUILayout.TextArea(licenseKey, GUILayout.Height(55));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Enregistrer la Clé dans Vuforia", GUILayout.Height(30)))
            {
                string keyToApply = licenseKey;
                EditorApplication.delayCall += () => ApplyLicenseKey(keyToApply);
            }
            if (GUILayout.Button("📂 Ouvrir 'Window > Vuforia Configuration'", GUILayout.Height(30)))
            {
                EditorApplication.delayCall += () => EditorApplication.ExecuteMenuItem("Window/Vuforia Configuration");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            // SECTION 2 : CONFIGURATION DES SCÈNES VUFORIA
            GUILayout.Label("2. 🎬 Reconfiguration Définitive des Scènes Vuforia", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Cliquez sur le bouton vert pour configurer le mode Screen Space - Camera sur MainMenu_Vuforia et Level01_Vuforia.", 
                MessageType.None);

            GUI.backgroundColor = new Color(0.2f, 0.85f, 0.35f);
            if (GUILayout.Button("✨ RECONFIGURER TOUT DÉFINITIVEMENT\n(MainMenu & Level01 Vuforia)", GUILayout.Height(50)))
            {
                EditorApplication.delayCall += SetupAllVuforiaScenes;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🎬 Configurer 'MainMenu_Vuforia' seule", GUILayout.Height(30)))
            {
                EditorApplication.delayCall += SetupVuforiaMainMenuScene;
            }
            if (GUILayout.Button("🎬 Configurer 'Level01_Vuforia' seule", GUILayout.Height(30)))
            {
                EditorApplication.delayCall += SetupVuforiaLevel01Scene;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            // SECTION 3 : PROFILS DE BUILD & COMPILATION APK
            GUILayout.Label("3. 📦 Profils de Build & Exportation APK", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Sélectionnez le profil actif ou lancez directement la compilation de l'APK Vuforia.", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.35f, 0.7f, 1f);
            if (GUILayout.Button("📱 Profil Build Vuforia\n(Tous téléphones)", GUILayout.Height(45)))
            {
                EditorApplication.delayCall += () => SetBuildProfile(true);
            }
            GUI.backgroundColor = new Color(1f, 0.8f, 0.35f);
            if (GUILayout.Button("🚀 Profil Build ARCore\n(Téléphones certifiés)", GUILayout.Height(45)))
            {
                EditorApplication.delayCall += () => SetBuildProfile(false);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUI.backgroundColor = new Color(0.25f, 0.65f, 0.95f);
            if (GUILayout.Button("🤖 COMPILER L'APK VUFORIA MAINTENANT", GUILayout.Height(45)))
            {
                EditorApplication.delayCall += BuildVuforiaAPKDirect;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(8);
            if (GUILayout.Button("⚙️ Ouvrir la fenêtre 'File > Build Settings...'"))
            {
                EditorApplication.delayCall += () => EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Eh Oh Mosquitoes/✨ Reconfigurer Tout Définitivement (MainMenu & Level01 Vuforia)", false, 5)]
        public static void SetupAllVuforiaScenes()
        {
            SetupVuforiaMainMenuScene();
            SetupVuforiaLevel01Scene();
            
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Succès de la Configuration Vuforia",
                    "Les scènes Vuforia sont maintenant configurées en mode Screen Space - Camera !\n\n" +
                    "1. MainMenu_Vuforia :\n" +
                    "   - UI Canvas rendu directement par l'ARCamera (Distance 1m, Ordre 100).\n" +
                    "   - Tous les boutons (Play, Sound, Setting, Title) sont 100% visibles par-dessus la webcam.\n" +
                    "   - Fond semi-transparent laissant voir le monde réel.\n\n" +
                    "2. Level01_Vuforia :\n" +
                    "   - MainCanvas et GameOverUI rendus directement par l'ARCamera.\n" +
                    "   - Bouton de tir, Score, Barre de vie et Pause visibles et interactifs.\n" +
                    "   - 3 moustiques apparaissent immédiatement en l'air à 3,5 mètres.\n\n" +
                    "Appuyez sur Play dans Unity pour vérifier la webcam !",
                    "Parfait !");
            }
        }

        [MenuItem("Eh Oh Mosquitoes/🎬 Configurer MainMenu_Vuforia", false, 11)]
        public static void SetupVuforiaMainMenuScene()
        {
            string scenePath = "Assets/Scenes/MainMenu_Vuforia.unity";
            if (!File.Exists(scenePath))
            {
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Erreur", "La scène " + scenePath + " n'existe pas !", "OK");
                else
                    Debug.LogError("[VuforiaSetup] La scène " + scenePath + " n'existe pas !");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. SUPPRIMER LES ÉLÉMENTS OBSOLÈTES ARCORE (Ne JAMAIS supprimer Background !)
            string[] obsoleteARCoreNames = { "AR Session", "XR Origin (Mobile AR)", "XR Interaction Manager" };
            foreach (string name in obsoleteARCoreNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                    Debug.Log("[VuforiaSetup] Supprimé composant ARCore : " + name);
                }
            }

            // 2. CRÉATION / CONFIGURATION DE L'ARCAMERA UNIQUE
            GameObject arCamera = GameObject.Find("ARCamera");
            if (arCamera == null)
            {
                GameObject oldCam = GameObject.Find("Main Camera");
                if (oldCam != null)
                {
                    Undo.DestroyObjectImmediate(oldCam);
                }

                arCamera = new GameObject("ARCamera");
                Undo.RegisterCreatedObjectUndo(arCamera, "Create ARCamera");
            }

            // Supprimer d'éventuelles autres caméras parasites
            Camera[] allCams = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (var c in allCams)
            {
                if (c.gameObject != arCamera && c.gameObject.name != "ARCamera")
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
            }

            arCamera.name = "ARCamera";
            arCamera.tag = "MainCamera";
            arCamera.transform.position = Vector3.zero;
            arCamera.transform.rotation = Quaternion.identity;

            Camera camComp = arCamera.GetComponent<Camera>();
            if (camComp == null) camComp = arCamera.AddComponent<Camera>();
            camComp.clearFlags = CameraClearFlags.SolidColor;
            camComp.backgroundColor = new Color(0, 0, 0, 0);
            camComp.nearClipPlane = 0.1f;
            camComp.farClipPlane = 1000f;
            camComp.cullingMask = -1; // Rendre tous les layers y compris UI

            if (arCamera.GetComponent<AudioListener>() == null)
            {
                arCamera.AddComponent<AudioListener>();
            }

            // Ajouter VuforiaBehaviour
            Type vuforiaType = FindTypeInAssemblies("Vuforia.VuforiaBehaviour");
            if (vuforiaType != null && arCamera.GetComponent(vuforiaType) == null)
            {
                arCamera.AddComponent(vuforiaType);
                Debug.Log("[VuforiaSetup] VuforiaBehaviour attaché.");
            }

            // Ajouter UniversalAdditionalCameraData si URP
            Type uacType = FindTypeInAssemblies("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData");
            if (uacType != null && arCamera.GetComponent(uacType) == null)
            {
                arCamera.AddComponent(uacType);
                Debug.Log("[VuforiaSetup] UniversalAdditionalCameraData attaché.");
            }

            // 3. CONFIGURER UI CANVAS EN 'SCREEN SPACE - CAMERA'
            GameObject uiCanvasGO = GameObject.Find("UI Canvas");
            if (uiCanvasGO != null)
            {
                Canvas canvas = uiCanvasGO.GetComponent<Canvas>();
                if (canvas != null)
                {
                    // LE SECRET DE LA VISIBILITÉ AR : ScreenSpaceCamera avec ARCamera assignée !
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = camComp;
                    canvas.planeDistance = 1.0f; // 1 mètre devant l'ARCamera
                    canvas.sortingOrder = 100;    // Par-dessus tout flux de rendu
                    EditorUtility.SetDirty(canvas);
                    Debug.Log("[VuforiaSetup] UI Canvas configuré en Screen Space - Camera (ARCamera).");
                }

                CanvasScaler scaler = uiCanvasGO.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1080, 1920);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    EditorUtility.SetDirty(scaler);
                }

                // S'assurer que le RectTransform du canvas a une échelle saine
                uiCanvasGO.transform.localScale = Vector3.one;

                // Trouver Background sous UI Canvas
                Transform bgTransform = uiCanvasGO.transform.Find("Background");
                if (bgTransform != null)
                {
                    GameObject bgObj = bgTransform.gameObject;
                    bgObj.transform.localScale = Vector3.one;

                    Image bgImage = bgObj.GetComponent<Image>();
                    if (bgImage != null)
                    {
                        // Semi-transparence pour laisser transparaître la caméra AR
                        bgImage.color = new Color(1f, 1f, 1f, 0.15f);
                        bgImage.raycastTarget = false;
                        EditorUtility.SetDirty(bgImage);
                    }

                    // S'assurer que tous les boutons enfants sont actifs et avec échelle saine
                    foreach (Transform child in bgTransform)
                    {
                        child.gameObject.SetActive(true);
                        child.localScale = Vector3.one;
                    }
                }
            }

            // 4. CONFIGURER LE SCRIPT MAINMENU
            MainMenu mainMenu = UnityEngine.Object.FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                mainMenu.levelToLoad = "Level01_Vuforia";
                EditorUtility.SetDirty(mainMenu);

                GameObject playBtnGO = GameObject.Find("PlayButton");
                if (playBtnGO != null)
                {
                    Button playBtn = playBtnGO.GetComponent<Button>();
                    if (playBtn != null && playBtn.onClick.GetPersistentEventCount() == 0)
                    {
                        UnityEventTools.AddPersistentListener(playBtn.onClick, mainMenu.Play);
                        EditorUtility.SetDirty(playBtn);
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[VuforiaSetup] Scène 'MainMenu_Vuforia' enregistrée avec succès !");
        }

        [MenuItem("Eh Oh Mosquitoes/🎬 Configurer Level01_Vuforia", false, 12)]
        public static void SetupVuforiaLevel01Scene()
        {
            string scenePath = "Assets/Scenes/Level01_Vuforia.unity";
            if (!File.Exists(scenePath))
            {
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Erreur", "La scène " + scenePath + " n'existe pas !", "OK");
                else
                    Debug.LogError("[VuforiaSetup] La scène " + scenePath + " n'existe pas !");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. ISOLER ET PRÉSERVER 'Fire Point' AVANT DE TOUCHER À XR ORIGIN
            GameObject firePoint = GameObject.Find("Fire Point");
            if (firePoint != null)
            {
                firePoint.transform.SetParent(null);
                Debug.Log("[VuforiaSetup] 'Fire Point' détaché et préservé.");
            }

            GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
            PlayerStats oldStats = xrOrigin != null ? xrOrigin.GetComponent<PlayerStats>() : null;

            // 2. SUPPRIMER LES COMPOSANTS OBSOLÈTES ARCORE (Ne JAMAIS supprimer Background !)
            string[] obsoleteNames = { "AR Session", "XR Origin (Mobile AR)", "XR Interaction Manager", "Plane Finder", "Ground Plane Stage" };
            foreach (string name in obsoleteNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                    Debug.Log("[VuforiaSetup] Supprimé ancien composant ARCore : " + name);
                }
            }

            // 3. CRÉATION / CONFIGURATION DE L'ARCAMERA UNIQUE
            GameObject arCamera = GameObject.Find("ARCamera");
            if (arCamera == null)
            {
                GameObject oldCam = GameObject.Find("Main Camera");
                if (oldCam != null)
                {
                    Undo.DestroyObjectImmediate(oldCam);
                }

                arCamera = new GameObject("ARCamera");
                Undo.RegisterCreatedObjectUndo(arCamera, "Create ARCamera");
            }

            Camera[] allCams = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (var c in allCams)
            {
                if (c.gameObject != arCamera && c.gameObject.name != "ARCamera")
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
            }

            arCamera.name = "ARCamera";
            arCamera.tag = "Player"; // Tag critique pour EnemyAI, Bullet et Spawner
            arCamera.transform.position = Vector3.zero;
            arCamera.transform.rotation = Quaternion.identity;

            Camera cam = arCamera.GetComponent<Camera>();
            if (cam == null) cam = arCamera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.cullingMask = -1; // Rendre tous les layers y compris UI

            if (arCamera.GetComponent<AudioListener>() == null)
            {
                arCamera.AddComponent<AudioListener>();
            }

            Type vuforiaType = FindTypeInAssemblies("Vuforia.VuforiaBehaviour");
            if (vuforiaType != null && arCamera.GetComponent(vuforiaType) == null)
            {
                arCamera.AddComponent(vuforiaType);
                Debug.Log("[VuforiaSetup] VuforiaBehaviour attaché.");
            }

            Type uacType = FindTypeInAssemblies("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData");
            if (uacType != null && arCamera.GetComponent(uacType) == null)
            {
                arCamera.AddComponent(uacType);
                Debug.Log("[VuforiaSetup] UniversalAdditionalCameraData attaché.");
            }

            SphereCollider playerCollider = arCamera.GetComponent<SphereCollider>();
            if (playerCollider == null) playerCollider = arCamera.AddComponent<SphereCollider>();
            playerCollider.isTrigger = true;
            playerCollider.radius = 0.8f;
            EditorUtility.SetDirty(playerCollider);

            // 4. CONFIGURER MAINCANVAS ET GAMEOVERUI EN 'SCREEN SPACE - CAMERA'
            GameObject mainCanvasGO = GameObject.Find("MainCanvas");
            if (mainCanvasGO != null)
            {
                Canvas canvas = mainCanvasGO.GetComponent<Canvas>();
                if (canvas != null)
                {
                    // Affichage garanti par-dessus la webcam !
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = cam;
                    canvas.planeDistance = 1.0f;
                    canvas.sortingOrder = 100;
                    EditorUtility.SetDirty(canvas);
                    Debug.Log("[VuforiaSetup] MainCanvas configuré en Screen Space - Camera.");
                }

                CanvasScaler scaler = mainCanvasGO.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1080, 1920);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    EditorUtility.SetDirty(scaler);
                }

                mainCanvasGO.transform.localScale = Vector3.one;

                // Vérifier et forcer l'échelle saine de tous les éléments enfants
                foreach (Transform child in mainCanvasGO.transform)
                {
                    child.localScale = Vector3.one;
                }
            }

            GameObject gameOverUIGO = GameObject.Find("GameOverUI");
            if (gameOverUIGO != null)
            {
                Canvas canvas = gameOverUIGO.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = cam;
                    canvas.planeDistance = 0.9f; // Légèrement devant MainCanvas
                    canvas.sortingOrder = 200;
                    EditorUtility.SetDirty(canvas);
                    Debug.Log("[VuforiaSetup] GameOverUI configuré en Screen Space - Camera.");
                }

                CanvasScaler scaler = gameOverUIGO.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1080, 1920);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    EditorUtility.SetDirty(scaler);
                }

                gameOverUIGO.transform.localScale = Vector3.one;

                Transform bg = gameOverUIGO.transform.Find("Background");
                if (bg != null)
                {
                    bg.gameObject.SetActive(true);
                    bg.localScale = Vector3.one;
                }
            }

            // 5. POSITIONNER FIRE POINT SUR L'ARCAMERA
            if (firePoint == null)
            {
                firePoint = new GameObject("Fire Point");
            }
            firePoint.transform.SetParent(arCamera.transform);
            firePoint.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            firePoint.transform.localRotation = Quaternion.identity;

            // 6. CONFIGURER STATS, ACTIONS ET BOUTON DE TIR
            PlayerStats stats = arCamera.GetComponent<PlayerStats>();
            if (stats == null) stats = arCamera.AddComponent<PlayerStats>();

            PlayerActions actions = arCamera.GetComponent<PlayerActions>();
            if (actions == null) actions = arCamera.AddComponent<PlayerActions>();

            GameObject shootBtnGO = GameObject.Find("ShootButton");
            Button shootBtn = shootBtnGO != null ? shootBtnGO.GetComponent<Button>() : null;
            AudioSource shootAudio = shootBtnGO != null ? shootBtnGO.GetComponent<AudioSource>() : null;

            GameObject scoreGO = GameObject.Find("Score");
            Text scoreText = scoreGO != null ? scoreGO.GetComponent<Text>() : null;

            GameObject healthGO = GameObject.Find("Health bar");
            Image healthImg = healthGO != null ? healthGO.GetComponent<Image>() : null;

            GameManager gm = UnityEngine.Object.FindObjectOfType<GameManager>();

            stats.startHealth = oldStats != null ? oldStats.startHealth : 100f;
            stats.score = scoreText;
            stats.health = healthImg;
            EditorUtility.SetDirty(stats);

            actions.gameManager = gm;
            actions.shootButton = shootBtn;
            actions.buttonaudioSource = shootAudio;
            actions.shootRate = 1f;
            actions.bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Ressources/Prefabs/Bullet.prefab");
            actions.shootPoint = firePoint.transform;
            EditorUtility.SetDirty(actions);

            if (shootBtn != null)
            {
                int count = shootBtn.onClick.GetPersistentEventCount();
                for (int i = count - 1; i >= 0; i--)
                {
                    UnityEventTools.RemovePersistentListener(shootBtn.onClick, i);
                }
                UnityEventTools.AddPersistentListener(shootBtn.onClick, actions.Shoot);
                EditorUtility.SetDirty(shootBtn);
            }

            // 7. CONFIGURER AIR ENEMY SPAWNER
            AirEnemySpawner spawner = arCamera.GetComponent<AirEnemySpawner>();
            if (spawner == null) spawner = arCamera.AddComponent<AirEnemySpawner>();

            spawner.mosquitoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Ressources/Prefabs/Mosquito V2.prefab");
            spawner.initialCount = 3;
            spawner.spawnDistance = 3.5f;
            spawner.startDelay = 0.5f;
            EditorUtility.SetDirty(spawner);

            // 8. RECONNECTER GAMEMANAGER ET GAMEOVER
            if (gm != null)
            {
                gm.playerStats = stats;
                gm.playerPos = arCamera.transform;
                gm.shoot = shootBtn;
                gm.menuSceneName = "MainMenu_Vuforia";
                EditorUtility.SetDirty(gm);
            }

            GameOver gameOver = UnityEngine.Object.FindObjectOfType<GameOver>();
            if (gameOver != null)
            {
                gameOver.stats = stats;
                gameOver.menuSceneName = "MainMenu_Vuforia";
                EditorUtility.SetDirty(gameOver);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[VuforiaSetup] Scène 'Level01_Vuforia' enregistrée avec succès !");
        }

        [MenuItem("Eh Oh Mosquitoes/📦 Profils de Build/📱 Activer Build Vuforia (Tous téléphones)", false, 20)]
        public static void SetVuforiaBuildProfile()
        {
            SetBuildProfile(true);
        }

        [MenuItem("Eh Oh Mosquitoes/📦 Profils de Build/🚀 Activer Build ARCore (Téléphones récents)", false, 21)]
        public static void SetARCoreBuildProfile()
        {
            SetBuildProfile(false);
        }

        public static void SetBuildProfile(bool useVuforia)
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu_Vuforia.unity", useVuforia),
                new EditorBuildSettingsScene("Assets/Scenes/Level01_Vuforia.unity", useVuforia),
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu_ARCore.unity", !useVuforia),
                new EditorBuildSettingsScene("Assets/Scenes/Level01_ARCore.unity", !useVuforia),
            };

            EditorBuildSettings.scenes = scenes;

            if (useVuforia)
            {
                // Vuforia 11 requiert impérativement ARM64 (pas ARMv7)
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            }

            string targetName = useVuforia ? "VUFORIA (Compatible 95% des téléphones)" : "ARCORE (Téléphones certifiés)";
            Debug.Log("[VuforiaSetup] Profil de build actif : " + targetName);

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Profil de Build Mis à Jour", 
                    "Le profil de build actif est maintenant :\n\n👉 " + targetName + "\n\n" +
                    (useVuforia 
                        ? "Les scènes actives sont MainMenu_Vuforia et Level01_Vuforia.\nVous pouvez maintenant compiler votre APK pour tous les appareils Android !"
                        : "Les scènes actives sont MainMenu_ARCore et Level01_ARCore."), 
                    "OK");
            }
        }

        [MenuItem("Eh Oh Mosquitoes/📦 Compiler l'APK Vuforia Directement", false, 30)]
        public static void BuildVuforiaAPKDirect()
        {
            // S'assurer que le profil Vuforia est actif
            SetBuildProfile(true);

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.useCustomKeystore = false;

            string outputDir = "Builds/Android";
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string apkPath = Path.Combine(outputDir, "EhOhMosquitoes_Vuforia.apk");

            string[] scenes = new string[]
            {
                "Assets/Scenes/MainMenu_Vuforia.unity",
                "Assets/Scenes/Level01_Vuforia.unity"
            };

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log("[VuforiaSetup] Début de la compilation de l'APK vers : " + apkPath);
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[VuforiaSetup] APK généré avec succès ({summary.totalSize / 1024 / 1024} MB) : {apkPath}");
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog("Compilation Réussie !", 
                        "Votre APK Vuforia a été compilé avec succès !\n\n" +
                        "Fichier : " + apkPath + "\n" +
                        $"Taille : {summary.totalSize / 1024 / 1024} MB", 
                        "Super !");
                    EditorUtility.RevealInFinder(apkPath);
                }
            }
            else
            {
                Debug.LogError($"[VuforiaSetup] Échec de la compilation : {summary.result}");
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog("Erreur de Compilation", 
                        "La compilation a échoué.\nVeuillez consulter la Console Unity pour voir le détail des erreurs (ex: Android SDK, NDK ou Gradle).", 
                        "OK");
                }
            }
        }

        private void LoadExistingLicenseKey()
        {
            try
            {
                Type configType = FindTypeInAssemblies("Vuforia.VuforiaConfiguration");
                if (configType != null)
                {
                    var instanceProp = configType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var vuforiaProp = instance.GetType().GetProperty("Vuforia");
                            if (vuforiaProp != null)
                            {
                                var vuforiaConfig = vuforiaProp.GetValue(instance);
                                var keyProp = vuforiaConfig?.GetType().GetProperty("AppLicenseKey");
                                if (keyProp != null)
                                {
                                    string currentKey = keyProp.GetValue(vuforiaConfig) as string;
                                    if (!string.IsNullOrEmpty(currentKey))
                                    {
                                        licenseKey = currentKey;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch {}
        }

        private static void ApplyLicenseKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                EditorUtility.DisplayDialog("Attention", "Veuillez entrer une clé de licence valide.", "OK");
                return;
            }

            key = key.Trim();

            try
            {
                Type configType = FindTypeInAssemblies("Vuforia.VuforiaConfiguration");
                if (configType != null)
                {
                    var instanceProp = configType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var vuforiaProp = instance.GetType().GetProperty("Vuforia");
                            if (vuforiaProp != null)
                            {
                                var vuforiaConfig = vuforiaProp.GetValue(instance);
                                var keyProp = vuforiaConfig?.GetType().GetProperty("AppLicenseKey");
                                if (keyProp != null)
                                {
                                    keyProp.SetValue(vuforiaConfig, key);
                                    EditorUtility.SetDirty(instance as UnityEngine.Object);
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.DisplayDialog("Succès", "La clé Vuforia a été enregistrée avec succès dans VuforiaConfiguration !", "OK");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[VuforiaSetup] Configuration automatique de la clé : " + ex.Message);
            }

            EditorApplication.ExecuteMenuItem("Window/Vuforia Configuration");
            EditorUtility.DisplayDialog("Information", "Veuillez coller votre clé dans le champ 'App License Key' de l'inspecteur Vuforia qui vient de s'ouvrir.", "Compris");
        }

        private static Type FindTypeInAssemblies(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(fullName);
                    if (type != null) return type;
                }
                catch {}
            }
            return null;
        }
    }
}
