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
    public abstract class AbstractFigure //базовый класс для всех фигур
    {


        /*Свойство являтся одной разновидностью члена класса. Оно сочетает в себе поле с методами доступа к нему.
       Преимущество свойства: его имя может быть использовано при присваивания аналогично имени обычной переменной, 
       но в действительности при обращении к свойству по имени автоматически вызываются его аксессоры get и set. 
       Свойства  лишь управляют доступом к полям, т.е. само свойство не предоставляет поле, и поэтому поле должно быть определено независимо от свойства.*/

        //неявный параметр value содержит значение, присваиваемое свойству

        //для изображения кривой
        public virtual Point firstPoint
        {
            set { p1 = value; } // код аксессора для записи в поле
            get { return p1; } // код аксессора для чтения из поля
        }
        public virtual Point secondPoint
        {
            set { p2 = value; }
            get { return p2; }
        }
        //для ширина линии
        public int lineWidth
        {
            set
            {
                if (value <= 0)
                    lWidth = 1;
                else
                    lWidth = value;
            }
            get { return lWidth; }
        }
        //Поля
        protected Point p1, p2; //позиции мыши
        protected int lWidth; //ширина линии
        protected Color primaryColor, secondaryColor, frameColor;
        public bool fill;
        //Методы
        public void loadColors(Color pc, Color sc, Color fc)
        {
            primaryColor = pc;
            secondaryColor = sc;
            frameColor = fc;
        }
        public abstract void draw(ref Graphics g);//рисование окончательной фигуры
        public abstract void drawFrame(ref Graphics g);//рисование "предварительной" фигуры
    }
}
