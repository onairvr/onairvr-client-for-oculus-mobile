using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRClientGroundGrid : MonoBehaviour {
    [SerializeField] private Color _color = Color.white;

    private void Awake() {
        _thisTransform = transform;
        _lineMaterial = new Material(Shader.Find("AirVRClient/Gridline"));
        _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnRenderObject() {
        _lineMaterial.SetPass(0);

        GL.PushMatrix();

        GL.Begin(GL.LINES);
        GL.Color(_color);

        for (int row = Mathf.CeilToInt(-_thisTransform.localScale.z / 2); row <= Mathf.FloorToInt(_thisTransform.localScale.z / 2); row++) {
            for (int col = Mathf.CeilToInt(-_thisTransform.localScale.x / 2); col <= Mathf.FloorToInt(_thisTransform.localScale.x / 2); col++) {
                GL.Vertex3(-_thisTransform.localScale.x / 2, 0.0f, row);
                GL.Vertex3(_thisTransform.localScale.x / 2, 0.0f, row);

                GL.Vertex3(col, 0.0f, -_thisTransform.localScale.z / 2);
                GL.Vertex3(col, 0.0f, _thisTransform.localScale.z / 2);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    private Transform _thisTransform;
    private Material _lineMaterial;
}
