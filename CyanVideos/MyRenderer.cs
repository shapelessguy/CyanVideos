using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    class MyRenderer : ToolStripProfessionalRenderer
    {
        public MyRenderer() : base(new MyColors()) { }
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.White;
            base.OnRenderArrow(e);
        }
    }

    class MyColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(10, 10, 10); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.Transparent; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.Transparent; }
        }
        public override Color MenuItemBorder
        {
            get { return Color.Transparent; }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(10, 10, 10); }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return Color.FromArgb(10, 10, 10); }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return Color.Red; }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return Color.FromArgb(10, 10, 10); }
        }
        public override Color CheckSelectedBackground
        {
            get { return Color.Red; }
        }
        public override Color CheckBackground
        {
            get { return Color.Red; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.Transparent; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.Transparent; }
        }

        public override Color ToolStripContentPanelGradientBegin
        {
            get { return Color.Transparent; }
        }

        public override Color ToolStripGradientBegin
        {
            get { return Color.Transparent; }
        }

        public override Color ToolStripGradientEnd
        {
            get { return Color.Transparent; }
        }

        public override Color OverflowButtonGradientBegin
        {
            get { return Color.Red; }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return Color.Red; }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return Color.Red; }
        }

        public override Color GripDark
        {
            get { return Color.Red; }
        }
        public override Color GripLight
        {
            get { return Color.Red; }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return Color.Red; }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return Color.Red; }
        }
        public override Color SeparatorDark
        {
            get { return Color.Red; }
        }
        public override Color SeparatorLight
        {
            get { return Color.Red; }
        }
        public override Color ButtonPressedBorder
        {
            get { return Color.Red; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return Color.Red; }
        }
        public override Color ToolStripBorder
        {
            get { return Color.Red; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return Color.Red; }
        }
    }
}
