using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_ListFightText", menuName = "SSO/UI/SSO_ListFightText")]
public class SSO_ListFightText : ScriptableObject
{
    public List<string> readyText;
    public List<string> startText;
    public List<string> victoryText;
    public Color colorMessage;
}