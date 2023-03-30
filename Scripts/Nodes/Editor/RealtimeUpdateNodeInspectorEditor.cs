using UnityEditor;
using VectorTerrain.Scripts.Nodes;
using XNodeEditor;

namespace Nodez.Editor
{
    [CustomEditor(typeof(TerrainNode), true)]
    public class RealtimeUpdateNodeInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainNode terrainNode = target as TerrainNode;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                if (terrainNode is not null) terrainNode.Updated();
            }
        }
    }

    [CustomNodeEditor(typeof(TerrainNode))]
    public class RealtimeUpdateNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            TerrainNode terrainNode = target as TerrainNode;
            EditorGUI.BeginChangeCheck();
            base.OnBodyGUI();
            if (EditorGUI.EndChangeCheck())
            {
                if (terrainNode is not null) terrainNode.Updated();
            }
        }
    }
}