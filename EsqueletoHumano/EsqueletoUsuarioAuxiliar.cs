﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EsqueletoHumano
{
    class EsqueletoUsuarioAuxiliar
    {
        private KinectSensor kinect;

        public EsqueletoUsuarioAuxiliar(KinectSensor kinect) {
            this.kinect = kinect;
        }
        private ColorImagePoint ConverterCoordenadasArticulacao (Joint articulacao, double larguraCanvas, double alturaCanvas)
        {
            //Converte as articulações do esqueleto de mapp para imagem

            ColorImagePoint posicaoArticulacao = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(articulacao.Position, kinect.ColorStream.Format);
            posicaoArticulacao.X = (int)(posicaoArticulacao.X * larguraCanvas) /kinect.ColorStream.FrameWidth;
            posicaoArticulacao.Y = (int)(posicaoArticulacao.Y * alturaCanvas) /kinect.ColorStream.FrameHeight;
            return posicaoArticulacao;
        }
        private Ellipse CriarComponenteVisualArticulacao(int diametroArticulacao, int larguraDesenho, Brush corDesenho)
        {
            //Desenha as articulações 
            Ellipse objetoArticulacao = new Ellipse();
            objetoArticulacao.Height = diametroArticulacao;
            objetoArticulacao.Width = diametroArticulacao;
            objetoArticulacao.StrokeThickness = larguraDesenho;
            objetoArticulacao.Stroke = corDesenho;
            return objetoArticulacao;
        }

        public void DesenharArticulacao (Joint articulacao, Canvas canvasParaDesenhar)
        {
            int diametroArticulacao = articulacao.JointType == JointType.Head ? 50 : 10;
            int larguraDesenho = 4;
            Brush corDesenho = Brushes.Red;
            Ellipse objetoArticulacao =CriarComponenteVisualArticulacao(diametroArticulacao, larguraDesenho,corDesenho);
            ColorImagePoint posicaoArticulacao = ConverterCoordenadasArticulacao(articulacao, canvasParaDesenhar.ActualWidth, canvasParaDesenhar.ActualHeight);
            double deslocamentoHorizontal = posicaoArticulacao.X - objetoArticulacao.Width / 2;
            double deslocamentoVertical = (posicaoArticulacao.Y - objetoArticulacao.Height / 2);
            if (deslocamentoVertical >= 0 && deslocamentoVertical < canvasParaDesenhar.ActualHeight && deslocamentoHorizontal >= 0 && deslocamentoHorizontal < canvasParaDesenhar.ActualWidth)
            {
                Canvas.SetLeft(objetoArticulacao, deslocamentoHorizontal);
                Canvas.SetTop(objetoArticulacao, deslocamentoVertical);
                Canvas.SetZIndex(objetoArticulacao, 100);
                canvasParaDesenhar.Children.Add(objetoArticulacao);
            }
        }
    }
}
