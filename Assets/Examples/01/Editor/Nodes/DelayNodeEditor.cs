using NodeGraph.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Example01.Editor
{
    [NodeTargetEditor(typeof(DelayNode))]
    public class DelayNodeEditor : BaseNodeView
    {
        private DelayNode node => target as DelayNode;
        
        protected override void OnInit()
        {
            var inputField = new FloatField("时间");
            inputField.labelElement.style.minWidth = 50;
            inputField.value = node.delay;    
            inputField.RegisterValueChangedCallback(OnValueChangedEvent);
            controlsContainer.Add(inputField);
        }

        private void OnValueChangedEvent(ChangeEvent<float> e)
        {
            node.ResetDelay(e.newValue);
        }

        // private void AddMGUIContainer()
        // {
        //     var content = new IMGUIContainer(Draw);
        //     content.style.backgroundColor = Color.black;
        //     mainContainer.Add(content); 
        // }
        //
        // private void Draw()
        // {
        //     GUILayout.BeginVertical();
        //
        //     GUILayout.BeginHorizontal();
        //     EditorGUILayout.LabelField("时间",GUILayout.Width(40));
        //     var newDelay = EditorGUILayout.FloatField(node.delay,GUILayout.Width(100));
        //     if (Math.Abs(newDelay - node.delay) > 0)
        //     {
        //         node.ResetDelay(newDelay);
        //     }
        //     GUILayout.EndHorizontal();
        //     
        //     GUILayout.EndVertical();
        // }

    }
}

