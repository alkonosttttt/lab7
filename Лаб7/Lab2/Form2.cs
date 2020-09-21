using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Lab2
{
	public partial class Form2 : Form
	{
		//ПЕРЕМЕННЫЕ
		//======================================================================================

		public string fileName; //имя файла, когда картинка сохраняется
		public bool fromFile = false; //флаг: существовал ли данный файл
		bool paintAction = false; // удержание кнопки мыши (флаг)
		bool changedCanvas = false; //изменение рисунка (флаг)
		Point start,finish; //точки (для рисования) начала фигуры и конца
		List<AbstractFigure> fstorage = new List<AbstractFigure>(); //список базовых фигур
		AbstractFigure toPaint; //базовая фигура
        //параметры рисования
        //класс Bitmap инкапсулирует точечный рисунок, состоящий из данных точек графического изображения и атрибутов рисунка.
        Bitmap canvas = new Bitmap(10,10);
		public Color backColor = Color.White;	//цвет фона
		public Color frameColor = Color.Black;		//цвет фигуры непосредственно при её рисовании
		public Color primaryColor = Color.Black;    //цвет линий
		public Color secondaryColor = Color.Black;    //цвет заливки
		public int lineWidth = 1; //толщина пера
		public bool solidFill = false; //по умолчанию заливки нет
        public int figureID = 0; //фигура по умолчанию - линия
		public int pictWidth=1,pictHeight=1;

		//РИСОВАНИЕ
		//======================================================================================
	
		public void drawCanvas() //рисование точечного рисунка
		{
			canvas.Dispose();                          // Dispose - явное освобождение ресурсов, т.е. "удаляем" старый точечный рисунок...
            canvas = new Bitmap(pictWidth,pictHeight); //...и создаём новый
			Graphics g = Graphics.FromImage(canvas);   //Graphics.FromImage cоздает новый объект Graphics из указанного объекта canvas
            g.Clear(backColor);//Очищаем всю поверхность рисования и выполняем заливку поверхности указанным цветом фона
            foreach (AbstractFigure go in fstorage)	go.draw(ref g);
			if(paintAction)	toPaint.drawFrame(ref g); //если зажата кнопка мыши, то рисуем временный рисунок
			g.Dispose();
		}

		private void initPainter() //определяется фигура по её id
		{   
			switch(figureID)
			{
				case 0:	toPaint = new GLine(); break;
				case 1: toPaint = new GCurve(); break;
				case 2: toPaint = new GRectangle(); break;
				case 3: toPaint = new GEllipse(); break;
				default: toPaint = new GLine(); break; //фигура по умолчанию - линия
			}
			//устанавливаются параметры фигуры
			toPaint.loadColors(primaryColor,secondaryColor,frameColor); 
			toPaint.firstPoint = start; //точка (для рисования) начала фигуры
            toPaint.secondPoint = start; //точка (для рисования) конца фигуры
            toPaint.lineWidth = lineWidth; //толщина пера
			toPaint.fill = solidFill; //наличие заливки
		}

		//I/O
		//======================================================================================

		public void SaveFile(string name)
		{
            //класс BinaryFormatter применяется для бинарной сериализации
            BinaryFormatter formatter = new BinaryFormatter();
            //Класс Stream представляет байтовый поток и является базовым для всех остальных классов потоков
            //Класс FileStream предоставляет реализацию абстрактного члена Stream в манере, подходящей для потоковой работы с файлами.
            //Класс FileStream представляет возможности по считыванию из файла и записи в файл как текстовый, так и бинарный
            Stream stream = new FileStream(name,FileMode.Create,FileAccess.Write,FileShare.None);//создаёт новый файл, позволяет др потокам получать доступ для записи до тех пор, пока он остаётся открытым
			formatter.Serialize(stream,pictWidth); //сериализация одним методом formatter.Serialize добавляет данные о ширине в файл stream
            formatter.Serialize(stream,pictHeight);//..о высоте..
			formatter.Serialize(stream,backColor); //..о цвете фона..
			formatter.Serialize(stream,fstorage);  //..о массиве базовых фигур..
			stream.Close();//закрываем поток
			changedCanvas = false; //флаг означает, что изменений нет
		}

		public void LoadFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);// открывает файл, позволяет др потокам получать доступ для чтения

            pictWidth = (int)formatter.Deserialize(stream); //десериализации  объекта stream -> преобразовать к типу int
            pictHeight = (int)formatter.Deserialize(stream);
			backColor = (Color)formatter.Deserialize(stream);
			fstorage = (List<AbstractFigure>)formatter.Deserialize(stream);
			stream.Close(); //закрываем поток
            drawCanvas(); //перерисовка
            Refresh(); //делает недоступной свою клиентскую область и немедленно перерисовывает себя и все дочерние элементы
        }

		//ВИД ФОРМЫ
		//======================================================================================

		public Form2()
		{
			InitializeComponent();
		}

		private void Form2_Shown(object sender, EventArgs e)
		{
			canvas = new Bitmap(pictWidth,pictHeight);
			Graphics.FromImage(canvas).Clear(backColor);//Graphics.FromImage cоздает новый объект Graphics из указанного объекта canvas
        }


		//ДИВЖЕНИЕ МЫШИ
		//======================================================================================

		private void Form2_MouseDown(object sender, MouseEventArgs e) //нажатие кнопки мыши
		{ 
			int eX = e.X - AutoScrollPosition.X; //определяются текущие координаты 
			int eY = e.Y - AutoScrollPosition.Y;
			if(e.Button==MouseButtons.Left && eX<=pictWidth && eY<=pictHeight)
			{
				start.X = eX; //устанавливаются координаты нажатия в качестве точки начала для рисования
				start.Y = eY;
				finish = start;
				initPainter(); //определяется фигура по её id
                paintAction = true; //удержание кнопки мыши (флаг)
            }
		}

		private void Form2_MouseMove(object sender, MouseEventArgs e) //передвижение мыши
		{ 
			int eX = e.X - AutoScrollPosition.X; //определяются текущие координаты
			int eY = e.Y - AutoScrollPosition.Y;
			if(paintAction) //если кнопка мыши нажата
			{
				finish.X = eX; //текущие координаты устанавливаются в качестве точки конца для рисования
				finish.Y = eY;
				toPaint.secondPoint = finish; //сохраняется текущая точка как точка конца
				drawCanvas(); //рисуем
				Refresh(); //делает недоступной свою клиентскую область и немедленно перерисовывает себя и все дочерние элементы
            }
        }

		private void Form2_MouseUp(object sender, MouseEventArgs e)//отпускание мыши
		{ 
			int eX = e.X - AutoScrollPosition.X; //определяются текущие координаты
            int eY = e.Y - AutoScrollPosition.Y;
			if(paintAction) //если кнопка мыши была нажата
            {
				paintAction = false; //устанавливается флаг - кнопка мыши не нажата
				finish.X = eX;	//определяются конечные координаты
				finish.Y = eY;
				toPaint.secondPoint = finish; //сохраняется текущая точка как точка конца
                changedCanvas = true; //изменения есть
				if(eX<=pictWidth && eY<=pictHeight && eX>=0 && eY>=0) fstorage.Add(toPaint); //если фигура внутри формы (окна), то в массив базовых фигур добавляется новая фигура
				drawCanvas(); //рисуем
				Refresh(); //делает недоступной свою клиентскую область и немедленно перерисовывает себя и все дочерние элементы
            }
		}

		//======================================================================================

		private void Form2_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.LightGray); //заполняем фон рабочей области рисунка
            e.Graphics.DrawImage(canvas,AutoScrollPosition.X,AutoScrollPosition.Y); //рисуем  объект canvas в заданном месте, используя исходный размер; AutoScrollPosition представляет местоположение видимой части элемента управления с возможностью прокрутки
        }

		private void Form2_Resize(object sender, EventArgs e)
		{
			drawCanvas(); //рисование точечного рисунка
            Refresh(); //делает недоступной свою клиентскую область и немедленно перерисовывает себя и все дочерние элементы
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //закрытие активной формы
        //======================================================================================

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(this.ParentForm.MdiChildren.Length==1)
			{
				((Form1)this.ParentForm).deactivateMenu(); //закрывается "последняя" дочерняя форм
            }
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(changedCanvas) //если рисунок изменялся
				switch(MessageBox.Show("Сохранить изменения в \""+this.Text+"\"?","Вопрос",MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						((Form1)this.ParentForm).saveFile();
					break;
					case DialogResult.Cancel:
						e.Cancel = true;
					break;
				}
		}

		//======================================================================================
	}

}
