using System;
using System.Collections;
using System.Runtime.InteropServices;
using Gtk;
using GLib;

namespace WebKit
{
	unsafe public class DOMDocument : GLib.Object
	{
		public DOMDocument (IntPtr raw) : base (raw) {}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_document_get_elements_by_tag_name (IntPtr raw, IntPtr tagname);
		public DOMNodeList get_elements_by_tag_name (string tag)
		{
//			fixed(char* local_tag=tag)
//				return new DOMNodeList(webkit_dom_document_get_elements_by_tag_name(this.Handle, local_tag));
			return new DOMNodeList(webkit_dom_document_get_elements_by_tag_name(this.Handle, Marshaller.StringToPtrGStrdup(tag)));
		}

		[DllImport ("libwebkitgtk")]
		//private static extern IntPtr webkit_dom_document_get_element_by_id(IntPtr raw, char* elementId);
		private static extern IntPtr webkit_dom_document_get_element_by_id(IntPtr raw, IntPtr elementId);
		public DOMElement get_element_by_id (string elementId)
		{
			//fixed(char* local_elementId=elementId)
			//	return new DOMElement(webkit_dom_document_get_element_by_id(this.Handle, local_elementId));
			return new DOMElement(webkit_dom_document_get_element_by_id(this.Handle, Marshaller.StringToPtrGStrdup(elementId)));
		}
	}

	unsafe public class DOMNodeList: GLib.Object, IEnumerable
	{
		public DOMNodeList (IntPtr raw) : base (raw) {}
		
		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_node_list_item (IntPtr raw, int element);
		
		[DllImport ("libwebkitgtk")]
		private static extern int webkit_dom_node_list_get_length (IntPtr raw);
		
		public IEnumerator GetEnumerator ()
		{
			for (int i = 0; i < webkit_dom_node_list_get_length(this.Handle); i++) {
				yield return new DOMNode(webkit_dom_node_list_item(this.Handle, i));
			}
		}
	}
	
	unsafe public class DOMNode : GLib.Object
	{
		public DOMNode (IntPtr raw) : base (raw) {}

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_node_get_text_content(IntPtr raw);
		public string get_text_content ()
		{
			IntPtr data = webkit_dom_node_get_text_content(this.Handle);
			return Marshaller.PtrToStringGFree (data);
		}
	}
	
	unsafe public class DOMElement : GLib.Object
	{
		public DOMElement (IntPtr raw) : base (raw) {}
		
		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_element_get_attribute(IntPtr raw, IntPtr name);
		public string get_attribute (string name)
		{
			IntPtr data = webkit_dom_element_get_attribute (this.Handle, Marshaller.StringToPtrGStrdup(name));
			return Marshaller.PtrToStringGFree(data);
		}

		[DllImport ("libwebkitgtk")]
		//private static extern void webkit_dom_element_set_attribute(IntPtr raw, char* name, char* value, IntPtr error);
		private static extern void webkit_dom_element_set_attribute(IntPtr raw, IntPtr name, IntPtr value, IntPtr error);
		public void set_attribute (string name, string value)
		{
			IntPtr error = new IntPtr();
			webkit_dom_element_set_attribute(this.Handle, Marshaller.StringToPtrGStrdup(name), Marshaller.StringToPtrGStrdup(value), error);
		}
	}
	
	public class DOMHTMLElement : GLib.Object
	{
		public DOMHTMLElement (IntPtr raw) : base (raw) {}
		
		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_dom_html_element_get_inner_text(IntPtr raw);
		
		public string get_inner_text ()
		{
			IntPtr data = webkit_dom_html_element_get_inner_text(this.Handle);
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
			set {
				Type args_type = value.Method.GetParameters()[1].ParameterType;
				this.AddSignalHandler("create-web-view", value, args_type);
			}
		}
		public Delegate WebViewReady {
			set { this.AddSignalHandler ("web-view-ready", value); }
		}
		public Delegate NewWindowPolicyDecisionRequested {
			set {
				Type args_type = value.Method.GetParameters()[1].ParameterType;
				this.AddSignalHandler ("new-window-policy-decision-requested", value, args_type);
			}
		}

		public Delegate CloseWebView {
			set { this.AddSignalHandler ("close-web-view", value); }
		}

		public Delegate TitleChanged {
			set { this.AddSignalHandler ("title-changed", value); }
		}

		public Delegate LoadFinished {
			set { this.AddSignalHandler ("load-finished", value); }
		}

		public Delegate DocumentLoadFinished {
			set { this.AddSignalHandler ("document-load-finished", value); }
		}

		public Delegate NavigationRequested {
			set {
				Type args_type = value.Method.GetParameters () [1].ParameterType;
				this.AddSignalHandler ("navigation-requested", value, args_type);
			}
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
		public DOMDocument get_dom_document()
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

	public class NetworkRequest : GLib.Object
	{
		public NetworkRequest (IntPtr raw) : base(raw) { }

		[DllImport ("libwebkitgtk")]
		private static extern IntPtr webkit_network_request_get_uri(IntPtr raw);
		public string Uri {
			get {
				IntPtr data = webkit_network_request_get_uri(this.Handle);
				return Marshaller.PtrToStringGFree(data);
			}
		}
	}
}
