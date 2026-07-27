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
        if(path.EndsWith("Level02.unity")){ValidateLevel02Aftershock(errors,path,all);ValidateLevel02Route(errors,path,all);}
        if(path.EndsWith("Level01.unity")){Transform desk=all.FirstOrDefault(t=>t.name=="AssetHub_CommandDesk");if(desk!=null){Collider[] physical=desk.GetComponentsInChildren<Collider>(true);if(physical.Any(c=>c.isTrigger||!c.enabled))errors.Add(path+": AssetHub_CommandDesk physical colliders must be enabled solids.");var player=all.FirstOrDefault(t=>t.CompareTag("Player"));if(player!=null&&Physics.GetIgnoreLayerCollision(desk.gameObject.layer,player.gameObject.layer))errors.Add(path+": desk layer does not collide with Player layer.");}Light[] lights=all.Where(t=>t.name=="AssetHub_ProtectionLighting").SelectMany(t=>t.GetComponentsInChildren<Light>(true)).ToArray();if(lights.Length!=2)errors.Add(path+": protection safe-zone requires exactly two restrained local lights.");if(lights.Any(l=>l.intensity<=0f||l.intensity>1f||l.range<2f||l.range>5f||l.transform.position.y<0f))errors.Add(path+": protection lighting values are unsafe.");}
    }
    private static void ValidateLevel02Aftershock(List<string> errors,string path,Transform[] all){var controllers=all.SelectMany(t=>t.GetComponents<AftershockController>()).ToArray();if(controllers.Length!=1){errors.Add(path+": expected exactly one active AftershockController, found "+controllers.Length);return;}var source=controllers[0].RumbleSource;if(source==null)errors.Add(path+": AftershockController RumbleSource is neither assigned nor auto-creatable.");else{if(source.playOnAwake)errors.Add(path+": "+Path(source.transform)+" must not play on awake.");if(!source.loop)errors.Add(path+": "+Path(source.transform)+" continuous rumble source must loop.");}if(all.SelectMany(t=>t.GetComponents<AudioSource>()).Count(s=>s.name=="RumbleSource")!=1)errors.Add(path+": expected exactly one dedicated RumbleSource AudioSource.");}
    public static List<string> FindLevel02RouteBlockers(Transform[] all){var errors=new List<string>();string[] names={"Level02Start","EvacuationRouteCheckpoint01","CommunicationsArea","AftershockSection","MedicalTentApproach","RouteSafeZone"};var points=names.Select(n=>all.FirstOrDefault(t=>t.name==n)).ToArray();if(points.Any(p=>p==null)){errors.Add("Level02 route checkpoints are incomplete.");return errors;}for(int segment=0;segment<points.Length-1;segment++){float distance=Vector3.Distance(points[segment].position,points[segment+1].position);int samples=Mathf.Max(2,Mathf.CeilToInt(distance/.3f));for(int i=0;i<=samples;i++){Vector3 p=Vector3.Lerp(points[segment].position,points[segment+1].position,i/(float)samples);foreach(var c in Physics.OverlapCapsule(p+Vector3.up*.38f,p+Vector3.up*1.62f,.38f,~0,QueryTriggerInteraction.Ignore)){if(c.bounds.size.y<.15f||c.GetComponent<CharacterController>()!=null||c.GetComponentInParent<AftershockRiskZone>()!=null)continue;string message="segment "+names[segment]+" -> "+names[segment+1]+" blocked by "+Path(c.transform)+" ["+c.GetType().Name+", layer "+c.gameObject.layer+", bounds "+c.bounds+"]";if(!errors.Contains(message))errors.Add(message);}}}return errors;}
    [MenuItem("Guardian Misti/Validation/Validate Level02 Route Clearance")] public static void ValidateLevel02RouteMenu(){var scene=EditorSceneManager.OpenScene("Assets/Scenes/Level02.unity",OpenSceneMode.Single);var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();var errors=FindLevel02RouteBlockers(all);if(errors.Count==0)Debug.Log("[Level02 Route] Clearance validation passed.");else foreach(string error in errors)Debug.LogError("[Level02 Route] "+error);}
    private static void ValidateLevel02Route(List<string> errors,string path,Transform[] all){foreach(string error in FindLevel02RouteBlockers(all))errors.Add(path+": "+error);var safe=all.SelectMany(t=>t.GetComponents<SafeZoneController>()).SingleOrDefault();var safeCollider=safe==null?null:safe.GetComponent<Collider>();if(safeCollider==null||!safeCollider.isTrigger)errors.Add(path+": final SafeZone collider must be an active trigger.");var tent=all.FirstOrDefault(t=>t.name=="AssetHub_MedicalTent_L02");var rear=tent==null?null:tent.GetComponentsInChildren<BoxCollider>(true).FirstOrDefault(c=>c.name=="RearWall");if(tent==null||rear==null)errors.Add(path+": MedicalTent wall collider structure is missing.");else if(rear.bounds.center.z<tent.position.z)errors.Add(path+": "+Path(rear.transform)+" seals the approach; rotate tent entrance toward Level02Start.");}
    private static string Path(Transform t){var parts=new List<string>();while(t!=null){parts.Add(t.name);t=t.parent;}parts.Reverse();return string.Join("/",parts);}
    private static void ValidateDeskPrefab(List<string> errors,GameObject prefab,string path){BoxCollider[] boxes=prefab.GetComponentsInChildren<BoxCollider>(true);foreach(string name in new[]{"TabletopCollider","LeftSupportCollider","RightSupportCollider"})if(!boxes.Any(c=>c.name==name))errors.Add(path+": missing compound collider "+name);if(boxes.Length<3||boxes.Any(c=>c.isTrigger))errors.Add(path+": desk needs at least three solid primitive colliders.");if(boxes.Any(c=>c.name=="TabletopCollider"&&c.center.y-c.size.y*.5f<1.3f))errors.Add(path+": tabletop intrudes into crouched clearance.");if(boxes.Any(c=>c.bounds.Contains(new Vector3(0,.65f,-.35f))))errors.Add(path+": central crouch opening is enclosed.");}
}
#endif
