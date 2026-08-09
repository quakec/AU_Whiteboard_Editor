using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AU_Whiteboard_Editor
{
    public sealed class ColorSelector : ComboBox
    {
        public ColorSelector()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                DropDownItem item = (DropDownItem)Items[e.Index];

                e.Graphics.DrawImage(item.Image, e.Bounds.Left, e.Bounds.Top);

                e.Graphics.DrawString(item.Value, e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + item.Image.Width, e.Bounds.Top + 2);
            }

            base.OnDrawItem(e);
        }
    }
    public sealed class DropDownItem
    {
        public string Value { get; set; }
        public Image Image { get; set; }
        public Color Color { get; set; }

        public DropDownItem() : this("") { }

        public DropDownItem(string value)
        {
            Value = value;
            Color = Color.FromName(value);
            Image = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(Image))
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    g.FillRectangle(brush, 3, 3, Image.Width - 4, Image.Height - 4);
                    g.DrawRectangle(Pens.LightGray, 2, 2, Image.Width - 3, Image.Height - 3);
                }
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }

}
