using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GuardianMistiScreenshotCapture
{
    private const string Output="Artifacts/Screenshots";
    [MenuItem("Guardian Misti/Capture Review Screenshots")]
    public static void CaptureScreenshots()
    {
        Directory.CreateDirectory(Output);
        CaptureCanvas("Assets/Scenes/MainMenu.unity","MainMenu");
        CaptureScene("Assets/Scenes/Level01.unity",new[]{new Shot("Level01_Start",new Vector3(0,1.65f,.3f),new Vector3(0,1.3f,4)),new Shot("Level01_Backpack",new Vector3(0,1.8f,6),new Vector3(-2,.6f,8)),new Shot("Level01_Door_Furniture",new Vector3(-1,1.8f,.8f),new Vector3(-4.8f,1.1f,2.2f)),new Shot("Level01_Terminal",new Vector3(0,1.8f,10),new Vector3(2,1,12)),new Shot("Level01_Exit",new Vector3(0,1.8f,14),new Vector3(0,2,17))});
        CaptureEarthquakeTimeline("Assets/Scenes/Level01.unity");CaptureHudVariant("Assets/Scenes/Level01.unity","TimerText","01:23.450","HUD_Timer");
        CaptureScene("Assets/Scenes/Level02.unity",new[]{new Shot("Level02_Entry",new Vector3(0,1.7f,.5f),new Vector3(0,1,6)),new Shot("Level02_Radio",new Vector3(0,1.8f,3),new Vector3(-3,.7f,5)),new Shot("Level02_AccessKey",new Vector3(0,1.8f,7),new Vector3(3,.7f,9)),new Shot("Level02_Beacon",new Vector3(0,2,9),new Vector3(0,1.6f,13)),new Shot("Level02_SafeZone",new Vector3(0,2,15),new Vector3(0,1,19))});
        CapturePanel("Assets/Scenes/Level01.unity","PausePanel","PausePanel");CapturePanel("Assets/Scenes/Level02.unity","CompletionPanel","CompletionPanel");
        AssetDatabase.Refresh();Debug.Log("GUARDIAN_MISTI_SCREENSHOTS_SUCCESS: "+Path.GetFullPath(Output));
    }
    private static void CaptureCanvas(string path,string file){var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true));var cam=all.Select(t=>t.GetComponent<Camera>()).First(c=>c!=null);var canvas=all.Select(t=>t.GetComponent<Canvas>()).First(c=>c!=null);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=1f;Render(cam,file);}
    private static void CaptureScene(string path,Shot[] shots){var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var cam=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Camera>(true)).First();foreach(var shot in shots){cam.transform.position=shot.Position;cam.transform.LookAt(shot.Target);Render(cam,shot.Name);}}
    private static void CaptureEarthquakeTimeline(string path){var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true));var cam=all.Select(t=>t.GetComponent<Camera>()).First(c=>c!=null);var canvas=all.Select(t=>t.GetComponent<Canvas>()).First(c=>c!=null);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=1f;var quake=all.Select(t=>t.GetComponent<EarthquakeController>()).First(q=>q!=null);var ui=all.Select(t=>t.GetComponent<EarthquakeUIController>()).First(q=>q!=null);quake.BeginSequence();ui.PresentState(quake.State);ui.PresentCountdown("El simulacro comenzará en "+quake.DisplayedCountdown+"...");Render(cam,"Earthquake_Countdown_3");quake.Tick(1.01f);ui.PresentCountdown("El simulacro comenzará en "+quake.DisplayedCountdown+"...");Render(cam,"Earthquake_Countdown_2");quake.Tick(1f);ui.PresentCountdown("El simulacro comenzará en "+quake.DisplayedCountdown+"...");Render(cam,"Earthquake_Countdown_1");quake.Tick(1f);ui.PresentState(quake.State);ui.PresentCountdown("¡SISMO!");Render(cam,"Earthquake_SISMO");quake.Tick(.8f);ui.PresentCountdown(string.Empty);Render(cam,"Earthquake_Countdown_Hidden");quake.Tick(7f);ui.PresentState(quake.State);Render(cam,"Earthquake_Moderate");quake.Tick(7f);ui.PresentState(quake.State);Render(cam,"Earthquake_Strong");quake.Tick(7f);ui.PresentState(quake.State);Render(cam,"Earthquake_Decreasing");quake.Tick(20f);ui.PresentState(quake.State);Render(cam,"Earthquake_Finished");}
    private static void CaptureHudVariant(string path,string textName,string value,string file){var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true));var cam=all.Select(t=>t.GetComponent<Camera>()).First(c=>c!=null);var canvas=all.Select(t=>t.GetComponent<Canvas>()).First(c=>c!=null);var text=all.First(t=>t.name==textName).GetComponent<TMPro.TMP_Text>();text.text=value;text.gameObject.SetActive(true);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=.2f;Render(cam,file);}
    private static void CapturePanel(string path,string panelName,string file){var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true));var cam=all.Select(t=>t.GetComponent<Camera>()).First(c=>c!=null);var canvas=all.Select(t=>t.GetComponent<Canvas>()).First(c=>c!=null);var panel=all.First(t=>t.name==panelName).gameObject;panel.SetActive(true);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=.2f;Render(cam,file);}
    private static void Render(Camera cam,string name){Canvas.ForceUpdateCanvases();var rt=new RenderTexture(1920,1080,24,RenderTextureFormat.ARGB32);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var tex=new Texture2D(1920,1080,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1920,1080),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(Output,name+".png"),tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);}
    private readonly struct Shot{public readonly string Name;public readonly Vector3 Position,Target;public Shot(string n,Vector3 p,Vector3 t){Name=n;Position=p;Target=t;}}
}
