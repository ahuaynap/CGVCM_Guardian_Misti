#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GuardianMistiEditModeTests
{
    private readonly List<Object> cleanup=new();
    [TearDown] public void TearDown(){Time.timeScale=1;foreach(var o in cleanup)if(o!=null)Object.DestroyImmediate(o);cleanup.Clear();}
    private InventoryItemDefinition Item(string id){var d=ScriptableObject.CreateInstance<InventoryItemDefinition>();cleanup.Add(d);Set(d,"<Id>k__BackingField",id);Set(d,"<Name>k__BackingField",id);return d;}
    private T Component<T>()where T:Component{var g=new GameObject(typeof(T).Name);cleanup.Add(g);return g.AddComponent<T>();}
    private static void Set(object o,string f,object v)=>o.GetType().GetField(f,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(o,v);
    [Test] public void DestroyedUnityObjectUsesUnityNullSemantics(){var g=new GameObject("Gone");Object.DestroyImmediate(g);Assert.True(g==null);Assert.False(ReferenceEquals(g,null));}
    [Test] public void InteractionUiToleratesDestroyedDependenciesAndRepeatedHide(){var ui=Component<InteractionUIController>();var prompt=Component<InteractionPromptUI>();var cross=Component<CrosshairUI>();Set(ui,"promptUI",prompt);Set(ui,"crosshairUI",cross);Object.DestroyImmediate(prompt.gameObject);Assert.DoesNotThrow(ui.Hide);Assert.DoesNotThrow(ui.Hide);}
    [Test] public void PromptHideIsIdempotent(){var g=new GameObject("Prompt");cleanup.Add(g);var text=g.AddComponent<TextMeshProUGUI>();var prompt=g.AddComponent<InteractionPromptUI>();Set(prompt,"promptText",text);prompt.Show("Abrir");Assert.DoesNotThrow(prompt.Hide);Assert.DoesNotThrow(prompt.Hide);Assert.False(g.activeSelf);}
    [Test] public void InventoryRejectsNullAndDuplicatesAndHasItem(){var m=Component<InventoryManager>();Assert.False(m.AddItem(null));var d=Item("one");Assert.True(m.AddItem(new InventoryItem(d)));Assert.False(m.AddItem(new InventoryItem(d)));Assert.True(m.HasItem("one"));Assert.False(m.HasItem(null));}
    [Test] public void ObjectivesProgressOnlyInOrderAndStopAfterCompletion(){var m=Component<ObjectivesManager>();Set(m,"objectives",new List<Objective>{new("a","A"),new("b","B")});Assert.False(m.TryCompleteObjective("b"));Assert.True(m.TryCompleteObjective("a"));Assert.True(m.TryCompleteObjective("b"));Assert.False(m.TryCompleteObjective("b"));}
    [Test] public void GameplayInputPauseResumeAndCompletionAreCentralized(){var g=new GameObject("Player");cleanup.Add(g);g.AddComponent<CharacterController>();var inputs=g.AddComponent<StarterAssetsInputs>();var pi=g.AddComponent<PlayerInput>();var movement=g.AddComponent<FirstPersonController>();var interaction=g.AddComponent<InteractionSystem>();var state=g.AddComponent<GameplayInputController>();Set(state,"playerInput",pi);Set(state,"movement",movement);Set(state,"starterInputs",inputs);Set(state,"interaction",interaction);state.EnterGameplay();Assert.True(movement.enabled);Assert.True(interaction.enabled);state.EnterPause();Assert.AreEqual(0,Time.timeScale);Assert.False(movement.enabled);Assert.False(interaction.enabled);state.EnterGameplay();Assert.AreEqual(1,Time.timeScale);Assert.True(movement.enabled);state.EnterCompletion();Assert.AreEqual(GameplayInputState.Completed,state.State);Assert.False(movement.enabled);state.EnterPause();Assert.AreEqual(GameplayInputState.Completed,state.State);}
    [Test] public void SceneConstantsAreStable(){Assert.AreEqual("MainMenu",SceneNames.MainMenu);Assert.AreEqual("Level01",SceneNames.Level01);Assert.AreEqual("Level02",SceneNames.Level02);}
    [Test] public void BeaconRequiresRadioAndKey(){var inv=Component<InventoryManager>();var beacon=Component<EmergencyBeaconController>();Assert.False(beacon.HasRequirements(inv));inv.AddItem(new InventoryItem(Item(GameIds.EmergencyRadio)));Assert.False(beacon.HasRequirements(inv));inv.AddItem(new InventoryItem(Item(GameIds.AccessKey)));Assert.True(beacon.HasRequirements(inv));}
    [Test] public void SceneLoaderRejectsRepeatedInvalidRequestsAndKeepsTimeScale(){var loader=Component<SceneLoader>();Time.timeScale=0;Assert.False(loader.TryLoadScene((GameScene)999));Assert.False(loader.TryLoadScene((GameScene)999));Assert.AreEqual(0,Time.timeScale);}
}
#endif
