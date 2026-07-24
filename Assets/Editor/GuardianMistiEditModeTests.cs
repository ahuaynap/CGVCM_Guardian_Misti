#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class GuardianMistiEditModeTests
{
    private readonly List<Object> cleanup=new();
    [TearDown] public void TearDown(){foreach(var o in cleanup)if(o!=null)Object.DestroyImmediate(o);cleanup.Clear();}
    private InventoryItemDefinition Item(string id){var d=ScriptableObject.CreateInstance<InventoryItemDefinition>();cleanup.Add(d);Set(d,"<Id>k__BackingField",id);Set(d,"<Name>k__BackingField",id);return d;}
    private T Component<T>()where T:Component{var g=new GameObject(typeof(T).Name);cleanup.Add(g);return g.AddComponent<T>();}
    private static void Set(object o,string f,object v)=>o.GetType().GetField(f,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(o,v);
    [Test] public void InventoryRejectsNullAndDuplicatesAndHasItem(){var m=Component<InventoryManager>();Assert.False(m.AddItem(null));var d=Item("one");Assert.True(m.AddItem(new InventoryItem(d)));Assert.False(m.AddItem(new InventoryItem(d)));Assert.True(m.HasItem("one"));Assert.False(m.HasItem(null));}
    [Test] public void ObjectivesProgressOnlyInOrderAndStopAfterCompletion(){var m=Component<ObjectivesManager>();Set(m,"objectives",new List<Objective>{new("a","A"),new("b","B")});Assert.False(m.TryCompleteObjective("b"));Assert.True(m.TryCompleteObjective("a"));Assert.True(m.TryCompleteObjective("b"));Assert.False(m.TryCompleteObjective("b"));Assert.True(m.IsSimulationCompleted);}
    [Test] public void SceneConstantsAreStable(){Assert.AreEqual("MainMenu",SceneNames.MainMenu);Assert.AreEqual("Level01",SceneNames.Level01);Assert.AreEqual("Level02",SceneNames.Level02);}
    [Test] public void BeaconRequiresRadioAndKey(){var inv=Component<InventoryManager>();var beacon=Component<EmergencyBeaconController>();Assert.False(beacon.HasRequirements(inv));inv.AddItem(new InventoryItem(Item(GameIds.EmergencyRadio)));Assert.False(beacon.HasRequirements(inv));inv.AddItem(new InventoryItem(Item(GameIds.AccessKey)));Assert.True(beacon.HasRequirements(inv));}
}

#endif
