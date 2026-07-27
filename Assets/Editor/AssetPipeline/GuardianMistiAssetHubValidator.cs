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
            if(semantic==GuardianMistiAssetHubPipeline.Semantic.Desk)ValidateDeskPrefab(errors,prefab,path);
        }
        ValidateScene(errors,"Assets/Scenes/Level01.unity",new[]{"AssetHub_CommandDesk","AssetHub_EmergencyBackpack","AssetHub_MedicalCrate_L01"});
        ValidateScene(errors,"Assets/Scenes/Level02.unity",new[]{"AssetHub_CommunicationsTower","AssetHub_PortableGenerator","AssetHub_MedicalCrate_L02_1","AssetHub_MedicalTent_L02"});
        return errors;
    }

    private static void ValidateScene(List<string> errors,string path,string[] required)
    {
        Scene scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);
        Transform[] all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
        foreach(string name in required)if(!all.Any(t=>t.name==name))errors.Add(path+": AssetHub placement missing: "+name);
        if(all.GroupBy(t=>t.name).Any(g=>required.Contains(g.Key)&&g.Count()>1))errors.Add(path+": duplicate AssetHub placement detected.");
        if(path.EndsWith("Level01.unity")&&all.SelectMany(t=>t.GetComponents<CollectibleItemController>()).Count()!=1)errors.Add(path+": backpack must retain exactly one collectible controller.");
        if(path.EndsWith("Level01.unity")){Transform desk=all.FirstOrDefault(t=>t.name=="AssetHub_CommandDesk");if(desk!=null){Collider[] physical=desk.GetComponentsInChildren<Collider>(true);if(physical.Any(c=>c.isTrigger||!c.enabled))errors.Add(path+": AssetHub_CommandDesk physical colliders must be enabled solids.");var player=all.FirstOrDefault(t=>t.CompareTag("Player"));if(player!=null&&Physics.GetIgnoreLayerCollision(desk.gameObject.layer,player.gameObject.layer))errors.Add(path+": desk layer does not collide with Player layer.");}Light[] lights=all.Where(t=>t.name=="AssetHub_ProtectionLighting").SelectMany(t=>t.GetComponentsInChildren<Light>(true)).ToArray();if(lights.Length!=2)errors.Add(path+": protection safe-zone requires exactly two restrained local lights.");if(lights.Any(l=>l.intensity<=0f||l.intensity>1f||l.range<2f||l.range>5f||l.transform.position.y<0f))errors.Add(path+": protection lighting values are unsafe.");}
    }
    private static void ValidateDeskPrefab(List<string> errors,GameObject prefab,string path){BoxCollider[] boxes=prefab.GetComponentsInChildren<BoxCollider>(true);foreach(string name in new[]{"TabletopCollider","LeftSupportCollider","RightSupportCollider"})if(!boxes.Any(c=>c.name==name))errors.Add(path+": missing compound collider "+name);if(boxes.Length<3||boxes.Any(c=>c.isTrigger))errors.Add(path+": desk needs at least three solid primitive colliders.");if(boxes.Any(c=>c.name=="TabletopCollider"&&c.center.y-c.size.y*.5f<1.3f))errors.Add(path+": tabletop intrudes into crouched clearance.");if(boxes.Any(c=>c.bounds.Contains(new Vector3(0,.65f,-.35f))))errors.Add(path+": central crouch opening is enclosed.");}
}
#endif
