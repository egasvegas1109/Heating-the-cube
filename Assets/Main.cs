using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Main : MonoBehaviour
{
    [SerializeField] GameObject electron;
    [SerializeField] Transform folder;
    [SerializeField] new GameObject camera;
    [SerializeField] double L = 10.0, H = 0.1, TAU = 0.001,tmax = 10.0, T = 100, ALPHA = 0.6, r,timeStart, timeEnd;
    [SerializeField] int N;
    [SerializeField] double[,,] cube, cubeNew;
    [SerializeField] GameObject lastSphere;
    [SerializeField] bool end = false;

    void Start()
    {
        N = (int)(L / H) + 1;
        GameObject cam = Instantiate(camera, new Vector3(0, N / 2, -N * 2), new Quaternion(0, 0, 0, 0));
        cam.GetComponent<Camera>().orthographicSize = N;

        r = TAU * ALPHA * ALPHA / (H * H); //Постоянный коэффициент

        cube = new double[N, N, N];
        cubeNew = new double[N, N, N];

        //граница 1 - ГУ второго рода
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
            {
                cubeNew[i, 0, j] = cube[i, 1, j]; // граница 1
                cubeNew[i, j, N - 1] = cube[i, j, N - 2]; // граница 2
                cubeNew[i, N - 1, j] = cube[i, N - 2, j]; // граница 3
                cubeNew[N - 1, i, j] = cube[N - 2, i, j]; // граница 5
                cubeNew[0, i, j] = cube[1, i, j]; // граница 6
                cubeNew[i, j, 0] = T; // граница 4

            }

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                for (int k = 0; k < N; k++)
                {
                    GameObject elect = Instantiate(electron, new Vector3(i, j, k), new Quaternion(0, 0, 0, 0));
                    elect.transform.SetParent(folder);
                    elect.GetComponent<MeshRenderer>().material.color = new Color((float)(cube[i, j, k] / 100), 0, 0);
                    elect.name = Convert.ToString(cube[i, j, k]);
                }
            }
        }

        timeStart = Time.time;
    }

    void Update ()
    {
        N = (int)(L / H) + 1;

        for (int i = 0; i < folder.transform.childCount; i++)
        {
            Destroy(folder.transform.GetChild(i).gameObject);
        }

        for (double time = 0; time < tmax; time += TAU)
        {

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    cubeNew[i, 0, j] = cube[i, 1, j]; // граница 1
                    cubeNew[i, j, N - 1] = cube[i, j, N - 2]; // граница 2
                    cubeNew[i, N - 1, j] = cube[i, N - 2, j]; // граница 3
                    cubeNew[N - 1, i, j] = cube[N - 2, i, j]; // граница 5
                    cubeNew[0, i, j] = cube[1, i, j]; // граница 6
                    cubeNew[i, j, 0] = T; // граница 4

                }

            //основные вычисления
            for (int i = 1; i < N - 1; i++)
                for (int j = 1; j < N - 1; j++)
                    for (int k = 1; k < N - 1; k++)
                    {
                        cubeNew[i, j, k] = cube[i, j, k] + r * (cube[i + 1, j, k] + cube[i - 1, j, k] + cube[i, j + 1, k] + cube[i, j - 1, k] +
                            cube[i, j, k + 1] + cube[i, j, k - 1] - 6 * cube[i, j, k]);
                    }

            //переприсваивание 
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    for (int k = 0; k < N; k++)
                        cube[i, j, k] = cubeNew[i, j, k];
        }

        for (int i = 0; i < N; i++)//округляю значения чтоб покрасивше было
        {
            for (int j = 0; j < N; j++)
            {
                for (int k = 0; k < N; k++)
                {
                    cube[i, j, k] = Math.Round(cube[i, j, k], 5);
                }
            }
        }

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                for (int k = 0; k < N; k++)
                {
                    GameObject elect = Instantiate(electron, new Vector3(i, j, k), new Quaternion(0, 0, 0, 0));
                    elect.transform.SetParent(folder);
                    elect.GetComponent<MeshRenderer>().material.color = new Color((float)(cube[i, j, k]/100), 0, 0);
                    elect.name = Convert.ToString(cube[i, j, k]);
                }
            }
        }

        lastSphere = folder.transform.GetChild(cube.Length + cube.Length - 1).gameObject;
        folder.transform.Rotate(new Vector3(-110, 0, 45));
        
        if (Convert.ToDouble(lastSphere.name) > 99 && !end)
        {
            timeEnd = Time.time;
            Debug.Log(Math.Round(timeEnd - timeStart, 5));
            Debug.Log("Нагрелось");
            end = true;
        }
    }
}
