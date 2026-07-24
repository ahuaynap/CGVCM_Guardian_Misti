using System;
using System.Collections.Generic;
using System.IO;
using StarterAssets;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GuardianMistiGameBuilder
{
    private const string Root = "Assets";
    private static readonly Color Navy = new(0.025f, .07f, .12f, 1);
    private static readonly Color Cyan = new(.05f, .7f, .82f, 1);
    private static readonly Color Orange = new(1f, .42f, .08f, 1);
    private static Material floorMat, wallMat, accentMat, dangerMat, safeMat;
    private static InventoryItemDefinition backpack, radio, key;

    [MenuItem("Guardian Misti/Build Complete Game")]
    public static void BuildCompleteGame()
    {
        try
        {
            EnsureFolders(); CreateMaterials(); CreateItems(); CreatePrefabs();
            BuildMainMenu(); BuildLevel(false); BuildLevel(true);
            EditorBuildSettings.scenes = new[] {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Level01.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Level02.unity", true)};
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("GUARDIAN_MISTI_BUILD_SUCCESS");
        }
        catch (Exception ex) { Debug.LogException(ex); throw; }
    }

    private static void EnsureFolders()
    {
        foreach (string path in new[]{"Assets/Editor","Assets/Scenes","Assets/Prefabs","Assets/Prefabs/Player","Assets/Prefabs/UI","Assets/Prefabs/Systems","Assets/Prefabs/Gameplay","Assets/ScriptableObjects","Assets/ScriptableObjects/Items","Assets/Art/Materials","Assets/Tests/EditMode"})
        {
            if (AssetDatabase.IsValidFolder(path)) continue;
            string parent=Path.GetDirectoryName(path).Replace('\\','/'); AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }

    private static void CreateMaterials()
    {
        floorMat=Mat("Assets/Art/Materials/GM_Floor.mat", new(.11f,.15f,.18f));
        wallMat=Mat("Assets/Art/Materials/GM_Wall.mat", new(.22f,.28f,.31f));
        accentMat=Mat("Assets/Art/Materials/GM_Accent.mat", Cyan);
        dangerMat=Mat("Assets/Art/Materials/GM_Danger.mat", Orange);
        safeMat=Mat("Assets/Art/Materials/GM_Safe.mat", new(.1f,.8f,.32f));
    }
    private static Material Mat(string path, Color color)
    {
        var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(m==null){ var shader=Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"); m=new Material(shader); AssetDatabase.CreateAsset(m,path); }
        m.color=color; EditorUtility.SetDirty(m); return m;
    }

    private static void CreateItems()
    {
        Sprite icon=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Icons/backpack.png");
        backpack=Item("EmergencyBackpack",GameIds.EmergencyBackpack,"Mochila de emergencia","Suministros esenciales para evacuar.",icon);
        radio=Item("EmergencyRadio",GameIds.EmergencyRadio,"Radio de emergencia","Radio portátil para pedir ayuda.",icon);
        key=Item("AccessKey",GameIds.AccessKey,"Llave de acceso","Abre el control de la baliza.",icon);
    }
    private static InventoryItemDefinition Item(string file,string id,string name,string desc,Sprite icon)
    {
        string path=$"Assets/ScriptableObjects/Items/{file}.asset";
        var item=AssetDatabase.LoadAssetAtPath<InventoryItemDefinition>(path);
        if(item==null){item=ScriptableObject.CreateInstance<InventoryItemDefinition>();AssetDatabase.CreateAsset(item,path);}
        Set(item,"<Id>k__BackingField",id);Set(item,"<Name>k__BackingField",name);Set(item,"<Description>k__BackingField",desc);Set(item,"<Icon>k__BackingField",icon);return item;
    }

    private static void CreatePrefabs()
    {
        SavePrefab(PlayerObject(),"Assets/Prefabs/Player/GameplayPlayer.prefab");
        SavePrefab(HudObject(),"Assets/Prefabs/UI/GameplayHUD.prefab");
        var systems=new GameObject("GameplaySystems"); systems.AddComponent<SceneLoader>(); systems.AddComponent<GameplayCursorController>(); SavePrefab(systems,"Assets/Prefabs/Systems/GameplaySystems.prefab");
        var door=Cube("InteractableDoor",Vector3.zero,new(1.8f,2.8f,.22f),accentMat);door.AddComponent<DoorController>();SavePrefab(door,"Assets/Prefabs/Gameplay/InteractableDoor.prefab");
        var collectible=Cube("CollectibleItem",Vector3.zero,new(.7f,.7f,.7f),dangerMat);collectible.AddComponent<CollectibleItemController>();SavePrefab(collectible,"Assets/Prefabs/Gameplay/CollectibleItem.prefab");
        var exit=Cube("LevelExit",Vector3.zero,new(3,2,.3f),safeMat);exit.GetComponent<BoxCollider>().isTrigger=true;exit.AddComponent<LevelExitController>();SavePrefab(exit,"Assets/Prefabs/Gameplay/LevelExit.prefab");
        var zone=Cube("SafeZone",Vector3.zero,new(5,.2f,5),safeMat);zone.GetComponent<BoxCollider>().isTrigger=true;zone.AddComponent<SafeZoneController>();SavePrefab(zone,"Assets/Prefabs/Gameplay/SafeZone.prefab");
    }
    private static void SavePrefab(GameObject go,string path){PrefabUtility.SaveAsPrefabAsset(go,path);UnityEngine.Object.DestroyImmediate(go);}

    private static GameObject PlayerObject()
    {
        var player=new GameObject("GameplayPlayer");player.tag="Player";player.AddComponent<CharacterController>();
        var inputs=player.AddComponent<StarterAssetsInputs>(); var pi=player.AddComponent<PlayerInput>();
        pi.actions=AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/StarterAssets/InputSystem/StarterAssets.inputactions");pi.defaultActionMap="Player";pi.notificationBehavior=PlayerNotifications.SendMessages;
        var target=new GameObject("CameraRoot");target.transform.SetParent(player.transform);target.transform.localPosition=new Vector3(0,1.65f,0);
        var camGo=new GameObject("Main Camera");camGo.tag="MainCamera";camGo.transform.SetParent(target.transform);camGo.transform.localPosition=Vector3.zero;var cam=camGo.AddComponent<Camera>();camGo.AddComponent<AudioListener>();
        var fpc=player.AddComponent<FirstPersonController>();fpc.CinemachineCameraTarget=target;fpc.GroundLayers=~0;
        player.AddComponent<InteractionSystem>(); return player;
    }

    private static GameObject HudObject()
    {
        var canvas=CanvasRoot("GameplayHUD");
        var hud=Rect("HUD",canvas.transform,Vector2.zero,Vector2.one);var cross=ImageObj("Crosshair",hud.transform,new Vector2(.5f,.5f),new Vector2(12,12),Color.white);cross.AddComponent<CrosshairUI>();
        Rect("InteractionPrompt",hud.transform,new Vector2(.5f,.38f),new Vector2(520,55));
        Rect("ObjectivePanel",hud.transform,new Vector2(.23f,.9f),new Vector2(560,90));
        Rect("InventoryPanel",hud.transform,new Vector2(.88f,.9f),new Vector2(210,90));
        var notes=Rect("Notifications",canvas.transform,Vector2.zero,Vector2.one);Rect("NotificationPanel",notes.transform,new Vector2(.5f,.78f),new Vector2(520,90));
        var screens=Rect("Screens",canvas.transform,Vector2.zero,Vector2.one);Rect("PausePanel",screens.transform,new Vector2(.5f,.5f),new Vector2(600,500));Rect("CompletionPanel",screens.transform,new Vector2(.5f,.5f),new Vector2(720,560));
        return canvas.gameObject;
    }

    private static void BuildMainMenu()
    {
        NewScene(); var cam=new GameObject("Main Camera");cam.tag="MainCamera";cam.AddComponent<Camera>().backgroundColor=Navy;cam.AddComponent<AudioListener>();
        EventSystem(); var app=new GameObject("App");var loader=app.AddComponent<SceneLoader>();var menu=app.AddComponent<MainMenuController>();Set(menu,"sceneLoader",loader);
        var canvas=CanvasRoot("Canvas");ImageObj("Background",canvas.transform,new Vector2(.5f,.5f),new Vector2(1920,1080),Navy);
        Text("Title",canvas.transform,"GUARDIAN MISTI",56,new Vector2(.5f,.78f),new Vector2(1100,100),Color.white,FontStyles.Bold);
        Text("Subtitle",canvas.transform,"Simulación de emergencia y supervivencia",25,new Vector2(.5f,.68f),new Vector2(900,60),new(.55f,.85f,.9f));
        ButtonObj("PlayButton",canvas.transform,"INICIAR SIMULACIÓN",new Vector2(.5f,.52f),menu.StartGame);
        var instructions=Text("Instructions",canvas.transform,"WASD: movimiento   •   Mouse: cámara   •   E: interactuar   •   Esc: pausa",20,new Vector2(.5f,.31f),new Vector2(1000,70),Color.white);
        Set(menu,"instructionsPanel",instructions.gameObject);
        ButtonObj("InstructionsButton",canvas.transform,"INSTRUCCIONES",new Vector2(.5f,.42f),menu.ToggleInstructions);
        ButtonObj("ExitButton",canvas.transform,"SALIR",new Vector2(.5f,.22f),menu.QuitGame); instructions.gameObject.SetActive(false);
        SaveScene("Assets/Scenes/MainMenu.unity");
    }

    private static void BuildLevel(bool level2)
    {
        NewScene(); string sceneName=level2?"Level02":"Level01";
        RenderSettings.ambientLight=new Color(.28f,.32f,.38f);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
        var light=new GameObject("Directional Light");var dl=light.AddComponent<Light>();dl.type=LightType.Directional;dl.intensity=1.25f;light.transform.rotation=Quaternion.Euler(48,-28,0);
        Cube("Ground",new Vector3(0,-.25f,10),new Vector3(18,.5f,28),floorMat);
        Cube("LeftWall",new Vector3(-9,2.2f,10),new Vector3(.4f,4.5f,28),wallMat);Cube("RightWall",new Vector3(9,2.2f,10),new Vector3(.4f,4.5f,28),wallMat);
        var sceneLoader=new GameObject("App").AddComponent<SceneLoader>();new GameObject("GameplayCursorController").AddComponent<GameplayCursorController>();
        EventSystem(); var player=PlayerObject();player.transform.position=new Vector3(0,1,0);var fpc=player.GetComponent<FirstPersonController>();var interaction=player.GetComponent<InteractionSystem>();var inputs=player.GetComponent<StarterAssetsInputs>();var camera=player.GetComponentInChildren<Camera>();
        var canvas=BuildGameplayHud(out var objectiveUI,out var inventoryUI,out var notificationUI,out var interactionUI,out var pausePanel,out var completionPanel,out var completionUI,out var completionController);
        Set(interaction,"playerCamera",camera);Set(interaction,"interactionUIController",interactionUI);
        var managers=new GameObject("GameplaySystems");var inventory=managers.AddComponent<InventoryManager>();Set(inventory,"inventoryUI",inventoryUI);Set(inventory,"notificationUI",notificationUI);
        var objectives=managers.AddComponent<ObjectivesManager>();Set(objectives,"objectiveUI",objectiveUI);SetObjectives(objectives,level2);
        var pause=managers.AddComponent<PauseController>();Set(pause,"pausePanel",pausePanel);Set(pause,"sceneLoader",sceneLoader);Set(pause,"gameplayBehaviours",new Behaviour[]{fpc,interaction});Set(pause,"starterInputs",inputs);
        Set(completionController,"sceneLoader",sceneLoader);Set(completionController,"gameplayBehaviours",new Behaviour[]{fpc,interaction});Set(completionController,"pauseController",pause);Set(completionController,"starterInputs",inputs);
        WirePauseButtons(pausePanel,pause);WireCompletionButtons(completionPanel,completionController);
        if(level2) PopulateLevel02(objectives,inventory,notificationUI,completionUI); else PopulateLevel01(objectives,inventory,notificationUI,sceneLoader);
        pausePanel.SetActive(false);completionPanel.SetActive(false);SaveScene($"Assets/Scenes/{sceneName}.unity");
    }

    private static Canvas BuildGameplayHud(out ObjectiveUI objectiveUI,out InventoryUI inventoryUI,out NotificationUI notificationUI,out InteractionUIController interactionUI,out GameObject pause,out GameObject completion,out GameCompletionUI completionUI,out GameCompletionController completionController)
    {
        var canvas=CanvasRoot("GameplayHUD");var hud=Rect("HUD",canvas.transform,Vector2.zero,Vector2.one);
        var crossImage=ImageObj("Crosshair",hud.transform,new Vector2(.5f,.5f),new Vector2(9,9),Color.white);var cross=crossImage.AddComponent<CrosshairUI>();Set(cross,"crossHair",crossImage.GetComponent<Image>());
        var prompt=Panel("InteractionPrompt",hud.transform,new Vector2(.5f,.38f),new Vector2(560,58));var promptText=Text("PromptText",prompt.transform,"",22,new Vector2(.5f,.5f),new Vector2(540,50),Color.white);var promptUI=prompt.AddComponent<InteractionPromptUI>();Set(promptUI,"promptText",promptText);
        var op=Panel("ObjectivePanel",hud.transform,new Vector2(.22f,.91f),new Vector2(650,90));Text("Label",op.transform,"OBJETIVO ACTUAL",15,new Vector2(.5f,.72f),new Vector2(610,25),Cyan,FontStyles.Bold);var ot=Text("ObjectiveText",op.transform,"",22,new Vector2(.5f,.36f),new Vector2(610,45),Color.white);objectiveUI=op.AddComponent<ObjectiveUI>();Set(objectiveUI,"objectiveText",ot);
        var ip=Panel("InventoryPanel",hud.transform,new Vector2(.89f,.91f),new Vector2(210,90));var ii=ImageObj("ItemIcon",ip.transform,new Vector2(.25f,.5f),new Vector2(55,55),Color.white);var it=Text("InventoryText",ip.transform,"x0",25,new Vector2(.68f,.5f),new Vector2(90,55),Color.white);inventoryUI=ip.AddComponent<InventoryUI>();Set(inventoryUI,"inventoryText",it);Set(inventoryUI,"inventoryImage",ii.GetComponent<Image>());
        var notifications=Rect("Notifications",canvas.transform,Vector2.zero,Vector2.one);var np=Panel("NotificationPanel",notifications.transform,new Vector2(.5f,.78f),new Vector2(520,90));var ni=ImageObj("Icon",np.transform,new Vector2(.12f,.5f),new Vector2(60,60),Color.white);var nt=Text("Title",np.transform,"",18,new Vector2(.58f,.68f),new Vector2(390,30),Cyan,FontStyles.Bold);var nd=Text("Description",np.transform,"",19,new Vector2(.58f,.35f),new Vector2(390,35),Color.white);notificationUI=np.AddComponent<NotificationUI>();Set(notificationUI,"icon",ni.GetComponent<Image>());Set(notificationUI,"titleText",nt);Set(notificationUI,"descriptionText",nd);np.SetActive(false);
        var screens=Rect("Screens",canvas.transform,Vector2.zero,Vector2.one);pause=FullPanel("PausePanel",screens.transform,"SIMULACIÓN EN PAUSA");ButtonObj("ResumeButton",pause.transform,"CONTINUAR",new Vector2(.5f,.52f),null);ButtonObj("RestartButton",pause.transform,"REINICIAR NIVEL",new Vector2(.5f,.40f),null);ButtonObj("MenuButton",pause.transform,"VOLVER AL MENÚ",new Vector2(.5f,.28f),null);
        completion=FullPanel("CompletionPanel",screens.transform,"MISIÓN COMPLETADA");Text("Message",completion.transform,"Has alcanzado la zona segura. El protocolo de emergencia fue completado.",22,new Vector2(.5f,.62f),new Vector2(780,80),Color.white);ButtonObj("RestartButton",completion.transform,"REINICIAR NIVEL",new Vector2(.5f,.47f),null);ButtonObj("MenuButton",completion.transform,"VOLVER AL MENÚ",new Vector2(.5f,.35f),null);ButtonObj("ExitButton",completion.transform,"SALIR",new Vector2(.5f,.23f),null);completionController=completion.AddComponent<GameCompletionController>();completionUI=completion.AddComponent<GameCompletionUI>();Set(completionUI,"controller",completionController);
        interactionUI=canvas.gameObject.AddComponent<InteractionUIController>();Set(interactionUI,"promptUI",promptUI);Set(interactionUI,"crosshairUI",cross);prompt.SetActive(false);return canvas;
    }

    private static void PopulateLevel01(ObjectivesManager om,InventoryManager im,NotificationUI note,SceneLoader loader)
    {
        var door=Cube("InitialDoor",new Vector3(0,1.4f,4),new Vector3(2.2f,2.8f,.25f),accentMat);var dc=door.AddComponent<DoorController>();Set(dc,"objectiveId",GameIds.Level01ExitRoom);
        var item=Cube("EmergencyBackpack",new Vector3(-2, .5f,8),new Vector3(.8f,1,.45f),dangerMat);var cc=item.AddComponent<CollectibleItemController>();Set(cc,"definition",backpack);Set(cc,"objectiveId",GameIds.Level01CollectBackpack);
        var terminal=Cube("EvacuationTerminal",new Vector3(2,1,12),new Vector3(1,2,.7f),accentMat);var tc=terminal.AddComponent<EvacuationTerminalController>();Set(tc,"notificationUI",note);Set(tc,"statusRenderer",terminal.GetComponent<Renderer>());
        var exit=Cube("LevelExit",new Vector3(0,1,17),new Vector3(5,2,.4f),safeMat);exit.GetComponent<BoxCollider>().isTrigger=true;var ec=exit.AddComponent<LevelExitController>();Set(ec,"sceneLoader",loader);Set(ec,"objectivesManager",om);Set(ec,"objectiveId",GameIds.Level01ReachExit);
        TextWorld("EVACUACIÓN",new Vector3(0,2.8f,16.7f),Color.green);
    }
    private static void PopulateLevel02(ObjectivesManager om,InventoryManager im,NotificationUI note,GameCompletionUI completion)
    {
        var r=Cube("EmergencyRadio",new Vector3(-3,.55f,5),new Vector3(.7f,.7f,.45f),dangerMat);var rc=r.AddComponent<CollectibleItemController>();Set(rc,"definition",radio);Set(rc,"objectiveId",GameIds.Level02CollectRadio);
        var k=Cube("AccessKey",new Vector3(3,.5f,9),new Vector3(.5f,.2f,.9f),accentMat);var kc=k.AddComponent<CollectibleItemController>();Set(kc,"definition",key);Set(kc,"objectiveId",GameIds.Level02CollectAccessKey);
        var b=Cube("EmergencyBeacon",new Vector3(0,1.5f,13),new Vector3(1.3f,3,1.3f),dangerMat);var bc=b.AddComponent<EmergencyBeaconController>();Set(bc,"notificationUI",note);Set(bc,"statusRenderer",b.GetComponent<Renderer>());
        var z=Cube("SafeZone",new Vector3(0,.08f,19),new Vector3(7,.16f,5),safeMat);z.GetComponent<BoxCollider>().isTrigger=true;var sc=z.AddComponent<SafeZoneController>();Set(sc,"objectivesManager",om);Set(sc,"completionUI",completion);TextWorld("ZONA SEGURA",new Vector3(0,2.5f,19),Color.green);
    }

    private static void SetObjectives(ObjectivesManager manager,bool l2)
    {
        var list=l2?new[]{new Objective(GameIds.Level02CollectRadio,"Encuentra la radio de emergencia."),new Objective(GameIds.Level02CollectAccessKey,"Encuentra la llave de acceso."),new Objective(GameIds.Level02ActivateBeacon,"Activa la baliza de emergencia."),new Objective(GameIds.Level02ReachSafeZone,"Llega a la zona segura.")}:new[]{new Objective(GameIds.Level01ExitRoom,"Sal de la habitación."),new Objective(GameIds.Level01CollectBackpack,"Encuentra la mochila de emergencia."),new Objective(GameIds.Level01ActivateEvacuation,"Activa la salida de evacuación."),new Objective(GameIds.Level01ReachExit,"Dirígete al punto de salida.")};
        var so=new SerializedObject(manager);var p=so.FindProperty("objectives");p.arraySize=list.Length;for(int i=0;i<list.Length;i++){var e=p.GetArrayElementAtIndex(i);e.FindPropertyRelative("id").stringValue=list[i].Id;e.FindPropertyRelative("description").stringValue=list[i].Description;}so.ApplyModifiedPropertiesWithoutUndo();
    }
    private static void WirePauseButtons(GameObject p,PauseController c){Wire(p,"ResumeButton",c.Resume);Wire(p,"RestartButton",c.Reload);Wire(p,"MenuButton",c.MainMenu);}
    private static void WireCompletionButtons(GameObject p,GameCompletionController c){Wire(p,"RestartButton",c.RestartCurrentLevel);Wire(p,"MenuButton",c.ReturnToMainMenu);Wire(p,"ExitButton",c.QuitGame);}
    private static void Wire(GameObject root,string name,UnityEngine.Events.UnityAction action){var b=Find(root.transform,name).GetComponent<Button>();b.onClick.RemoveAllListeners();UnityEventTools.AddPersistentListener(b.onClick,action);}

    private static void NewScene(){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);}
    private static void SaveScene(string path){EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),path);}
    private static void EventSystem(){var e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<InputSystemUIInputModule>();}
    private static Canvas CanvasRoot(string name){var g=new GameObject(name);var c=g.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var s=g.AddComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);g.AddComponent<GraphicRaycaster>();return c;}
    private static GameObject Rect(string name,Transform parent,Vector2 min,Vector2 max){var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;return g;}
    private static GameObject Panel(string name,Transform parent,Vector2 anchor,Vector2 size){var g=ImageObj(name,parent,anchor,size,new Color(0.02f,.06f,.1f,.88f));return g;}
    private static GameObject FullPanel(string name,Transform parent,string title){var g=ImageObj(name,parent,new Vector2(.5f,.5f),new Vector2(1920,1080),new Color(0.01f,.025f,.05f,.94f));Text("Title",g.transform,title,48,new Vector2(.5f,.76f),new Vector2(1000,90),Color.white,FontStyles.Bold);return g;}
    private static GameObject ImageObj(string name,Transform parent,Vector2 anchor,Vector2 size,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;r.anchorMin=r.anchorMax=anchor;r.sizeDelta=size;r.anchoredPosition=Vector2.zero;g.GetComponent<Image>().color=color;return g;}
    private static TextMeshProUGUI Text(string name,Transform parent,string value,float size,Vector2 anchor,Vector2 dims,Color color,FontStyles style=FontStyles.Normal){var g=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;r.anchorMin=r.anchorMax=anchor;r.sizeDelta=dims;r.anchoredPosition=Vector2.zero;var t=g.GetComponent<TextMeshProUGUI>();t.text=value;t.fontSize=size;t.color=color;t.alignment=TextAlignmentOptions.Center;t.fontStyle=style;return t;}
    private static Button ButtonObj(string name,Transform parent,string label,Vector2 anchor,UnityEngine.Events.UnityAction action){var g=ImageObj(name,parent,anchor,new Vector2(420,68),new Color(.04f,.34f,.44f,.96f));var b=g.AddComponent<Button>();var t=Text("Label",g.transform,label,20,new Vector2(.5f,.5f),new Vector2(400,60),Color.white,FontStyles.Bold);if(action!=null)UnityEventTools.AddPersistentListener(b.onClick,action);return b;}
    private static GameObject Cube(string name,Vector3 pos,Vector3 scale,Material mat){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=mat;return g;}
    private static void TextWorld(string text,Vector3 pos,Color color){var g=new GameObject(text);g.transform.position=pos;g.transform.rotation=Quaternion.Euler(0,180,0);var t=g.AddComponent<TextMeshPro>();t.text=text;t.fontSize=3;t.alignment=TextAlignmentOptions.Center;t.color=color;}
    private static Transform Find(Transform root,string name){foreach(Transform t in root.GetComponentsInChildren<Transform>(true))if(t.name==name)return t;throw new Exception($"Missing child {name}");}
    private static void Set(UnityEngine.Object target,string property,object value){var so=new SerializedObject(target);var p=so.FindProperty(property);if(p==null)throw new Exception($"{target.GetType().Name}.{property} not serialized");switch(value){case UnityEngine.Object o:p.objectReferenceValue=o;break;case string s:p.stringValue=s;break;case bool b:p.boolValue=b;break;case Behaviour[] a:p.arraySize=a.Length;for(int i=0;i<a.Length;i++)p.GetArrayElementAtIndex(i).objectReferenceValue=a[i];break;default:throw new Exception($"Unsupported serialized value {value?.GetType()}");}so.ApplyModifiedPropertiesWithoutUndo();}
}
