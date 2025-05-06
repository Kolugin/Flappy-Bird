using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Flappy_Bird
{
    public partial class Form1 : Form
    {
        private float birdY = 200; // Текущая вертикальная позиция птицы
        private float velocity = 0; // Скорость птицы
        private float acceleration = 0.5f; // Гравитация

        private int pipeGap = 150; // Расстояние между трубами
        private int score = 0; // Счет
        private int pipeSpeed = 5; // Скорость движения труб
        private bool gameOver = false; // Состояние игры

        private Timer gameTimer = new Timer();
        private Timer pipeTimer = new Timer();

        private int pipeSpawnInterval = 5000; // Интервал появления труб (в мс)

        private List<Rectangle> pipes = new List<Rectangle>(); // Список труб

        public Form1()
        {
            InitializeComponent();
            this.Text = "Flappy Bird";
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized; // Полноэкранный режим
            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;

            gameTimer.Interval = 8; // Интервал обновления игры
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            pipeTimer.Interval = pipeSpawnInterval; // Таймер для появления труб
            pipeTimer.Tick += PipeTimer_Tick;
            pipeTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            velocity += acceleration; // Ускорение птицы
            birdY += velocity;

            // Перемещение труб
            for (int i = 0; i < pipes.Count; i++)
            {
                var pipe = pipes[i];
                pipes[i] = new Rectangle(pipe.X - pipeSpeed, pipe.Y, pipe.Width, pipe.Height);
            }

            // Удаление труб, вышедших за границы экрана
            pipes.RemoveAll(pipe => pipe.X + pipe.Width < 0);

            // Проверка столкновений
            foreach (var pipe in pipes)
            {
                if (pipe.IntersectsWith(new Rectangle(40, (int)birdY, 30, 30)))
                {
                    gameOver = true;
                    gameTimer.Stop();
                    pipeTimer.Stop();
                }
            }

            // Проверка выхода птицы за экран
            if (birdY < 0 || birdY > this.ClientSize.Height)
            {
                gameOver = true;
                gameTimer.Stop();
                pipeTimer.Stop();
            }

            this.Invalidate(); // Перерисовка формы
        }

        private void PipeTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            // Создание новых труб
            int pipeHeight = new Random().Next(100, this.ClientSize.Height - pipeGap - 100);
            int pipeWidth = new Random().Next(45, 240); // Случайная ширина трубы
            pipes.Add(new Rectangle(this.ClientSize.Width, 0, pipeWidth, pipeHeight)); // Верхняя труба
            pipes.Add(new Rectangle(this.ClientSize.Width, pipeHeight + pipeGap, pipeWidth, this.ClientSize.Height - pipeHeight - pipeGap)); // Нижняя труба

            score++;
            AdjustGameDifficulty();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && !gameOver)
            {
                velocity = -8; // Рывок вверх
            }

            if (e.KeyCode == Keys.R && gameOver)
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            birdY = this.ClientSize.Height / 2; // Центр экрана
            velocity = 0;
            pipes.Clear(); // Очистка списка труб
            score = 0;
            pipeSpeed = 5;
            pipeSpawnInterval = 5000;
            gameOver = false;
            gameTimer.Start();
            pipeTimer.Interval = pipeSpawnInterval;
            pipeTimer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем птицу (состоит из нескольких фигур)
            g.FillEllipse(Brushes.Yellow, 40, birdY, 30, 30); // Тело
            g.FillRectangle(Brushes.Orange, 35, birdY + 10, 10, 10); // Клюв
            g.FillEllipse(Brushes.Black, 55, birdY + 5, 5, 5); // Глаз

            // Рисуем трубы
            foreach (var pipe in pipes)
            {
                g.FillRectangle(Brushes.Green, pipe);
            }

            // Рисуем счет
            g.DrawString($"Score: {score}", new Font("Arial", 16), Brushes.Black, 10, 10);

            // Сообщение об окончании игры
            if (gameOver)
            {
                g.DrawString("Game Over! Press R to Restart", new Font("Arial", 16), Brushes.Red, this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2);
            }
        }

        private void AdjustGameDifficulty()
        {
            // Увеличиваем скорость труб и сокращаем интервал появления при достижении определенных очков
            if (score == 10 || score == 20 || score == 50 || score == 100 ||
                score == 200 || score == 500 || score == 1000)
            {
                pipeSpeed++; // Увеличиваем скорость труб
                pipeSpawnInterval = Math.Max(1000, pipeSpawnInterval - 200); // Уменьшаем интервал появления труб
                pipeTimer.Interval = pipeSpawnInterval; // Применяем новое значение
            }
        }
    }
}
