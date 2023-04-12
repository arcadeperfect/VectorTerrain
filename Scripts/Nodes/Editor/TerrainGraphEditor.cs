using System;
using System.Linq;
using _scripts.Swooper.Nodes;
using UnityEditor;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Nodes;
using XNodeEditor;

namespace Nodez.Editor
{
    [NodeGraphEditor.CustomNodeGraphEditorAttribute(typeof(TerrainGraph))]
    public class TerrainGraphEditor: NodeGraphEditor
    {
        public override void AddContextMenuItems(GenericMenu menu)
        {
            Type[] types = NodeEditorReflection.GetDerivedTypes(typeof(TerrainNode));
            
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            var nodeTypes = NodeEditorReflection.nodeTypes.OrderBy(type => GetNodeMenuOrder(type)).ToArray();
            for (int i = 0; i < nodeTypes.Length; i++) {
                Type type = nodeTypes[i];
                if(!types.Contains(type)) continue;
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
                    NodeEditorWindow.current.AutoConnect(node);
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