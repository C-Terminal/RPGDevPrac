using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        GUIStyle nodeStyle;
        bool dragging = false;

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

                ProcessEvents();
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    OnGuiNode(node);
                }
            }


        }

        private void ProcessEvents()
        {
            //gets event that triggered on GUI to be called
            if (Event.current.type == EventType.MouseDown && !dragging)
            {
                dragging = true;
            }
            else if (Event.current.type == EventType.MouseDrag && dragging)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && dragging)
            {
                dragging = false;
            }
        }

        private void OnGuiNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            string newText = EditorGUILayout.TextField(node.text);
            string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

            if (EditorGUI.EndChangeCheck())
            {
                //ensure to pass in a scriptable object that the function can handle
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                node.text = newText;
                node.uniqueID = newUniqueID;
            }

            GUILayout.EndArea();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            //loading 2d texture included in unity
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
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