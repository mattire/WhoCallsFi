package md5b1823cc34a283e5868f52dd92bb3ef97;


public class IncomingCallReceiver
	extends android.telephony.PhoneStateListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCallStateChanged:(ILjava/lang/String;)V:GetOnCallStateChanged_ILjava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("WhoCallsFi.IncomingCallReceiver, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", IncomingCallReceiver.class, __md_methods);
	}


	public IncomingCallReceiver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == IncomingCallReceiver.class)
			mono.android.TypeManager.Activate ("WhoCallsFi.IncomingCallReceiver, WhoCallsFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCallStateChanged (int p0, java.lang.String p1)
	{
		n_onCallStateChanged (p0, p1);
	}

	private native void n_onCallStateChanged (int p0, java.lang.String p1);

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
