using AuxiliarKinect.FuncoesBasicas;
using Microsoft.Kinect;
using System;
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



            kinect = kinectSensor;
            slider.Value = kinect.ElevationAngle;
            kinect.DepthStream.Enable();
            kinect.SkeletonStream.Enable();
            kinect.ColorStream.Enable();
            kinect.AllFramesReady += kinect_AllFramesReady;
            /*  slider.Value = kinect.ElevationAngle;
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

            kinect.DepthFrameReady += kinect_DepthFrameReady;
           
            /*ERROOOOO
                        kinect = kinectSensor;
                        kinect.ColorStream.Enable
                        (ColorImageFormat.RgbResolution640x480Fps30);
                        kinect.ColorFrameReady += kinect_ColorFrameReady;*/
        }



        //------


        private byte[] ObterImagemSensorRGB(ColorImageFrame quadro)
        {
            if (quadro == null) return null;
            using (quadro)
            {
                byte[] bytesImagem = new byte[quadro.PixelDataLength];
                quadro.CopyPixelDataTo(bytesImagem);
                return bytesImagem;
            }
        }
        private void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            byte[] imagem = ObterImagemSensorRGB(e.OpenColorImageFrame());
            if (chkEscalaCinza.IsChecked.HasValue &&
            chkEscalaCinza.IsChecked.Value)
                ReconhecerDistancia(e.OpenDepthImageFrame(), imagem, 2000);
            if (imagem != null)
                imagemCamera.Source =
                BitmapSource.Create(kinect.ColorStream.FrameWidth,
                kinect.ColorStream.FrameHeight,
                96, 96, PixelFormats.Bgr32, null,
                imagem,
                kinect.ColorStream.FrameBytesPerPixel
                * kinect.ColorStream.FrameWidth);
        }
        private void ReconhecerDistancia(DepthImageFrame quadro, byte[] bytesImagem, int distanciaMaxima)
        {
            //if (quadro == null || bytesImagem == null) return null;
            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade =
                new DepthImagePixel[quadro.PixelDataLength];
                quadro.CopyDepthImagePixelDataTo(imagemProfundidade);
                for (int indice = 0;
                indice < imagemProfundidade.Length;
                indice++)
                {
                    if (imagemProfundidade[indice].Depth < distanciaMaxima)
                    {
                        int indiceImageCores = indice * 4;
                        byte maiorValorCor =
                        Math.Max(bytesImagem[indiceImageCores],
                        Math.Max(bytesImagem[indiceImageCores + 1],
                        bytesImagem[indiceImageCores + 2]));
                        bytesImagem[indiceImageCores] = maiorValorCor;
                        bytesImagem[indiceImageCores + 1] = maiorValorCor;
                        bytesImagem[indiceImageCores + 2] = maiorValorCor;
                    }
                }
            }
        }
        //---
        private void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            imagemCamera.Source = ReconhecerHumanos(e.OpenDepthImageFrame());
        }
        private BitmapSource ReconhecerHumanos(DepthImageFrame quadro)
        {
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

        /* private BitmapSource ObterImagemSensorRGB(ColorImageFrame quadro)
         {
             if (quadro == null) return null;
             using (quadro)
             {
                 byte[] bytesImagem = new byte[quadro.PixelDataLength];
                 quadro.CopyPixelDataTo(bytesImagem);
                 if (chkEscalaCinza.IsChecked.HasValue &&
                 chkEscalaCinza.IsChecked.Value)
                     for (int indice = 0;
                     indice < bytesImagem.Length;
                     indice += quadro.BytesPerPixel)
                     {
                         byte maiorValorCor = Math.Max(bytesImagem[indice],
                         Math.Max(bytesImagem[indice + 1],
                         bytesImagem[indice + 2]));
                         bytesImagem[indice] = maiorValorCor;
                         bytesImagem[indice + 1] = maiorValorCor;
                         bytesImagem[indice + 2] = maiorValorCor;
                     }
                 return BitmapSource.Create(quadro.Width, quadro.Height,
                 96, 96, PixelFormats.Bgr32, null, bytesImagem,
                 quadro.Width * quadro.BytesPerPixel);
             }

    }*/
    /* private void kinect_ColorFrameReady(object sender,
     ColorImageFrameReadyEventArgs e)
     {
         imagemCamera.Source = ObterImagemSensorRGB(e.OpenColorImageFrame());
     }

     */
    private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        kinect.ElevationAngle = Convert.ToInt32(slider.Value);
    }

    }
    
}
