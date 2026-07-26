#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GuardianMistiAssetHubValidator
{
    public static IReadOnlyList<string> Validate()
    {
        var errors=new List<string>();
        foreach(GuardianMistiAssetHubPipeline.Semantic semantic in Enum.GetValues(typeof(GuardianMistiAssetHubPipeline.Semantic)))
        {
            string source=GuardianMistiAssetHubPipeline.ResolveSource(semantic);
            if(string.IsNullOrEmpty(source))errors.Add("AssetHub: source model missing for "+semantic);
            string path=GuardianMistiAssetHubPipeline.GetPrefabPath(semantic);
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(prefab==null){errors.Add("AssetHub: generated prefab missing: "+path);continue;}
            if(prefab.transform.localPosition!=Vector3.zero||prefab.transform.localRotation!=Quaternion.identity||prefab.transform.localScale!=Vector3.one)errors.Add(path+": root transform must be identity.");
            if(prefab.GetComponentsInChildren<Camera>(true).Length>0||prefab.GetComponentsInChildren<Light>(true).Length>0)errors.Add(path+": imported camera or light is not allowed.");
            if(prefab.GetComponentsInChildren<Collider>(true).Length==0)errors.Add(path+": optimized primitive colliders missing.");
            if(prefab.GetComponentsInChildren<MeshCollider>(true).Any())errors.Add(path+": MeshCollider is not permitted in the validated batch.");
            if(prefab.GetComponentsInChildren<Renderer>(true).Any(r=>r.sharedMaterials.Any(m=>m==null||m.shader==null||!m.shader.name.Contains("Universal Render Pipeline"))))errors.Add(path+": renderer requires valid URP materials.");
        }
        ValidateScene(errors,"Assets/Scenes/Level01.unity",new[]{"AssetHub_CommandDesk","AssetHub_EmergencyBackpack","AssetHub_MedicalCrate_L01"});
        ValidateScene(errors,"Assets/Scenes/Level02.unity",new[]{"AssetHub_CommunicationsTower","AssetHub_PortableGenerator","AssetHub_MedicalCrate_L02_1"});
        ValidateScene(errors,"Assets/Scenes/Level03.unity",new[]{"AssetHub_MedicalTent","AssetHub_CommandDesk_L03","AssetHub_PortableGenerator_L03","AssetHub_MedicalCrate_L03_1"});
        return errors;
    }

    private static void ValidateScene(List<string> errors,string path,string[] required)
    {
        Scene scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);
        Transform[] all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
        foreach(string name in required)if(!all.Any(t=>t.name==name))errors.Add(path+": AssetHub placement missing: "+name);
        if(all.GroupBy(t=>t.name).Any(g=>required.Contains(g.Key)&&g.Count()>1))errors.Add(path+": duplicate AssetHub placement detected.");
        if(path.EndsWith("Level01.unity")&&all.SelectMany(t=>t.GetComponents<CollectibleItemController>()).Count()!=1)errors.Add(path+": backpack must retain exactly one collectible controller.");
        if(path.EndsWith("Level03.unity")&&!all.Any(t=>t.GetComponent<Level03CampCompletion>()!=null))errors.Add(path+": final camp completion trigger missing.");
    }
}
#endif
