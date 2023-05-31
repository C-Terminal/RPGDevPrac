using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR 
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = System.Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }
            //never called normally in an exported game - this ensures it is
            OnValidate();

        }
#endif

        //Called when change made in inspector or when scriptable object loaded
        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.uniqueID] = node;

            }
        }
        // any object that allows you to do a for-loop over them, or the contents are enumerable
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childID in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }

            }

        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueID = System.Guid.NewGuid().ToString();
            parent.children.Add(newNode.uniqueID);
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.uniqueID);
            }
        }
    }
}
