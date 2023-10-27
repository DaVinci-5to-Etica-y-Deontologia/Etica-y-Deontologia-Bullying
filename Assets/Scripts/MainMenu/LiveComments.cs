using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

public class LiveComments {
	
	public int NumRoom = 2;
	CookieContainer ccNico;
	ManualResetEvent getCookieDone;
	List<CommentClient> clients;
	
	public LiveComments() {
		ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };

		getCookieDone = new ManualResetEvent(false);
		var worker = new BackgroundWorker();
		worker.DoWork += new DoWorkEventHandler(DoGetCookie);		
		worker.RunWorkerAsync();
	}

	public void Run(string liveID) {
		var worker = new BackgroundWorker();
		worker.DoWork += new DoWorkEventHandler(DoGetComment);
		worker.RunWorkerAsync(liveID);
	}
	
	public void DoGetCookie(object sender, DoWorkEventArgs ev) {
		try {
			var ckfile = "cookie.dat";
			var acfile = "account.info";
			if (File.Exists(ckfile))
				ccNico = readCookie(ckfile);
			else
				Debug.Log("Can not found 'cookie.dat'");
			if (!NicoLiveAPI.IsLogin(ccNico)) {
				if (!File.Exists(acfile)) {
					Debug.Log("Can not found 'account.info'");
					return;
				}
				Debug.Log("Try to login...");
				ccNico = nicoLogin(acfile);
				if (!NicoLiveAPI.IsLogin(ccNico)) {
					ccNico = null;
					return;
				}
 			}
			getCookieDone.Set();
		}
		catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	CookieContainer nicoLogin(string file) {
		var accInfos = File.ReadAllLines(file);
		if (accInfos.Length < 2)
			throw new Exception(file + " is broken");
		var cc = NicoLiveAPI.Login(accInfos);
		writeCookie(cc);
		return cc;
	}

	CookieContainer readCookie(string file) {
		var container = new CookieContainer();
		var data = File.ReadAllText(file).Trim();
		var cookie = new Cookie("user_session", data, "/", ".nicovideo.jp");
		container.Add(cookie);
		return container;
	}

	void writeCookie(CookieContainer cc) {
		var file = "cookie.dat";
		var uri = new Uri("http://live.nicovideo.jp");
		string data = null;
		foreach(Cookie cookie in cc.GetCookies(uri)) {
			if (cookie.Name == "user_session")
			data = cookie.Value;
		}
		File.WriteAllText(file, data);
	}
	
	public void DoGetComment(object sender, DoWorkEventArgs ev) {
		try {
			if (ccNico == null) {
				Debug.Log("Cookie is nothing");
				return;
			}
			getCookieDone.WaitOne();
			var liveID = (string)ev.Argument;
			var info = NicoLiveAPI.GetPlayerStatus(ccNico, liveID);
			clients = NicoLiveAPI.GetComments(info, NumRoom);
		}
		catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
	
	public string[] Res {
		get {
			var res = StateObject.Res;
			if (res.Count == 0)
				return null;
			return res.Dequeue();
		}
	}

	public void DisConnect() {
		if (clients != null)
			clients.ForEach(x=>x.Disconnect());
	}
}

static class NicoLiveAPI {
	
	public static CookieContainer Login(string[] account) {
		var url = "https://secure.nicovideo.jp/secure/login";
		var cc = new CookieContainer();
		var param = new Dictionary<string, string>();
		param.Add("mail", account[0]);
		param.Add("password", account[1]);
		
		post(url, ref cc, param);
		return cc;
	}

	
	public static bool IsLogin(CookieContainer cc) {
		var url = "http://live.nicovideo.jp/notifybox";
		var notifybox = get(url, ref cc);
		var ret = notifybox.Length > 124;
		
		Debug.Log("Login: " + ret);
		return ret;
	}

	public static Dictionary<string, string> GetPlayerStatus(CookieContainer cc, string liveID) {
		var url = "http://live.nicovideo.jp/api/getplayerstatus?v=" + liveID;
		var xdoc = XDocument.Parse(get(url, ref cc));
		var ret = new Dictionary<string, string>();

		var status = xdoc.Element("getplayerstatus").Attribute("status").Value;
		if (status != "ok") {
			var code = xdoc.Descendants("code").Single().Value;
			throw new Exception("PlayerStatus is error on: " + code);
		}
		var ms = xdoc.Descendants("ms").Single();
		ret.Add("base_time", xdoc.Descendants("base_time").Single().Value);
		ret.Add("addr", ms.Element("addr").Value);
		ret.Add("port", ms.Element("port").Value);
		ret.Add("thread", ms.Element("thread").Value);
		ret.Add("comnID", xdoc.Descendants("default_community").Single().Value);
		ret.Add("room_label", xdoc.Descendants("room_label").Single().Value);
		return ret;
	}
	
	public static List<CommentClient> GetComments(Dictionary<string, string> info, int numRoom) {
		if (numRoom < 0 && numRoom > 4) 
			throw new Exception("numRoom is out of range");

		var addr = info["addr"];
		var port = info["port"];
		var thread = info["thread"];
		var comnID = info["comnID"];
		var room = info["room_label"];

		string[] arena = new string[] {addr, port, thread};

		if (comnID != room)
		switch (room) {
			case "立ち見A列": arena = calcInfo(arena, -1); break;
			case "立ち見B列": arena = calcInfo(arena, -2); break;
			case "立ち見C列": arena = calcInfo(arena, -3); break;
			default:
				throw new Exception("Response is not followed room_label");
		}

		var ret = new List<CommentClient>();
		for (var i = 0; i < numRoom; i++) {
			var inf = calcInfo(arena, i);
			var client = new CommentClient(inf[0], inf[1], inf[2], i);
			client.StartRecive();
			ret.Add(client);
		}
		return ret;
	}

	static string[] calcInfo(string[] info, int delta) {
		var addr = info[0];
		var port = info[1];
		var thread = info[2];
		int ad = addr[5] - '0';
		int po = int.Parse(port);
		int th = int.Parse(thread);
		if (delta > 0 && po + delta > 2814) {
			po = 2805 + po + delta - 2814 - 1;
			if (ad == 4)
				ad = 1;
			else
				ad++;
		}
		else if (delta < 0 && po + delta < 2805) {
			po = 2814 + po + delta - 2805 + 1;
			if (ad == 1)
				ad = 4;
			else
				ad--;
		}
		else if (po > 2814 || po < 2805)
			throw new Exception("Unexpected port is used");
		else
			po += delta;

		th += delta;
		addr = addr.Substring(0, 5) + (char)(ad + '0') + addr.Substring(6);
		return new string[] { addr, po.ToString(), th.ToString() };
	}
	
	static string get(string url, ref CookieContainer cc) {
		var req = (HttpWebRequest)WebRequest.Create(url);
		req.CookieContainer = cc;
		
		string ret;
		using(var res = req.GetResponse())
		using(var resStream = res.GetResponseStream())
		using(var sr = new StreamReader(resStream, Encoding.GetEncoding("UTF-8"))) {
			ret = sr.ReadToEnd();
		}
		return ret;
	}
	
	static string post(string url, ref CookieContainer cc, Dictionary<string, string> param) {
		var postData = string.Join("&",
			param.Select(x=>x.Key + "=" + System.Uri.EscapeUriString(x.Value)).ToArray());
		
		byte[] postDataBytes = Encoding.ASCII.GetBytes(postData);
		
		var req = (HttpWebRequest)WebRequest.Create(url);
		req.CookieContainer = cc;
		req.Method = "POST";
		req.ContentType = "application/x-www-form-urlencoded";
		req.ContentLength = postDataBytes.Length;
		
		using(var reqStream = req.GetRequestStream()) {
			reqStream.Write(postDataBytes, 0, postDataBytes.Length);
		};
		
		string ret;
		using(var res = req.GetResponse())
		using(var resStream = res.GetResponseStream())
		using(var sr = new StreamReader(resStream, Encoding.GetEncoding("UTF-8"))) {
			ret = sr.ReadToEnd();
		}
		return ret;
	}
}

class StateObject {
	public Socket workSocket = null;
	public const int BufferSize = 1024;
	public byte[] buffer = new byte[BufferSize];
	public string content = "";
	public static Queue<string[]> Res = new Queue<string[]>();
}

class CommentClient {

	bool allDone = false;
	StateObject state;
	Socket sock;
	string thread;
	string roomLabel;

	public static int FromRes = 0;

	public void Disconnect() {
		allDone = true;
		sock.Disconnect(false);
	}
	
	public CommentClient(string addr, string port, string thread, int roomIx) {
		try {
			this.thread = thread;
			this.roomLabel = toRoomLabel(roomIx);
			state = new StateObject();
			
			var hostaddr = Dns.GetHostEntry(addr).AddressList[0];
			var ephost = new IPEndPoint(hostaddr, int.Parse(port));
			sock = new Socket(AddressFamily.InterNetwork,
			                  SocketType.Stream, ProtocolType.Tcp);
			
			sock.Connect(ephost);
			state.workSocket = sock;
		}
		catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	string toRoomLabel(int roomIx) {
		switch (roomIx) {
		case 0: return "アリーナ";
		case 1: return "立ち見A";
		case 2: return "立ち見B";
		case 3: return "立ち見C";
		default: return "Error";
		}
	}

	public void StartRecive() {
		var worker = new BackgroundWorker();
		worker.DoWork += new DoWorkEventHandler(DoReceive);
		worker.RunWorkerAsync();
	}
	
	void DoReceive(object sender, DoWorkEventArgs ev) {
		try {
			allDone = false;
			send(string.Format("<thread thread=\"{0}\" version=\"20061206\" res_from=\"{1}\"/>\0", thread, FromRes));
			sock.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                  new AsyncCallback(ReadCallback), state);
			var count = 0;
			while (true) {
				send("\0");
				while (count < 600) {
					if (allDone)
						return;
					Thread.Sleep(1000);
					count++;
				}
				count = 0;
			}
		}	
		catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
	
	public void ReadCallback(IAsyncResult ar) {
		var state = (StateObject)ar.AsyncState;
		var handler = state.workSocket;
		
		var bytesRead = handler.EndReceive(ar);
		if (bytesRead > 0) {
			var content = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
			state.content = resProc(content, state.content);
		}
		if (!allDone) {
			handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
			                     new AsyncCallback(ReadCallback), state);
		}
	}
	
	string resProc(string content, string prev) {
		string xdocs;
		if (content.StartsWith("<chat")) {
			xdocs = content;
		}
		else {
			xdocs = prev + content;
		}
		
		foreach (string line in xdocs.Split('\0')) {
			if (line.StartsWith("<thread"))	{
				// var th = XElement.Parse(line);			
				// ticket = th.Attribute("ticket").Value;
				// srvTime = th.Attribute("server_time").Value;
				// DateTimeStart = DateTime.Now;
				Debug.Log(line);
				continue;
			}
			if (line.StartsWith("<chat_result")) {
				continue;
			}
			if (!line.EndsWith("</chat>")) {
				return line;
			}
			if (line.StartsWith("<chat ")) {
				chatProc(line);
			}
		}
		return "";
	}
	
	void chatProc(string chat_el) {
		var xelem = XElement.Parse(chat_el);
		var chat = xelem.Value;
		var id = xelem.Attribute("user_id").Value;
		var no = xelem.Attribute("no").Value;
		var premattr = xelem.Attribute("premium");
		string prem;
		
		if (premattr == null)
			prem = "0";
		else
			prem = premattr.Value;
		
		StateObject.Res.Enqueue(new string[]{chat, no, prem, id, roomLabel});
		
		if (chat == "/disconnect" && prem == "3")
			allDone = true;
	}
	
	void send(string data) {
		var byteData = Encoding.UTF8.GetBytes(data);
		sock.Send(byteData, 0, byteData.Length, 0);
	}
	
}