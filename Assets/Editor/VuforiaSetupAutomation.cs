using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
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
            window.minSize = new Vector2(500, 580);
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
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15, alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("🎮 Eh Oh Mosquitoes - Gestionnaire AR", titleStyle);
            EditorGUILayout.HelpBox("Ce panneau nettoie et reconfigure proprement vos scènes Vuforia SANS AUCUN DOUBLON (même si elles ont déjà été modifiées).", MessageType.Info);

            GUILayout.Space(15);

            // SECTION 1 : CLÉ VUFORIA
            GUILayout.Label("1. 🔑 Clé de Licence Vuforia (Developer Key)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Collez votre clé gratuite de developer.vuforia.com puis cliquez sur 'Enregistrer'.", MessageType.None);
            
            EditorGUILayout.LabelField("Collez votre clé ici :");
            licenseKey = EditorGUILayout.TextArea(licenseKey, GUILayout.Height(65));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Enregistrer la Clé dans Vuforia", GUILayout.Height(30)))
            {
                ApplyLicenseKey(licenseKey);
            }
            if (GUILayout.Button("📂 Ouvrir 'Window > Vuforia Configuration'", GUILayout.Height(30)))
            {
                EditorApplication.ExecuteMenuItem("Window/Vuforia Configuration");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            // SECTION 2 : CONFIGURATION DES SCÈNES VUFORIA
            GUILayout.Label("2. 🎬 Nettoyage & Configuration des Scènes Vuforia", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Supprime les doublons, enlève les fonds noirs opaques, installe l'ARCamera unique et configure le spawner aérien de 3 moustiques sans sol ni marqueur.", MessageType.None);

            GUI.backgroundColor = new Color(0.3f, 0.85f, 0.4f);
            if (GUILayout.Button("✨ TOUT CONFIGURER PROPREMENT (MainMenu & Level01)", GUILayout.Height(42)))
            {
                SetupAllVuforiaScenes();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🎬 Nettoyer 'MainMenu_Vuforia'", GUILayout.Height(32)))
            {
                SetupVuforiaMainMenuScene();
            }
            if (GUILayout.Button("🎬 Nettoyer 'Level01_Vuforia'", GUILayout.Height(32)))
            {
                SetupVuforiaLevel01Scene();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            // SECTION 3 : PROFILS DE BUILD
            GUILayout.Label("3. 📦 Profils de Build Android", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Sélectionne les bonnes scènes dans Build Settings pour exporter soit l'APK Vuforia (95% des téléphones), soit l'APK ARCore.", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
            if (GUILayout.Button("📱 Profil Build Vuforia\n(Tous téléphones Android)", GUILayout.Height(45)))
            {
                SetBuildProfile(useVuforia: true);
            }
            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("🚀 Profil Build ARCore\n(Téléphones récents certifiés)", GUILayout.Height(45)))
            {
                SetBuildProfile(useVuforia: false);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("⚙️ Ouvrir File > Build Settings..."))
            {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Eh Oh Mosquitoes/✨ Tout Configurer (MainMenu & Level01 Vuforia)", false, 5)]
        public static void SetupAllVuforiaScenes()
        {
            SetupVuforiaMainMenuScene();
            SetupVuforiaLevel01Scene();
            EditorUtility.DisplayDialog("Succès Total", 
                "Nettoyage et configuration réussis sans aucun doublon !\n\n" +
                "1. MainMenu_Vuforia : Caméra réelle visible en direct + UI flottante.\n" +
                "2. Level01_Vuforia : Caméra réelle + 3 moustiques aériens immédiats + Fire Point et Tir 100% connectés !\n\n" +
                "Vous pouvez maintenant tester avec la webcam ou exporter l'APK !", 
                "Super !");
        }

        [MenuItem("Eh Oh Mosquitoes/🎬 Configurer Scène MainMenu_Vuforia (Automatique)", false, 11)]
        public static void SetupVuforiaMainMenuScene()
        {
            string scenePath = "Assets/Scenes/MainMenu_Vuforia.unity";
            if (!File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("Erreur", "La scène " + scenePath + " n'existe pas !", "OK");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. Supprimer le fond opaque Background s'il est présent
            GameObject bgObj = GameObject.Find("Background");
            if (bgObj != null)
            {
                Undo.DestroyObjectImmediate(bgObj);
                Debug.Log("[VuforiaSetup] Background opaque supprimé de MainMenu_Vuforia.");
            }

            // 2. Supprimer les éléments obsolètes ARCore
            string[] obsoleteNames = { "AR Session", "XR Origin (Mobile AR)", "XR Interaction Manager" };
            foreach (string name in obsoleteNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                    Debug.Log("[VuforiaSetup] Supprimé ancien composant ARCore : " + name);
                }
            }

            // 3. Gestion anti-doublon de l'ARCamera
            // Rechercher si une ARCamera existe déjà
            GameObject arCamera = GameObject.Find("ARCamera");
            if (arCamera == null)
            {
                // Vérifier si une Main Camera simple existe
                GameObject oldCam = GameObject.Find("Main Camera");
                if (oldCam != null)
                {
                    Undo.DestroyObjectImmediate(oldCam);
                }

                // Créer l'ARCamera Vuforia officielle
                bool created = EditorApplication.ExecuteMenuItem("GameObject/Vuforia Engine/AR Camera");
                arCamera = Selection.activeGameObject;

                if (!created || arCamera == null || arCamera.GetComponent<Camera>() == null)
                {
                    arCamera = new GameObject("ARCamera");
                    Camera cam = arCamera.AddComponent<Camera>();
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0, 0, 0, 0);
                    arCamera.AddComponent<AudioListener>();

                    Type vuforiaType = Type.GetType("Vuforia.VuforiaBehaviour, VuforiaEngine") 
                        ?? Type.GetType("Vuforia.VuforiaBehaviour, VuforiaScripts")
                        ?? Type.GetType("Vuforia.VuforiaBehaviour, Unity.VuforiaEngine");
                    if (vuforiaType != null)
                    {
                        arCamera.AddComponent(vuforiaType);
                    }
                }
            }

            // S'assurer qu'il n'y a pas d'autres caméras parasites
            Camera[] allCams = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (var c in allCams)
            {
                if (c.gameObject != arCamera && c.name != "ARCamera")
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
            }

            if (arCamera != null)
            {
                arCamera.name = "ARCamera";
                arCamera.tag = "MainCamera";
                Camera cam = arCamera.GetComponent<Camera>();
                if (cam != null)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0, 0, 0, 0);
                }
                Debug.Log("[VuforiaSetup] ARCamera Vuforia unique validée pour MainMenu_Vuforia.");
            }

            // 4. Configurer le script MainMenu
            MainMenu mainMenu = UnityEngine.Object.FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                mainMenu.levelToLoad = "Level01_Vuforia";
                EditorUtility.SetDirty(mainMenu);
                Debug.Log("[VuforiaSetup] MainMenu configuré pour charger Level01_Vuforia.");
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[VuforiaSetup] Scène 'MainMenu_Vuforia' nettoyée et enregistrée avec succès !");
        }

        [MenuItem("Eh Oh Mosquitoes/🎬 Configurer Scène Level01_Vuforia (Automatique)", false, 12)]
        public static void SetupVuforiaLevel01Scene()
        {
            string scenePath = "Assets/Scenes/Level01_Vuforia.unity";
            if (!File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("Erreur", "La scène " + scenePath + " n'existe pas !", "OK");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. Sauvegarder les données de l'ancien XR Origin s'il existe encore
            GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
            PlayerActions oldActions = xrOrigin != null ? xrOrigin.GetComponent<PlayerActions>() : null;
            PlayerStats oldStats = xrOrigin != null ? xrOrigin.GetComponent<PlayerStats>() : null;

            // 2. Nettoyer les éléments obsolètes (Ground Plane, Plane Finder, ARCore, Background opaque)
            string[] obsoleteNames = { "Background", "AR Session", "XR Interaction Manager", "Plane Finder", "Ground Plane Stage" };
            foreach (string name in obsoleteNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                    Debug.Log("[VuforiaSetup] Supprimé composant non désiré : " + name);
                }
            }

            // 3. Gestion anti-doublon de l'ARCamera
            GameObject arCamera = GameObject.Find("ARCamera");
            if (arCamera == null)
            {
                // Chercher si un objet caméra existe déjà
                Camera existingCam = UnityEngine.Object.FindObjectOfType<Camera>();
                if (existingCam != null)
                {
                    arCamera = existingCam.gameObject;
                }
                else
                {
                    bool created = EditorApplication.ExecuteMenuItem("GameObject/Vuforia Engine/AR Camera");
                    arCamera = Selection.activeGameObject;

                    if (!created || arCamera == null || arCamera.GetComponent<Camera>() == null)
                    {
                        arCamera = new GameObject("ARCamera");
                        Camera cam = arCamera.AddComponent<Camera>();
                        cam.clearFlags = CameraClearFlags.SolidColor;
                        cam.backgroundColor = new Color(0, 0, 0, 0);
                        arCamera.AddComponent<AudioListener>();

                        Type vuforiaType = Type.GetType("Vuforia.VuforiaBehaviour, VuforiaEngine") 
                            ?? Type.GetType("Vuforia.VuforiaBehaviour, VuforiaScripts")
                            ?? Type.GetType("Vuforia.VuforiaBehaviour, Unity.VuforiaEngine");
                        if (vuforiaType != null)
                        {
                            arCamera.AddComponent(vuforiaType);
                        }
                    }
                }
            }

            // Nettoyer les éventuelles autres caméras parasites
            Camera[] allCams = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (var c in allCams)
            {
                if (c.gameObject != arCamera && c.gameObject.name != "ARCamera")
                {
                    Undo.DestroyObjectImmediate(c.gameObject);
                }
            }

            arCamera.name = "ARCamera";
            arCamera.tag = "Player"; // Tag critique : EnemyAI et Bullet s'accrochent au tag Player

            Camera arCamComponent = arCamera.GetComponent<Camera>();
            if (arCamComponent != null)
            {
                arCamComponent.clearFlags = CameraClearFlags.SolidColor;
                arCamComponent.backgroundColor = new Color(0, 0, 0, 0);
                arCamComponent.nearClipPlane = 0.1f;
                arCamComponent.farClipPlane = 1000f;
            }

            // 4. Gestion anti-doublon de Fire Point
            GameObject firePoint = GameObject.Find("Fire Point");
            if (firePoint == null)
            {
                firePoint = new GameObject("Fire Point");
            }
            firePoint.transform.SetParent(arCamera.transform);
            firePoint.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            firePoint.transform.localRotation = Quaternion.identity;
            Debug.Log("[VuforiaSetup] 'Fire Point' positionné à l'avant de l'ARCamera.");

            // 5. Configurer PlayerActions et PlayerStats sur l'ARCamera (sans doublon)
            PlayerActions newActions = arCamera.GetComponent<PlayerActions>();
            if (newActions == null) newActions = arCamera.AddComponent<PlayerActions>();

            PlayerStats newStats = arCamera.GetComponent<PlayerStats>();
            if (newStats == null) newStats = arCamera.AddComponent<PlayerStats>();

            // Éléments UI
            GameObject shootBtnGO = GameObject.Find("ShootButton");
            Button shootBtn = shootBtnGO != null ? shootBtnGO.GetComponent<Button>() : null;
            AudioSource btnAudio = shootBtnGO != null ? shootBtnGO.GetComponent<AudioSource>() : null;

            GameObject scoreGO = GameObject.Find("Score");
            Text scoreText = scoreGO != null ? scoreGO.GetComponent<Text>() : null;

            GameObject healthGO = GameObject.Find("Health bar");
            Image healthImg = healthGO != null ? healthGO.GetComponent<Image>() : null;

            GameManager gm = UnityEngine.Object.FindObjectOfType<GameManager>();

            newStats.startHealth = oldStats != null ? oldStats.startHealth : 100f;
            newStats.score = scoreText;
            newStats.health = healthImg;
            EditorUtility.SetDirty(newStats);

            newActions.gameManager = gm;
            newActions.shootButton = shootBtn;
            newActions.buttonaudioSource = btnAudio;
            newActions.shootRate = 1f;
            newActions.bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Ressources/Prefabs/Bullet.prefab");
            newActions.shootPoint = firePoint.transform;
            EditorUtility.SetDirty(newActions);

            // Reconnecter le bouton Shoot sans dupliquer les listeners
            if (shootBtn != null)
            {
                // Vider les anciens listeners pour éviter tout tir multiple
                int count = shootBtn.onClick.GetPersistentEventCount();
                for (int i = count - 1; i >= 0; i--)
                {
                    UnityEventTools.RemovePersistentListener(shootBtn.onClick, i);
                }
                UnityEventTools.AddPersistentListener(shootBtn.onClick, newActions.Shoot);
                EditorUtility.SetDirty(shootBtn);
                Debug.Log("[VuforiaSetup] ShootButton relié proprement à PlayerActions.Shoot.");
            }

            // 6. Gestion anti-doublon du Spawner Aérien
            AirEnemySpawner spawner = arCamera.GetComponent<AirEnemySpawner>();
            if (spawner == null) spawner = arCamera.AddComponent<AirEnemySpawner>();

            spawner.mosquitoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Ressources/Prefabs/Mosquito V2.prefab");
            spawner.initialCount = 3;
            spawner.spawnDistance = 3.5f;
            spawner.startDelay = 0.5f;
            EditorUtility.SetDirty(spawner);
            Debug.Log("[VuforiaSetup] AirEnemySpawner configuré (3 moustiques immédiats dans les airs).");

            // 7. Supprimer l'ancien XR Origin s'il existait encore
            if (xrOrigin != null && xrOrigin != arCamera)
            {
                Undo.DestroyObjectImmediate(xrOrigin);
                Debug.Log("[VuforiaSetup] Ancien XR Origin supprimé.");
            }

            // 8. Reconnecter GameManager et GameOver
            if (gm != null)
            {
                gm.playerStats = newStats;
                gm.playerPos = arCamera.transform;
                gm.menuSceneName = "MainMenu_Vuforia";
                EditorUtility.SetDirty(gm);
                Debug.Log("[VuforiaSetup] GameManager relié à la nouvelle ARCamera.");
            }

            GameOver gameOver = UnityEngine.Object.FindObjectOfType<GameOver>();
            if (gameOver != null)
            {
                gameOver.stats = newStats;
                gameOver.menuSceneName = "MainMenu_Vuforia";
                EditorUtility.SetDirty(gameOver);
                Debug.Log("[VuforiaSetup] GameOver relié à la nouvelle ARCamera.");
            }

            // 9. Reconnecter MainCanvas
            GameObject mainCanvasGO = GameObject.Find("MainCanvas");
            if (mainCanvasGO != null)
            {
                Canvas canvas = mainCanvasGO.GetComponent<Canvas>();
                if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    canvas.worldCamera = arCamComponent;
                    EditorUtility.SetDirty(canvas);
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[VuforiaSetup] Scène 'Level01_Vuforia' nettoyée et enregistrée avec succès !");
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

            string targetName = useVuforia ? "VUFORIA (Compatible 95% des téléphones)" : "ARCORE (Téléphones certifiés)";
            Debug.Log("[VuforiaSetup] Profil de build actif : " + targetName);

            EditorUtility.DisplayDialog("Profil de Build Mis à Jour", 
                "Le profil de build actif est maintenant :\n\n👉 " + targetName + "\n\n" +
                (useVuforia 
                    ? "Les scènes actives sont MainMenu_Vuforia et Level01_Vuforia.\nVous pouvez maintenant compiler votre APK pour tous les appareils Android !"
                    : "Les scènes actives sont MainMenu_ARCore et Level01_ARCore."), 
                "OK");
        }

        private void LoadExistingLicenseKey()
        {
            try
            {
                Type configType = Type.GetType("Vuforia.VuforiaConfiguration, VuforiaEngine") 
                    ?? Type.GetType("Vuforia.VuforiaConfiguration, VuforiaScripts")
                    ?? Type.GetType("Vuforia.VuforiaConfiguration, Unity.VuforiaEngine");

                if (configType != null)
                {
                    var instanceProp = configType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var vuforiaProp = instance.GetType().GetProperty("Vuforia");
                            if (vuforiaProp != null)
                            {
                                var vuforiaConfig = vuforiaProp.GetValue(instance);
                                var keyProp = vuforiaConfig.GetType().GetProperty("AppLicenseKey");
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
                Type configType = Type.GetType("Vuforia.VuforiaConfiguration, VuforiaEngine") 
                    ?? Type.GetType("Vuforia.VuforiaConfiguration, VuforiaScripts")
                    ?? Type.GetType("Vuforia.VuforiaConfiguration, Unity.VuforiaEngine");

                if (configType != null)
                {
                    var instanceProp = configType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var vuforiaProp = instance.GetType().GetProperty("Vuforia");
                            if (vuforiaProp != null)
                            {
                                var vuforiaConfig = vuforiaProp.GetValue(instance);
                                var keyProp = vuforiaConfig.GetType().GetProperty("AppLicenseKey");
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
    }
}
