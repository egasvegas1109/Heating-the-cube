using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] GameObject electron;
    [SerializeField] Transform folder;
    [SerializeField] GameObject camera;
    [SerializeField] double a = 100.0, L = 70.0, x = 0.0, time = 0.0, tmax = 1.0, tau = 0.0001, h = 5, e = 300, A = 5;

    double phi(double A, double x, double L)
    {
        return A * x / L;
    }

    double psi1 = 0;

    double psi2(double A, double e, double time)
    {
        return A * Math.Pow(e, -time);
    }

    void Start()
    {
        int n = (int)(L / h) + 1;
        GameObject cam = Instantiate(camera, new Vector3(n / 2 + 1 / (n), n / 2, -n * 2), new Quaternion(0, 0, 0, 0));
        cam.GetComponent<Camera>().orthographicSize = n;
    }

    // Update is called once per frame
    void Update ()
    {

        for (int i = 0; i < folder.transform.childCount; i++)
        {
            Destroy(folder.transform.GetChild(i).gameObject);
        }

        //Объявление начальных значений
        int n = (int)(L / h) + 1;
        double r = a * tau / (h * h); //Постоянный коэффициент

        double[,,] u = new double[n, n, n];
        double[,,] un = new double[n, n, n];
        u[0,0,0] = psi1; //Начальное значение в левой краевой точке
        x += h; //Шаг по координате

        for (int i = 1; i < n - 1; i++) //Задаем начальные значения
        {
            for (int j = 0; j < n - 1; j++)
            {
                for (int k = 0; k < n - 1; k++)
                {
                    u[i, j, k] = phi(A, x, L);
                }
            }

            x += h;
        }

        u[n - 1, n - 1, n - 1] = psi2(A, e, time); //Начальное значение в правой краевой точке

        do
        {
            for (int i = 1; i < n - 1; i++) //Вычисляем новые значения в точках
            {
                for (int j = 1; j < n - 1; j++)
                {
                    for (int k = 1; k < n - 1; k++)
                    {
                        un[i, j, k] = u[i, j, k] + r * (u[i + 1, j, k] + u[i - 1, j, k] + u[i, j + 1, k] + u[i, j - 1, k] + u[i, j, k + 1] + u[i, j, k - 1] - 6 * u[i, j, k]);
                    }
                }
            }
            for (int i = 1; i < n - 1; i++) //Переприсваиваем новые значения в массив u
            {
                for (int j = 1; j < n - 1; j++)
                {
                    for (int k = 1; k < n - 1; k++)
                    {
                        u[i, j, k] = un[i, j , k];
                    }
                }
            }
            time += tau; //Шаг по времени
            u[0, 0, 0] = psi1;
            u[n - 1, n - 1, n - 1] = psi2(A, e, time);

        } while (time <= tmax);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Debug.Log(Math.Round(u[i, j, k], 5));
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    GameObject elect = Instantiate(electron, new Vector3(i, j, k), new Quaternion(0, 0, 0, 0));
                    elect.transform.SetParent(folder);
                    elect.GetComponent<MeshRenderer>().material.color = new Color((float)(u[i, j, k]), 0, 0);
                    //elect.GetComponent<MeshRenderer>().material.color = new Color((float)Math.Cos((u[i, j, k] / 15) / 255f), (float)Math.Cos((u[i, j, k] / 5) / 255f), (float)Math.Cos((u[i, j, k] / 5) / 255f), 5f);
                    elect.name = Convert.ToString(u[i, j, k]);
                }
            }
        }

        folder.transform.Rotate(new Vector3(-20, 45, -20));
    }
}
