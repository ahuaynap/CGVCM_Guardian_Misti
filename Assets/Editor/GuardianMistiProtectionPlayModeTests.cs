#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class GuardianMistiProtectionPlayModeTests
{
    [UnityTest] public IEnumerator TriggerIsReachableNonBlockingAndMovementCameraRemainEnabled()
    {
        yield return new EnterPlayMode();
        var zone=new GameObject("ProtectionZone");var trigger=zone.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.size=new Vector3(2.42f,1.12f,1.45f);zone.AddComponent<EarthquakeProtectionTrigger>();
        var player=new GameObject("Player");player.tag="Player";var character=player.AddComponent<CharacterController>();character.radius=.38f;character.height=2f;var cameraObject=new GameObject("Camera");cameraObject.transform.SetParent(player.transform);var camera=cameraObject.AddComponent<Camera>();
        player.transform.position=new Vector3(0,0,-2f);Assert.True(System.Array.TrueForAll(zone.GetComponents<Collider>(),c=>c.isTrigger));character.enabled=false;player.transform.position=zone.transform.position;character.enabled=true;Physics.SyncTransforms();yield return new WaitForFixedUpdate();Assert.True(trigger.bounds.Contains(player.transform.position));Assert.True(trigger.isTrigger);Assert.True(camera.enabled);Object.Destroy(player);Object.Destroy(zone);yield return new ExitPlayMode();
    }

    [UnityTest] public IEnumerator CountdownEntryIsRejectedAndValidEntryRecordsOnce()
    {
        yield return new EnterPlayMode();
        var session=New<SimulationSession>("Session");Invoke(session,"Awake");var quake=Quake(3f,8f);Assert.True(quake.BeginSequence());Assert.False(quake.TryMarkProtectionEntered());Assert.False(session.ProtectionReached);quake.Tick(3.1f);Assert.True(quake.TryMarkProtectionEntered());float first=session.TimeToProtection;quake.MarkProtectionExited();quake.Tick(.5f);Assert.True(quake.TryMarkProtectionEntered());Assert.AreEqual(first,session.TimeToProtection);yield return null;Object.Destroy(quake.gameObject);Object.Destroy(session.gameObject);yield return new ExitPlayMode();
    }

    [UnityTest] public IEnumerator DwellAndEarlyExitPolicyCompletesOnlyAfterEarthquake()
    {
        yield return new EnterPlayMode();
        var manager=New<ObjectivesManager>("Objectives");Set(manager,"objectives",new List<Objective>{new(GameIds.Level01Protect,"Protégete durante el sismo."),new(GameIds.Level01ExitRoom,"Sal de la habitación.")});var quake=Quake(0f,1f);Set(quake,"objectivesManager",manager);quake.BeginSequence();quake.Tick(.01f);Assert.True(quake.TryMarkProtectionEntered());Assert.AreEqual(0f,EarthquakeProtectionTrigger.CalculateDwellProgress(1f,.1f,false,quake.State,2f));quake.MarkProtectionExited();Assert.True(manager.IsCurrentObjective(GameIds.Level01Protect));Assert.True(quake.TryMarkProtectionEntered());quake.MarkProtectionDwellSatisfied();quake.MarkProtectionExited();Assert.True(quake.ProtectionDwellSatisfied);Assert.True(manager.IsCurrentObjective(GameIds.Level01Protect));quake.Tick(2f);Assert.True(manager.IsCurrentObjective(GameIds.Level01ExitRoom));yield return null;Object.Destroy(quake.gameObject);Object.Destroy(manager.gameObject);yield return new ExitPlayMode();
    }

    [UnityTest] public IEnumerator BuiltLevel01HasSingleReachableProtectionZoneAndCleanGuidanceLifecycle()
    {
        yield return new EnterPlayMode();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Level01);yield return null;var zones=Object.FindObjectsByType<EarthquakeProtectionTrigger>(FindObjectsInactive.Include);Assert.AreEqual(1,zones.Length);var zone=zones[0];Assert.AreEqual(GameIds.Level01Protect,zone.ObjectiveId);Assert.True(zone.GetComponent<BoxCollider>().isTrigger);Assert.GreaterOrEqual(zone.GetComponent<BoxCollider>().size.x,.76f+.25f);var player=Object.FindFirstObjectByType<StarterAssets.FirstPersonController>();Assert.NotNull(player);Assert.True(player.enabled);Assert.True(player.GetComponentInChildren<Camera>().enabled);var transforms=Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);var prompt=System.Array.Find(transforms,t=>t.name=="ProtectionContextPrompt")?.gameObject;var hint=System.Array.Find(transforms,t=>t.name=="ProtectionObjectiveHint")?.gameObject;Assert.NotNull(prompt);Assert.NotNull(hint);Assert.False(prompt.activeSelf);Assert.False(hint.activeSelf);yield return new ExitPlayMode();
    }

    [UnityTest] public IEnumerator SuccessfulProtectionAdvancesAndCleansGuidance()
    {
        yield return new EnterPlayMode();UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Level01);yield return null;var q=Object.FindFirstObjectByType<EarthquakeController>();q.Tick(10f);Assert.True(q.TryMarkProtectionEntered());q.MarkProtectionDwellSatisfied();q.Tick(100f);yield return null;Assert.True(q.ProtectionSucceeded);Assert.True(q.ProtectionResolved);Assert.True(ObjectivesManager.Instance.IsCurrentObjective(GameIds.Level01ExitRoom));AssertGuidanceHidden();yield return new ExitPlayMode();
    }
    [UnityTest] public IEnumerator MissingProtectionRecordsFailureAndStillAdvances()
    {
        yield return new EnterPlayMode();UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Level01);yield return null;var q=Object.FindFirstObjectByType<EarthquakeController>();q.Tick(10f);q.Tick(100f);q.TickProtectionResolution(2f);yield return null;Assert.True(q.ProtectionFailed);Assert.True(SimulationSession.Instance.ProtectionFailed);Assert.True(ObjectivesManager.Instance.IsCurrentObjective(GameIds.Level01ExitRoom));var note=Object.FindFirstObjectByType<NotificationUI>(FindObjectsInactive.Include);Assert.True(note.gameObject.activeSelf);StringAssert.Contains("No alcanzaste",note.GetComponentInChildren<TMPro.TMP_Text>(true).text+string.Join(" ",System.Array.ConvertAll(note.GetComponentsInChildren<TMPro.TMP_Text>(true),t=>t.text)));AssertGuidanceHidden();yield return new ExitPlayMode();
    }
    [UnityTest] public IEnumerator EarlyExitFailsButDoorOpensAndBecomesPassable()
    {
        yield return new EnterPlayMode();UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Level01);yield return null;var q=Object.FindFirstObjectByType<EarthquakeController>();q.Tick(10f);Assert.True(q.TryMarkProtectionEntered());q.MarkProtectionExited();q.Tick(100f);q.TickProtectionResolution(2f);yield return null;Assert.True(q.ProtectionFailed);Assert.True(ObjectivesManager.Instance.IsCurrentObjective(GameIds.Level01ExitRoom));var door=Object.FindFirstObjectByType<DoorController>();Assert.True(door.CanOpen);door.Interact();yield return new WaitForSecondsRealtime(1.2f);Assert.AreEqual("Puerta abierta",door.Prompt);var blocker=door.GetComponent<Collider>();Assert.False(blocker.enabled);Assert.False(Physics.Linecast(new Vector3(0,1.2f,4.2f),new Vector3(0,1.2f,5.8f),~0,QueryTriggerInteraction.Ignore));AssertGuidanceHidden();yield return new ExitPlayMode();
    }
    [UnityTest] public IEnumerator Level01MouseLookIsDirectClampedAndCrouchIndependent()
    {
        yield return new EnterPlayMode();UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Level01);yield return null;var look=Object.FindFirstObjectByType<PlayerLookController>();Assert.NotNull(look);Assert.AreEqual(1,PlayerLookController.CountActiveLookControllers(look.gameObject));float yaw=look.Yaw;look.ApplyLookDelta(new Vector2(10f,5f),true,.001f);Assert.Greater(Mathf.Abs(Mathf.DeltaAngle(yaw,look.Yaw)),10f);Assert.AreEqual(-7.5f,look.Pitch,.1f);look.ApplyLookDelta(new Vector2(0f,1000f),true,.001f);Assert.That(look.Pitch,Is.InRange(look.MinimumPitch,look.MaximumPitch));var crouch=look.GetComponent<PlayerCrouchController>();float sensitivity=look.MouseSensitivity;crouch.BeginCrouch();Assert.True(crouch.IsCrouching);Assert.AreEqual(sensitivity,look.MouseSensitivity);yield return new ExitPlayMode();
    }
    [UnityTest] public IEnumerator GameplayScenesUseSeparatedSingleLookHierarchy()
    {
        yield return new EnterPlayMode();foreach(string scene in new[]{SceneNames.Level01,SceneNames.Level02}){UnityEngine.SceneManagement.SceneManager.LoadScene(scene);yield return null;var look=Object.FindFirstObjectByType<PlayerLookController>();Assert.NotNull(look,scene);Assert.AreEqual(1,PlayerLookController.CountActiveLookControllers(look.gameObject),scene);Assert.AreEqual(look.CameraHeightPivot,look.CameraLookPivot.parent);Assert.AreEqual(look.CameraLookPivot,look.CameraShakePivot.parent);Assert.AreEqual(look.CameraShakePivot,look.MainCamera.transform.parent);}yield return new ExitPlayMode();
    }
    private static void AssertGuidanceHidden(){var all=Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);foreach(string n in new[]{"ProtectionObjectiveHint","ProtectionContextPrompt","ProtectionWorldIndicator"}){var go=System.Array.Find(all,t=>t.name==n)?.gameObject;Assert.NotNull(go);Assert.False(go.activeSelf,n+" remained active");}}
    private static T New<T>(string name) where T:Component=>new GameObject(name).AddComponent<T>();
    private static EarthquakeController Quake(float countdown,float duration){var q=New<EarthquakeController>("Quake");var p=ScriptableObject.CreateInstance<EarthquakeProfile>();Set(p,"<PreparationCountdown>k__BackingField",countdown);Set(p,"<Duration>k__BackingField",duration);Set(q,"profile",p);return q;}
    private static void Set(object target,string field,object value)=>target.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);
    private static void Invoke(object target,string method)=>target.GetType().GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,null);
}
#endif
