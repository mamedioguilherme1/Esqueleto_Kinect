using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace AuxiliarKinect.FuncoesBasicas
{
    public static class InicializadorKinect
    {
        //Elevacao do Angulo do kinect
        public static KinectSensor
                InicializarPrimeiroSensor(int anguloElevacaoInicial)
        {
            KinectSensor kinect = KinectSensor.KinectSensors.First(sensor => sensor.Status == KinectStatus.Connected);
            kinect.Start();
            kinect.ElevationAngle = anguloElevacaoInicial;
            return kinect;
        }
    }
}
