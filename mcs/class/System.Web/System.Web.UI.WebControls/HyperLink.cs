//
// System.Web.UI.WebControls.Label.cs
//
// Authors:
//	Ben Maurer (bmaurer@novell.com)
//
// (C) 2005-2010 Novell, Inc (http://www.novell.com)
//
// TODO: Are we missing something in LoadViewState?
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.ComponentModel;
using System.Security.Permissions;

namespace System.Web.UI.WebControls
{
	// CAS
	[AspNetHostingPermissionAttribute (SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermissionAttribute (SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	// attributes
	[ControlBuilder(typeof(HyperLinkControlBuilder))]
	[DataBindingHandler("System.Web.UI.Design.HyperLinkDataBindingHandler, " + Consts.AssemblySystem_Design)]
	[ParseChildren (false)]
	[ToolboxData("<{0}:HyperLink runat=\"server\">HyperLink</{0}:HyperLink>")]
	[DefaultProperty("Text")]
	[Designer("System.Web.UI.Design.WebControls.HyperLinkDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	public class HyperLink : WebControl
	{
		public HyperLink () : base (HtmlTextWriterTag.A)
		{
		}
				
		protected override void AddAttributesToRender (HtmlTextWriter w)
		{
			base.AddAttributesToRender (w);
			AddDisplayStyleAttribute (w);
			if (!IsEnabled)
				return;
			// add attributes - only if they're not empty
			string t = Target;
			string s = NavigateUrl;
			if (s.Length > 0)
#if TARGET_J2EE
				w.AddAttribute (HtmlTextWriterAttribute.Href, ResolveClientUrl (s, String.Compare (t, "_blank", StringComparison.InvariantCultureIgnoreCase) != 0));
#else
				w.AddAttribute (HtmlTextWriterAttribute.Href, ResolveClientUrl (s));
#endif
			if (t.Length > 0)
				w.AddAttribute (HtmlTextWriterAttribute.Target, t);
		}
		
		protected override void AddParsedSubObject (object obj)
		{
			if (HasControls ()) {
				base.AddParsedSubObject (obj);
				return;
			}
			
			LiteralControl lc = obj as LiteralControl;

			if (lc == null) {
				string s = Text;
				if (s.Length != 0) {
					Text = null;
					Controls.Add (new LiteralControl (s));
				}
				base.AddParsedSubObject (obj);
			} else
				Text = lc.Text;
		}

		[MonoTODO ("Why override?")]
		protected override void LoadViewState (object savedState)
		{
			base.LoadViewState (savedState);
		}
		
		protected internal override void RenderContents (HtmlTextWriter w)	
		{
			if (HasControls () || HasRenderMethodDelegate ()) {
				base.RenderContents (w);
				return;
			}
			string image_url = ImageUrl;
			if (!String.IsNullOrEmpty (image_url)) {
				string str = ToolTip;
				if (!String.IsNullOrEmpty (str))
					w.AddAttribute (HtmlTextWriterAttribute.Title, str);

				w.AddAttribute (HtmlTextWriterAttribute.Src, ResolveClientUrl (image_url));
				str = Text;
#if !NET_4_0
				if (!String.IsNullOrEmpty (str))
#endif
					w.AddAttribute (HtmlTextWriterAttribute.Alt, str);
#if !NET_4_0
				w.AddStyleAttribute (HtmlTextWriterStyle.BorderWidth, "0px");
#endif
				w.RenderBeginTag (HtmlTextWriterTag.Img);
				w.RenderEndTag ();
			} else
				w.Write (Text);
		}

		[Bindable(true)]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, " + Consts.AssemblySystem_Design, typeof(System.Drawing.Design.UITypeEditor))]
		[UrlProperty]
		[WebSysDescription ("")]
		[WebCategory ("Appearance")]
		public virtual string ImageUrl {
			get { return ViewState.GetString ("ImageUrl", String.Empty); }
			set { ViewState ["ImageUrl"] = value; }	
		}
		
		[Bindable(true)]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.UrlEditor, " + Consts.AssemblySystem_Design, typeof(System.Drawing.Design.UITypeEditor))]
		[UrlProperty]
		[WebSysDescription ("")]
		[WebCategory ("Navigation")]
		public string NavigateUrl {
			get { return ViewState.GetString ("NavigateUrl", String.Empty); }
			set { ViewState ["NavigateUrl"] = value; }	
		}
		
		[DefaultValue("")]
		[TypeConverter(typeof(System.Web.UI.WebControls.TargetConverter))]
		[WebSysDescription ("")]
		[WebCategory ("Navigation")]
		public string Target {
			get { return ViewState.GetString ("Target", String.Empty); }
			set { ViewState ["Target"] = value; }
		}
		
		[Bindable(true)]
		[DefaultValue("")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[Localizable (true)]
		[WebSysDescription ("")]
		[WebCategory ("Appearance")]
		public virtual string Text {
			get { return ViewState.GetString ("Text", String.Empty); }
			set {
				ViewState ["Text"] = value;
				if (HasControls ())
					Controls.Clear ();
			}
		}
#if NET_4_0
		public override bool SupportsDisabledAttribute {
			get { return RenderingCompatibilityLessThan40; }
		}
#endif
	}
}
