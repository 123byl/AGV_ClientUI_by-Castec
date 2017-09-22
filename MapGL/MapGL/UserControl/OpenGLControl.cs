using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using System.IO;
using SharpGL.SceneGraph.Assets;

namespace MapGL
{
    public partial class OpenGLControl : SharpGL.OpenGLControl
    {
        public OpenGLControl()
        {
            InitializeComponent();
        }
    }
}
