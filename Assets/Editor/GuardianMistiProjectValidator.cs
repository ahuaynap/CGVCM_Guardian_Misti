using System;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GuardianMistiProjectValidator
{
    private static readonly List<string> Errors=new();
    private static readonly string[] Scenes={"Assets/Scenes/MainMenu.unity","Assets/Scenes/Level01.unity","Assets/Scenes/Level02.unity"};
    private static readonly string[] Prefabs={"Assets/Prefabs/Player/GameplayPlayer.prefab","Assets/Prefabs/UI/GameplayHUD.prefab","Assets/Prefabs/Systems/GameplaySystems.prefab","Assets/Prefabs/Gameplay/InteractableDoor.prefab","Assets/Prefabs/Gameplay/CollectibleItem.prefab","Assets/Prefabs/Gameplay/LevelExit.prefab","Assets/Prefabs/Gameplay/SafeZone.prefab","Assets/Prefabs/Gameplay/EvacuationTerminal.prefab","Assets/Prefabs/Gameplay/EmergencyBeacon.prefab","Assets/Prefabs/Gameplay/EmergencyBackpack.prefab","Assets/Prefabs/Gameplay/EmergencyRadio.prefab","Assets/Prefabs/Gameplay/AccessKey.prefab","Assets/Prefabs/Environment/FacilityModule.prefab"};
    [MenuItem("Guardian Misti/Validate Project")]
    public static void ValidateProject()
    {
        Errors.Clear();Time.timeScale=1f;
        foreach(string p in Scenes.Concat(Prefabs))NeedAsset(p);
        foreach(string p in new[]{"Assets/ScriptableObjects/Items/EmergencyBackpack.asset","Assets/ScriptableObjects/Items/EmergencyRadio.asset","Assets/ScriptableObjects/Items/AccessKey.asset","Assets/Settings/GuardianMisti/MainMenuVolume.asset","Assets/Settings/GuardianMisti/Level01Volume.asset","Assets/Settings/GuardianMisti/Level02Volume.asset"})NeedAsset(p);
        var enabled=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();if(!enabled.SequenceEqual(Scenes))Error("Build scenes must be MainMenu, Level01, Level02 only and in order.");
        ValidateItem("EmergencyBackpack",GameIds.EmergencyBackpack);ValidateItem("EmergencyRadio",GameIds.EmergencyRadio);ValidateItem("AccessKey",GameIds.AccessKey);
        ValidateScene(Scenes[0],false,false);ValidateScene(Scenes[1],true,false);ValidateScene(Scenes[2],true,true);
        if(Errors.Count>0){foreach(string e in Errors)Debug.LogError("VALIDATION: "+e);throw new Exception($"Guardian Misti validation failed with {Errors.Count} actionable error(s).");}
        Debug.Log("GUARDIAN_MISTI_VALIDATION_SUCCESS");
    }
    private static void ValidateScene(string path,bool gameplay,bool final)
    {
        if(!System.IO.File.Exists(path))return;var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var roots=scene.GetRootGameObjects();var all=roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).Select(t=>t.gameObject).ToArray();
        if(roots.Count(r=>r.name=="Generated_GuardianMisti")!=1)Error(path+": exactly one Generated_GuardianMisti root is required.");
        Exactly<EventSystem>(all,path);Exactly<AudioListener>(all,path);Need<Canvas>(all,path);Need<SceneLoader>(all,path);
        var es=all.SelectMany(g=>g.GetComponents<InputSystemUIInputModule>()).ToArray();if(es.Length!=1)Error(path+": EventSystem must use exactly one InputSystemUIInputModule.");
        foreach(var go in all){if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go)>0)Error(path+": Missing Script on "+go.name);foreach(var r in go.GetComponents<Renderer>()){if(r.sharedMaterials.Any(m=>m==null))Error(path+": renderer has missing material on "+go.name);foreach(var m in r.sharedMaterials)if(m!=null&&(m.shader==null||m.shader.name.Contains("Error")))Error(path+": invalid/pink shader on "+go.name);}}
        ValidateSameSceneReferences(scene,all);
        var scaler=all.SelectMany(g=>g.GetComponents<CanvasScaler>()).FirstOrDefault();if(scaler==null||scaler.uiScaleMode!=CanvasScaler.ScaleMode.ScaleWithScreenSize)Error(path+": CanvasScaler must use Scale With Screen Size.");
        if(!gameplay){Need<MainMenuController>(all,path);RequireText(all,"GUARDIAN MISTI",path);RequireText(all,"INICIAR SIMULACIÓN",path);RequireText(all,"INSTRUCCIONES",path);RequireText(all,"SALIR",path);ValidateButtons(all,path,new[]{"PlayButton","InstructionsButton","ExitButton"});return;}
        Exactly<PlayerInput>(all,path);Exactly<StarterAssetsInputs>(all,path);Exactly<FirstPersonController>(all,path);Exactly<GameplayInputController>(all,path);Exactly<InteractionSystem>(all,path);Exactly<InteractionUIController>(all,path);Need<ObjectivesManager>(all,path);Need<InventoryManager>(all,path);Need<PauseController>(all,path);Need<GameplayCursorController>(all,path);
        var player=all.FirstOrDefault(g=>g.name=="GameplayPlayer");if(player==null||!player.CompareTag("Player"))Error(path+": GameplayPlayer missing or not tagged Player.");else ValidatePlayer(player,path);
        var input=all.SelectMany(g=>g.GetComponents<PlayerInput>()).SingleOrDefault();if(input==null||input.actions==null||input.defaultActionMap!="Player")Error(path+": PlayerInput actions/default Player map are not configured.");
        var interaction=all.SelectMany(g=>g.GetComponents<InteractionSystem>()).SingleOrDefault();RequireRef(interaction,"playerCamera",path);RequireRef(interaction,"interactionUIController",path);
        var ui=all.SelectMany(g=>g.GetComponents<InteractionUIController>()).SingleOrDefault();RequireRef(ui,"promptUI",path);RequireRef(ui,"crosshairUI",path);
        var state=all.SelectMany(g=>g.GetComponents<GameplayInputController>()).SingleOrDefault();foreach(string f in new[]{"playerInput","movement","starterInputs","interaction"})RequireRef(state,f,path);
        if(all.Count(g=>g.name=="GlobalVolume")!=1||all.SelectMany(g=>g.GetComponents<Volume>()).All(v=>!v.isGlobal||v.sharedProfile==null))Error(path+": configured project-owned Global Volume required.");
        if(all.SelectMany(g=>g.GetComponents<Light>()).Count()<2)Error(path+": directional plus local guidance lighting required.");
        foreach(var i in all.SelectMany(g=>g.GetComponents<MonoBehaviour>()).Where(m=>m is IInteractable)){if(i.GetComponent<Collider>()==null)Error(path+": interactable without collider: "+i.name);}
        var ids=all.SelectMany(g=>g.GetComponents<ObjectivesManager>()).First().Objectives.Select(o=>o.Id).ToArray();var expected=final?new[]{GameIds.Level02CollectRadio,GameIds.Level02CollectAccessKey,GameIds.Level02ActivateBeacon,GameIds.Level02ReachSafeZone}:new[]{GameIds.Level01Protect,GameIds.Level01ExitRoom,GameIds.Level01CollectBackpack,GameIds.Level01ActivateEvacuation,GameIds.Level01ReachExit};if(!ids.SequenceEqual(expected))Error(path+": objective IDs/order do not match required flow.");
        ValidateButtons(all,path,new[]{"ResumeButton","RestartButton","MenuButton"});RequireText(all,"SIMULACIÓN EN PAUSA",path);
        if(final){Need<EmergencyBeaconController>(all,path);var zone=all.SelectMany(g=>g.GetComponents<SafeZoneController>()).SingleOrDefault();if(zone==null||zone.GetComponent<Collider>()==null||!zone.GetComponent<Collider>().isTrigger)Error(path+": one-shot SafeZone trigger missing.");ValidateButtons(all,path,new[]{"ExitButton"});RequireText(all,"MISIÓN COMPLETADA",path);}else{var exit=all.SelectMany(g=>g.GetComponents<LevelExitController>()).SingleOrDefault();if(exit==null||exit.GetComponent<Collider>()==null||!exit.GetComponent<Collider>().isTrigger)Error(path+": LevelExit trigger missing.");}
        foreach(string visual in final?new[]{"EmergencyRadio","AccessKey","EmergencyBeacon","RescuePlatform","VolcanicRock"}:new[]{"InitialDoor","EmergencyBackpack","EvacuationTerminal","EmergencySupplyCrate","EmergencyLight"})if(!all.Any(g=>g.name==visual))Error(path+": composed visual missing: "+visual);
    }
    private static void ValidatePlayer(GameObject player,string path){var c=player.GetComponent<CharacterController>();if(c==null)Error(path+": CharacterController missing.");else{if(c.height<1.7f||c.radius<.3f||c.skinWidth<=0||c.stepOffset<=0)Error(path+": CharacterController dimensions are unsafe.");float bottom=player.transform.position.y+c.center.y-c.height*.5f;if(bottom<-.02f)Error(path+": player spawn intersects ground.");}if(player.GetComponents<PlayerInput>().Length!=1||player.GetComponents<StarterAssetsInputs>().Length!=1||player.GetComponents<FirstPersonController>().Length!=1)Error(path+": duplicate input/movement components on player.");}
    private static void ValidateSameSceneReferences(Scene scene,GameObject[] all){foreach(var b in all.SelectMany(g=>g.GetComponents<MonoBehaviour>()).Where(x=>x!=null)){var so=new SerializedObject(b);var p=so.GetIterator();while(p.NextVisible(true)){if(p.propertyType!=SerializedPropertyType.ObjectReference||p.objectReferenceValue==null)continue;if(p.objectReferenceValue is Component c&&c.gameObject.scene.IsValid()&&c.gameObject.scene!=scene)Error(scene.path+": cross-scene reference "+b.name+"."+p.propertyPath);if(p.objectReferenceValue is GameObject g&&g.scene.IsValid()&&g.scene!=scene)Error(scene.path+": cross-scene reference "+b.name+"."+p.propertyPath);}}}
    private static void ValidateButtons(GameObject[] all,string path,string[] names){foreach(string n in names){var buttons=all.Where(g=>g.name==n).SelectMany(g=>g.GetComponents<Button>()).ToArray();if(buttons.Length==0)Error(path+": missing button "+n);else if(buttons.All(b=>b.onClick.GetPersistentEventCount()==0))Error(path+": button not wired: "+n);}}
    private static void RequireText(GameObject[] all,string value,string path){if(!all.SelectMany(g=>g.GetComponents<TMPro.TMP_Text>()).Any(t=>t.text.Contains(value)))Error(path+": required Spanish/UI text missing: "+value);}
    private static void RequireRef(UnityEngine.Object target,string field,string path){if(target==null){Error(path+": missing target for reference "+field);return;}var p=new SerializedObject(target).FindProperty(field);if(p==null||p.objectReferenceValue==null)Error(path+": null serialized reference "+target.GetType().Name+"."+field);}
    private static void Need<T>(GameObject[] all,string path)where T:Component{if(!all.Any(g=>g.GetComponent<T>()!=null))Error(path+": missing "+typeof(T).Name);}
    private static void Exactly<T>(GameObject[] all,string path)where T:Component{int n=all.Sum(g=>g.GetComponents<T>().Length);if(n!=1)Error(path+$": expected exactly one {typeof(T).Name}, found {n}.");}
    private static void NeedAsset(string p){if(!System.IO.File.Exists(p)&&AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p)==null)Error("Missing required asset: "+p);}
    private static void ValidateItem(string file,string id){var a=AssetDatabase.LoadAssetAtPath<InventoryItemDefinition>($"Assets/ScriptableObjects/Items/{file}.asset");if(a!=null&&a.Id!=id)Error($"{file}: expected ID {id}, got {a.Id}.");}
    private static void Error(string e)=>Errors.Add(e);
}
