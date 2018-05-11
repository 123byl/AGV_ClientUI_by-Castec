using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CtBind {

    /// <summary>
    /// 實作IBindableComponent之控制項
    /// </summary>
    public class Bindable {

        private Bindable() {
        }
        
        /// <summary>
        /// 可綁定的StripLabel控制項
        /// </summary>
        public class ToolStripStatusLabel : System.Windows.Forms.ToolStripStatusLabel, IBindableComponent {

            #region IBindableComponent Members

            private BindingContext bindingContext;
            private ControlBindingsCollection dataBindings;

            public BindingContext BindingContext {
                get {
                    if (bindingContext == null) {
                        bindingContext = new BindingContext();
                    }
                    return bindingContext;
                }
                set {
                    bindingContext = value;
                }
            }

            public ControlBindingsCollection DataBindings {
                get {
                    if (dataBindings == null) {
                        dataBindings = new ControlBindingsCollection(this);
                    }
                    return dataBindings;
                }
            }

            #endregion IBindableComponent Members
        }

        /// <summary>
        /// 可綁定的StripPropgressBar控制項
        /// </summary>
        public class ToolStripProgressBar : System.Windows.Forms.ToolStripProgressBar, IBindableComponent {

            #region IBindableComponent Members

            private BindingContext bindingContext;
            private ControlBindingsCollection dataBindings;

            public BindingContext BindingContext {
                get {
                    if (bindingContext == null) {
                        bindingContext = new BindingContext();
                    }
                    return bindingContext;
                }
                set {
                    bindingContext = value;
                }
            }

            public ControlBindingsCollection DataBindings {
                get {
                    if (dataBindings == null) {
                        dataBindings = new ControlBindingsCollection(this);
                    }
                    return dataBindings;
                }
            }

            #endregion IBindableComponent Members
        }

        /// <summary>
        /// 可綁定的ToolStripMenuItem控制項
        /// </summary>
        public class ToolStripMenuItem : System.Windows.Forms.ToolStripMenuItem, IBindableComponent {

            #region IBindableComponent Members

            private BindingContext bindingContext;
            private ControlBindingsCollection dataBindings;

            public BindingContext BindingContext {
                get {
                    if (bindingContext == null) {
                        bindingContext = new BindingContext();
                    }
                    return bindingContext;
                }
                set {
                    bindingContext = value;
                }
            }

            public ControlBindingsCollection DataBindings {
                get {
                    if (dataBindings == null) {
                        dataBindings = new ControlBindingsCollection(this);
                    }
                    return dataBindings;
                }
            }

            #endregion IBindableComponent Members
        }

        /// <summary>
        /// 可綁定的StripButton控制項
        /// </summary>
        public class ToolStripButton: System.Windows.Forms.ToolStripButton, IBindableComponent {
            
            #region IBindableComponent Members

            private BindingContext bindingContext;
            private ControlBindingsCollection dataBindings;

            public BindingContext BindingContext {
                get {
                    if (bindingContext == null) {
                        bindingContext = new BindingContext();
                    }
                    return bindingContext;
                }
                set {
                    bindingContext = value;
                }
            }

            public ControlBindingsCollection DataBindings {
                get {
                    if (dataBindings == null) {
                        dataBindings = new ControlBindingsCollection(this);
                    }
                    return dataBindings;
                }
            }

            #endregion IBindableComponent Members

        }

        /// <summary>
        /// 可綁定的StripTextBox控制項
        /// </summary>
        public class ToolStripTextBox : System.Windows.Forms.ToolStripTextBox, IBindableComponent {


            #region IBindableComponent Members

            private BindingContext bindingContext;
            private ControlBindingsCollection dataBindings;

            public BindingContext BindingContext {
                get {
                    if (bindingContext == null) {
                        bindingContext = new BindingContext();
                    }
                    return bindingContext;
                }
                set {
                    bindingContext = value;
                }
            }

            public ControlBindingsCollection DataBindings {
                get {
                    if (dataBindings == null) {
                        dataBindings = new ControlBindingsCollection(this);
                    }
                    return dataBindings;
                }
            }

            #endregion IBindableComponent Members

        }

    }

    /// <summary>
    /// 繼承後可編輯的控制項
    /// </summary>
    public class Inheritable {

        [Designer(typeof(InheritableDesigner))]
        public class DataGridView : System.Windows.Forms.DataGridView {}

        [Designer(typeof(InheritableDesigner))]
        public class TableLayoutPanel : System.Windows.Forms.TableLayoutPanel {}

        [Designer(typeof(InheritableDesigner))]
        public class Panel : System.Windows.Forms.Panel { }

    }

    /// <summary>
    /// 繼承後可編輯的屬性
    /// </summary>
    public class InheritableDesigner : ControlDesigner {
        protected override InheritanceAttribute InheritanceAttribute {
            get {
                if (base.InheritanceAttribute == InheritanceAttribute.InheritedReadOnly) {
                    return InheritanceAttribute.Inherited;
                } else {
                    return base.InheritanceAttribute;
                }
            }
        }
    }

}