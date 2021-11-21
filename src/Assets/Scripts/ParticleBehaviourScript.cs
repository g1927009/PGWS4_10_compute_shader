using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;// Marshal���g��

struct Particle
{
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 col;
}

public class ParticleBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Material material;// �`��}�e���A��

    [SerializeField]
    private ComputeShader computeShader;

    private int updateKernel;
    private ComputeBuffer buffer;// Particle�z��

    static int THREAD_NUM = 64;
    static int PARTICLE_NUM = ((65536 + THREAD_NUM - 1) / THREAD_NUM) * THREAD_NUM;// THREAD_NUM�̔{���ɂ���

    void OnEnable()// �I�u�W�F�N�g���L���ɂȂ����Ƃ��ɌĂ΂��
    {
        // �p�[�e�B�N���̏����i�[����o�b�t�@
        buffer = new ComputeBuffer(
            PARTICLE_NUM,
            Marshal.SizeOf(typeof(Particle)),
            ComputeBufferType.Default);

        //������
        var initKernel = computeShader.FindKernel("initialize");
        computeShader.SetBuffer(initKernel, "Particales", buffer);
        computeShader.Dispatch(initKernel, PARTICLE_NUM / THREAD_NUM, 1, 1);

        // �X�V�֐��̐ݒ�
        updateKernel = computeShader.FindKernel("update");
        computeShader.SetBuffer(updateKernel, "Particles", buffer);

        // �`��p�}�e���A���̐ݒ�
        material.SetBuffer("Particles", buffer);
    }

    void OnDisable() // �I�u�W�F�N�g�������ɂȂ����Ƃ��ɌĂ΂��
    {
        buffer.Release();// �o�b�t�@�̉��
    }

    void Start()
    {
        
    }

    void Update()
    {
        // ������
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.Dispatch(updateKernel, PARTICLE_NUM / THREAD_NUM, 1, 1);
    }

    void OnRenderObject()
    {
        // �`��
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, PARTICLE_NUM);
    }
}
