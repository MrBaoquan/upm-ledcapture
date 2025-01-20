using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UNIHper;
using System.Xml.Serialization;
using UnityEngine.Scripting;

public class LEDScreen
{
    [XmlAttribute]
    public string ID = "";
    public SerializableVector2 position = new SerializableVector2();
    public SerializableVector2 size = new SerializableVector2();
}

[Preserve]
[SerializedAt(AppPath.StreamingDir)]
public class LEDConfig : UConfig
{
    [XmlElement("LEDScreen")]
    public List<LEDScreen> LEDScreens = new List<LEDScreen>();

    // Write your comments here
    protected override string Comment()
    {
        return @"
        Write your comments here...
        ";
    }

    // Called once after the config data is loaded
    protected override void OnLoaded()
    {
        if (LEDScreens.Count <= 0)
        {
            LEDScreens.Add(new LEDScreen());
            this.Save();
        }
    }
}
