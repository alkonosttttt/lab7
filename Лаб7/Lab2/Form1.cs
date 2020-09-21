using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
	public partial class Form1 : Form
	{
		public int wCount = 0; //количество созданных дочерних окон
		public int lineWidth = 1;   //ширина пера по умолчанию
		public int pictHeight=600,pictWidth=800; //параметры нового окна
		public bool solidFill = false; //заливка фигуры (флаг)
		public int figureID = 0; //тип фигуры

		public void newWindow()
		{
			Form2 f2 = new Form2(); //создание нового окна
			f2.MdiParent = this;
			//параметры рисования
			f2.lineWidth = lineWidth;
			f2.primaryColor = primColorDialog.Color;////цвет пера, границы фигуры
            f2.secondaryColor = secondColorDialog.Color;////цвет заливки
            f2.backColor = backColorDialog.Color;//цвет фона
            f2.solidFill = solidFill; //заливка фигуры (флаг)
            f2.figureID = figureID;
			//параметры формы
			f2.pictHeight = pictHeight;
			f2.pictWidth = pictWidth;
			f2.AutoScrollMinSize = new Size(pictWidth,pictHeight);// минимальный размер для автоматической прокрутки
            f2.AutoScroll = true; //разрешение прокрутки
			f2.Text = "Рисунок "+(++wCount);
			f2.Show();
		}

		//ИНТЕРФЕЙС
		//======================================================================================

		public void activateMenu() //доступность пунктов меню "Файл" и кнопки-"дискетки"
		{
			saveToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
		}

		public void deactivateMenu() //недоступность пунктов меню "Файл" и кнопки-"дискетки"
        {
			saveToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
		}


		//ОПЕРАЦИИ С ФАЙЛОМ (МЕНЮ "ФАЙЛ")
		//======================================================================================

		public void openFile() //Открыть 
        {//метод openFileDialog позволяет просматривать папки  компьютера и выбирать файлы, которые требуется открыть. 
            if (openFileDialog1.ShowDialog()==DialogResult.OK)//Когда выбран файл
            {
				newWindow(); //создание нового окна
				((Form2)this.ActiveMdiChild).LoadFile(openFileDialog1.FileName); //загрузка файла "в форму"
				((Form2)this.ActiveMdiChild).fromFile = true; //флаг - файл существует
				((Form2)this.ActiveMdiChild).fileName = openFileDialog1.FileName; //возвращает строку, содержащую имя файла, выбранного в диалоговом окне.
                this.ActiveMdiChild.Text = openFileDialog1.FileName; //устанавливает в качестве заголовка окна имя файла
			}
			activateMenu();//доступны пункты меню "Файл" и кнопка-"дискетки"
        }

		public void saveAsFile() //Сохранить как
		{
			saveFileDialog1.InitialDirectory = Environment.CurrentDirectory; // возвращает текущую папку, отображенную диалоговым окном файла.
            if (saveFileDialog1.ShowDialog()==DialogResult.OK) //Когда нажата кнопка "Сохранить"
			{
				((Form2)this.ActiveMdiChild).SaveFile(saveFileDialog1.FileName); //файл сохраняется
				((Form2)this.ActiveMdiChild).fileName = saveFileDialog1.FileName;
				((Form2)this.ActiveMdiChild).fromFile = true; //флаг-файл существует
                this.ActiveMdiChild.Text = saveFileDialog1.FileName; //отображается изменённое название
			}
		}

		public void saveFile() //Сохранить
		{
			if(((Form2)this.ActiveMdiChild).fromFile) //если файл уже существовал 
				((Form2)this.ActiveMdiChild).SaveFile(((Form2)this.ActiveMdiChild).fileName); //схраняется изменённый файл
			else //если сохраняем новый файл -> Сохрнить как
				saveAsFile(); 
		}

		//НАСТРОЙКИ ИЗОБРАЖЕНИЯ (МЕНЮ "СВОЙСТВА")
		//======================================================================================
		
		public void setPenWidth() //Толщина линии
		{
			penWidthDialog pwd = new penWidthDialog(); 
			pwd.Val = lineWidth; //устанавливается "старое" значение ширины линии 
			if(pwd.ShowDialog()==DialogResult.OK) //нажали ОК
			{
				foreach(Form f2 in this.MdiChildren) ((Form2)f2).lineWidth = pwd.Val; //применяеся ко всем дочерним окнам
			//	lineWidth = pwd.Val; //устанавливается выбранное значение ширины линии 
			}
		}
		
		public void setPictureSize() //Размер изображения
		{
			pictureSizeDialog psd = new pictureSizeDialog();
			if(psd.ShowDialog()==DialogResult.OK) //нажали ОК
            {
				pictHeight = psd.newHeight; //устанавливается выбранное значение высоты изображения
				pictWidth = psd.newWidth; //устанавливается выбранное значение ширины изображения
            }
		}
		
		public void setPrimaryColor() //цвет пера, границы фигуры
		{
			if(primColorDialog.ShowDialog()==DialogResult.OK) //нажали ОК
            {
				foreach(Form f2 in this.MdiChildren) ((Form2)f2).primaryColor = primColorDialog.Color; //применяеся ко всем дочерним окнам
          	}

		}

		public void setSecondaryColor() //цвет заливки
		{
			if(secondColorDialog.ShowDialog()==DialogResult.OK) //нажали ОК
            {
				foreach(Form f2 in this.MdiChildren) ((Form2)f2).secondaryColor = secondColorDialog.Color; //применяеся ко всем дочерним окнам
            }
		}

		//МЕНЮ "ФИГУРЫ" и соответствующие кнопки в панели
		//======================================================================================

		public void allDown()
		{ //все "флажки" сняты
			lieToolStripMenuItem.Checked = false; //свойства -> линия
            rectangleToolStripMenuItem.Checked = false; //свойства -> прямоугольник 
			ellipseToolStripMenuItem.Checked = false; //свойства -> эллипс 
            curveToolStripMenuItem.Checked = false; //свойства -> кривая

        }

		public void setFigureType()
		{
			foreach(Form f2 in this.MdiChildren) ((Form2)f2).figureID = figureID; //тип выбранной фигуры применяется для всех активных окон
		}

		public void setFill() //свойства -> заливка
        {
			if(fillToolStripMenuItem.Checked)
			{
				solidFill = true;
                //fillToolStripMenuItem.Checked = true; --установлено автоматическое переключение отметки пункта меню (true свойство CheckOnClick).

            }
            else{
				solidFill = false;
				//fillToolStripMenuItem.Checked = false;

			}
			foreach(Form f2 in this.MdiChildren) ((Form2)f2).solidFill = solidFill; //применяется для всех активных окон

        }

		public void setLine()//линия
		{
			allDown();
			lieToolStripMenuItem.Checked = true;
			figureID = 0;
			setFigureType();
		}

		public void setCurve()//кривая
		{
			allDown();
			curveToolStripMenuItem.Checked = true;
			figureID = 1;
			setFigureType();
		}

		public void setRectangle()//прямоугольник
		{
			allDown();
			rectangleToolStripMenuItem.Checked = true;
			figureID = 2;
			setFigureType();
		}
        
		public void setEllipse()//эллипс
		{
			allDown();
			ellipseToolStripMenuItem.Checked = true;
			figureID = 3;
			setFigureType();
		}

		//======================================================================================

		public Form1()
		{
			InitializeComponent();
		}

		private void newToolStripMenuItem1_Click(object sender, EventArgs e) //меню "Файл" -> "Создать"  
        {
			newWindow();
			activateMenu();
		}

		private void oPenToolStripMenuItem_Click(object sender, EventArgs e) //меню "Файл" ->"Открыть"  
        {
			openFile();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e) //меню "Файл" ->"Сохранить" 
        {
			saveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) //меню "Файл" ->"Сохранить как"
        {
			saveAsFile();
		}

		private void lineWidthToolStripMenuItem_Click(object sender, EventArgs e)//меню "Свойства" ->"Толщина линий"
        {
			setPenWidth();
		}

		private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)//меню "Свойства" ->"Цвет заливки"
        {
			setSecondaryColor();
		}

		private void lineColorToolStripMenuItem_Click(object sender, EventArgs e)//меню "Свойства" ->"Цвет линий"
        {
			setPrimaryColor();
		}

		private void newPicturesSizeToolStripMenuItem_Click(object sender, EventArgs e)//меню "Свойства" ->"Размер изображений"
        {
			setPictureSize();
		}

		private void fillToolStripMenuItem_Click(object sender, EventArgs e)//меню "Фигура" ->"Заливка"
        {
			setFill();
		}

		private void lieToolStripMenuItem_Click(object sender, EventArgs e)//меню "Фигура" ->"Линия"
        {
			setLine();
		}

		private void curveToolStripMenuItem_Click(object sender, EventArgs e)//меню "Фигура" ->"Кривая"
        {
			setCurve();
		}

		private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)//меню "Фигура" ->"Прямоугольник"
        {
			setRectangle();
		}

		private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)//меню "Фигура" ->"Эллипс"
        {
			setEllipse();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)//Панель -> "Создать"
        {
			newWindow();
			activateMenu();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)//Панель -> "Открыть"
        {
			openFile();
		}

		private void toolStripButton3_Click(object sender, EventArgs e)//Панель -> "Сохранить"
        {
			saveFile();
		}

		private void toolStripButton4_Click(object sender, EventArgs e)//Панель -> "Тощина линий"
        {
			setPenWidth();
		}

		private void toolStripButton5_Click(object sender, EventArgs e)//Панель -> "Цвет линий"
        {
			setPrimaryColor();
		}

		private void toolStripButton6_Click(object sender, EventArgs e)//Панель -> "Цвет заливки"
        {
			setSecondaryColor();
		}

		private void toolStripButton7_Click(object sender, EventArgs e)//Панель -> "Размер изображений"
        {
			setPictureSize();
		}

		private void toolStripButton8_Click(object sender, EventArgs e)//Панель -> "Линия"
        {
			setLine();
		}

		private void toolStripButton9_Click(object sender, EventArgs e)//Панель -> "Кривая"
        {
			setCurve();
		}

		private void toolStripButton10_Click(object sender, EventArgs e)//Панель -> "Прямоугольник"
        {
			setRectangle();
		}
       
        private void toolStripButton11_Click(object sender, EventArgs e)//Панель -> "Эллипс"
        {
            setEllipse();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)//Панель -> "Заливка"
        {
            setFill();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }


	}
}
