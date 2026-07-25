using TMPro;
using UnityEngine;
public sealed class SimulationResultsUI : MonoBehaviour
{
 [SerializeField] private TMP_Text totalTimeValue,scoreValue,gradeValue,level01Value,level02Value,incorrectValue,hazardsValue,pausesValue,bestTimeValue;
 private void OnEnable(){SimulationSession.Instance?.StopTimer();Refresh();}
 public void Refresh(){var s=SimulationSession.Instance;if(s==null)return;bool hasBest=PlayerPrefs.HasKey(SimulationSession.BestTimeKey);float best=PlayerPrefs.GetFloat(SimulationSession.BestTimeKey,0f);Set(totalTimeValue,SimulationSession.FormatTime(s.TotalTime));Set(scoreValue,s.Score.ToString());Set(gradeValue,s.Grade);Set(level01Value,SimulationSession.FormatTime(s.Level01Time));Set(level02Value,SimulationSession.FormatTime(s.Level02Time));Set(incorrectValue,s.IncorrectInteractions.ToString());Set(hazardsValue,s.HazardContacts.ToString());Set(pausesValue,s.Pauses.ToString());Set(bestTimeValue,hasBest?SimulationSession.FormatTime(best):"--");}
 private static void Set(TMP_Text field,string value){if(field!=null)field.text=value;}
}
