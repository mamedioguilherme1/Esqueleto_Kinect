using AuxiliarKinect.FuncoesBasicas;
using EsqueletoHumano.Auxiliar;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace EsqueletoHumano
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinect;
        ColorImageFrameReadyEventArgs e;
        public MainWindow()
        {
            InitializeComponent();
            InicializarSeletor();

        }
        private void InicializarKinect(KinectSensor kinectSensor)
        {
            //Método de inicialização do kinect
            kinect = kinectSensor;
            slider.Value = kinect.ElevationAngle;
            kinect.DepthStream.Enable();
            kinect.SkeletonStream.Enable();
            kinect.ColorStream.Enable();
            kinect.AllFramesReady += kinect_AllFramesReady;
            
        }
       
        private byte[] ObterImagemSensorRGB(ColorImageFrame quadro)
        {
            //Método para gerar a imagem rgbd 
            if (quadro == null) return null;
            using (quadro)
            {
                byte[] bytesImagem = new byte[quadro.PixelDataLength];
                quadro.CopyPixelDataTo(bytesImagem);
                return bytesImagem;
            }
        }
        //-
        private void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //Método de receber a informação do sensor de profundidade
           
            byte[] imagem = ObterImagemSensorRGB(e.OpenColorImageFrame());
            if (chkEscalaCinza.IsChecked.HasValue && chkEscalaCinza.IsChecked.Value)
                ReconhecerDistancia(e.OpenDepthImageFrame(), imagem, 2000);
            if (imagem != null)
                canvasKinect.Background = new ImageBrush(BitmapSource.Create(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight,96, 96, PixelFormats.Bgr32,
                    null,imagem,kinect.ColorStream.FrameBytesPerPixel* kinect.ColorStream.FrameWidth));
            canvasKinect.Children.Clear();
            DesenharEsqueletoUsuario(e.OpenSkeletonFrame());
            e.OpenSkeletonFrame();
        }
        private void ReconhecerDistancia(DepthImageFrame quadro, byte[] bytesImagem, int distanciaMaxima)
        {   
            //Método que reconhece a profundidade da imagem
            if (quadro == null || bytesImagem == null) return;
            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade =
                new DepthImagePixel[quadro.PixelDataLength];
                quadro.CopyDepthImagePixelDataTo(imagemProfundidade);
                DepthImagePoint[] pontosImagemProfundidade =
                new DepthImagePoint[640 * 480];
                kinect.CoordinateMapper
                .MapColorFrameToDepthFrame(kinect.ColorStream.Format,
                kinect.DepthStream.Format, imagemProfundidade,
                pontosImagemProfundidade);
                for (int i = 0; i < pontosImagemProfundidade.Length; i++)
                {
                    var point = pontosImagemProfundidade[i];
                    if (point.Depth < distanciaMaxima &&
                    KinectSensor.IsKnownPoint(point))
                    {
                        var pixelDataIndex = i * 4;
                        byte maiorValorCor =
                        Math.Max(bytesImagem[pixelDataIndex],
                        Math.Max(bytesImagem[pixelDataIndex + 1],
                        bytesImagem[pixelDataIndex + 2]));
                        bytesImagem[pixelDataIndex] = maiorValorCor;
                        bytesImagem[pixelDataIndex + 1] = maiorValorCor;
                        bytesImagem[pixelDataIndex + 2] = maiorValorCor;
                    }
                }
            }
        }

        private void DesenharEsqueletoUsuario(SkeletonFrame quadro)
        {
            if (quadro == null) return;
            using (quadro)
                quadro.DesenharEsqueletoUsuario(kinect, canvasKinect);
        }
        private BitmapSource ReconhecerHumanos(DepthImageFrame quadro)
        {
            //Método para reconhecer humanos
            if (quadro == null) return null;
            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade = new DepthImagePixel[quadro.PixelDataLength];
                quadro.CopyDepthImagePixelDataTo(imagemProfundidade);
                byte[] bytesImagem = new byte[imagemProfundidade.Length * 4];
                for (int indice = 0; indice < bytesImagem.Length; indice += 4)
                {
                    if (imagemProfundidade[indice / 4].PlayerIndex != 0)
                    {
                        bytesImagem[indice + 1] = 255;
                    }
                }
                return BitmapSource.Create(quadro.Width, quadro.Height,
                96, 96, PixelFormats.Bgr32, null, bytesImagem,
                quadro.Width * 4);
            }
        }
        private void InicializarSeletor()
        {

            InicializadorKinect inicializador = new InicializadorKinect();
            InicializarKinect(inicializador.SeletorKinect.Kinect);
            inicializador.MetodoInicializadorKinect = InicializarKinect;
            seletorSensorUI.KinectSensorChooser = inicializador.SeletorKinect;

        }

    private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        kinect.ElevationAngle = Convert.ToInt32(slider.Value);
    }

    }
    
}
