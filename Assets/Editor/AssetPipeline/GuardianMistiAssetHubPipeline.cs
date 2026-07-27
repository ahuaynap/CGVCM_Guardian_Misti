#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class GuardianMistiAssetHubPipeline
{
    public const string ModelRoot = "Assets/Art/Models";
    public const string GeneratedRoot = "Assets/Art/Generated/AssetHub";
    public const string MaterialRoot = "Assets/Art/Materials/AssetHub/Generated";
    public const string PrefabRoot = "Assets/Art/Prefabs";

    public enum Semantic { Backpack, Tent, Tower, Door, Desk, Generator, Crate }

    private sealed class Definition
    {
        public Semantic Semantic;
        public string Token;
        public string Prefab;
        public Vector3 Size;
        public Color Color;
        public Definition(Semantic semantic, string token, string prefab, Vector3 size, Color color)
        { Semantic=semantic;Token=token;Prefab=prefab;Size=size;Color=color; }
    }

    private static readonly Definition[] Definitions =
    {
        new(Semantic.Backpack,"medical_backpack","Equipment/EmergencyBackpack_Visual.prefab",new Vector3(.48f,.68f,.28f),new Color(.72f,.055f,.045f)),
        new(Semantic.Tent,"green_medical_tent","Environment/MedicalTent.prefab",new Vector3(5.8f,3.1f,7f),new Color(.12f,.42f,.18f)),
        new(Semantic.Tower,"communications_tower","Infrastructure/CommunicationsTower.prefab",new Vector3(2.2f,10f,2.2f),new Color(.36f,.42f,.46f)),
        new(Semantic.Door,"sliding_hangar_door","Infrastructure/FacilityDoor.prefab",new Vector3(4.4f,3.4f,.5f),new Color(.16f,.25f,.29f)),
        new(Semantic.Desk,"command_desk","Furniture/CommandDesk.prefab",new Vector3(3.1f,1.55f,1.65f),new Color(.10f,.12f,.14f)),
        new(Semantic.Generator,"portable_generator","Equipment/PortableGenerator.prefab",new Vector3(1.45f,1.15f,.85f),new Color(.92f,.28f,.035f)),
        new(Semantic.Crate,"medical_crate","Equipment/MedicalCrate.prefab",new Vector3(.9f,.65f,.65f),new Color(.38f,.42f,.43f))
    };

    [MenuItem("Guardian Misti/AssetHub/Generate Validated Batch")]
    public static void GenerateValidatedBatch()
    {
        EnsureFolders();
        foreach (Definition definition in Definitions)
        {
            string source=ResolveSource(definition.Semantic);
            if(string.IsNullOrEmpty(source)){Debug.LogError("[AssetHub] Missing source for "+definition.Semantic);continue;}
            ConfigureImporter(source,definition.Semantic==Semantic.Door);
            CreateMaterial(definition);
            CreatePrefab(definition,source);
            Debug.Log("[AssetHub] Generated "+definition.Semantic+" from "+source);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static string ResolveSource(Semantic semantic)
    {
        Definition d=Definitions.First(x=>x.Semantic==semantic);
        return AssetDatabase.FindAssets("t:Model",new[]{ModelRoot})
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault(p=>Normalize(Path.GetFileNameWithoutExtension(p)).Contains(d.Token));
    }

    public static string GetPrefabPath(Semantic semantic) =>
        PrefabRoot+"/"+Definitions.First(x=>x.Semantic==semantic).Prefab;

    public static bool IsDoorSuitable()
    {
        string path=ResolveSource(Semantic.Door);
        GameObject model=AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if(model==null)return false;
        string[] names=model.GetComponentsInChildren<Transform>(true).Select(t=>t.name.ToLowerInvariant()).ToArray();
        return names.Any(n=>n.Contains("leaf")||n.Contains("panel")||n.Contains("door")) &&
               names.Any(n=>n.Contains("frame")||n.Contains("rail"));
    }

    public static void IntegrateLevel01()
    {
        Scene scene=EditorSceneManager.OpenScene("Assets/Scenes/Level01.unity");
        ClearNamedGroups("Generated_AssetHub_Level01");
        GameObject generated=EnsureGroup("GeneratedEnvironment/GameplayObjects/Generated_AssetHub_Level01");
        GameObject zone=FindObject("Generated_InteriorProtectionZone");
        if(zone!=null)AttachReplacingVisual(zone,Semantic.Desk,"AssetHub_CommandDesk",true);
        GameObject backpack=FindObject("EmergencyBackpack");
        if(backpack!=null)AttachReplacingVisual(backpack,Semantic.Backpack,"AssetHub_EmergencyBackpack",false);
        Place(generated.transform,Semantic.Crate,"AssetHub_MedicalCrate_L01",new Vector3(3.25f,.02f,8.4f),Quaternion.Euler(0,-15,0));
        ConfigureProtectionLighting(zone);
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
    }

    public static void IntegrateLevel02()
    {
        Scene scene=EditorSceneManager.OpenScene("Assets/Scenes/Level02.unity");
        ClearNamedGroups("Generated_AssetHub_Level02");
        GameObject group=EnsureGroup("GeneratedEnvironment/Landmarks/Generated_AssetHub_Level02");
        Place(group.transform,Semantic.Tower,"AssetHub_CommunicationsTower",new Vector3(-8f,0,13f),Quaternion.identity);
        Place(group.transform,Semantic.Generator,"AssetHub_PortableGenerator",new Vector3(-5.7f,0,13.2f),Quaternion.Euler(0,25,0));
        for(int i=0;i<3;i++)Place(group.transform,Semantic.Crate,"AssetHub_MedicalCrate_L02_"+(i+1),new Vector3(5.8f+i*.95f,0,7.5f+i*.35f),Quaternion.Euler(0,-8+i*12,0));
        Place(group.transform,Semantic.Tent,"AssetHub_MedicalTent_L02",new Vector3(0,0,18.2f),Quaternion.Euler(0,180,0));
        Place(group.transform,Semantic.Desk,"AssetHub_CommandDesk_L02",new Vector3(-1.2f,0,19.5f),Quaternion.Euler(0,180,0));
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureImporter(string path,bool preserveHierarchy)
    {
        if(AssetImporter.GetAtPath(path) is not ModelImporter importer)return;
        importer.globalScale=1f;importer.useFileScale=true;importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.animationType=ModelImporterAnimationType.None;importer.addCollider=false;importer.meshCompression=ModelImporterMeshCompression.Off;importer.isReadable=false;importer.optimizeMeshPolygons=true;importer.optimizeMeshVertices=true;importer.preserveHierarchy=preserveHierarchy;
        importer.SaveAndReimport();
    }

    private static void CreateMaterial(Definition d)
    {
        string path=MaterialRoot+"/"+d.Semantic+".mat";Material material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
        material.shader=Shader.Find("Universal Render Pipeline/Lit");material.SetColor("_BaseColor",d.Color);material.SetFloat("_Smoothness",(d.Semantic==Semantic.Tower||d.Semantic==Semantic.Door)?.5f:.22f);
        if(d.Semantic==Semantic.Tent){string texture=AssetDatabase.FindAssets("Color",new[]{"Assets/Art/Textures/AssetHub/Green_Medical_Tent"}).Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();if(!string.IsNullOrEmpty(texture))material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(texture));}
        EditorUtility.SetDirty(material);
    }

    private static void CreatePrefab(Definition d,string source)
    {
        GameObject model=AssetDatabase.LoadAssetAtPath<GameObject>(source);if(model==null)return;
        var root=new GameObject(d.Semantic+"AssetRoot");var marker=root.AddComponent<AssetHubGeneratedMarker>();marker.Configure(d.Semantic.ToString());
        var visual=new GameObject("Visual");visual.transform.SetParent(root.transform,false);var instance=(GameObject)PrefabUtility.InstantiatePrefab(model);instance.name="ImportedModel";instance.transform.SetParent(visual.transform,false);
        Bounds bounds=RendererBounds(instance);float scale=Mathf.Min(d.Size.x/Mathf.Max(.001f,bounds.size.x),Mathf.Min(d.Size.y/Mathf.Max(.001f,bounds.size.y),d.Size.z/Mathf.Max(.001f,bounds.size.z)));instance.transform.localScale=Vector3.one*scale;
        bounds=RendererBounds(instance);instance.transform.position-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        Material material=AssetDatabase.LoadAssetAtPath<Material>(MaterialRoot+"/"+d.Semantic+".mat");foreach(Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))renderer.sharedMaterials=Enumerable.Repeat(material,renderer.sharedMaterials.Length).ToArray();
        var colliders=new GameObject("Colliders");colliders.transform.SetParent(root.transform,false);AddColliderStrategy(colliders.transform,d);
        PrefabUtility.SaveAsPrefabAsset(root,GetPrefabPath(d.Semantic));UnityEngine.Object.DestroyImmediate(root);
    }

    private static void AddColliderStrategy(Transform parent,Definition d)
    {
        void Box(string name,Vector3 center,Vector3 size){var go=new GameObject(name);go.transform.SetParent(parent,false);var c=go.AddComponent<BoxCollider>();c.center=center;c.size=size;}
        switch(d.Semantic)
        {
            case Semantic.Desk: Box("TabletopCollider",new Vector3(0,1.45f,0),new Vector3(3.1f,.2f,1.65f));Box("LeftSupportCollider",new Vector3(-1.32f,.69f,0),new Vector3(.34f,1.38f,1.42f));Box("RightSupportCollider",new Vector3(1.32f,.69f,0),new Vector3(.34f,1.38f,1.42f));Box("RearSupportCollider",new Vector3(0,.43f,.72f),new Vector3(2.3f,.86f,.12f));break;
            case Semantic.Tent: Box("LeftWall",new Vector3(-2.75f,1.45f,.4f),new Vector3(.18f,2.9f,6.2f));Box("RightWall",new Vector3(2.75f,1.45f,.4f),new Vector3(.18f,2.9f,6.2f));Box("RearWall",new Vector3(0,1.45f,3.35f),new Vector3(5.5f,2.9f,.18f));break;
            case Semantic.Door: Box("LeftFrame",new Vector3(-2.1f,1.7f,0),new Vector3(.2f,3.4f,.45f));Box("RightFrame",new Vector3(2.1f,1.7f,0),new Vector3(.2f,3.4f,.45f));Box("TopFrame",new Vector3(0,3.25f,0),new Vector3(4.2f,.25f,.45f));break;
            case Semantic.Tower: Box("TowerBase",new Vector3(0,5,0),new Vector3(1.3f,10,1.3f));break;
            default: Box(d.Semantic+"Body",d.Size*.5f,d.Size);break;
        }
    }

    private static void AttachReplacingVisual(GameObject gameplay,Semantic semantic,string name,bool disableOldColliders)
    {
        AssetHubGeneratedMarker prior=gameplay.GetComponentsInChildren<AssetHubGeneratedMarker>(true).FirstOrDefault(m=>m.SemanticId==semantic.ToString());if(prior!=null)return;
        foreach(Renderer renderer in gameplay.GetComponentsInChildren<Renderer>(true))renderer.enabled=false;
        if(disableOldColliders)foreach(Collider collider in gameplay.GetComponentsInChildren<Collider>(true))if(!collider.isTrigger)collider.enabled=false;
        Transform parent=disableOldColliders?(gameplay.GetComponentInChildren<EarthquakeProtectionTrigger>(true)?.transform??gameplay.transform):gameplay.transform;
        GameObject visual=Place(parent,semantic,name,gameplay.transform.position,gameplay.transform.rotation);
        if(!disableOldColliders)foreach(Collider collider in visual.GetComponentsInChildren<Collider>(true))collider.enabled=false;
    }

    private static GameObject Place(Transform parent,Semantic semantic,string name,Vector3 position,Quaternion rotation)
    {
        Transform existing=parent.GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name==name);if(existing!=null)return existing.gameObject;
        GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(GetPrefabPath(semantic));var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab);go.name=name;go.transform.SetParent(parent,false);go.transform.position=position;go.transform.rotation=rotation;return go;
    }

    private static GameObject EnsureGroup(string path)
    {
        Transform current=null;foreach(string part in path.Split('/')){Transform next=current==null?SceneManager.GetActiveScene().GetRootGameObjects().Select(g=>g.transform).FirstOrDefault(t=>t.name==part):current.Find(part);if(next==null){var go=new GameObject(part);if(current!=null)go.transform.SetParent(current,false);next=go.transform;}current=next;}return current.gameObject;
    }

    private static void ClearNamedGroups(string name){foreach(Transform transform in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Where(t=>t.name==name).ToArray())UnityEngine.Object.DestroyImmediate(transform.gameObject);}
    private static void ConfigureProtectionLighting(GameObject zone)
    {
        if(zone==null)return;Transform old=zone.GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name=="AssetHub_ProtectionLighting");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
        var root=new GameObject("AssetHub_ProtectionLighting");root.transform.SetParent(zone.transform,false);
        Light cyan=CreateLight(root.transform,"CyanSafetyFill",new Vector3(0,1.65f,-.15f),LightType.Point,new Color(.18f,.82f,.92f),.52f,3.2f);cyan.shadows=LightShadows.None;
        Light white=CreateLight(root.transform,"SoftApproachLight",new Vector3(0,2.2f,-1.4f),LightType.Spot,new Color(.86f,.94f,1f),.72f,4.2f);white.spotAngle=58f;white.transform.rotation=Quaternion.Euler(52f,0,0);white.shadows=LightShadows.None;
        GameplayDiagnostics diagnostics=UnityEngine.Object.FindFirstObjectByType<GameplayDiagnostics>();if(diagnostics!=null){SerializedObject so=new SerializedObject(diagnostics);so.FindProperty("protectionDesk").objectReferenceValue=zone.GetComponentsInChildren<AssetHubGeneratedMarker>(true).FirstOrDefault(m=>m.SemanticId==Semantic.Desk.ToString())?.transform;so.FindProperty("protectionTrigger").objectReferenceValue=zone.GetComponentInChildren<EarthquakeProtectionTrigger>(true);so.ApplyModifiedPropertiesWithoutUndo();}
    }
    private static Light CreateLight(Transform parent,string name,Vector3 localPosition,LightType type,Color color,float intensity,float range){var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=localPosition;var light=go.AddComponent<Light>();light.type=type;light.color=color;light.intensity=intensity;light.range=range;light.renderMode=LightRenderMode.Auto;return light;}
    private static GameObject FindObject(string name)=>UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).FirstOrDefault(t=>t.name==name)?.gameObject;
    private static Bounds RendererBounds(GameObject go){Renderer[] renderers=go.GetComponentsInChildren<Renderer>(true);Bounds b=renderers.Length==0?new Bounds(Vector3.zero,Vector3.one):renderers[0].bounds;for(int i=1;i<renderers.Length;i++)b.Encapsulate(renderers[i].bounds);return b;}
    private static string Normalize(string value)=>value.ToLowerInvariant().Replace("-","_").Replace(" ","_");
    private static Material CreateSimpleMaterial(string name,Color color){string path=MaterialRoot+"/"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}material.color=color;return material;}
    private static void SetObject(UnityEngine.Object target,string property,UnityEngine.Object value){var so=new SerializedObject(target);so.FindProperty(property).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
    private static void EnsureFolders(){foreach(string path in new[]{GeneratedRoot,MaterialRoot,PrefabRoot+"/Equipment",PrefabRoot+"/Furniture",PrefabRoot+"/Infrastructure",PrefabRoot+"/Environment",PrefabRoot+"/Gameplay"})EnsureFolder(path);}
    private static void EnsureFolder(string path){if(AssetDatabase.IsValidFolder(path))return;string parent=Path.GetDirectoryName(path).Replace('\\','/');EnsureFolder(parent);AssetDatabase.CreateFolder(parent,Path.GetFileName(path));}
}
#endif
