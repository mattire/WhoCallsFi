package md5b1823cc34a283e5868f52dd92bb3ef97;


public class WhoCallsServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhoCallsFi.WhoCallsServiceBinder, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", WhoCallsServiceBinder.class, __md_methods);
	}


	public WhoCallsServiceBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == WhoCallsServiceBinder.class)
			mono.android.TypeManager.Activate ("WhoCallsFi.WhoCallsServiceBinder, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public WhoCallsServiceBinder (md5b1823cc34a283e5868f52dd92bb3ef97.WhoCallsService p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == WhoCallsServiceBinder.class)
			mono.android.TypeManager.Activate ("WhoCallsFi.WhoCallsServiceBinder, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "WhoCallsFi.WhoCallsService, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
