using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Nodes;
using XNodeEditor;

namespace VectorTerrain.Scripts.Editor
{
    [CustomNodeGraphEditor(typeof(TerrainGraph))]
    public class TerrainGraphEditor: NodeGraphEditor
    {
        
        /// <summary>
        /// Override this method to limit the allowed nodes in the context menu to those intended for a terrain graph
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="compatibleType"></param>
        /// <param name="direction"></param>
        
        public override void AddContextMenuItems(GenericMenu menu, Type compatibleType = null, XNode.NodePort.IO direction = XNode.NodePort.IO.Input)
        {
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            Type[] nodeTypes;
            var allowedTypes = NodeEditorReflection.GetDerivedTypes(typeof(TerrainNode)).OrderBy(GetNodeMenuOrder).ToArray();
            
            if (compatibleType != null && NodeEditorPreferences.GetSettings().createFilter) 
            {
                var compatibleTypes = NodeEditorUtilities.GetCompatibleNodesTypes(NodeEditorReflection.nodeTypes, compatibleType, direction).OrderBy(GetNodeMenuOrder).ToArray();
                nodeTypes = compatibleTypes.Intersect(allowedTypes).ToArray();
            } 
            else 
            {
                nodeTypes = allowedTypes;
            }
            
            
            for (int i = 0; i < nodeTypes.Length; i++) {
                Type type = nodeTypes[i];

                //Get node context menu path
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                // Check if user is allowed to add more of given node type
                XNode.Node.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib)) {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                // Add node entry to context menu
                if (disallowed) menu.AddItem(new GUIContent(path), false, null);
                else menu.AddItem(new GUIContent(path), false, () => {
                    XNode.Node node = CreateNode(type, pos);
                    if (node != null) NodeEditorWindow.current.AutoConnect(node); // handle null nodes to avoid nullref exceptions
                });
            }
            menu.AddSeparator("");
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0) menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
            else menu.AddDisabledItem(new GUIContent("Paste"));
            menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }
    }
}