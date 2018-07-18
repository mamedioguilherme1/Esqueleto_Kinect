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

            kinect.DepthFrameReady += kinect_DepthFrameReady;
           
            /*ERROOOOO
                        kinect = kinectSensor;
                        kinect.ColorStream.Enable
                        (ColorImageFormat.RgbResolution640x480Fps30);
                        kinect.ColorFrameReady += kinect_ColorFrameReady;*/
        }
        private void kinect_DepthFrameReady (object sender, DepthImageFrameReadyEventArgs e)
        {
           
        }
        private BitmapSource ReconhecerHumanos (DepthImageFrame quadro)
        {
            if (quadro == null) return null;
            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade =
                new DepthImagePixel[quadro.PixelDataLength];
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

        private BitmapSource ObterImagemSensorRGB(ColorImageFrame quadro)
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

        }
        private void kinect_ColorFrameReady(object sender,
        ColorImageFrameReadyEventArgs e)
        {
            imagemCamera.Source =
            ObterImagemSensorRGB(e.OpenColorImageFrame());
        }

        private void slider_DragCompleted(object sender,System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            kinect.ElevationAngle = Convert.ToInt32(slider.Value);
        }
    }
}
