using System.Collections;
using System.Collections.Generic;
using UNIHper.Editor;
using UnityEditor;

namespace LEDCapture.Editor
{
    [InitializeOnLoad]
    public static class ModuleAutomation
    {
        static ModuleAutomation()
        {
            AssemblyCfgUtil.AddAssembly("LEDCapture.Runtime");

            var _assetPath = "Packages/com.parful.ledcapture/Assets/UIs";

            if (AddressableUtil.IsEntryExist(_assetPath))
                return;

            AddressableUtil.AddToLabel("com.parful.ledcapture", _assetPath);

            ResCfgUtil.AddPersistenceItem(
                new ResCfgUtil.ResourceItem
                {
                    driver = "Addressable",
                    label = "com.parful.ledcapture",
                    type = "GameObject"
                }
            );
        }
    }
}
