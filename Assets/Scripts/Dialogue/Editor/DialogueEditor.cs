using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;


        [MenuItem("Window/Dialogue Editor")]
        private static void ShowEditorWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue");
            window.Show();
        }

        private void OnGUI()
        {
            if (selectedDialogue == null) EditorGUILayout.LabelField("No Dialogue selected");

            else
            {

                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.LabelField("Node:");
                    string newText = EditorGUILayout.TextField(node.text);
                    string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

                    if (EditorGUI.EndChangeCheck())
                    {
                        //ensure to pass in a scriptable object that the function can handle
                        Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                        node.text = newText;
                        node.uniqueID = newUniqueID;
                    }

                }
            }

        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }
    }

}