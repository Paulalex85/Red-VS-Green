using System; 
using System.Threading; 
using System.Net; 
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class Echange_Server_Class
	{
		public bool Echange_en_cours = false;
		public string Info = "";
		public string msg = "";

		private string adresse_ip;
		TcpClient client = new TcpClient (); // Creates a TCP Client 
		NetworkStream stream; //Creats a NetworkStream (used for sending and receiving data) 
		IPAddress ip; 
		int port;
		Thread _thread = null;
		Compteur_Time _timer_ping_connection = new Compteur_Time (3000f);


		public Echange_Server_Class ()
		{
			Initialization_IP_PORT ();
		}

		public void Initialization_IP_PORT()
		{
			if (!IsolatedStorageSettings.ApplicationSettings.Contains ("adresse_ip")) {
				IsolatedStorageSettings.ApplicationSettings ["adresse_ip"] = "37.187.107.94";
			}
			if (!IsolatedStorageSettings.ApplicationSettings.Contains ("port")) {
				IsolatedStorageSettings.ApplicationSettings ["port"] = "1337";
			}

			adresse_ip = (string)IsolatedStorageSettings.ApplicationSettings ["adresse_ip"];
			port = int.Parse ((string)IsolatedStorageSettings.ApplicationSettings ["port"]);
			ip = IPAddress.Parse(adresse_ip);
		}

		public string Recuperer_Info()
		{
			string jambon = Info;
			Echange_en_cours = false;
			Info = "";
			msg = "";
			_thread = null;
			_timer_ping_connection._timer = 0f;

			return jambon;
		}

		public void Annulation_Transfert()
		{
			Echange_en_cours = false;
			Info = "";
			msg = "";
			_thread = null;
			_timer_ping_connection._timer = 0f;
		}

		public void Reinitialization_Connection ()
		{
			Echange_en_cours = false;
			Info = "";
			msg = "";
			_thread = null;
			_timer_ping_connection._timer = 0f;
			CloseConnection ();
		}

		public bool Info_Attente_Check()
		{
			if (Info != "") {
				return true;
			} else {
				return false;
			}
		}

		public bool Test_Connection(float timer)
		{
			if (Echange_en_cours) {
				if (_timer_ping_connection.IncreaseTimer (timer)) {
					Echange_en_cours = false;
					_thread = null;
					return true;
				}
			}
			return false;
		}

		public void CloseConnection()
		{
			if (client.Connected) {
				client.Close ();
			}
		}

		private string Connection()
		{
			try {
				client.Connect(ip, port);
			} catch (Exception ex) {
				return ex.ToString ();
			}
			return "";
		}

		private void clientSend()
		{
			if (!client.Connected) {
				Connection ();
			}
			try {
				Echange_en_cours = true;
				stream = client.GetStream (); //Gets The Stream of The Connection
				byte[] data; // creates a new byte without mentioning the size of it cuz its a byte used for sending
				data = Encoding.Default.GetBytes (msg); // put the msg in the byte ( it automaticly uses the size of the msg )
				int length = data.Length; // Gets the length of the byte data
				byte[] datalength = new byte[4]; // Creates a new byte with length of 4
				datalength = BitConverter.GetBytes (length); //put the length in a byte to send it
				stream.Write (data, 0, data.Length); //Sends the real data

				//RECEPTION
				Info = clientReceive ();
			} catch (Exception ex) {
				Reinitialization_Connection ();
			}
		}

		private string clientReceive()
		{
			stream = client.GetStream (); //Gets The Stream of The Connection
			byte[] data_receive = new byte[1024];
			byte[] data;
			stream.Read (data_receive, 0, data_receive.Length);

			int i = 0;
			while (data_receive [i] != 0) {
				i++;
			}
			data = new byte[i];
			Array.Copy (data_receive, data, i);
			return Encoding.ASCII.GetString (data);
		}

		public  void Nouveau_Joueur(string name, string color)
		{
			Echange_en_cours = true;
			msg = "1 " + name + " " + color;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Check_Name(string name)
		{
			Echange_en_cours = true;
			msg = "2 " + name;
			_thread = new Thread (clientSend );
			_thread.Start ();
		}

		public  void Main_Menu_Info(string id, string name)
		{
			Echange_en_cours = true;
			msg = "3 " + id + " " + name;
			_thread = new Thread (clientSend );
			_thread.Start ();
		}

		public  void Client_Valid_Or_Refuse_Game(string id_joueur, string id_partie, string code, string result)
		{
			Echange_en_cours = true;
			msg = "4 " + id_joueur + " " + id_partie + " " + code + " " + result;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Joueur_Pret_Partie(string id, string id_partie,string code)
		{
			Echange_en_cours = true;
			msg = "5 " + id + " " + id_partie + " " + code;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Joueur_Jouer_Coup(string id, string id_partie,string detail_coup, string code)
		{
			Echange_en_cours = true;
			msg = "6 " + id + " " + id_partie + " " + detail_coup + " " + code;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Verifier_Adversaire_Jouer(string id, string id_partie,string code)
		{
			Echange_en_cours = true;
			msg = "7 " + id + " " + id_partie + " " + code;
			_thread = new Thread (clientSend );
			_thread.Start ();
		}

		public  void Ajout_Joueur_Liste_Attente(string id, string code)
		{
			Echange_en_cours = true;
			msg = "8 " + id + " " + code;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Quitter_Liste_Attente(string id)
		{
			Echange_en_cours = true;
			msg = "9 " + id;
			_thread = new Thread (clientSend );
			_thread.Start ();
		}

		public  void Obtenir_info_partie_en_cours(string id_partie, string id_joueur)
		{
			Echange_en_cours = true;
			msg = "10 " + id_partie + " " + id_joueur;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public  void Verifier_Partie_trouver(string id_joueur, string code)
		{
			Echange_en_cours = true;
			msg = "11 " + id_joueur + " " + code;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public void Recuperer_Temps_Partie (string id_partie, string id)
		{
			Echange_en_cours = true;
			msg = "12 " + id_partie + " " + id;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}

		public void Abandonner_Partie (string id, string id_partie, string code)
		{
			Echange_en_cours = true;
			msg = "13 " + id + " " + id_partie + " " + code;
			_thread = new Thread (clientSend);
			_thread.Start ();
		}
	}
}