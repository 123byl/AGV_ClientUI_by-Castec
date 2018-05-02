using System.Windows.Forms;

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
}