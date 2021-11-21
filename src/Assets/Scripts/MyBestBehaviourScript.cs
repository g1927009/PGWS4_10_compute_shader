using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;// Marshalを使う

public class MyBestBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private ComputeShader compute_shader;
    private int kernel;

    private GameObject cube;

    private ComputeBuffer buffer;
    private Vector3 center = Vector3.zero;

    private float radius = 0.2f;
    private float add = 1.0f/1000.0f;

    void OnEnable()// オブジェクトが有効になったときに呼ばれる
    {
        buffer = new ComputeBuffer(1, sizeof(float) * 4);
        kernel = compute_shader.FindKernel("CSMain"); // プログラム番号の取得
        compute_shader.SetBuffer(kernel, "Result", buffer); // バッファの設定
    }
        // Start is called before the first frame update
        void Start()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    // Update is called once per frame
    void Update()
    {
        compute_shader.SetFloats("position", center.x, center.y);
        compute_shader.SetFloat("time", Time.time);
        compute_shader.SetFloat("r", radius);
        compute_shader.SetFloat("add", add);
        compute_shader.Dispatch(kernel, 8, 8, 1);

        var data = new float[4];
        buffer.GetData(data);

        Vector3 pos = cube.transform.localPosition;
        pos.x = data[0];
        pos.y = data[1];
        radius = data[2];
        add = data[3];
        cube.transform.localPosition = pos;
    }
    private void OnDestroy()
    {
        buffer.Release();
    }
}
