using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddAnnotation))]
public class AddAnnotationEditor : Editor
{
    private AddAnnotation mytarget;
    private string annotationId, annotationTitle, annotationContents;
    bool showWindow = false, startTrackingMouse=false;

    private void OnEnable()
    {
        mytarget = (AddAnnotation)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Save to Json", GUILayout.Height(30), GUILayout.Width(200)))
        {
            mytarget.SaveToJSON();
        }
        if (GUILayout.Button("Update Data", GUILayout.Height(30), GUILayout.Width(200)))
        {
            mytarget.UpdateData();
        }
    }
    public void OnSceneGUI()
    {
        Handles.BeginGUI();
        //add new label button, click it will pops up another window
        if (GUILayout.Button("Add new Label", GUILayout.Height(30), GUILayout.Width(200)))
        {
            showWindow = true;
        }

        if (showWindow)
        {
            Rect windowSize = new Rect(0, 100, 300, 200);
            Rect Window = GUI.Window(0, windowSize, TitleInquiryWindow, "Enter Label Name");
        }
        if (startTrackingMouse)
        {
            EditorGUILayout.HelpBox("Now, click on where you want to place the label on the object", MessageType.Info);
        }
        if (startTrackingMouse && Event.current.type == EventType.MouseDown)
        {
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                mytarget.AddAnnotations(annotationId, annotationTitle,annotationContents, hit.point);
            }

            startTrackingMouse = false;
        }

        Handles.EndGUI();
    }
    public void TitleInquiryWindow(int windowID)
    {
        EditorGUILayout.LabelField("Please enter the label text");
        annotationId= EditorGUILayout.TextField("Annotation ID", annotationId);
        annotationTitle = EditorGUILayout.TextField("Annotation Title", annotationTitle);
        annotationContents = EditorGUILayout.TextField("Annotation Contents", annotationContents);
        if (GUILayout.Button("Ok", GUILayout.Height(30), GUILayout.Width(200)))
        {
            
                showWindow = false;

                startTrackingMouse = true;


        }
    }
}
