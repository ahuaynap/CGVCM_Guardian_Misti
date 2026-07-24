using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class GuardianMistiProjectValidator
{
    private static readonly List<string> Errors = new();
    private static readonly string[] Scenes={"Assets/Scenes/MainMenu.unity","Assets/Scenes/Level01.unity","Assets/Scenes/Level02.unity"};
    private static readonly string[] Prefabs={"Assets/Prefabs/Player/GameplayPlayer.prefab","Assets/Prefabs/UI/GameplayHUD.prefab","Assets/Prefabs/Systems/GameplaySystems.prefab","Assets/Prefabs/Gameplay/InteractableDoor.prefab","Assets/Prefabs/Gameplay/CollectibleItem.prefab","Assets/Prefabs/Gameplay/LevelExit.prefab","Assets/Prefabs/Gameplay/SafeZone.prefab"};
    [MenuItem("Guardian Misti/Validate Project")]
    public static void ValidateProject()
    {
        Errors.Clear();
        foreach(string p in Scenes) NeedAsset(p); foreach(string p in Prefabs) NeedAsset(p);
        foreach(string p in new[]{"Assets/ScriptableObjects/Items/EmergencyBackpack.asset","Assets/ScriptableObjects/Items/EmergencyRadio.asset","Assets/ScriptableObjects/Items/AccessKey.asset"}) NeedAsset(p);
        var enabled=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();
        if(!enabled.SequenceEqual(Scenes)) Errors.Add("Enabled build scenes must be MainMenu, Level01, Level02 in exact order.");
        ValidateItem("EmergencyBackpack",GameIds.EmergencyBackpack);ValidateItem("EmergencyRadio",GameIds.EmergencyRadio);ValidateItem("AccessKey",GameIds.AccessKey);
        if(System.IO.File.Exists(Scenes[0])) ValidateScene(Scenes[0],false,false);
        if(System.IO.File.Exists(Scenes[1])) ValidateScene(Scenes[1],true,false);
        if(System.IO.File.Exists(Scenes[2])) ValidateScene(Scenes[2],true,true);
        if(Errors.Count>0){foreach(string e in Errors)Debug.LogError("VALIDATION: "+e);throw new Exception($"Guardian Misti validation failed with {Errors.Count} error(s).");}
        Debug.Log("GUARDIAN_MISTI_VALIDATION_SUCCESS");
    }
    private static void NeedAsset(string p){if(!System.IO.File.Exists(p)&&AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p)==null)Errors.Add("Missing asset: "+p);}
    private static void ValidateItem(string file,string id){var a=AssetDatabase.LoadAssetAtPath<InventoryItemDefinition>($"Assets/ScriptableObjects/Items/{file}.asset");if(a!=null&&a.Id!=id)Errors.Add($"{file} ID is '{a.Id}', expected '{id}'.");}
    private static void ValidateScene(string path,bool gameplay,bool final)
    {
        var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var roots=scene.GetRootGameObjects();
        Need<Canvas>(roots,path);Need<SceneLoader>(roots,path);if(roots.SelectMany(r=>r.GetComponentsInChildren<EventSystem>(true)).Count()!=1)Errors.Add(path+" must contain exactly one EventSystem.");
        if(path.EndsWith("MainMenu.unity"))Need<MainMenuController>(roots,path);
        if(gameplay){Need<StarterAssets.FirstPersonController>(roots,path);Need<InteractionSystem>(roots,path);Need<ObjectivesManager>(roots,path);Need<InventoryManager>(roots,path);Need<GameplayCursorController>(roots,path);Need<PauseController>(roots,path);var player=roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).FirstOrDefault(t=>t.name=="GameplayPlayer");if(player==null||!player.CompareTag("Player"))Errors.Add(path+" player missing or not tagged Player.");}
        if(!final&&path.EndsWith("Level01.unity")){var x=roots.SelectMany(r=>r.GetComponentsInChildren<LevelExitController>(true)).FirstOrDefault();if(x==null||!x.GetComponent<Collider>().isTrigger)Errors.Add("Level01 exit trigger missing or invalid.");}
        if(final){var z=roots.SelectMany(r=>r.GetComponentsInChildren<SafeZoneController>(true)).FirstOrDefault();if(z==null||!z.GetComponent<Collider>().isTrigger)Errors.Add("Level02 safe zone trigger missing or invalid.");}
        foreach(var go in roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).Select(t=>t.gameObject)){if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go)>0)Errors.Add(path+" has Missing Script on "+go.name);var renderer=go.GetComponent<Renderer>();if(renderer!=null&&renderer.sharedMaterials.Any(m=>m==null))Errors.Add(path+" renderer has missing material: "+go.name);}
        var om=roots.SelectMany(r=>r.GetComponentsInChildren<ObjectivesManager>(true)).FirstOrDefault();if(om!=null&&om.Objectives.Select(o=>o.Id).Distinct().Count()!=om.Objectives.Count)Errors.Add(path+" objective IDs are not unique.");
    }
    private static void Need<T>(GameObject[] roots,string path)where T:Component{if(!roots.Any(r=>r.GetComponentInChildren<T>(true)!=null))Errors.Add(path+" missing "+typeof(T).Name);}
}
