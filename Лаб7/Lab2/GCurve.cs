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
    [Serializable()]
    public class GCurve : AbstractFigure 
    {
        List<Point> curve = new List<Point>(); //список curve класса Point (динамический массив)

        /*Свойство являтся одной разновидностью члена класса. Оно сочетает в себе поле с методами доступа к нему.
        Преимущество свойства: его имя может быть использовано при присваивания аналогично имени обычной переменной, 
        но в действительности при обращении к свойству по имени автоматически вызываются его аксессоры get и set. 
        Свойства  лишь управляют доступом к полям, т.е. само свойство не предоставляет поле, и поэтому поле должно быть определено независимо от свойства.*/
        public override Point firstPoint
        {
            get // код аксессора для чтения из поля
            {
                return base.firstPoint;
            }
            set // код аксессора для записи в поле
            {
                base.firstPoint = value; //неявный параметр value содержит значение, присваиваемое свойству
                curve.Add(value);        //base используется для доступа к членам базового класса из производного класса
            }
        }
        public override Point secondPoint
        {
            get // код аксессора для чтения из поля
            {
                return base.secondPoint;
            }
            set // код аксессора для записи в поле
            {
                base.secondPoint = value;
                curve.Add(value);
            }
        }
        public override void drawFrame(ref Graphics g) //рисование "предварительной" кривой
        {
            Pen p = new Pen(frameColor); //перо определённого цвета (см. форму2) с толщиной по умолчанию
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //пунктир
            g.DrawCurve(p, curve.ToArray<Point>());
            p.Dispose(); //сброс
        }
        public override void draw(ref Graphics g) //рисование окончательной кривой (при отпускании)
        {
            Pen p = new Pen(primaryColor, lWidth); //перо выбранного цвета с выбранной толщиной 
            g.DrawCurve(p, curve.ToArray<Point>());
            p.Dispose(); //сброс
        }
    }
}
