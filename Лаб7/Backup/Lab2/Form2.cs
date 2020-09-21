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
		//VARIABLES
		//======================================================================================

		public string fileName; //file name whehe picture is saved
		public bool fromFile = false;
		bool paintAction = false;
		bool changedCanvas = false; 
		Point start,finish;
		List<AbstractFigure> fstorage = new List<AbstractFigure>();
		AbstractFigure toPaint;
		//painting stuff
		Bitmap canvas = new Bitmap(10,10);
		public Color backColor = Color.Aquamarine;		//color of background
		public Color frameColor = Color.Black;		//color of temporary figure
		public Color primaryColor = Color.Black;    //lines color
		public Color secondaryColor = Color.Black;    //fill color
		public int lineWidth = 2;
		public bool solidFill = false;
		public int figureID = 0; //line is default
		public int pictWidth=1,pictHeight=1;

		//DRAWING
		//======================================================================================
	
		public void drawCanvas()
		{
			canvas.Dispose();						   //destroy old bitmap...
			canvas = new Bitmap(pictWidth,pictHeight); //...and create a new one
			Graphics g = Graphics.FromImage(canvas);
			g.Clear(backColor);
			foreach(AbstractFigure go in fstorage)
				go.draw(ref g);
			if(paintAction)
				toPaint.drawFrame(ref g); //draw temporary figure
			g.Dispose();
		}

		private void initPainter()
		{   //set type of object to paint
			switch(figureID)
			{
				case 0:	toPaint = new GLine(); break;
				case 1: toPaint = new GCurve(); break;
				case 2: toPaint = new GRectangle(); break;
				case 3: toPaint = new GEllipse(); break;
				default: toPaint = new GLine(); break; //if figure is unknown - set as line
			}
			//set properties of new figure
			toPaint.loadColors(primaryColor,secondaryColor,frameColor);
			toPaint.firstPoint = start;
			toPaint.secondPoint = start;
			toPaint.lineWidth = lineWidth;
			toPaint.fill = solidFill;
		}

		//I/O
		//======================================================================================

		public void SaveFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name,FileMode.Create,FileAccess.Write,FileShare.None);
			formatter.Serialize(stream,pictWidth);
			formatter.Serialize(stream,pictHeight);
			formatter.Serialize(stream,backColor); //save background color
			formatter.Serialize(stream,fstorage);  //save figure storage
			stream.Close();
			changedCanvas = false;
		}

		public void LoadFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
			pictWidth = (int)formatter.Deserialize(stream);
			pictHeight = (int)formatter.Deserialize(stream);
			backColor = (Color)formatter.Deserialize(stream);
			fstorage = (List<AbstractFigure>)formatter.Deserialize(stream);
			stream.Close();
			//repaint
			drawCanvas();
			Refresh();
		}

		//FORM APPEARANCE
		//======================================================================================

		public Form2()
		{
			InitializeComponent();
		}

		private void Form2_Shown(object sender, EventArgs e)
		{
			canvas = new Bitmap(pictWidth,pictHeight);
			Graphics.FromImage(canvas).Clear(backColor);
		}

		private void Form2_Activated(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setWindowSizeCaption(pictWidth,pictHeight);
		}

		//MOUSE EVENTS
		//======================================================================================

		private void Form2_MouseDown(object sender, MouseEventArgs e)
		{ //start painting
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if(e.Button==MouseButtons.Left && eX<=pictWidth && eY<=pictHeight)
			{
				start.X = eX;
				start.Y = eY;
				finish = start;
				initPainter();
				paintAction = true;
			}
		}

		private void Form2_MouseMove(object sender, MouseEventArgs e)
		{ //painting
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if(paintAction)
			{
				finish.X = eX;
				finish.Y = eY;
				toPaint.secondPoint = finish; //store current finish point
				drawCanvas(); //repaint canvas
				Refresh();
			}
			((Form1)this.ParentForm).setMousePositionCaption(eX,eY);
		}

		private void Form2_MouseUp(object sender, MouseEventArgs e)
		{ //paint is finished
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if(paintAction)
			{
				paintAction = false;
				finish.X = eX;	
				finish.Y = eY;
				toPaint.secondPoint = finish;
				changedCanvas = true; //somethig was painted
				if(eX<=pictWidth && eY<=pictHeight && eX>=0 && eY>=0) //check - new figure must be inside of canvas
					fstorage.Add(toPaint); //add new figure to storage
				drawCanvas(); //repaint
				Refresh();
			}
		}

		private void Form2_MouseLeave(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setMousePositionCaption(-1,-1);
		}

		//FORM REPAINTING
		//======================================================================================

		private void Form2_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.LightGray);
			e.Graphics.DrawImage(canvas,AutoScrollPosition.X,AutoScrollPosition.Y); //just paint image
		}

		private void Form2_Resize(object sender, EventArgs e)
		{
			drawCanvas();
			Refresh();
		}

		//CLOSE ACTION
		//======================================================================================

		private void Form2_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(this.ParentForm.MdiChildren.Length==1)
			{
				((Form1)this.ParentForm).deactivateMenu(); //last child window was closed 
				((Form1)this.ParentForm).setMousePositionCaption(-1,-1);
				((Form1)this.ParentForm).setWindowSizeCaption(-1,-1);
			}
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(changedCanvas) //ask only if something was changed
				switch(MessageBox.Show("Сохранить изменения в \""+this.Text+"\"?","Вопрос",MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						((Form1)this.ParentForm).saveFile();
					break;
					case DialogResult.Cancel:
						e.Cancel = true;
					break;
				}
				//NO otherwise
		}

		//======================================================================================
	}

	//CLASSES
	//======================================================================================

	[Serializable()]
	public abstract class AbstractFigure //base class of all figures
	{
		//Properties
		public virtual Point firstPoint{
			set{ p1 = value;}
			get{ return p1; }
		}
		public virtual Point secondPoint{
			set{ p2 = value;}
			get{ return p2; }
		}
		public int lineWidth{
			set{
				if(value<=0)
					lWidth = 1;
				else
					lWidth = value;
			}
			get{ return lWidth; }
		}
		//Fileds
		protected Point p1,p2; //position
		protected int lWidth; //drawing line width
		protected Color primaryColor,secondaryColor,frameColor;
		public bool fill;
		//Methods
		public void loadColors(Color pc,Color sc,Color fc)
		{
			primaryColor = pc;
			secondaryColor = sc;
			frameColor = fc;
		}
		public abstract void draw(ref Graphics g);
		public abstract void drawFrame(ref Graphics g);
	}

	[Serializable()]
	public class GRectangle: AbstractFigure //rectangle
	{
		[NonSerialized()] int x,y,w,h;
		void transform()
		{
			if(p2.X>p1.X) //for X
			{
				x = p1.X;
				w = p2.X-p1.X;
			}else{
				x = p2.X;
				w = p1.X-p2.X;
			}
			if(p2.Y>p1.Y) //for Y
			{
				y = p1.Y;
				h = p2.Y-p1.Y;
			}else{
				y = p2.Y;
				h = p1.Y-p2.Y;
			}
		}
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			transform();
			g.DrawRectangle(p,x,y,w,h);
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor,lWidth);
			transform();
			if(fill)
				g.FillRectangle(new SolidBrush(secondaryColor),x,y,w,h);
			g.DrawRectangle(p,x,y,w,h);
			p.Dispose();
		}
	}

	[Serializable()]
	public class GEllipse: AbstractFigure //ellipse
	{
		[NonSerialized()] int x,y,w,h;
		void transform()
		{
			if(p2.X>p1.X) //for X
			{
				x = p1.X;
				w = p2.X-p1.X;
			}else{
				x = p2.X;
				w = p1.X-p2.X;
			}
			if(p2.Y>p1.Y) //for Y
			{
				y = p1.Y;
				h = p2.Y-p1.Y;
			}else{
				y = p2.Y;
				h = p1.Y-p2.Y;
			}
		}
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			transform();
			g.DrawEllipse(p,x,y,w,h);
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor,lWidth);
			transform();
			if(fill)
				g.FillEllipse(new SolidBrush(secondaryColor),x,y,w,h);
			g.DrawEllipse(p,x,y,w,h);
			p.Dispose();
		}
	}

	[Serializable()]
	public class GLine: AbstractFigure //line
	{
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawLine(p,p1,p2);
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor,lWidth);
			g.DrawLine(p,p1,p2);
			p.Dispose();
		}
	}

	[Serializable()]
	public class GCurve: AbstractFigure //curve line
	{
		List<Point>curve = new List<Point>();
		public override Point firstPoint
		{
			get{ return base.firstPoint; }
			set{
				base.firstPoint = value;
				curve.Add(value);
			}
		}
		public override Point secondPoint
		{
			get{ return base.secondPoint; }
			set{
				base.secondPoint = value;
				curve.Add(value);
			}
		}
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawCurve(p,curve.ToArray<Point>());
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor,lWidth);
			g.DrawCurve(p,curve.ToArray<Point>());
			p.Dispose();
		}
	}
}
