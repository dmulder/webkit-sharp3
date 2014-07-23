using System;
using System.Collections;
using System.Runtime.InteropServices;
using Gtk;
using GLib;

namespace WebKit
{
	class DOMDocument : GLib.Object
	{
		public DOMDocument (IntPtr raw) : base (raw) {}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_document_get_elements_by_tag_name (IntPtr raw, Char[] tagname);

		public DOMNodeList get_elements_by_tag_name (string tag)
		{
			return new DOMNodeList(webkit_dom_document_get_elements_by_tag_name(base.Handle, tag.ToCharArray()));
		}
	}

	class DOMNodeList: GLib.Object, IEnumerable
	{
		public DOMNodeList (IntPtr raw) : base (raw) {}
		
		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_node_list_item (IntPtr raw, int element);
		
		[DllImport ("libwebkitgtk")]
		private static extern int webkit_dom_node_list_get_length (IntPtr raw);
		
		public IEnumerator GetEnumerator ()
		{
			for (int i = 0; i < webkit_dom_node_list_get_length(base.Handle); i++) {
				yield return new DOMNode(webkit_dom_node_list_item(base.Handle, i));
			}
		}
	}
	
	class DOMNode : GLib.Object
	{
		public DOMNode (IntPtr raw) : base (raw) {}

		[DllImport ("libwebkitgtk")]
		private static extern Char[] webkit_dom_node_get_text_content(IntPtr raw);
		public string get_text_content ()
		{
			return new string(webkit_dom_node_get_text_content(this.Handle));
		}
	}
	
	class DOMElement : GLib.Object
	{
		public DOMElement (IntPtr raw) : base (raw) {}
		
		
	}
	
	class DOMHTMLElement : GLib.Object
	{
		public DOMHTMLElement (IntPtr raw) : base (raw) {}
		
		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_html_element_get_inner_text(IntPtr raw);
		
		public string get_inner_text ()
		{
			IntPtr data = webkit_dom_html_element_get_inner_text(base.Handle);
			return Marshaller.PtrToStringGFree(data);
		}
	}

	public class WebFrame : Gtk.Widget
	{
	}

	public class WebView : Gtk.Widget
	{
		[DllImport ("libgobject-2.0-0.dll.so")]
		private static extern void g_object_set(IntPtr raw, Char[] name, GLib.Value value);
		[DllImport ("libgobject-2.0-0.dll.so")]
		private static extern void g_object_get(IntPtr raw, Char[] name, GLib.Value value);

		[DllImport ("libwebkitgtk")]
		private static extern void webkit_web_view_set_settings(IntPtr web_view, IntPtr settings);
		public WebSettings settings {
			set { webkit_web_view_set_settings(this.Handle, value.Handle); }
		}

		public Delegate CreateWebView {
			set { Type args_type = value.Method.GetParameters()[1].ParameterType;
				this.AddSignalHandler("create-web-view", value, args_type); }
		}
		public Delegate WebViewReady {
			set { this.AddSignalHandler ("web-view-ready", value); }
		}
		public Delegate NewWindowPolicyDecisionRequested {
			set { this.AddSignalHandler ("new-window-policy-decision-requested", value); }
		}

		public Delegate CloseWebView {
			set { this.AddSignalHandler ("close-web-view", value); }
		}

		public Delegate TitleChanged {
			set { this.AddSignalHandler ("title-changed", value); }
		}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_web_view_new();
		public WebView () : base(webkit_web_view_new()) { }

		[DllImport ("libwebkitgtk")]
		private static extern void webkit_web_view_open(IntPtr web_view, Char[] uri);
		public void open (String uri)
		{
			webkit_web_view_open(this.Handle, uri.ToCharArray());
		}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_web_view_get_dom_document (IntPtr raw);
		DOMDocument get_dom_document()
		{
			return new DOMDocument(webkit_web_view_get_dom_document(this.Handle));
		}
	}

	public class WebSettings : GLib.Object
	{
		[DllImport ("g_object_link.so")]
		private static extern void SetStringProperty(IntPtr gobject, Char[] property_name, Char[] value);
		public String user_agent {
			set { SetStringProperty (this.Handle, "user-agent".ToCharArray (), value.ToCharArray ()); }
		}

		[DllImport ("g_object_link.so")]
		private static extern void SetBoolProperty(IntPtr gobject, Char[] property_name, bool value);
		public bool enable_spell_checking {
			set { SetBoolProperty (this.Handle, "enable-spell-checking".ToCharArray (), value); }
		}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_web_settings_new();
		public WebSettings () : base(webkit_web_settings_new()) { }
	}
}
